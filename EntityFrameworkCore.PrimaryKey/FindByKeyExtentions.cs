using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.PrimaryKey
{
    public static class FindByKeyExtentions
    {
        public static TEntity Find<TEntity>(this IQueryable<TEntity> source, IDictionary<string, object> dictionary) where TEntity : class
        {
            return source.FirstOrDefault(dictionary.BuildLambda<TEntity>());
        }

        public static Task<TEntity> FindAsync<TEntity>(this IQueryable<TEntity> source, IDictionary<string, object> dictionary, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class
        {
            return source.FirstOrDefaultAsync(dictionary.BuildLambda<TEntity>(), cancellationToken);
        }

        public static Expression<Func<TEntity, bool>> BuildLambda<TEntity>(this IDictionary<string, object> dictionary) where TEntity : class
        {
            var type = typeof(TEntity);
            var pe = Expression.Parameter(type, "item");
            Expression expression = dictionary.BuildExpression<TEntity>(pe);

            if (expression == null)
            {
                return null;
            }
            var lambda = Expression.Lambda<Func<TEntity, bool>>(expression, pe);
            return lambda;
        }
        static internal Expression BuildExpression<TEntity>(this IDictionary<string, object> dictionary, ParameterExpression parametr) where TEntity : class
        {
            if (!dictionary.Any())
            {
                return null;
            }

            Expression final = null;
            Expression left = null;
            foreach (var keyValue in dictionary)
            {
                var propertyExp = Expression.Property(parametr, keyValue.Key);
                var valueExp = Expression.Constant(keyValue.Value);
                var curExp = Expression.Equal(propertyExp, valueExp);
                if (left == null)
                {
                    left = curExp;
                }
                else
                {
                    var rigth = curExp;
                    final = Expression.AndAlso(left, rigth);
                    left = final;
                }
            }
            return final ?? left;
        }
    }
}
