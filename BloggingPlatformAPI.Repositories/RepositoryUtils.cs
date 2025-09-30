using Microsoft.EntityFrameworkCore;

namespace BloggingPlatformAPI.Repositories;

public class RepositoryUtils: IRepositoryUtils
{
    public bool Any<TSource>(DbSet<TSource> dbSet, Func<TSource, bool> anyPredicate) where TSource : class
    {
        return dbSet.Any(anyPredicate);
    }
}