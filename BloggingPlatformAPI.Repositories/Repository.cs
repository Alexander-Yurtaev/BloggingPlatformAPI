using BloggingPlatformAPI.DataContext;
using BloggingPlatformAPI.EntityModels;

namespace BloggingPlatformAPI.Repositories;

public class Repository : IRepository
{
    private readonly BloggingPlatformDataContext _dbContext;
    private readonly IRepositoryUtils _repositoryUtils;

    public Repository(BloggingPlatformDataContext dbContext, IRepositoryUtils repositoryUtils)
    {
        _dbContext = dbContext;
        _repositoryUtils = repositoryUtils;
    }

    public async Task<Post> Create(Post post)
    {
        ArgumentNullException.ThrowIfNull(post);

        if (post.Id > 0)
        {
            var postIsExists = _repositoryUtils.Any(_dbContext.Posts, p => p.Id == post.Id);
            if (postIsExists)
            {
                throw new ArgumentException($"Post with Id = {post.Id} is exists.");
            }
        }

        if (string.IsNullOrEmpty(post.Title))
        {
            throw new ArgumentException("Post must have a title.");
        }

        if (string.IsNullOrEmpty(post.Content))
        {
            throw new ArgumentException("Post must have a content.");
        }

        await _dbContext.Posts.AddAsync(post, CancellationToken.None);
        await _dbContext.SaveChangesAsync();

        return post;
    }

    public async Task<Post> Update(int id, Post post)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Post>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<Post> Get(int id, string? term = null)
    {
        throw new NotImplementedException();
    }
}