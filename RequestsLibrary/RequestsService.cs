using RequestsLibrary.Utils;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RequestsLibrary
{
    public class RequestsService 
    {
        private static CookieContainer _cookies;
        private static DateTime _lastSessionStarted;
        private const int AuthSessionLimit = 15;

        public static async Task<T> PostAsync<T>(string url, string date) where T : class
        {
            RestClient client;

            try
            {
                client = new RestClient(url);
            }
            catch (Exception)
            {
                throw;
            }

            var request = new RestRequest(Method.POST);
            request.AddParameter(new Parameter
            {
                Name = "text/json",
                Value = date,
                Type = ParameterType.RequestBody
            });

            var responce = await client.ExecuteTaskAsync(request);

            if (responce.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return responce.Content.DeserializeTo<T>();
            }

            throw new Exception(responce.StatusCode.ToString() + responce.StatusDescription);

            //throw new HttpResponceException(responce.StatusCode.ToString(), responce.StatusDescription);
        }

        public static async Task<T> PostAsync<T>(string url, string date, string bearer) where T : class
        {
            RestClient client;

            if (!bearer.Contains("Bearer"))
            {
                bearer = "Bearer " + bearer;
            }

            try
            {
                client = new RestClient(url);
            }
            catch (Exception)
            {
                throw;
            }

            var request = new RestRequest(Method.POST);
            request.AddParameter(new Parameter
            {
                Name = "text/json",
                Value = date,
                Type = ParameterType.RequestBody
            });

            request.AddHeader(
                "Authorization", bearer);

            var responce = await client.ExecuteTaskAsync(request);

            if (responce.StatusCode == System.Net.HttpStatusCode.OK || responce.StatusCode == System.Net.HttpStatusCode.Unauthorized) 
            {
                return responce.Content.DeserializeTo<T>();
            }

            throw new Exception(responce.StatusCode.ToString() + responce.StatusDescription);
        }

        public static async Task<T> PostAsync<T>(string url, string date, CookieContainer cookies) where T : class
        {
            RestClient client;

            try
            {
                client = new RestClient(url)
                {
                    CookieContainer = cookies
                };
            }
            catch (Exception)
            {
                throw;
            }

            var request = new RestRequest(Method.POST);
            request.AddParameter(new Parameter
            {
                Name = "text/json",
                Value = date,
                Type = ParameterType.RequestBody
            });

            var responce = await client.ExecuteTaskAsync(request);

            if (responce.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return responce.Content.DeserializeTo<T>();
            }

            throw new Exception(responce.StatusCode.ToString() + responce.StatusDescription);
        }

        public static async Task<T> PostAsync<T>(string url, string date, NetworkCredential credential) where T : class
        {
            return await PostAsync<T>(url, date, GetCookies(credential));
        }

        public static async Task<T> PostAsync<T>(string url, string date, string authUrl, string login, string hash) where T : class
        {
            return await PostAsync<T>(url, date, GetCookies(authUrl, login, hash));
        }

        

        public static async Task<T> GetAsync<T>(string url) where T : class
        {
            RestClient client;

            try
            {
                client = new RestClient(url);
            }
            catch (Exception)
            {
                throw;
            }

            var request = new RestRequest(Method.GET);

            var responce = await client.ExecuteTaskAsync(request);

            if (responce.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return responce.Content.DeserializeTo<T>();
            }

            throw new Exception(responce.StatusCode.ToString() + responce.StatusDescription);

            //throw new HttpResponceException(responce.StatusCode.ToString(), responce.StatusDescription);
        }

        public static async Task<T> GetAsync<T>(string url, string bearer) where T : class
        {
            RestClient client;

            if (!bearer.Contains("Bearer"))
            {
                bearer = "Bearer " + bearer;
            }

            try
            {
                client = new RestClient(url);
            }
            catch (Exception)
            {
                throw;
            }

            var request = new RestRequest(Method.GET);

            request.AddHeader(
                "Authorization", bearer);

            var responce = await client.ExecuteTaskAsync(request);

            if (responce.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return responce.Content.DeserializeTo<T>();
            }

            throw new Exception(responce.StatusCode.ToString() + responce.StatusDescription);
        }


        public static async Task<T> GetAsync<T>(string url, CookieContainer cookies) where T : class
        {
            RestClient client;

            try
            {
                client = new RestClient(url)
                {
                    CookieContainer = cookies
                };
            }
            catch (Exception)
            {
                throw;
            }

            var request = new RestRequest(Method.GET);

            var responce = await client.ExecuteTaskAsync(request);

            if (responce.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return responce.Content.DeserializeTo<T>();
            }

            throw new Exception(responce.StatusCode.ToString() + responce.StatusDescription);

            
        }

        public static async Task<T> GetAsync<T>(string url, NetworkCredential credential) where T : class
        {
            return await GetAsync<T>(url,GetCookies(credential));
        }

        public static async Task<T> GetAsync<T>(string url, string authUrl, string login, string hash) where T : class
        {
            return await GetAsync<T>(url, GetCookies(authUrl, login, hash));
        }

        #region PrivateRegion 
        private static CookieContainer GetCookies(string authUrl, string login, string hash)
        {
            return GetCookies(new NetworkCredential(login, hash, authUrl));
        }

        private static CookieContainer GetCookies(NetworkCredential credential)
        {
            var sesionLimitIsExceeded = DateTime.Now - new TimeSpan(0, 0, AuthSessionLimit, 0) > _lastSessionStarted;

            if(_cookies == null || sesionLimitIsExceeded)
            {
                var request = new RestRequest(Method.POST);
                request.AddParameter("USER_LOGIN", credential.UserName);
                request.AddParameter("USER_HASH", credential.Password);

                var responce = new RestClient(credential.Domain).Execute(request);

                var newCookieConteiner = new CookieContainer();

                foreach(var cookie in responce.Cookies)
                {
                    newCookieConteiner.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
                }

                _lastSessionStarted = DateTime.Now;
                _cookies = newCookieConteiner;
            }

            return _cookies;
        }



        #endregion

    }
}
