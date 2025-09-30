using System.Diagnostics;
using FluentAssertions;

namespace BloggingPlatformAPI.Tests;

public class RepositoryGetTests : RepositoryBaseTests
{
    #region GetAll

    /// <summary>
    /// Проверка успешного получения всех постов из базы данных
    /// </summary>
    [Fact]
    public async Task GetAll_ReturnsAllPosts()
    {
        // Arrange
        var posts = CreatePosts(5);
        await DbContext.Posts.AddRangeAsync(posts);
        await DbContext.SaveChangesAsync();

        // Act
        var expected = await Repository.GetAll();

        // Assert
        expected.Should().NotBeNull();
        expected.Count().Should().Be(posts.Count);
        posts.All(p => expected.Contains(p)).Should().BeTrue();
    }

    /// <summary>
    /// Проверка возврата пустого списка при отсутствии постов в базе
    /// </summary>
    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoPosts()
    {
        // Arrange

        // Act
        var expected = await Repository.GetAll();

        // Assert
        expected.Should().NotBeNull();
        expected.Count().Should().Be(0);
    }

    /// <summary>
    /// Проверка производительности метода при большом количестве постов
    /// </summary>
    [Fact]
    public async Task GetAll_PerformanceTest()
    {
        // Arrange
        var posts = CreatePosts(100);
        await DbContext.Posts.AddRangeAsync(posts);
        await DbContext.SaveChangesAsync();

        var sw = new Stopwatch();
        var iterationCount = 1000;
        var totalTime = 0L;

        // Act
        for (int i = 0; i < iterationCount; i++)
        {
            sw.Restart();
            await Repository.GetAll();
            sw.Stop();
            totalTime += sw.ElapsedMilliseconds;
        }

        // Assert
        var averageTime = totalTime / iterationCount;
        averageTime.Should().BeLessThan(1);
    }

    #endregion GetAll

    #region Get

    /// <summary>
    /// Проверка получения поста по существующему ID
    /// </summary>
    [Fact]
    public async Task Get_ValidId_ReturnsPost()
    {
        // Arrange
        var post = CreatePost(CreatePostType.Simple);
        var id = (await DbContext.Posts.AddAsync(post)).Entity.Id;
        await DbContext.SaveChangesAsync();

        // Act
        var expected = await Repository.GetById(id);

        // Assert
        expected.Should().NotBeNull();
        expected.Id.Should().Be(id);
    }

    /// <summary>
    /// Проверка выброса исключения при передаче недопустимого ID(<= 0)
    /// </summary>
    [Fact]
    public async Task Get_InvalidId_ThrowsException()
    {
        // Arrange

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await Repository.GetById(-Random.Shared.Next(1, 500));
        });
    }

    /// <summary>
    /// Проверка обработки несуществующего ID
    /// </summary>
    [Fact]
    public async Task Get_NonExistentId_ThrowsException()
    {
        // Arrange

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await Repository.GetById(Random.Shared.Next(1, 500));
        });
    }

    /// <summary>
    /// Проверка поиска поста по термину в заголовке
    /// </summary>
    [Theory]
    [InlineData($"#1")]
    [InlineData($"#2")]
    [InlineData($"#3")]
    public async Task Get_WithSearchTerm_ReturnsMatchingPost(string term)
    {
        // Arrange
        var posts = CreatePosts(3);
        await DbContext.Posts.AddRangeAsync(posts);
        await DbContext.SaveChangesAsync();

        // Act
        var expected = await Repository.Find(term);

        // Assert
        expected.Should().NotBeNull();
        expected.Count().Should().Be(1);
        expected.First().Title.Should().Contain(term);
    }

    /// <summary>
    /// Проверка поиска поста по термину в контенте
    /// </summary>
    [Theory]
    [InlineData($"#1")]
    [InlineData($"#2")]
    [InlineData($"#3")]
    public async Task Get_WithSearchTerm_InContent(string term)
    {
        // Arrange
        var posts = CreatePosts(3);
        await DbContext.Posts.AddRangeAsync(posts);
        await DbContext.SaveChangesAsync();

        // Act
        var expected = await Repository.Find(term);

        // Assert
        expected.Should().NotBeNull();
        expected.Count().Should().Be(1);
        expected.First().Title.Should().Contain(term);
    }

    /// <summary>
    /// Проверка поиска поста по термину в тегах
    /// </summary>
    [Theory]
    [InlineData($"#1")]
    [InlineData($"#2")]
    [InlineData($"#3")]
    public async Task Get_WithSearchTerm_InTags(string term)
    {
        // Arrange
        var posts = CreatePosts(3);
        await DbContext.Posts.AddRangeAsync(posts);
        await DbContext.SaveChangesAsync();

        // Act
        var expected = await Repository.Find(term);

        // Assert
        expected.Should().NotBeNull();
        expected.Count().Should().Be(1);
        expected.First().Title.Should().Contain(term);
    }

    /// <summary>
    /// Проверка корректной работы при пустом поисковом термине
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Get_EmptyOrNullSearchTerm_ReturnsPost(string? term)
    {
        // Arrange

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
        {
            await Repository.Find(term);
        });
    }

    /// <summary>
    /// Проверка комбинированного поиска по нескольким параметрам
    /// </summary>
    [Theory]
    [InlineData($"#1")]
    [InlineData($"#2")]
    [InlineData($"#3")]
    public async Task Get_CombinationSearch_ReturnsPost(string term)
    {
        // Arrange
        var posts = CreatePosts(3);
        await DbContext.Posts.AddRangeAsync(posts);
        await DbContext.SaveChangesAsync();

        // Act
        var expected = await Repository.Find(term);

        // Assert
        expected.Should().NotBeNull();
        expected.Count().Should().Be(1);
        expected.First().Title.Should().Contain(term);
    }

    /// <summary>
    /// Проверка производительности при получении одного поста
    /// </summary>
    [Fact]
    public async Task Get_PerformanceTest_SinglePost()
    {
        // Arrange
        var posts = CreatePosts(100);
        await DbContext.Posts.AddRangeAsync(posts);
        await DbContext.SaveChangesAsync();

        var totalTime = 0L;
        var iterationCount = 1000;
        var sw = new Stopwatch();

        // Act
        for (int i = 0; i < iterationCount; i++)
        {
            sw.Restart();
            await Repository.GetById(1);
            sw.Stop();
            totalTime += sw.ElapsedMilliseconds;
        }
        var averageTime = totalTime / iterationCount;

        // Assert
        averageTime.Should().BeLessThan(1);
    }

    #endregion Get
}