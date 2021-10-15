using System.Threading.Tasks;
using EFCore.MultiTenant.Extensions;
using EFCore.MultiTenant.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.MultiTenant.Middlewares
{
    public class TenentMiddleware
    {
        private readonly RequestDelegate _next;

        public TenentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            //cria uma instância do TenantData
            var tenant = httpContext.RequestServices.GetRequiredService<TenantData>();

            //Obtém o TenantId a usando o método de extensão do HttpContext
            tenant.TenantId = httpContext.GetTenantId();

            //Chama o próximo middleware
            await _next(httpContext);
        }

    }
}