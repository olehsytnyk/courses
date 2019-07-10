using EfCoreSample.Contracts;
using EfCoreSample.Contracts.V1.Responses;
using EfCoreSample.Doman.Entities;
using EfCoreSample.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EfCoreSample.Controllers.V1
{
    public class ProjectController : Controller
    {
        private readonly IPostService _postService;

        public ProjectController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        public IActionResult GetAll()
        {
            return Ok(_postService);
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromRoute] Project postRequest)
        {
            var post = new Project { Title = postRequest.Title, Discription = postRequest.Discription, Status = postRequest.Status };


            await _postService.CreatePost(post);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{Id}", post.Id.ToString());

            var response = new PostResponse { Id = post.Id };
            return Created(locationUri, response);
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromBody] Project postId)
        {
            var post = await _postService.GetPostById(postId.Id);

            if (post == null)
                return NotFound();

            return Ok(post);
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromBody] Project postId,[FromBody] Project title,[FromBody] Project discription)
        {
            var post = new Project
            {
                Id = postId.Id,
                Title = title.Title,
                Discription = discription.Discription
            };

            var updated = await _postService.UpdatePost(post);

            if (updated)
                return Ok(post);

            return NotFound();
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromBody]Project postId)
        {
            var deleted = await _postService.DeletePost(postId.Id);

            if (deleted)
                return NoContent();

            return NotFound();
        }
    }
}
