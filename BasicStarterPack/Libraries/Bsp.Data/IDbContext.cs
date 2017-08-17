using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Bsp.Core; 

namespace Bsp.Data {
    public interface IDbContext {
        DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;

        int SaveChanges();
    }
}