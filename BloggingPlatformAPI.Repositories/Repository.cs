using System.Data;
using BloggingPlatformAPI.DataContext;
using BloggingPlatformAPI.EntityModels;
using BloggingPlatformAPI.Repositories.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BloggingPlatformAPI.Repositories;

public class Repository : IRepository
{
    private readonly BloggingPlatformDataContext _dbContext;

    public Repository(BloggingPlatformDataContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Create a new post.
    /// </summary>
    /// <param name="post"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<Post> Create(Post post)
    {
        ArgumentNullException.ThrowIfNull(post);

        if (post.Id > 0)
        {
            var postIsExists = _dbContext.Posts.Any(p => p.Id == post.Id);
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

    /// <summary>
    /// Update a post.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedPost"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidDataException"></exception>
    /// <exception cref="NoNullAllowedException"></exception>
    public async Task<Post> Update(int id, Post updatedPost)
    {
        ArgumentNullException.ThrowIfNull(updatedPost);

        if (id <= 0)
        {
            throw new ArgumentException("The id must be greater than 0.");
        }

        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
        if (post is null)
        {
            throw new NotFoundException($"Post with Id = {id} not found.");
        }

        if (post.IsDeleted)
        {
            throw new ConflictException($"Post with Id={id} has been deleted.");
        }

        if (string.IsNullOrEmpty(updatedPost.Title))
        {
            throw new InvalidDataException("Post must have a title.");
        }

        if (string.IsNullOrEmpty(updatedPost.Content))
        {
            throw new InvalidDataException("Post must have a content.");
        }

        if (updatedPost.Tags.Contains(null))
        {
            throw new NoNullAllowedException("Tag must not contains null.");
        }

        post.Title = updatedPost.Title;
        post.Content = updatedPost.Content;
        post.Category = updatedPost.Category;
        post.Tags = updatedPost.Tags;
        post.IsDeleted = updatedPost.IsDeleted;

        await _dbContext.SaveChangesAsync();

        return post;
    }

    /// <summary>
    /// Delete a post.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotFoundException"></exception>
    /// <exception cref="ConflictException"></exception>
    public async Task Delete(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Id must be greater than 0.");
        }

        var post = await _dbContext.Posts.FindAsync(id);
        if (post is null)
        {
            throw new NotFoundException($"Post with Id={id} not found.");
        }

        if (post.IsDeleted)
        {
            throw new ConflictException($"Post with Id={id} has been deleted.");
        }

        post.IsDeleted = true;
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Get all posts with IsDelete=false.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Post>> GetAll()
    {
        var posts = await _dbContext.Posts.Where(p => p.IsDeleted == false).ToListAsync();
        return posts;
    }

    /// <summary>
    /// Get a post by Id with IsDelete=false.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotFoundException"></exception>
    /// <exception cref="ConflictException"></exception>
    public async Task<Post> GetById(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("The id must be great than 0.");
        }

        Post? post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);

        if (post is null)
        {
            throw new NotFoundException($"Post with Id={id} not found.");
        }

        if (post.IsDeleted)
        {
            throw new ConflictException($"Post with Id={id} was deleted.");
        }

        return post;
    }

    /// <summary>
    /// Find posts by term.
    /// </summary>
    /// <param name="term"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task<IEnumerable<Post>> Find(string term)
    {
        if (string.IsNullOrEmpty(term))
        {
            throw new ArgumentOutOfRangeException("The term value is not specified.");
        }

        IEnumerable<Post> posts = await _dbContext.Posts
            .Where(p => p.IsDeleted == false)
            .Where(p => string.IsNullOrEmpty(term) || (p.Title.Contains(term) || p.Content.Contains(term) || p.Tags.Contains(term)))
            .ToListAsync();

        return posts;
    }
}