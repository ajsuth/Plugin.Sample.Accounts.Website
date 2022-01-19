using Microsoft.Owin.Security.Cookies;
using Sitecore.Annotations;
using Sitecore.Commerce;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Providers;
using Sitecore.Commerce.Services;
using Sitecore.Commerce.Services.Customers;
using Sitecore.Configuration;
using Sitecore.Owin.Authentication.Pipelines.CookieAuthentication.SignIn;

namespace Plugin.Sample.Accounts.Website.Arguments
{
    public class CommerceSignInArgs : SignInArgs
    {
        public CommerceSignInArgs([NotNull] CookieResponseSignInContext context, [NotNull] string shopName) : base(context)
        {
            CreateCommerceCustomer = Sitecore.Context.Site.Name == "Storefront" && context?.Identity?.Name.ToLower() != "sitecore\\admin"; ;

            if (!CreateCommerceCustomer)
            {
                return;
            }

            IShopProvider shopProvider = (IShopProvider)Factory.CreateObject(Constants.KnownServiceProviderNames.ShopProvider, true);
            Shop = shopProvider.GetShop(shopName);

            IAuthenticationTokenProvider tokenProvider = (IAuthenticationTokenProvider)Factory.CreateObject(Constants.KnownServiceProviderNames.AuthenticationTokenProvider, true);
            AuthenticationToken = tokenProvider.GetToken(Shop);

            IRequestCurrencyProvider currencyCodeProvider = (IRequestCurrencyProvider)Factory.CreateObject(Constants.KnownServiceProviderNames.RequestCurrencyCodeProvider, true);
            CurrencyCode = currencyCodeProvider.GetCurrencyCode(Shop);

            Properties = new PropertyCollection();

            Result = new CreateUserResult();
        }

        public bool CreateCommerceCustomer { get; set; }

        public Shop Shop { get; set; }

        public AuthenticationToken AuthenticationToken { get; set; }

        public string CurrencyCode { get; set; }

        public PropertyCollection Properties { get; set; }

        public ServiceProviderResult Result { get; set; }
    }
}