﻿using System;
using System.Collections.Generic;
using System.Linq;
using NFlex.Core;
using System.Data.Entity;
using System.Linq.Expressions;
using EntityFramework.Extensions;

namespace NFlex.Data.EF
{
    public abstract class Repository<TAggregateRoot,TKey>:IRepository<TAggregateRoot,TKey> where TAggregateRoot:class,IAggregateRoot<TKey>
    {
        protected IDbContext _dbContext { get; private set; }

        protected Repository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private DbSet<TAggregateRoot> Set
        {
            get { return _dbContext.Set<TAggregateRoot>(); }
        }

        /// <summary>
        /// 查找实体集合
        /// </summary>
        public IQueryable<TAggregateRoot> Queryable
        {
            get { return Set; }
        }

        /// <summary>
        /// 查找实体集合
        /// </summary>
        /// <param name="predicate">条件</param>
        public IQueryable<TAggregateRoot> QueryableAsNoTracking {
            get { return Set.AsNoTracking(); }
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public void Add(TAggregateRoot entity)
        {
            Set.Add(entity);
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entities">实体</param>
        public void Add(IEnumerable<TAggregateRoot> entities)
        {
            Set.AddRange(entities);
        }

        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="entity">实体</param>
        public void Update(TAggregateRoot entity)
        {
            AttachIfNot(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// 移除实体
        /// </summary>
        /// <param name="entity">实体</param>
        public void Remove(TAggregateRoot entity)
        {
                Set.Remove(entity);
        }

        /// <summary>
        /// 移除实体集合
        /// </summary>
        /// <param name="entities">实体集合</param>
        public void Remove(IEnumerable<TAggregateRoot> entities)
        {
            if (entities == null) return;

            var list = entities.ToList();
            if (!list.Any()) return;

            foreach (var entity in list)
                Remove(entity);
        }

        /// <summary>
        /// 移除实体
        /// </summary>
        /// <param name="predicate">条件</param>
        public void Remove(Expression<Func<TAggregateRoot, bool>> predicate)
        {
            var entities = Set.Where(predicate);
            Remove(entities);
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="id">实体标示</param>
        /// <returns></returns>
        public TAggregateRoot Single(TKey id)
        {
            return Set.Find(id);
        }

        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="predicate">条件</param>
        public TAggregateRoot Single(Expression<Func<TAggregateRoot, bool>> predicate)
        {
            return Set.FirstOrDefault(predicate);
        }

        /// <summary>
        /// 获取实体个数
        /// </summary>
        /// <param name="predicate">条件</param>
        public int Count(Expression<Func<TAggregateRoot, bool>> predicate = null)
        {
            return Set.Where(predicate).Count();
        }

        /// <summary>
        /// 判断实体是否存在
        /// </summary>
        /// <param name="predicate">条件</param>
        public bool Exists(Expression<Func<TAggregateRoot, bool>> predicate)
        {
            return Set.Any(predicate);
        }

        protected virtual bool AttachIfNot(TAggregateRoot entity)
        {
            if (!Set.Local.Contains(entity))
            {
                Set.Attach(entity);
                return true;
            }
            return false;
        }
    }

    public abstract class Repository<TAggregateRoot>:Repository<TAggregateRoot,Guid>
        where TAggregateRoot:class,IAggregateRoot<Guid>
    {
        protected Repository(IDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}
