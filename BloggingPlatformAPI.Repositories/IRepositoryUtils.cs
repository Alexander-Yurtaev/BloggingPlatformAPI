using Microsoft.EntityFrameworkCore;

namespace BloggingPlatformAPI.Repositories;

public interface IRepositoryUtils
{
    bool Any<TSource>(DbSet<TSource> dbSet, Func<TSource, bool> anyPredicate) where TSource : class;
}