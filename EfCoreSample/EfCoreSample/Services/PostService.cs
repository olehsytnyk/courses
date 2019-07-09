using EfCoreSample.Domain;
using EfCoreSample.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EfCoreSample.Services
{
    public class PostService : IPostService
    {
        private readonly EfCoreSampleDbContext _dataContext;

        public PostService(EfCoreSampleDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            return await _dataContext.Posts.ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            return await _dataContext.Posts.SingleOrDefaultAsync(x => x.Id == postId);
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            await _dataContext.Posts.AddAsync(post);
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
        }

        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            _dataContext.Posts.Update(postToUpdate);
            var updated = await _dataContext.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await GetPostByIdAsync(postId);
            _dataContext.Posts.Remove(post);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;
        }

        //private readonly List<Post> _posts;

        //public PostService()
        //{
        //    _posts = new List<Post>();
        //    for (var i = 0; i < 5; i++)
        //    {
        //        _posts.Add(new Post
        //        {
        //            Id = Guid.NewGuid(),
        //            Name = $"Post Name {i}"
        //        });
        //    }
        //}
    }
}

