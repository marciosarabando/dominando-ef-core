using System;
using EFCore.MultiTenant.Data;
using EFCore.MultiTenant.Data.Interceptors;
using EFCore.MultiTenant.Data.ModelFactory;
using EFCore.MultiTenant.Extensions;
using EFCore.MultiTenant.Middlewares;
using EFCore.MultiTenant.Provider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace EFCore.MultiTenant
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private const string _defaultConnection = "DefaultConnection";
        private const string _tenant_1 = "Tenant-1";
        private const string _tenant_2 = "Tenant-2";
        private const string _custom = "custom";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<TenantData>();
            services.AddScoped<StrategySchemaInterceptor>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EFCore.MultiTenant", Version = "v1" });
            });

            /* Estratégia 01 - Identificador na tabela
            services.AddDbContext<ApplicationContext>(p =>
            p.UseSqlServer("server=localhost,1433; database=DEV.IO.ENTITY-3; User ID=SA;Password=Sara@1980docker; pooling=true;")
            .LogTo(Console.WriteLine)
            .EnableSensitiveDataLogging());*/

            // Estratégia 02 - Separação por Schema
            /*services.AddDbContext<ApplicationContext>((provider, options) =>
            //{
                options.UseSqlServer("server=localhost,1433; database=DEV.IO.ENTITY-3; User ID=SA;Password=Sara@1980docker; pooling=true;")
                .LogTo(Console.WriteLine)
                .ReplaceService<IModelCacheKeyFactory, StrategySchemaModelCacheKey>()
                .EnableSensitiveDataLogging();

                //var interceptor = provider.GetRequiredService<StrategySchemaInterceptor>();
                //options.AddInterceptors(interceptor);
            //});*/

            //Estratégia 03 - Banco de Dados

            //Necessário para o acesso do HttpContext usando em serviços
            services.AddHttpContextAccessor();

            services.AddScoped<ApplicationContext>(provider =>
            {
                var optionBuilder = new DbContextOptionsBuilder<ApplicationContext>();

                var httpContext = provider.GetService<IHttpContextAccessor>()?.HttpContext;

                var tenantId = httpContext?.GetTenantId();

                //var connectionString = Configuration.GetConnectionString(_custom).Replace("DEV.IO.TENANT-CUSTOM", tenantId);
                var connectionString = Configuration.GetConnectionString(tenantId);

                optionBuilder
                    .UseSqlServer(connectionString)
                    .LogTo(Console.WriteLine)
                    .EnableSensitiveDataLogging();

                return new ApplicationContext(optionBuilder.Options);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EFCore.MultiTenant v1"));
            }

            //DataBaseInicialize(app);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            //Usado na estratégia 01 e 02
            //app.UseMiddleware<TenentMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /*
        private void DataBaseInicialize(IApplicationBuilder app)
        {
            using var db = app.ApplicationServices
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<ApplicationContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            for (var i = 1; i <= 5; i++)
            {
                db.People.Add(new Domain.Person { Name = $"Person {i}" });
                db.Products.Add(new Domain.Product { Description = $"Description {i}" });
            }

            db.SaveChanges();
        }*/
    }
}
