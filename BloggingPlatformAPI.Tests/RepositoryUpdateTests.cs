using BloggingPlatformAPI.Repositories.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;

namespace BloggingPlatformAPI.Tests;

public class RepositoryUpdateTests : RepositoryBaseTests
{
    /// <summary>
    /// Проверка успешного обновления существующего поста с корректными данными
    /// </summary>
    [Fact]
    public async Task Update_ValidPost_Success()
    {
        // Arrange
        var post = CreatePost(CreatePostType.Simple);
        var id = (await DbContext.Posts.AddAsync(post)).Entity.Id;
        await DbContext.SaveChangesAsync();

        var newTitle = "New Post";
        post.Title = newTitle;

        var newContent = "New Content";
        post.Content = newContent;

        List<string> newTags = ["First", "Second"];
        post.Tags = newTags;

        // Act
        await Repository.Update(post.Id, post);

        // Assert
        var updatedPost = await DbContext.Posts.FirstAsync(p => p.Id == id);
        updatedPost.Should().NotBeNull();
        updatedPost.Id.Should().Be(id);
        updatedPost.Title.Should().Be(newTitle);
        updatedPost.Content.Should().Be(newContent);
        updatedPost.Tags.Should().NotBeNull();
        updatedPost.Tags.Count.Should().Be(newTags.Count);
        newTags.All(nt => updatedPost.Tags.Contains(nt));
    }

    /// <summary>
    /// Проверка выброса исключения при попытке обновить несуществующий пост
    /// </summary>
    [Fact]
    public async Task Update_NonExistentPost_ThrowsException()
    {
        // Arrange
        var post = CreatePost(CreatePostType.Simple);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await Repository.Update(1, post);
        });
    }

    /// <summary>
    /// Проверка обработки отрицательного или нулевого ID поста
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task Update_InvalidId_ThrowsException(int id)
    {
        // Arrange
        var post = CreatePost(CreatePostType.Simple);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await Repository.Update(id, post);
        });
    }

    /// <summary>
    /// Проверка валидации пустого заголовка при обновлении
    /// </summary>
    [Fact]
    public async Task Update_EmptyTitle_ThrowsException()
    {
        // Arrange
        var post = CreatePost(CreatePostType.Simple);
        var id = (await DbContext.Posts.AddAsync(post)).Entity.Id;
        await DbContext.SaveChangesAsync();

        post.Title = "";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(async () =>
        {
            await Repository.Update(post.Id, post);
        });
    }

    /// <summary>
    /// Проверка валидации пустого контента при обновлении
    /// </summary>
    [Fact]
    public async Task Update_EmptyContent_ThrowsException()
    {
        // Arrange
        var post = CreatePost(CreatePostType.Simple);
        var id = (await DbContext.Posts.AddAsync(post)).Entity.Id;
        await DbContext.SaveChangesAsync();

        post.Content = "";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(async () =>
        {
            await Repository.Update(post.Id, post);
        });
    }

    /// <summary>
    /// Проверка обработки null-объекта поста
    /// </summary>
    [Fact]
    public async Task Update_NullPost_ThrowsException()
    {
        // Arrange
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await Repository.Update(10, null);
        });
    }

    /// <summary>
    /// Проверка производительности операции обновления
    /// </summary>
    [Fact]
    public async Task Update_PerformanceTest()
    {
        // Arrange
        var post = CreatePost(CreatePostType.Simple);
        var id = (await DbContext.Posts.AddAsync(post)).Entity.Id;
        await DbContext.SaveChangesAsync();

        var sw = new Stopwatch();
        var iterationCount = 1000;
        var totalTime = 0L;

        // Act
        for (int i = 0; i < iterationCount; i++)
        {
            sw.Restart();
            await Repository.Update(post.Id, post);
            sw.Stop();
            totalTime += sw.ElapsedMilliseconds;
        }

        var averageTime = totalTime / iterationCount;

        // Assert
        averageTime.Should().BeLessThan(1);
    }

    /// <summary>
    /// Проверка того, что дата создания поста не меняется при обновлении
    /// </summary>
    [Fact]
    public async Task Update_ExistingPost_CreatedDateUnchanged()
    {
        // Arrange
        var post = CreatePost(CreatePostType.Simple);
        var id = (await DbContext.Posts.AddAsync(post)).Entity.Id;
        await DbContext.SaveChangesAsync();

        var oldCreatedAt = (await DbContext.Posts.FirstAsync(p => p.Id == id)).CreatedAt;
        post.Title = "New Post";

        // Act
        await Repository.Update(post.Id, post);

        // Assert
        var updatedPost = await DbContext.Posts.FirstAsync(p => p.Id == id);
        updatedPost.Should().NotBeNull();
        updatedPost.Id.Should().Be(id);
        updatedPost.CreatedAt.Should().Be(oldCreatedAt);
    }

    /// <summary>
    /// Проверка того, что дата создания поста не меняется при обновлении
    /// </summary>
    [Fact]
    public async Task Update_ExistingPost_UpdateDateChanged()
    {
        // Arrange
        var post = CreatePost(CreatePostType.Simple);
        var id = (await DbContext.Posts.AddAsync(post)).Entity.Id;
        await DbContext.SaveChangesAsync();

        var oldUpdatedAt = (await DbContext.Posts.FirstAsync(p => p.Id == id)).UpdatedAt;
        post.Title = "New Post";

        // Act
        await Repository.Update(id, post);

        // Assert
        var updatedPost = await DbContext.Posts.FirstAsync(p => p.Id == id);
        updatedPost.Should().NotBeNull();
        updatedPost.Id.Should().Be(id);
        updatedPost.UpdatedAt.Should().BeAfter(oldUpdatedAt);
    }

    /// <summary>
    /// Проверка обработки null-значения для тегов при обновлении
    /// </summary>
    [Fact]
    public async Task Update_ExistingPost_TagsNull_ThrowsException()
    {
        // Arrange
        var post = CreatePost(CreatePostType.Simple);
        var id = (await DbContext.Posts.AddAsync(post)).Entity.Id;
        await DbContext.SaveChangesAsync();

        post.Tags.Add(null);

        // Act & Assert
        await Assert.ThrowsAsync<NoNullAllowedException>(async () =>
        {
            await Repository.Update(id, post);
        });
    }

    /// <summary>
    /// Проверка корректной обработки пустого списка тегов
    /// </summary>
    [Fact]
    public async Task Update_ExistingPost_TagsEmpty_Success()
    {
        // Arrange
        var post = CreatePost(CreatePostType.Simple);
        var id = (await DbContext.Posts.AddAsync(post)).Entity.Id;
        await DbContext.SaveChangesAsync();

        post.Tags.Clear();

        // Act
        await Repository.Update(id, post);

        // Assert
        var updatedPost = await DbContext.Posts.FindAsync(id);
        updatedPost.Should().NotBeNull();
        updatedPost.Tags.Should().NotBeNull();
        updatedPost.Tags.Count.Should().Be(0);
    }
}
