using BloggingPlatformAPI.EntityModels;
using BloggingPlatformAPI.Repositories;
using BloggingPlatformAPI.Repositories.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BloggingPlatformAPI.MinimalApi.MapGets;

/// <summary>
/// 
/// </summary>
public static class BlogMapGet
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <param name="logger"></param>
    public static void AddBlogMapGets(this WebApplication app, ILogger logger)
    {
        app.MapPost("blog/", async ([FromServices] IRepository repository, [FromBody] Post post) =>
            {
                try
                {
                    var newPost = await repository.Create(post);
                    return Results.Ok(newPost);
                }
                catch (Exception ex) when (ex is ArgumentException or ArgumentNullException)
                {
                    logger.LogError("Create: {0}", ex.Message);
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    logger.LogError("Create: {0}", ex.Message);
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("Create")
            .WithSummary("Create")
            .WithDescription("Create a new post")
            .Produces<Post>()
            .Produces<string>(StatusCodes.Status400BadRequest);

        app.MapPut("blog/{id:int}", async ([FromServices] IRepository repository, [FromRoute] int id, [FromBody] Post post) =>
            {
                try
                {
                    var newPost = await repository.Update(id, post);
                    return Results.Ok(newPost);
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidDataException or NoNullAllowedException)
                {
                    logger.LogError("Update: {0}", ex.Message);
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex) when (ex is NotFoundException)
                {
                    logger.LogError("Update: {0}", ex.Message);
                    return Results.NotFound(ex.Message);
                }
                catch (Exception ex) when (ex is ConflictException)
                {
                    logger.LogError("Update: {0}", ex.Message);
                    return Results.Conflict(ex.Message);
                }
                catch (Exception ex)
                {
                    logger.LogError("Update: {0}", ex.Message);
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("Update")
            .WithSummary("Update")
            .WithDescription("Update a post")
            .Produces<Post>()
            .Produces<string>(StatusCodes.Status400BadRequest);

        app.MapDelete("blog/{id:int}", async ([FromServices] IRepository repository, [FromRoute] int id) =>
            {
                try
                {
                    await repository.Delete(id);
                    return Results.NoContent();
                }
                catch (ArgumentException ex)
                {
                    logger.LogError("Delete: {0}", ex.Message);
                    return Results.BadRequest(ex.Message);
                }
                catch (ConflictException ex)
                {
                    logger.LogError("Delete: {0}", ex.Message);
                    return Results.Conflict(ex.Message);
                }
                catch (NotFoundException ex)
                {
                    logger.LogError("Delete: {0}", ex.Message);
                    return Results.NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    logger.LogError("Delete: {0}", ex.Message);
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("Delete")
            .WithSummary("Delete")
            .WithDescription("Delete a post")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status409Conflict)
            .Produces<string>(StatusCodes.Status404NotFound);

        app.MapGet("blog/", async ([FromServices] IRepository repository) =>
            {
                try
                {
                    var posts = await repository.GetAll();
                    return Results.Ok(posts);
                }
                catch (Exception ex)
                {
                    logger.LogError("Delete: {0}", ex.Message);
                    logger.LogError(ex.Message);
                    return Results.InternalServerError(ex.Message);
                }
            })
            .WithName("GetAllPosts")
            .WithSummary("GetAllPosts")
            .WithDescription("Get all non-deleted posts")
            .Produces<IEnumerable<Post>>()
            .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapGet("blog/{id:int}", async ([FromRoute] int id, [FromServices] IRepository repository) =>
            {
                try
                {
                    var post = await repository.GetById(id);
                    return Results.Ok(post);
                }
                catch (NotFoundException ex)
                {
                    logger.LogError("GetById: {0}", ex.Message);
                    return Results.NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    logger.LogError("GetById: {0}", ex.Message);
                    return Results.InternalServerError(ex.Message);
                }
            })
            .WithName("GetPostById")
            .WithSummary("GetPostById")
            .WithDescription("Get a post by Id")
            .Produces<Post>()
            .Produces<string>(StatusCodes.Status404NotFound)
            .Produces<string>(StatusCodes.Status500InternalServerError);

        app.MapGet("blog/search", async ([FromQuery(Name = "term")] string term, [FromServices] IRepository repository) =>
            {
                try
                {
                    var posts = await repository.Find(term);
                    return Results.Ok(posts);
                }
                catch (Exception ex)
                {
                    logger.LogError("Search: {0}", ex.Message);
                    return Results.InternalServerError(ex.Message);
                }
            })
            .WithName("Search")
            .WithSummary("Search")
            .WithDescription("Get posts by term")
            .Produces<IEnumerable<Post>>()
            .Produces<string>(StatusCodes.Status500InternalServerError);
    }
}