using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trackily.Services.DataAccess
{
    public static class DbServiceExtension
    {
        // Methods adapted from https://stackoverflow.com/a/51261289.

        public static IEnumerable<TEntity> LeftComplementRight<TEntity, TKey>(this IEnumerable<TEntity> left,
                                                                              IEnumerable<TEntity> right,
                                                                              Func<TEntity, TKey> keyRetrievalFunction)
        {
            var leftSet = left.ToList();
            var rightSet = right.ToList();

            var leftSetKeys = leftSet.Select(keyRetrievalFunction);
            var rightSetKeys = rightSet.Select(keyRetrievalFunction);

            var deltaKeys = leftSetKeys.Except(rightSetKeys);
            var leftComplementRightSet = leftSet.Where(i => deltaKeys.Contains(keyRetrievalFunction.Invoke(i)));
            return leftComplementRightSet;
        }

        public static IEnumerable<TEntity> Intersect<TEntity, TKey>(this IEnumerable<TEntity> left,
                                                                    IEnumerable<TEntity> right,
                                                                    Func<TEntity, TKey> keyRetrievalFunction)
        {
            var leftSet = left.ToList();
            var rightSet = right.ToList();

            var leftSetKeys = leftSet.Select(keyRetrievalFunction);
            var rightSetKeys = rightSet.Select(keyRetrievalFunction);

            var intersectKeys = leftSetKeys.Intersect(rightSetKeys);
            var intersectionEntities = leftSet.Where(i => intersectKeys.Contains(keyRetrievalFunction.Invoke(i)));
            return intersectionEntities;
        }
    }
}
