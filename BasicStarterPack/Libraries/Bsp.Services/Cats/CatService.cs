using System;
using Bsp.Core.Domain.Cats;
using Bsp.Core.Data;

namespace Bsp.Services.Cats {
    public partial class CatService : ICatService {

        private readonly IRepository<Cat> _catRepository;

        public CatService(IRepository<Cat> catRepository) {
            this._catRepository = catRepository;
        }

        public virtual void InsertCat(Cat cat)
        {
            if (cat == null)
                throw new ArgumentNullException(nameof(cat));

            _catRepository.Insert(cat);
        }
    }
}
