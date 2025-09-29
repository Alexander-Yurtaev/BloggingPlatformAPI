using BloggingPlatformAPI.EntityModels;

namespace BloggingPlatformAPI.Repositories;

public interface IRepository
{
    Post Create(Post post);
    Post Update(int id, Post post);
    void Delete(int id);
    IEnumerable<Post> GetAll();
    Post Get(int id, string? term = null);
}