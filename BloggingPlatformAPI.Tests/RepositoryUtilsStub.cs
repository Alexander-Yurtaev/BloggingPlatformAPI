using BloggingPlatformAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BloggingPlatformAPI.Tests;

public class RepositoryUtilsStub : IRepositoryUtils
{
    private readonly bool _postExists;

    public RepositoryUtilsStub(bool postExists)
    {
        _postExists = postExists;
    }

    public bool Any<TSource>(DbSet<TSource> dbSet, Func<TSource, bool> anyPredicate) where TSource : class
    {
        return _postExists;
    }
}