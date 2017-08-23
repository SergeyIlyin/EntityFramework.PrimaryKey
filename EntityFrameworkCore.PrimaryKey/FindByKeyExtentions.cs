using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.PrimaryKey
{
    public static class FindByKeyExtentions
    {
        public static TEntity Find<TEntity>(this IQueryable<TEntity> source, PrimaryKeyDictionary<TEntity> primaryKeyDictionary) where TEntity : class
        {
            return source.FirstOrDefault(primaryKeyDictionary.BuildLambda());
        }
        public static  Task<TEntity> FindAsync<TEntity>(this IQueryable<TEntity> source, PrimaryKeyDictionary<TEntity> primaryKeyDictionary, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class
        {
            
            return  source.FirstOrDefaultAsync(primaryKeyDictionary.BuildLambda(), cancellationToken);
        }

        static internal Expression<Func<TEntity, bool>> BuildLambda<TEntity>(this PrimaryKeyDictionary<TEntity> keyDictionary) where TEntity : class
        {
            var type = typeof(TEntity);
            var pe = Expression.Parameter(type, "item");
            Expression expression = keyDictionary.BuildExpression(pe);

            if (expression == null)
            {
                return null;
            }
            var lambda = Expression.Lambda<Func<TEntity, bool>>(expression, pe);
            return lambda;
        }

        static internal Expression BuildExpression<TEntity>(this PrimaryKeyDictionary<TEntity> keyDictionary, ParameterExpression parametr) where TEntity : class
        {
            if (!keyDictionary.Any())
            {
                return null;
            }

            Expression final = null;
            Expression left = null;
            foreach (var keyValue in keyDictionary)
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
