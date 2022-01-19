using Microsoft.Owin.Security.Cookies;
using Sitecore.Abstractions;
using Sitecore.Diagnostics;
using Sitecore.Owin.Authentication.Pipelines.CookieAuthentication.SignIn;
using Sitecore.Annotations;
using Sitecore.Owin.Authentication.Security;
using Sitecore;
using Sitecore.Commerce.XA.Foundation.Common.Context;
using Sitecore.Owin.Authentication.Configuration;
using Plugin.Sample.Accounts.Website.Arguments;

namespace Plugin.Sample.Accounts.Website.Services
{
    public class CookieAuthenticationProvider : Sitecore.Owin.Authentication.Security.Cookies.DefaultCookieAuthenticationProvider
    {
        private Sitecore.Sites.SiteContext _site;

        [UsedImplicitly]
        public CookieAuthenticationProvider(
            BaseCorePipelineManager corePipelineManager,
            IStorefrontContext storefrontContext,
            [NotNull] FederatedAuthenticationConfiguration federatedAuthenticationConfiguration) : base(corePipelineManager)
        {
            StorefrontContext = storefrontContext;
            FederatedAuthenticationConfiguration = federatedAuthenticationConfiguration;
        }

        protected FederatedAuthenticationConfiguration FederatedAuthenticationConfiguration { get; }

        internal Sitecore.Sites.SiteContext Site
        {
            get => _site ?? Context.Site;
            set => _site = value;
        }

        public IStorefrontContext StorefrontContext { get; set; }

        public override void ResponseSignIn([NotNull] CookieResponseSignInContext context)
        {
            Assert.ArgumentNotNull(context, nameof(context));

            var arg = new CommerceSignInArgs(context, StorefrontContext?.CurrentStorefront?.ShopName)
            {
                Site = Site,
                AuthenticationSource = AuthenticationSource
            };

            SignInPipeline.Run(CorePipelineManager, arg);

            if (!arg.Success)
            {
                throw new AbortSignInException();
            }

            this.OnResponseSignIn(context);
        }
    }
}