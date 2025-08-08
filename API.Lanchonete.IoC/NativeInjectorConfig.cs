using API.Lanchonete.Business.Business;
using API.Lanchonete.Data.Repositories;
using API.Lanchonete.Domain.Interfaces.Business;
using API.Lanchonete.Domain.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace API.Lanchonete.IoC
{
    public static class NativeInjectorConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            #region Business
            services.AddScoped<IPerfilBusiness, PerfilBusiness>();
            #endregion

            #region Repository
            services.AddScoped<IPerfilEFRepository, PerfilRepository>();
            #endregion

            #region AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            #endregion

        }
    }
}
