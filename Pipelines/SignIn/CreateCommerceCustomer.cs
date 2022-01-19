using System;
using System.Linq;
using Plugin.Sample.Accounts.Website.Arguments;
using Sitecore.Annotations;
using Sitecore.Commerce;
using Sitecore.Commerce.Engine.Connect.Pipelines;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Customers;
using Sitecore.Commerce.Plugin.Customers;
using Sitecore.Commerce.Services.Customers;
using Sitecore.Diagnostics;
using Sitecore.Owin.Authentication.Pipelines.CookieAuthentication.SignIn;

namespace Plugin.Sample.Accounts.Website.Pipelines.SignIn
{
    public class CreateCommerceCustomer : PipelineProcessor
    {
        public CreateCommerceCustomer([NotNull] IEntityFactory entityFactory)
        {
            EntityFactory = entityFactory;
        }

        /// <summary>
        /// Gets the entity factory.
        /// </summary>
        /// <value>
        /// The entity factory.
        /// </value>
        public IEntityFactory EntityFactory { get; private set; }

        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>      
        public virtual void Process(SignInArgs args)
        {
            var request = args as CommerceSignInArgs;

            if (!request.CreateCommerceCustomer)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(request.User?.InnerUser?.Profile?.Email))
            {
                return;
            }

            Assert.IsNotNull(request.User, "request.User");
            Assert.IsNotNull(request.Shop?.Name, "request.Shop.Name");
            var email = request.User.InnerUser.Profile.Email;

            var container = GetContainer(request.Shop.Name, string.Empty, currency: request.CurrencyCode);

            //Check if user already created 
            var result = request.Result as CreateUserResult;

            var customerId = string.Concat(PipelineUtility.CustomersPrefix, request.User.UserName);
            var detailsView = GetEntityView(container, customerId, string.Empty, "Details", string.Empty, result);
            if (result.Success && !string.IsNullOrEmpty(detailsView.EntityId))
            {
                var user = EntityFactory.Create<CommerceUser>("CommerceUser");
                user.Email = email;
                user.UserName = request.User.UserName;
                user.ExternalId = detailsView.EntityId;
                result.CommerceUser = user;

                request.Properties.Add(new PropertyItem
                {
                    Key = "UserId",
                    Value = result.CommerceUser.ExternalId
                });

                return;
            }

            var view = GetEntityView(container, string.Empty, string.Empty, "Details", "AddCustomer", result);
            if (!result.Success)
            {
                return;
            }

            view.Properties.FirstOrDefault(p => p.Name.Equals("Domain", StringComparison.OrdinalIgnoreCase)).Value = request.User.UserName.Split('\\')[0];
            view.Properties.FirstOrDefault(p => p.Name.Equals("LoginName", StringComparison.OrdinalIgnoreCase)).Value = request.User.UserName.Split('\\')[1];
            view.Properties.FirstOrDefault(p => p.Name.Equals("AccountStatus", StringComparison.OrdinalIgnoreCase)).Value = "ActiveAccount";

            if (!string.IsNullOrEmpty(email))
            {
                view.Properties.FirstOrDefault(p => p.Name.Equals("Email", StringComparison.OrdinalIgnoreCase)).Value = email;
            }

            var command = DoAction(container, view, result);
            if (command != null && command.ResponseCode.Equals("ok", StringComparison.OrdinalIgnoreCase))
            {
                var user = EntityFactory.Create<CommerceUser>("CommerceUser");
                user.Email = email;
                user.UserName = request.User.UserName;
                user.ExternalId = command.Models.OfType<CustomerAdded>().FirstOrDefault().CustomerId;
                result.CommerceUser = user;

                request.Properties.Add(new PropertyItem
                {
                    Key = "UserId",
                    Value = result.CommerceUser.ExternalId
                });
            }
        }
    }
}