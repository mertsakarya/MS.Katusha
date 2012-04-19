using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Repositories.DB.Base
{
    public static class RepositoryHelper
    {
        public static TEntity Add<TEntity>(DbContext context, TEntity entity) where TEntity : BaseModel
        {
            return ExecuteAdd(context, entity);
        }

        private static TEntity ExecuteAdd<TEntity>(DbContext context, TEntity entity) where TEntity : BaseModel
        {
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.CreationDate = entity.ModifiedDate;
            entity.DeletionDate = new DateTimeOffset(new DateTime(1900, 1, 1));
            entity.Deleted = false;
            context.Set<TEntity>().Add(entity);
            return entity;
        }

        public static TEntity AddWithGuid<TEntity>(DbContext context, TEntity entity) where TEntity : BaseGuidModel
        {
            entity = AddWithGuid(context, entity, (entity.Guid == Guid.Empty) ? Guid.NewGuid(): entity.Guid);
            return entity;
        }

        public static TEntity AddWithGuid<TEntity>(DbContext context, TEntity entity, Guid guid) where TEntity : BaseGuidModel
        {
            entity.Guid = guid;
            return ExecuteAdd(context, entity);
        }

        public static TEntity Update<TEntity>(DbContext context, TEntity entity, params Expression<Func<TEntity, object>>[] expressionParams) where TEntity : BaseModel
        {
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            var expressions = expressionParams.ToList();
            expressions.Add(p => p.ModifiedDate);
            AttachAndSetAsModified(context, entity, expressions.ToArray());
            return entity;
        }

        public static TEntity Delete<TEntity>(DbContext context, TEntity entity) where TEntity : BaseModel
        {
            if (context.Entry(entity).State == EntityState.Detached) {
                context.Set<TEntity>().Attach(entity);
            }
            return context.Set<TEntity>().Remove(entity);
        }

        public static IQueryable<T> Query<T>(IQueryable<T> dbSet, Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includeExpressionParams) where T : class
        {
            if (includeExpressionParams != null)
                dbSet = includeExpressionParams.Aggregate(dbSet, (current, expression) => current.Include(expression));
            if (filter != null)
                dbSet = dbSet.Where(filter);
            return dbSet;
        }

        private static void AttachAndSetAsModified<TEntity>(DbContext context, TEntity entity, params Expression<Func<TEntity, object>>[] expressionParams) where TEntity : class
        {
            if (context.Entry(entity).State == EntityState.Detached)
                context.Set<TEntity>().Attach(entity);
            SetAsModified(context, entity, expressionParams);
        }

        private static void SetAsModified<TEntity>(DbContext context, TEntity entity, params Expression<Func<TEntity, object>>[] expressionParams) where TEntity : class
        {
            expressionParams.ToList().ForEach(expression => context.Entry(entity).Property(expression).IsModified = true);
        }

        public static TEntity SoftDelete<TEntity>(DbContext context, TEntity entity) where TEntity : BaseModel
        {
            context.Entry(entity).State = EntityState.Modified;
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.DeletionDate= entity.ModifiedDate;
            entity.Deleted = true;
            var expressions = new Expression<Func<TEntity, object>>[]
                            {
                                p => p.ModifiedDate,
                                p => p.Deleted,
                                p => p.DeletionDate
                            };
            AttachAndSetAsModified(context, entity, expressions);
            return entity;
        }
    }
}
