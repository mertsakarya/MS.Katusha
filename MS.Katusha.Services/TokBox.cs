//
// OpenTok .NET Library
// Last Updated November 16, 2011
// https://github.com/opentok/Opentok-.NET-SDK
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Configuration;
using System.Text;
using System.Net;
using System.Web;
using System.Security.Cryptography;

namespace MS.Katusha.Services
{
    public class TokBox
    {
        public string CreateSession(string location)
        {
            var options = new Dictionary<string, object>();

            return CreateSession(location, options);
        }

        public string CreateSession(string location, Dictionary<string, object> options)
        {
            var appSettings = ConfigurationManager.AppSettings;
            options.Add("location", location);
            options.Add("partner_id", appSettings["opentok_key"]);

            var xmlDoc = CreateSessionId(string.Format("{0}/session/create", appSettings["opentok_server"]), options);

            var sessionId = xmlDoc.GetElementsByTagName("session_id")[0].ChildNodes[0].Value;

            return sessionId;
        }

        public string GenerateToken(string sessionId)
        {
            var options = new Dictionary<string, object>();

            return GenerateToken(sessionId, options);
        }

        public string GenerateToken(string sessionId, Dictionary<string, object> options)
        {
            var appSettings = ConfigurationManager.AppSettings;

            options.Add("session_id", sessionId);
            options.Add("create_time", (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
            options.Add("nonce", RandomNumber(0, 999999));
            if (!options.ContainsKey(TokenPropertyConstants.Role)) {
                options.Add(TokenPropertyConstants.Role, "publisher");
            }
            // Convert expire time to Unix Timestamp
            if (options.ContainsKey(TokenPropertyConstants.ExpireTime)) {
                var origin = new DateTime(1970, 1, 1, 0, 0, 0);
                var expireTime = (DateTime) options[TokenPropertyConstants.ExpireTime];
                var diff = expireTime - origin;
                options[TokenPropertyConstants.ExpireTime] = Math.Floor(diff.TotalSeconds);
            }

            string dataString = options.Aggregate(string.Empty, (current, pair) => current + (pair.Key + "=" + HttpUtility.UrlEncode(pair.Value.ToString()) + "&"));
            dataString = dataString.TrimEnd('&');

            string sig = SignString(dataString, appSettings["opentok_secret"].Trim());
            string token = string.Format("{0}{1}", appSettings["opentok_token_sentinel"], EncodeTo64(string.Format("partner_id={0}&sdk_version={1}&sig={2}:{3}", appSettings["opentok_key"], appSettings["opentok_sdk_version"], sig, dataString)));

            return token;
        }

        private static string EncodeTo64(string data)
        {
            var encDataByte = Encoding.UTF8.GetBytes(data);
            var encodedData = Convert.ToBase64String(encDataByte);

            return encodedData;
        }

        private int RandomNumber(int min, int max)
        {
            var random = new Random();
            return random.Next(min, max);
        }

        private string SignString(string message, string key)
        {
            var encoding = new ASCIIEncoding();

            var keyByte = encoding.GetBytes(key);

            var hmacsha1 = new HMACSHA1(keyByte);

            byte[] messageBytes = encoding.GetBytes(message);
            byte[] hashmessage = hmacsha1.ComputeHash(messageBytes);

            //Make sure to utilize ToLower() method, else an exception willl be thrown
            //Exception: 1006::Connecting to server to fetch session info failed.
            string result = ByteToString(hashmessage).ToLower();

            return result;
        }

        private static string ByteToString(byte[] buff)
        {
            var sbinary = "";

            for (var i = 0; i < buff.Length; i++) {
                sbinary += buff[i].ToString("X2");
            }
            return (sbinary);
        }

        private XmlDocument CreateSessionId(string uri, Dictionary<string, object> dict)
        {
            var xmlDoc = new XmlDocument();
            var appSettings = ConfigurationManager.AppSettings;

            var postData = dict.Aggregate(string.Empty, (current, pair) => current + (pair.Key + "=" + HttpUtility.UrlEncode(pair.Value.ToString()) + "&"));
            postData = postData.Substring(0, postData.Length - 1);
            var postBytes = Encoding.UTF8.GetBytes(postData);

            var request = (HttpWebRequest) WebRequest.Create(uri);
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postBytes.Length;
            request.Headers.Add("X-TB-PARTNER-AUTH", string.Format("{0}:{1}", appSettings["opentok_key"], appSettings["opentok_secret"].Trim()));

            using (var requestStream = request.GetRequestStream()) {
                requestStream.Write(postBytes, 0, postBytes.Length);
            }

            using (var response = (HttpWebResponse) request.GetResponse()) {
                if(response.StatusCode == HttpStatusCode.OK) {
                    using (var reader = XmlReader.Create(response.GetResponseStream(), new XmlReaderSettings {CloseInput = true})) {
                        xmlDoc.Load(reader);
                    }
                }
            }

            return xmlDoc;
        }
    }

    public static class SessionPropertyConstants
    {
        public const string EchosupressionEnabled = "echoSuppression.enabled";
        public const string MultiplexerNumoutputstreams = "multiplexer.numOutputStreams";
        public const string MultiplexerSwitchtype = "multiplexer.switchType";
        public const string MultiplexerSwitchtimeout = "multiplexer.switchTimeout";
        public const string P2PPreference = "p2p.preference";
    }

    public static class TokenPropertyConstants
    {
        public const string Role = "role";
        public const string ExpireTime = "expire_time";
        public const string ConnectionData = "connection_data";
    }

    public static class RoleConstants
    {
        public const string Subscriber = "subscriber";
        public const string Publisher = "publisher";
        public const string Moderator = "moderator";    
    }
}
