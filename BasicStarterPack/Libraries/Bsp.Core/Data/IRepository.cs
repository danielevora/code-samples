using System.Collections.Generic;
using System.Linq;

namespace Bsp.Core.Data
{
    public partial interface IRepository<TEntity> where TEntity : BaseEntity
    {
        void Insert(TEntity entity);
    }
}