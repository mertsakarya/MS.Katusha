using System;
using System.Net;

namespace MS.Payment.SpreedlyCore.Manager
{
    internal class CoreApiWebClient : WebClient
    {
        Uri _responseUri;

        public Uri ResponseUri
        {
            get { return _responseUri; }
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            try {
                var response = base.GetWebResponse(request);
                _responseUri = response.ResponseUri;
                return response;
            } catch(WebException ex) {
                _responseUri = ex.Response.ResponseUri;
                return null;
            }
        }
    }
}