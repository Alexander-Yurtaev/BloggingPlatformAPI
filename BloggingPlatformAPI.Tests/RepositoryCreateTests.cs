using BloggingPlatformAPI.DataContext;
using BloggingPlatformAPI.EntityModels;
using BloggingPlatformAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BloggingPlatformAPI.Tests;

public class RepositoryCreateTests
{
    private readonly Mock<BloggingPlatformDataContext> _mockDbContext;
    private readonly Mock<DbSet<Post>> _mockPosts;
    
    public RepositoryCreateTests()
    {
        _mockDbContext = new Mock<BloggingPlatformDataContext>();
        _mockPosts = new Mock<DbSet<Post>>(_mockDbContext.Object.Posts);
        _mockDbContext.Setup(o => o.Posts).Returns(() => _mockPosts.Object);
    }

    [Fact]
    public async Task Create_ValidPost_ReturnsCreatedPost()
    {
        // Arrange
        var post = CreatePost(CreatePostType.Simple);

        _mockDbContext.Setup(x => x.Posts.AddAsync(post, default));
        
        _mockPosts.Setup(x => x.AddAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()));

        _mockDbContext.Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        var repositoryUtilsStub = new RepositoryUtilsStub(true);
        var repository = new Repository(_mockDbContext.Object, repositoryUtilsStub);

        // Act
        await repository.Create(post);

        // Assert
        _mockDbContext.Verify(c => c.Posts.AddAsync(post, default), Times.Once());
        _mockDbContext.Verify(c => c.SaveChangesAsync(default), Times.Once());
    }

    // Тест с существующим ID
    [Fact]
    public async Task Create_PostWithExistingId_ThrowsException()
    {
        // Arrange
        var post = CreatePost(CreatePostType.WithId);

        var repositoryUtilsStub = new RepositoryUtilsStub(true);
        var repository = new Repository(_mockDbContext.Object, repositoryUtilsStub);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await repository.Create(post);
        });
    }

    // Тест с null-объектом
    [Fact]
    public async Task Create_NullPost_ThrowsException()
    {
        // Arrange
        var repositoryUtilsStub = new RepositoryUtilsStub(true);
        var repository = new Repository(_mockDbContext.Object, repositoryUtilsStub);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await repository.Create(null);
        });
    }

    // Тест с пустым заголовком
    [Fact]
    public async Task Create_PostWithEmptyTitle_ThrowsException()
    {
        // Arrange
        var repositoryUtilsStub = new RepositoryUtilsStub(true);
        var repository = new Repository(_mockDbContext.Object, repositoryUtilsStub);

        var post = CreatePost(CreatePostType.WithEmptyTitle);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await repository.Create(post);
        });
    }

    // Тест с пустым контентом
    [Fact]
    public async Task Create_PostWithEmptyContent_ThrowsException()
    {
        // Arrange
        var repositoryUtilsStub = new RepositoryUtilsStub(true);
        var repository = new Repository(_mockDbContext.Object, repositoryUtilsStub);

        var post = CreatePost(CreatePostType.WithEmptyContent);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await repository.Create(post);
        });
    }

    #region Private Methods

    private Post CreatePost(CreatePostType createPostType)
    {
        switch (createPostType)
        {
            case CreatePostType.Simple:
                return new Post
                {
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

    private enum CreatePostType
    {
        Simple,
        WithId,
        WithEmptyTitle,
        WithEmptyContent,
    }

    #endregion
}