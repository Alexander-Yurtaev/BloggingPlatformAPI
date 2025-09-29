using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BloggingPlatformAPI.EntityModels;

public class Post
{
    // ID поста (автоинкремент)
    public int Id { get; set; }

    // Заголовок поста
    [StringLength(255)]
    public string Title { get; set; } = null!;

    // Содержание поста
    public string Content { get; set; } = null!;

    // Категория
    [StringLength(100)]
    public string? Category { get; set; }

    // Теги (массив строк)
    [JsonConverter(typeof(JsonStringArrayConverter))]
    public List<string> Tags { get; set; } = new List<string>();

    // Дата создания
    public DateTimeOffset CreatedAt { get; set; }

    // Дата последнего обновления
    public DateTimeOffset UpdatedAt { get; set; }

    // Конструктор по умолчанию
    public Post()
    {
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    // Перегрузка конструктора
    public Post(string title, string content, string? category, List<string> tags)
    {
        Title = title;
        Content = content;
        Category = category;
        Tags = tags;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

// Конвертер для работы с JSONB в PostgreSQL
public class JsonStringArrayConverter : JsonConverter<List<string>>
{
    public override List<string> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        JsonSerializer.Deserialize<List<string>>(ref reader, options) ?? new List<string>();

    public override void Write(
        Utf8JsonWriter writer,
        List<string> value,
        JsonSerializerOptions options) =>
        JsonSerializer.Serialize(writer, value, options);
}