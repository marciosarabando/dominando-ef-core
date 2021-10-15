using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCore.MultiTenant.Data.ModelFactory
{
    /*
    public class StrategySchemaModelCacheKey : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
        {
            var model = new
            {
                Type = context.GetType(),
                Schema = (context as ApplicationContext)?._tenant.TenantId
            };

            return model;
        }
    }*/
}