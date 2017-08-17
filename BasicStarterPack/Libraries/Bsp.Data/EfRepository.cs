using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Bsp.Core;
using Bsp.Core.Data;

namespace Bsp.Data {
    public partial class EfRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity {
        private readonly IDbContext _context;
        private DbSet<TEntity> _entities;

        public void Insert(TEntity entity) {
            Entities.Add(entity);
            _context.SaveChanges();
        }

        protected virtual DbSet<TEntity> Entities {
            get {
                if (_entities == null)
                    _entities = _context.Set<TEntity>();
                return _entities;
            }
        }
    }
}

