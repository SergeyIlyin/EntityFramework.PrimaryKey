using System;
using System.Linq;
using System.Linq.Expressions;

namespace EntityFrameworkCore.PrimaryKey
{
    public static class FindByKeyExtentions
    {
        public static TEntity Find<TEntity>(this IQueryable<TEntity> source, PrimaryKeyDictionary<TEntity> primaryKeyDictionary) where TEntity : class
        {
            return source.FirstOrDefault(primaryKeyDictionary.MakeExpression());
        }

        static internal Expression<Func<TEntity, bool>> MakeExpression<TEntity>(this PrimaryKeyDictionary<TEntity> keyDictionary) where TEntity : class
        {
            var type = typeof(TEntity);
            var pe = Expression.Parameter(type, "item");
            Expression expression = keyDictionary.MakeExpression(pe);
            if (expression == null)
            {
                return null;
            }
            var lambda = Expression.Lambda<Func<TEntity, bool>>(expression, pe);
            return lambda;
        }

        static internal Expression MakeExpression<TEntity>(this PrimaryKeyDictionary<TEntity> keyDictionary, ParameterExpression parametr) where TEntity : class
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
