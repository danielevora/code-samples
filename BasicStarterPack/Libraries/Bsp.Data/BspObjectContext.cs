using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bsp.Data.Mapping.Cats;
using Bsp.Core.Domain.Cats;

namespace Bsp.Data {
    public class BspObjectContext : DbContext {
        public DbSet<Cat> Cats { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            //TODO: use connection string here
            //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Demo;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.AddConfiguration(new CatMap());
        }
    }
}