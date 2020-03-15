using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STP.Common.Exceptions;
using STP.Common.Models;
using STP.Identity.Domain.Entities;
using STP.Identity.Infrastructure.Abstraction;
using STP.Identity.Infrastructure.Services;
using STP.Infrastructure.FileService;
using STP.Interfaces;
using STP.Interfaces.Enums;

namespace STP.Identity.API.Controllers
{
    [Route("api/avatar")]
    [ApiController]
    public class UserAvatarController : Controller
    {
        private readonly UserManagerService _userManager;
        private readonly IUserAvatarManagerService _avatarManager;
        private readonly IFileService _fileService;
        private readonly IValidator<UploadFileDTO> _fileUploadValidator;
        private readonly string _savePath;
        private readonly string _defaultAvatar;

        public UserAvatarController(IFileService fileService, UserManagerService userManager, 
            IUserAvatarManagerService avatarManager,
            IValidator<UploadFileDTO> fileUploadValidator)
        {
            _defaultAvatar = "avatarimage.jpg";
            _userManager = userManager;
            _fileService = fileService;
            _avatarManager = avatarManager;
            _fileUploadValidator = fileUploadValidator;
            _savePath = Path.Combine(
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
            "avatars");
            _defaultAvatar = "avatarimage.jpg";
        }

        /// <summary>
        /// Get user avatar
        /// </summary>
        ///<remarks>
        /// Sample request:
        ///
        ///     Get /avatar/{id}  
        /// </remarks>
        /// <param name="id">id user</param>
        /// <returns>FileSream</returns>
        /// <response code="200">If successful returned user avatar</response>
        /// <response code="404">If not found item of users</response>
        [HttpGet("{id}")]
        public async Task<Stream> GetUserAvatar([FromRoute]string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException(ErrorCode.UserNotFound);
            }
            Console.WriteLine(Path.Combine(_savePath, _defaultAvatar));
            var avatar = await _avatarManager.GetByUserIdAsync(id);
            if (avatar == null)
                return await _fileService.DownloadFileAsync(Path.Combine(_savePath, _defaultAvatar));
            else
                return await _fileService.DownloadFileAsync(Path.Combine(_savePath, id, Path.ChangeExtension(avatar.FileName, avatar.FileExtension)));
        }

        /// <summary>
        /// Upload a user avatar
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        /// <response code="200">If successful request</response>
        /// <response code="2410">If couldn't update user avatar</response>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostUserUploadAvatar([FromForm]UploadFileDTO files)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var resultPath = await _fileService.UploadFileAsync(
                files.FormFile.OpenReadStream(),
                Path.Combine(_savePath, user.Id),
                Path.GetExtension(files.FormFile.FileName),
                Path.GetFileName(files.FormFile.FileName));

            var avatar = await _avatarManager.GetByUserIdAsync(user.Id);
            if (avatar == null)
            {
                user.UserAvatar = new UserAvatar
                {
                    FileExtension = Path.GetExtension(resultPath),
                    FileName = Path.GetFileName(resultPath),
                };
                await _userManager.UpdateAsync(user);
            }
            else
            {
                await _avatarManager.UpdateAvatarAsync(user.Id,
                    Path.GetFileName(resultPath),
                    Path.GetExtension(resultPath));
            }

            return Ok();
        }
    }
}