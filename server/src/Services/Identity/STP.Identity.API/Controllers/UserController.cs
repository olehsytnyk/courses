using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using STP.Common.Exceptions;
using STP.Identity.Domain.DTOs;
using STP.Identity.Domain.DTOs.User;
using STP.Identity.Domain.Entities;
using STP.Identity.Infrastructure.Services;
using STP.Interfaces.Enums;
using STP.Interfaces.Messages;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace STP.Identity.API.Controllers
{
    [Route("api/users")]
    [Produces("application/json")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserManagerService _userManager;
        private readonly IMapper _mapper;
        private readonly IMessageBus _messageBus;

        public UserController(UserManagerService userManager, IMapper mapper, IMessageBus messageBus)
        {
            _userManager = userManager;
            _mapper = mapper;
            _messageBus = messageBus;
        }

        /// <summary>
        /// Return list of Users
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /users
        ///     
        /// </remarks>
        /// <param name="page">page</param>
        /// <param name="pageSize">page size</param>
        /// <returns>list of Users</returns>
        /// <response code="200">If successful returned list of users</response>
        /// <response code="404">If not found item of users</response> 
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetUsers(int page = 1, int pageSize = 5)
        {
            var users = await _userManager.GetAllAsync(page, pageSize);

            var mappedUser = _mapper.Map<UserDto[]>(users);

            return Ok(mappedUser);
        }

        /// <summary>
        /// Return list of Users
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /users/search/{id}
        ///     
        /// </remarks>
        /// <param name="text"> user's id</param>
        /// <returns>list of Users</returns>
        /// <response code="200">If successful returned list of users</response>
        /// <response code="404">If not found item of users</response> 
        [HttpGet("search/{text}")]
        public async Task<ActionResult<UserDto>> SearchUsers([FromRoute]string text)
        {
            var users = await _userManager.FullTextSearchAsync(text);

            var mappedUser = _mapper.Map<UserDto[]>(users);

            return Ok(mappedUser);
        }

        /// <summary>
        /// Return user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /users/{id}
        ///     
        /// </remarks>
        /// <param name="id"> user's id</param>
        /// <returns>user</returns>
        /// <response code="200">If successful returned list of users</response>
        /// <response code="404">If not found item of users</response> 
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById([FromRoute]string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException(ErrorCode.UserNotFound);
            }

            var mappedUser = _mapper.Map<UserDto>(user);

            return Ok(mappedUser);
        }

        /// <summary>
        /// Create a new User
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /users
        ///     {
        ///         "UserName": "Vasy_Pupkin",
        ///         "Email": "Pupkin@Vasy.com",
        ///         "Password": "ffnfnhD43$",
        ///         "PasswordConfirm": "ffnfnhD43$"
        ///     }
        ///     
        /// </remarks>
        /// <param name="userDto">user's dto</param>
        /// <returns>Created user</returns>
        /// <response code="200">If successful created new user</response>
        /// <response code="400">If didn't creat new user</response> 
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody]CreateUserDto userDto)
        {
            var mappedUser = _mapper.Map<User>(userDto);

            var result = await _userManager.CreateAsync(mappedUser, userDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            var createdUser = await _userManager.FindByNameAsync(userDto.UserName);

            var createdMappedUser = _mapper.Map<UserDto>(createdUser);

            return Ok(createdMappedUser);
        }

        /// <summary>
        /// Patch User
        /// </summary>
        /// <remarks>
        /// Samples request:
        ///
        ///     PATCH: /users
        ///     {
        ///         [
        ///             {"op": "replace", "path": "/FirstName", "value": "Bob"},
        ///             {"op": "add", "path": "/foo", "value": "Bob"},
        ///             {"op": "remove", "path": "/foo" }
        ///         ]
        ///     }  
        ///     
        /// </remarks>
        /// <param name="userPatch"> user's dto</param>
        /// <returns>Patched user</returns>
        /// <response code="200">If successful patched user</response> 
        /// <response code="400">If didn't patch user</response> 
        [Authorize]
        [HttpPatch]
        public async Task<IActionResult> PatchUser([FromBody]JsonPatchDocument<UpdateUserDto> userPatch)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var userDto = _mapper.Map<UpdateUserDto>(user);

            userPatch.ApplyTo(userDto, ModelState);

            TryValidateModel(userDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(userDto, user);

            var result = await _userManager.UpdateAsync(user);

            var mappedUser = _mapper.Map<UserDto>(user); 

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            UserDtoI userDtoI = _mapper.Map<UserDtoI>(mappedUser);
            _messageBus.Publish(userDtoI, "exc.User", RabbitExchangeType.DirectExchange, $"Update:{userDtoI.Id}");

            return Ok(mappedUser);
        }

        /// <summary>
        /// Delete a specific user.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE: /users
        ///     
        /// </remarks>
        /// <returns>Status code</returns>
        /// <response code="204">If successful delated user</response>
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await _userManager.DeleteAsync(user);

            return NoContent();
        }

        /// <summary>
        /// Put User
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /users
        ///     {
        ///         "UserName": "username",
        ///         "FirstName": "name1",
        ///         "LastName": "lastname1",
        ///         "Gender": "0",
        ///         "DateOfBirth": 345
        ///     }
        ///     
        /// </remarks>
        /// <param name="userDto"> user's dto</param>
        /// <returns>Status code</returns>
        /// <response code="200">If successful puted user</response>
        /// <response code="400">If failed to put user</response>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> PutUser([FromBody]UpdateUserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var map = _mapper.Map(userDto, user);

            var result = await _userManager.UpdateAsync(map);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            var mappedUser = _mapper.Map<UserDto>(user);

            UserDtoI userDtoI = _mapper.Map<UserDtoI>(mappedUser);
            _messageBus.Publish(userDtoI, "exc.User", RabbitExchangeType.DirectExchange, $"Update:{userDtoI.Id}");

            return Ok(mappedUser);
        }


        /// <summary>
        /// Change password current user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /users/changepassword
        ///     {
        ///         "OldPassword": "Pass123$",
        ///         "NewPassword": "NewPass123$",
        ///         "ConfirmNewPassword": "NewPass123$"
        ///     }
        ///     
        /// </remarks>
        /// <param name="changePasswordDto">ChangePasswordDto</param>
        /// <returns>Status code</returns>
        /// <response code="200">If successful changed user's password</response>
        /// <response code="400">If failed changed user's password</response>
        [Authorize]
        [HttpPut("changepassword")]
        public async Task<IActionResult> ChangePasswordUser([FromBody]ChangePasswordDto changePasswordDto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(userId);

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                throw new InvalidDataException(ErrorCode.InvalidUser, "Wrong OldPassword");
            }
            return Ok();
        }

        [Authorize(Policy = "internal")]
        [HttpGet("{id}/exist")]
        public async Task<ActionResult<string>> IsUserExistAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
                return Ok("OK");

            return Ok("Wrong");
        }
    }
}
