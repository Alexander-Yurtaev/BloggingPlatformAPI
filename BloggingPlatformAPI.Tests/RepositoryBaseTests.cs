using BloggingPlatformAPI.DataContext;
using BloggingPlatformAPI.EntityModels;
using BloggingPlatformAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BloggingPlatformAPI.Tests;

public abstract class RepositoryBaseTests
{
    protected readonly BloggingPlatformDataContext DbContext;
    protected readonly IRepository Repository;

    protected RepositoryBaseTests()
    {
        // Создаем опции для InMemory базы данных
        var options = new DbContextOptionsBuilder<BloggingPlatformDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Создаем контекст и репозиторий
        DbContext = new BloggingPlatformDataContext(options);
        Repository = new Repository(DbContext);
    }

    #region Private Methods

    protected List<Post> CreatePosts(int count)
    {
        var result = new List<Post>();

        for (int i = 1; i <= count; i++)
        {
            var post = CreatePost(CreatePostType.Simple);
            post.Title = $"Post #{i}";
            post.Content = $"This is Post #{i}";
            result.Add(post);
        }

        return result;
    }

    protected Post CreatePost(CreatePostType createPostType)
    {
        switch (createPostType)
        {
            case CreatePostType.Simple:
                return new Post
                {
                    Id = 0,
                    Title = "First Test Post",
                    Content = "This is The first test post!",
                    Category = "Technology",
                    Tags = new List<string> { "csharp", "postgresql", "efcore" }
                };
            case CreatePostType.WithId:
                return new Post
                {
                    Id = 1,
                    Title = "First Test Post",
                    Content = "This is The first test post!",
                    Category = "Technology",
                    Tags = new List<string> { "csharp", "postgresql", "efcore" }
                };
            case CreatePostType.WithEmptyTitle:
                return new Post
                {
                    Id = 1,
                    Title = "",
                    Content = "This is The first test post!",
                    Category = "Technology",
                    Tags = new List<string> { "csharp", "postgresql", "efcore" }
                };
            case CreatePostType.WithEmptyContent:
                return new Post
                {
                    Id = 1,
                    Title = "First Test Post",
                    Content = "",
                    Category = "Technology",
                    Tags = new List<string> { "csharp", "postgresql", "efcore" }
                };
            default:
                throw new ArgumentOutOfRangeException(nameof(createPostType), createPostType, null);
        }
    }

    protected enum CreatePostType
    {
        Simple,
        WithId,
        WithEmptyTitle,
        WithEmptyContent
    }

    #endregion
}