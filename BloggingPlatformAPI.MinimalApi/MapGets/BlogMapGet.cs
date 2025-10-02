using BloggingPlatformAPI.EntityModels;
using BloggingPlatformAPI.Repositories;
using BloggingPlatformAPI.Repositories.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BloggingPlatformAPI.MinimalApi.MapGets;

public static class BlogMapGet
{
    public static void AddBlogMapGets(this WebApplication app)
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
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("Create")
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
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex) when (ex is NotFoundException)
                {
                    return Results.NotFound(ex.Message);
                }
                catch (Exception ex) when (ex is ConflictException)
                {
                    return Results.Conflict(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("Update")
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
                    return Results.BadRequest(ex.Message);
                }
                catch (ConflictException ex)
                {
                    return Results.Conflict(ex.Message);
                }
                catch (NotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
            .WithName("Delete")
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
                    return Results.InternalServerError(ex.Message);
                }
            })
            .WithName("GetAllPosts")
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
                    return Results.NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            })
            .WithName("GetPostById")
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
                    return Results.InternalServerError(ex.Message);
                }
            })
            .WithName("Find")
            .Produces<IEnumerable<Post>>()
            .Produces<string>(StatusCodes.Status500InternalServerError);
    }
}