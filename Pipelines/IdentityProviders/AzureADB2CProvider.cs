using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Sitecore.Abstractions;
using Sitecore.Diagnostics;
using Sitecore.Owin.Authentication.Configuration;
using Sitecore.Owin.Authentication.Extensions;
using Sitecore.Owin.Authentication.Pipelines.IdentityProviders;
using Sitecore.Owin.Authentication.Services;
using System.Threading.Tasks;


namespace Plugin.Sample.Accounts.Website.Pipelines.IdentityProviders
{
    public class AzureADB2CProvider : IdentityProvidersProcessor
    {
        public AzureADB2CProvider(FederatedAuthenticationConfiguration federatedAuthenticationConfiguration,
                                    ICookieManager cookieManager,
                                    BaseSettings settings)
            : base(federatedAuthenticationConfiguration, cookieManager, settings)
        {
        }

        protected override string IdentityProviderName => "AzureB2C";

        protected override void ProcessCore(IdentityProvidersArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));

            var identityProvider = GetIdentityProvider();
            var authenticationType = GetAuthenticationType();

            string tenant = Settings.GetSetting("Sitecore.Feature.Accounts.AzureB2C.Tenant");
            string signupsigninpolicy = Settings.GetSetting("Sitecore.Feature.Accounts.AzureB2C.Policy");
            string clientId = Settings.GetSetting("Sitecore.Feature.Accounts.AzureB2C.ClientId");
            string aadInstanceraw = Settings.GetSetting("Sitecore.Feature.Accounts.AzureB2C.AadInstance");
            var aadInstance = string.Format(aadInstanceraw, tenant, signupsigninpolicy);
            var metaAddress = $"{aadInstance}/v2.0/.well-known/openid-configuration";
            var redirectUri = Settings.GetSetting("Sitecore.Feature.Accounts.AzureB2C.RedirectUri");
            var options = new OpenIdConnectAuthenticationOptions(authenticationType)
            {
                Caption = identityProvider.Caption,
                AuthenticationMode = AuthenticationMode.Passive,
                RedirectUri = redirectUri,
                ClientId = clientId,
                Authority = aadInstance,
                MetadataAddress = metaAddress,
                UseTokenLifetime = true,
                TokenValidationParameters = new TokenValidationParameters() { NameClaimType = "name" },

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = context =>
                    {
                        var debugClaims = context.AuthenticationTicket.Identity?.Claims;
                        context.AuthenticationTicket.Identity.ApplyClaimsTransformations(new TransformationContext(this.FederatedAuthenticationConfiguration, identityProvider));
                        return Task.CompletedTask;
                    }
                }
            };

            args.App.UseOpenIdConnectAuthentication(options);
        }
    }
}