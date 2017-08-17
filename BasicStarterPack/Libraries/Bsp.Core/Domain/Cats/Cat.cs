using System;

namespace Bsp.Core.Domain.Cats {

    public partial class Cat : BaseEntity {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
    
}