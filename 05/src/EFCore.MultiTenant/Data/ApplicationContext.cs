using EFCore.MultiTenant.Domain;
using EFCore.MultiTenant.Provider;
using Microsoft.EntityFrameworkCore;

namespace EFCore.MultiTenant.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Product> Products { get; set; }

        //public readonly TenantData _tenant;

        public ApplicationContext(DbContextOptions<ApplicationContext> options/*, TenantData tenant*/) : base(options)
        {
            /*_tenant = tenant;*/
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultSchema(_tenant.TenantId);

            modelBuilder.Entity<Person>().HasData(
                new Person { Id = 1, Name = "Person 1", TenantId = "Tenant-1" },
                new Person { Id = 2, Name = "Person 2", TenantId = "Tenant-2" },
                new Person { Id = 3, Name = "Person 2", TenantId = "Tenant-2" }
            );
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Description = "Description 1", TenantId = "Tenant-1" },
                new Product { Id = 2, Description = "Description 2", TenantId = "Tenant-2" },
                new Product { Id = 3, Description = "Description 3", TenantId = "Tenant-2" }
            );

            //modelBuilder.Entity<Person>().HasQueryFilter(p => p.TenantId == _tenant.TenantId);
            //modelBuilder.Entity<Product>().HasQueryFilter(p => p.TenantId == _tenant.TenantId);
        }
    }
}