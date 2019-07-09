using EfCoreSample.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EfCoreSample.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetPosts();

        Task<bool> CreatePost(Post post);

        Task<Post> GetPostById(Guid postId);

        Task<bool> UpdatePost(Post postToUpdate);

        Task<bool> DeletePost(Guid postId);
    }
}
