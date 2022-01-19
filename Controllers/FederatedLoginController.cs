using Sitecore.Abstractions;
using Sitecore.Commerce.XA.Foundation.Common.Controllers;
using System.Linq;
using System.Web.Mvc;

namespace Plugin.Sample.Accounts.Website.Controllers
{
    public class FederatedLoginController : BaseCommerceStandardController
    {
        private readonly BaseCorePipelineManager _pipelineManager;
        public FederatedLoginController(BaseCorePipelineManager pipelineManager)
        {
            _pipelineManager = pipelineManager;
        }

        public ActionResult Login()
        {
            var args = new Sitecore.Pipelines.GetSignInUrlInfo.GetSignInUrlInfoArgs("Storefront", "/");
            Sitecore.Pipelines.GetSignInUrlInfo.GetSignInUrlInfoPipeline.Run(_pipelineManager, args);
            ViewBag.SignInUrl = args.Result.FirstOrDefault()?.Href;
            throw new System.Exception("my exception");
            return View();
        }
    }
}