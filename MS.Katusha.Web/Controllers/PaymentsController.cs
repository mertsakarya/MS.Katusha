using System;
using System.Configuration;
using System.Web.Mvc;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Attributes;
using MS.Katusha.Infrastructure.Exceptions.Services;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Web.Controllers
{
    [KatushaFilter(ExceptionView = "KatushaException", IsAuthenticated = true, MustHaveGender = true, MustHaveProfile = true)]
    public class PaymentsController : KatushaController
    {
        private readonly IPaypalService _paypalService;
        private readonly IProductService _productService;

        public PaymentsController(IPaypalService paypalService, IProductService productService, IResourceService resourceService, IUserService userService, IProfileService profileService, IStateService stateService, IConversationService conversationService, ITokBoxService tokBoxService)
            : base(resourceService, userService, profileService, stateService, conversationService, tokBoxService)
        {
            _paypalService = paypalService;
            _productService = productService;
        }

        public ActionResult PaypalCancel()
        {
            return View("KatushaException", new HandleErrorInfo(new Exception("WHYYYYY???"), "PaypayCancel", "Payments"));
        }

        public ActionResult Needed(string key)
        {
            ViewBag.HasLayout = false;
            ProductNames productName;
            if (!Enum.TryParse(key, true, out productName))
                throw new KatushaProductNameNotFoundException(key);
            return View("NeedsPayment", productName);
        }

        public ActionResult PaypalSetCheckout(string key)
        {
            try {
                ProductNames productName;
                if(!Enum.TryParse(key, true, out productName))
                    throw new KatushaProductNameNotFoundException(key);
                var referrer = (Request.UrlReferrer != null) ? Request.UrlReferrer.ToString() : "";
                var product = _productService.GetProductByName(productName);
                var token = _paypalService.SetExpressCheckout(KatushaUser, product, referrer, 1, "Billing message");
                if (token == null) throw new Exception("TOKEN NULL");
                var paypalPaymentTest = ConfigurationManager.AppSettings["PaypalSandbox"] == "true";
                return Redirect("https://www."+((paypalPaymentTest) ? "sandbox." :"")+"paypal.com/webscr?cmd=_express-checkout&token=" + token);
            } catch (KatushaProductNameNotFoundException pex) {
                return View("KatushaException", new HandleErrorInfo(pex, "PaypalSetCheckout", "Payments"));
            }
        }

        public ActionResult PaypalGetCheckout(string token, string PayerID)
        {
            var result = _paypalService.GetExpressCheckoutDetails(KatushaUser, token);
            result.PayerId = PayerID;
            result.User = KatushaUser;
            result.Token = token;
            return View(result);
        }

        public ActionResult PaypalDoCheckout(string token, string payerId)
        {
            var result = _paypalService.DoExpressCheckoutPayment(KatushaUser, token, payerId);
            if(result.Errors.Count > 0 || String.IsNullOrWhiteSpace(result.Referrer))
                return View(result);
            return new RedirectResult(result.Referrer);
        }

    }
}
