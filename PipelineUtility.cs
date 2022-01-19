using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.Services;
using Sitecore.Diagnostics;
using System;

namespace Plugin.Sample.Accounts.Website
{
	public static class PipelineUtility
	{
		public const string CartLineItemIdDelimiter = "|";
		public const string SellableItemIdIdDelimiter = ",";
		public const string SellableItemsIdIdDelimiter = "|";
		public const string CustomersPrefix = "Entity-Customer-";

		public static SystemMessage CreateSystemMessage(Exception ex)
		{
			var systemMessage = new SystemMessage()
			{
				Message = ex.Message
			};
			if (ex.InnerException != null && !ex.Message.Equals(ex.InnerException.Message, StringComparison.OrdinalIgnoreCase))
			{
				systemMessage.Message = systemMessage.Message + " - " + ex.InnerException.Message;
			}
			return systemMessage;
		}

		public static SystemMessage CreateSystemMessage(string message)
		{
			return new SystemMessage() { Message = message };
		}

		public static void ValidateArguments<TRequest, TResult>(ServicePipelineArgs args, out TRequest request, out TResult result) where TRequest : ServiceProviderRequest where TResult : ServiceProviderResult
		{
			Assert.ArgumentNotNull(args, nameof(args));
			Assert.ArgumentNotNull(args.Request, "args.Request");
			Assert.ArgumentNotNull(args.Request.RequestContext, "args.Request.RequestContext");
			Assert.ArgumentNotNull(args.Result, "args.Result");
			request = args.Request as TRequest;
			result = args.Result as TResult;
			Assert.IsNotNull(request, "The parameter args.Request was not of the expected type.  Expected {0}.  Actual {1}.", new object[2]
			{ typeof (TRequest).Name, args.Request.GetType().Name });
			Assert.IsNotNull(result, "The parameter args.Result was not of the expected type.  Expected {0}.  Actual {1}.", new object[2]
			{ typeof (TResult).Name, args.Result.GetType().Name });
		}
	}
}