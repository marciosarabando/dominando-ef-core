using Microsoft.AspNetCore.Http;

namespace EFCore.MultiTenant.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetTenantId(this HttpContext httpContext)
        {
            //Recupera da Rota qual é o Tenent que está acessando o recurso
            var tenant = httpContext.Request.Path.Value.Split('/', System.StringSplitOptions.RemoveEmptyEntries)?[0];

            return tenant;
        }
    }
}