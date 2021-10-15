using EFCore.MultiTenant.Domain.Abstract;

namespace EFCore.MultiTenant.Domain
{
    public class Product : BaseEntity
    {
        public string Description { get; set; }
    }
}