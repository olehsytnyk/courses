using EfCoreSample.Doman.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EfCoreSample.Services
{
    public interface IPostService
    {
        Task<List<Project>> GetPosts();

        Task<bool> CreatePost(Project post);

        Task<Project> GetPostById(Guid postId);

        Task<bool> UpdatePost(Project postToUpdate);

        Task<bool> DeletePost(Guid postId);

    }
}
