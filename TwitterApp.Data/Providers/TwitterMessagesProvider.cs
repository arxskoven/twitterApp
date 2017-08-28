using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using TwitterApp.Common.Interfaces;

namespace TwitterApp.Data.Providers
{
    /// <summary>
    /// Twitter message provider.
    /// Only HttpClient from .Net framework used, without implementing thir party libraries.
    /// </summary>
    public class TwitterMessagesProvider : IMessagesProvider
    {
        private string _customerKey;
        private string _customerSecret;
        private string _accessToken;
        private string _accessTokenSecret;
        
        /// <summary>
        /// HMACSHA signature, necessary for twitter update status method.
        /// </summary>
        readonly HMACSHA1 sigHasher;

        /// <summary>
        /// Default EpochUtc DateTime
        /// </summary>
        readonly DateTime epochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="customerKey">Customer Key from twitter</param>
        /// <param name="customerSecret">Customer Secret Key from twitter</param>
        /// <param name="accessToken">Access Token from twitter</param>
        /// <param name="accessTokenSecret">Access Token Secret from twitter</param>
        public TwitterMessagesProvider(string customerKey, string customerSecret, string accessToken, string accessTokenSecret)
        {
            _customerKey = customerKey;
            _customerSecret = customerSecret;
            _accessToken = accessToken;
            _accessTokenSecret = accessTokenSecret;
            sigHasher = new HMACSHA1(new ASCIIEncoding().GetBytes(string.Format("{0}&{1}", customerSecret, accessTokenSecret)));
        }

        /// <summary>
        /// Get Messages method
        /// </summary>
        /// <param name="userName">Username for twitter, place id or user</param>
        /// <param name="count">Top messages to select</param>
        /// <param name="hashTag">Filter results containing the hashTag, If null or empty, it will return every result.</param>
        /// <returns>List of twitter (string only)</returns>
        public async Task<IList<string>> GetMessages(string userName, int count, string hashTag)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            if (count <= 0)
            {
                throw new ArgumentException(nameof(count));
            }

            var accessToken = await GetAccessToken();

            var requestUserTimeline = new HttpRequestMessage(HttpMethod.Get, $"https://api.twitter.com/1.1/statuses/user_timeline.json?count={count}&screen_name={userName}&trim_user=1&exclude_replies=1");
            requestUserTimeline.Headers.Add("Authorization", "Bearer " + accessToken);
            using (var httpClient = new HttpClient())
            {
                var responseUserTimeLine = await httpClient.SendAsync(requestUserTimeline);
                var serializer = new JavaScriptSerializer();
                dynamic json = serializer.Deserialize<object>(await responseUserTimeLine.Content.ReadAsStringAsync());
                var enumerableTweets = (json as IEnumerable<dynamic>);

                if (enumerableTweets == null)
                {
                    return null;
                }

                var result = enumerableTweets.Select(t => (string)(t["text"].ToString())).ToList();
                if (!string.IsNullOrWhiteSpace(hashTag))
                {
                    result = result.Where(r => r.IndexOf(hashTag, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList();
                }

                return result;
            }
        }

        /// <summary>
        /// Send new message to twitter
        /// </summary>
        /// <param name="message">Tweet to publish</param>
        /// <returns>True if succeed</returns>
        public async Task<bool> SendNewMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            var data = new Dictionary<string, string> {
                { "status", message },
                { "trim_user", "1" }
            };            

            var fullUrl = "https://api.twitter.com/1.1/statuses/update.json";

            // Timestamps are in seconds since 1/1/1970.
            var timestamp = (int)((DateTime.UtcNow - epochUtc).TotalSeconds);

            // Add all the OAuth headers we'll need to use when constructing the hash.
            data.Add("oauth_consumer_key", _customerKey);
            data.Add("oauth_signature_method", "HMAC-SHA1");
            data.Add("oauth_timestamp", timestamp.ToString());
            data.Add("oauth_nonce", "a"); // Required, but Twitter doesn't appear to use it, so "a" will do.
            data.Add("oauth_token", _accessToken);
            data.Add("oauth_version", "1.0");

            // Generate the OAuth signature and add it to our payload.
            data.Add("oauth_signature", GenerateSignature(fullUrl, data));

            // Build the OAuth HTTP Header from the data.
            string oAuthHeader = GenerateOAuthHeader(data);

            // Build the form data (exclude OAuth stuff that's already in the header).
            var formData = new FormUrlEncodedContent(data.Where(kvp => !kvp.Key.StartsWith("oauth_")));

            var t = await SendRequest(fullUrl, oAuthHeader, formData);
            return true;
        }

        /// <summary>
        /// Default send request method
        /// </summary>
        /// <param name="fullUrl">Url to hit by POST</param>
        /// <param name="oAuthHeader">Auth header string</param>
        /// <param name="formData">Content to publish</param>
        /// <returns>Response</returns>
        private async Task<string> SendRequest(string fullUrl, string oAuthHeader, FormUrlEncodedContent formData)
        {
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("Authorization", oAuthHeader);

                var httpResp = await http.PostAsync(fullUrl, formData);
                var respBody = await httpResp.Content.ReadAsStringAsync();

                return respBody;
            }
        }

        /// <summary>
        /// Generates auth header string
        /// </summary>
        /// <param name="data">Header content</param>
        /// <returns>header string</returns>
        private string GenerateOAuthHeader(Dictionary<string, string> data)
        {
            return "OAuth " + string.Join(
                ", ",
                data
                    .Where(kvp => kvp.Key.StartsWith("oauth_"))
                    .Select(kvp => string.Format("{0}=\"{1}\"", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value)))
                    .OrderBy(s => s)
            );
        }

        /// <summary>
        /// Generates signature string
        /// </summary>
        /// <param name="url">Url to sign</param>
        /// <param name="data">Content</param>
        /// <returns>Signed string</returns>
        private string GenerateSignature(string url, Dictionary<string, string> data)
        {
            var sigString = string.Join(
                "&",
                data
                    .Union(data)
                    .Select(kvp => string.Format("{0}={1}", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value)))
                    .OrderBy(s => s)
            );

            var fullSigData = string.Format(
                "{0}&{1}&{2}",
                "POST",
                Uri.EscapeDataString(url),
                Uri.EscapeDataString(sigString.ToString())
            );

            return Convert.ToBase64String(sigHasher.ComputeHash(new ASCIIEncoding().GetBytes(fullSigData.ToString())));
        }

        /// <summary>
        /// Gets twitter access token.
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetAccessToken()
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/oauth2/token ");
                var customerInfo = Convert.ToBase64String(new UTF8Encoding()
                                          .GetBytes(_customerKey + ":" + _customerSecret));
                request.Headers.Add("Authorization", "Basic " + customerInfo);
                request.Content = new StringContent("grant_type=client_credentials",
                                                        Encoding.UTF8, "application/x-www-form-urlencoded");

                var response = await httpClient.SendAsync(request);

                var json = await response.Content.ReadAsStringAsync();
                var serializer = new JavaScriptSerializer();
                dynamic item = serializer.Deserialize<object>(json);
                return item["access_token"];
            }
        }
    }
}
