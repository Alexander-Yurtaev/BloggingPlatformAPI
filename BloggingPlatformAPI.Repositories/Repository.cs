using BloggingPlatformAPI.DataContext;
using BloggingPlatformAPI.EntityModels;

namespace BloggingPlatformAPI.Repositories;

public class Repository : IRepository
{
    private readonly BloggingPlatformDataContext _dbContext;

    public Repository(BloggingPlatformDataContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Post Create(Post post)
    {
        throw new NotImplementedException();
    }

    public Post Update(int id, Post post)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Post> GetAll()
    {
        throw new NotImplementedException();
    }

    public Post Get(int id, string? term = null)
    {
        throw new NotImplementedException();
    }
}