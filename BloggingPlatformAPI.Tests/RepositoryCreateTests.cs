using BloggingPlatformAPI.EntityModels;
using BloggingPlatformAPI.Repositories;
using FluentAssertions;

namespace BloggingPlatformAPI.Tests;

public class RepositoryCreateTests : RepositoryBaseTests
{
    [Fact]
    public async Task Create_ValidPost_ReturnsCreatedPost()
    {
        // Arrange
        var post = CreatePost(CreatePostType.Simple);

        var repository = new Repository(DbContext);

        // Act
        var savedPost = await repository.Create(post);

        // Assert
        DbContext.Posts.Any().Should().BeTrue();
        DbContext.Posts.Count().Should().Be(1);
        DbContext.Posts.First().Id.Should().NotBe(0);
        savedPost.Id.Should().NotBe(0);
    }

    // Тест с существующим ID
    [Fact]
    public async Task Create_PostWithExistingId_ThrowsException()
    {
        // Arrange
        var post = CreatePost(CreatePostType.WithId);
        await DbContext.Posts.AddAsync(post);
        await DbContext.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await Repository.Create(post);
        });
    }

    // Тест с null-объектом
    [Fact]
    public async Task Create_NullPost_ThrowsException()
    {
        // Arrange
        Post? post = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await Repository.Create(post);
        });
    }

    // Тест с пустым заголовком
    [Fact]
    public async Task Create_PostWithEmptyTitle_ThrowsException()
    {
        // Arrange
        var post = CreatePost(CreatePostType.WithEmptyTitle);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await Repository.Create(post);
        });
    }

    // Тест с пустым контентом
    [Fact]
    public async Task Create_PostWithEmptyContent_ThrowsException()
    {
        // Arrange
        var post = CreatePost(CreatePostType.WithEmptyContent);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await Repository.Create(post);
        });
    }
}