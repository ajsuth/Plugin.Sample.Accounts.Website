using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Plugin.Sample.Accounts.Website.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Plugin.Sample.Accounts.Website.Infrastructure
{
    public class ServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            var descriptor =
                new ServiceDescriptor(
                    typeof(Sitecore.Owin.Authentication.Security.Cookies.DefaultCookieAuthenticationProvider),
                    typeof(CookieAuthenticationProvider),
                    ServiceLifetime.Transient);
            serviceCollection.Replace(descriptor);
        }
    }
}