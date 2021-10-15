using System.Data.Common;
using EFCore.MultiTenant.Provider;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFCore.MultiTenant.Data.Interceptors
{
    public class StrategySchemaInterceptor : DbCommandInterceptor
    {
        private readonly TenantData _tenantData;
        public StrategySchemaInterceptor(TenantData tenantData)
        {
            _tenantData = tenantData;
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            ReplaceShema(command);

            return base.ReaderExecuting(command, eventData, result);
        }

        private void ReplaceShema(DbCommand command)
        {
            //FROM PRODUCT -> FROM [TENANT_1].PRODUCTS
            command.CommandText = command.CommandText
                .Replace("FROM ", $" FROM [{ _tenantData.TenantId }].")
                .Replace("JOIN ", $" JOIN [{ _tenantData.TenantId }].");
        }
    }
}