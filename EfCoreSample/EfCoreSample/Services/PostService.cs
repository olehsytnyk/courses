using EfCoreSample.Doman.Entities;
using EfCoreSample.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
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

        public async Task<List<Project>> GetPosts()
        {
            return await _dataContext.Projects.ToListAsync();
        }

        public async Task<Project> GetPostById(Guid Id)
        {
            return await _dataContext.Projects.SingleOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<bool> CreatePost(Project post)
        {
            await _dataContext.Projects.AddAsync(post);
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
        }

        public async Task<bool> UpdatePost(Project postToUpdate)
        {
            _dataContext.Projects.Update(postToUpdate);
            var updated = await _dataContext.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeletePost(Guid postId)
        {
            var post = await GetPostById(postId);

            if (post == null)
                return false;

            _dataContext.Projects.Remove(post);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;
        }
    }
}

