using EFCore.MultiTenant.Domain.Abstract;

namespace EFCore.MultiTenant.Domain
{
    public class Person : BaseEntity
    {
        public string Name { get; set; }
    }
}