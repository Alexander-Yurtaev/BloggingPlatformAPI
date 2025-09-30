using BloggingPlatformAPI.EntityModels;

namespace BloggingPlatformAPI.Repositories;

public interface IRepository
{
    Task<Post> Create(Post post);
    Task<Post> Update(int id, Post post);
    Task Delete(int id);
    Task<IEnumerable<Post>> GetAll();
    Task<Post> Get(int id, string? term = null);
}