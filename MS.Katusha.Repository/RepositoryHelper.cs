using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Repository
{
    public static class RepositoryHelper
    {
        public static TEntity Add<TEntity>(DbContext context, TEntity entity) where TEntity : BaseModel
        {
            entity.ModifiedDate = DateTime.Now.ToUniversalTime();
            entity.CreationDate = entity.ModifiedDate;
            context.Set<TEntity>().Add(entity);
            return entity;
        }

        public static TEntity AddWithGuid<TEntity>(DbContext context, TEntity entity) where TEntity : BaseGuidModel
        {
            entity = AddWithGuid(context, entity, Guid.NewGuid());
            return entity;
        }

        public static TEntity AddWithGuid<TEntity>(DbContext context, TEntity entity, Guid guid) where TEntity : BaseGuidModel
        {
            entity.ModifiedDate = DateTime.Now.ToUniversalTime();
            entity.CreationDate = entity.ModifiedDate;
            entity.Guid = guid;
            context.Set<TEntity>().Add(entity);
            return entity;
        }

        public static TEntity Update<TEntity>(DbContext context, TEntity entity, params Expression<Func<TEntity, object>>[] expressionParams) where TEntity : BaseModel
        {
            entity.ModifiedDate = DateTime.Now.ToUniversalTime();
            List<Expression<Func<TEntity, object>>> expressions = expressionParams.ToList();
            expressions.Add(p => p.ModifiedDate);
            AttachAndSetAsModified(context, entity, expressionParams);
            return entity;
        }

        public static TEntity Delete<TEntity>(DbContext context, TEntity entity) where TEntity : BaseModel
        {
            return context.Set<TEntity>().Remove(entity);
        }

        public static IQueryable<T> Query<T>(IQueryable<T> dbSet, Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressionParams) where T : class
        {
            if (includeExpressionParams != null)
            {
                foreach (Expression<Func<T, object>> expression in includeExpressionParams)
                {
                    dbSet = dbSet.Include(expression);
                }
            }
            if (filter != null)
            {
                dbSet = dbSet.Where(filter);
            }
            return dbSet;
        }

        private static void AttachAndSetAsModified<TEntity>(DbContext context, TEntity entity, params Expression<Func<TEntity, object>>[] expressionParams) where TEntity : class
        {
            if (context.Entry(entity).State == EntityState.Detached)
            {
                context.Set<TEntity>().Attach(entity);
            }
            SetAsModified(context, entity, expressionParams);
        }

        private static void SetAsModified<TEntity>(DbContext context, TEntity entity, params Expression<Func<TEntity, object>>[] expressionParams) where TEntity : class
        {
            expressionParams.ToList().ForEach(expression => context.Entry(entity).Property(expression).IsModified = true);
        }

    }
}
