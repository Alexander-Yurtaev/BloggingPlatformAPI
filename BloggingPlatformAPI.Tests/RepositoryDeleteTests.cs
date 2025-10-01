using BloggingPlatformAPI.Repositories.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BloggingPlatformAPI.Tests;

public class RepositoryDeleteTests : RepositoryBaseTests
{
    /// <summary>
    /// Проверка успешного удаления существующего поста
    /// </summary>
    [Fact]
    public async Task Delete_ExistingPost_Success()
    {
        // Arrange
        var post = CreatePost(CreatePostType.Simple);
        var id = (await DbContext.Posts.AddAsync(post)).Entity.Id;
        await DbContext.SaveChangesAsync();

        // Act
        await Repository.Delete(id);

        // Assert
        var deletedPost = await DbContext.Posts.FindAsync(id);
        deletedPost.Should().NotBeNull();
        deletedPost.Id.Should().BeGreaterThan(0);
        deletedPost.IsDeleted.Should().BeTrue();
    }

    /// <summary>
    /// Проверка попытки удаления несуществующего поста
    /// </summary>
    [Fact]
    public async Task Delete_NonExistentPost_ThrowsException()
    {
        // Arrange
        var id = 1;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await Repository.Delete(id);
        });
    }

    /// <summary>
    /// Проверка попытки удаления уже удаленного поста
    /// </summary>
    [Fact]
    public async Task Delete_HasDeletedPost_ThrowsException()
    {
        // Arrange
        var post = CreatePost(CreatePostType.Simple);
        post.IsDeleted = true;
        var id = (await DbContext.Posts.AddAsync(post)).Entity.Id;
        await DbContext.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(async () =>
        {
            await Repository.Delete(id);
        });
    }

    /// <summary>
    /// Проверка удаления поста с отрицательным ID
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task Delete_ZeroId_Or_InvalidId_ThrowsException(int id)
    {
        // Arrange

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await Repository.Delete(id);
        });
    }

    /// <summary>
    /// Проверка удаления нескольких постов одновременно
    /// </summary>
    [Fact]
    public async Task Delete_MultiplePosts_Success()
    {
        // Arrange
        var posts = CreatePosts(10);
        await DbContext.Posts.AddRangeAsync(posts);
        await DbContext.SaveChangesAsync();

        // Act
        foreach (var id in posts.Select(p => p.Id))
        {
            await Repository.Delete(id);
        }
        
        // Assert
        (await DbContext.Posts.AnyAsync(p => p.IsDeleted == false)).Should().BeFalse();
    }
}
