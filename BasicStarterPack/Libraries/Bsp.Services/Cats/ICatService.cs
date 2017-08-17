using System;
using Bsp.Core.Domain.Cats;

namespace Bsp.Services.Cats {
    public partial interface ICatService {
        void InsertCat(Cat cat);
    }
}
