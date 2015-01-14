using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

namespace 博客园登录
{
    public class HttpService
    {
        private static HttpService _httpService;
        public static HttpService Current
        {
            get 
            {
                if (_httpService == null)
                {
                    _httpService = new HttpService();
                }                    
                return _httpService;
            }
        }
        
        private HttpService()
        {

        }

        private HttpCookie loginCookie;
        public HttpCookie LoginCookie
        {
            get { return loginCookie; }
            set { loginCookie = value; }
        }

        private List<HttpCookie> cookieList;
        public List<HttpCookie> CookieList
        {
            get { return cookieList; }
            set { cookieList = value; }
        }

        public string message = null;

        

        public async Task<string> LoginPageGet()
        {
            string loginUrl = "http://passport.cnblogs.com/login.aspx";
            string responseString = null;

            HttpRequestMessage request;
            HttpResponseMessage response;
            HttpResponseHeaderCollection responseHeaders;
            HttpBaseProtocolFilter filter;
            HttpClient httpClient;
            try
            {
                filter = new HttpBaseProtocolFilter();
                httpClient = new HttpClient(filter);
                httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko");
                request = new HttpRequestMessage(HttpMethod.Get, new Uri(loginUrl));
                response = await filter.SendRequestAsync(request);
                responseString = await response.Content.ReadAsStringAsync();
                responseHeaders = response.Headers;
                httpClient.Dispose();
                filter.Dispose();
                request.Dispose();
                response.Dispose();
            }
            catch(Exception ex)
            {
                message = ex.Message.ToString();
            }
            return responseString;
        }

        public async Task<MemoryStream> DownloadImage(string result)
        {
            string imageUrl = "http://passport.cnblogs.com" + new Regex("id=\"c_login_logincaptcha_CaptchaImage\" src=\"(.*?)\"").Match(result).Groups[1].Value;
            HttpClient httpClient;
            HttpRequestMessage request;
            HttpResponseMessage response;
            Stream responseStream;
            MemoryStream ImageStreamForUI = null;
            try
            {
                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko");
                request = new HttpRequestMessage(HttpMethod.Get, new Uri(imageUrl));
                response = await httpClient.SendRequestAsync(request);
                int length = (int)response.Content.Headers.ContentLength;
                responseStream = (await response.Content.ReadAsInputStreamAsync()).AsStreamForRead();
                ImageStreamForUI = new MemoryStream(length);
                int read1 = 0;
                byte[] responseBytes = new byte[length];
                do
                {
                    read1 = await responseStream.ReadAsync(responseBytes, 0, responseBytes.Length);
                } while (read1 != 0);
                ImageStreamForUI.Write(responseBytes, 0, length);
                httpClient.Dispose();
                request.Dispose();
                response.Dispose();
            }
            catch(Exception ex)
            {
                message = ex.Message.ToString();
            }
            ImageStreamForUI.Seek(0, SeekOrigin.Begin);
            return ImageStreamForUI;
        }

        public async Task<string> LoginBlog(string UserName, string UserPassword, string CheckCode,string result)
        {
            string loginUrl = "http://passport.cnblogs.com/login.aspx";
            string responseString = null;

            HttpRequestMessage request;
            HttpResponseMessage response;
            HttpBaseProtocolFilter filter;
            HttpClient httpClient;
            List<HttpCookie> list = new List<HttpCookie>();

            try
            {
                filter = new HttpBaseProtocolFilter();
                httpClient = new HttpClient(filter);
                httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko");
                request = new HttpRequestMessage(HttpMethod.Post, new Uri(loginUrl));

                //string __VIEWSTATE = "/wEPDwUKLTM1MjEzOTU2MGQYAQUeX19Db250cm9sc1JlcXVpcmVQb3N0QmFja0tleV9fFgEFC2Noa1JlbWVtYmVy4b/ZXiH+8FthXlmKpjSEgi7XBNU=";
                //string __EVENTVALIDATION = "/wEdAAYIqCk3Gcmu25zI9fQWqoC7hI6Xi65hwcQ8/QoQCF8JIahXufbhIqPmwKf992GTkd2Mxo6xcg+Ng5CZxsqMUGnVMKtTyqevv9cjRp4Oh+9VMaKeKEbp39eHc9mbdvkCgxCM74oSoIAJofLsQdCCbtmog/0fDw==";
                string LBD_VCID_c_login_logincaptcha = new Regex("id=\"LBD_VCID_c_login_logincaptcha\" value=\"(.*?)\"").Match(result).Groups[1].Value;
                string __EVENTVALIDATION = new Regex("id=\"__EVENTVALIDATION\" value=\"(.*?)\"").Match(result).Groups[1].Value;
                string __VIEWSTATE = new Regex("id=\"__VIEWSTATE\" value=\"(.*?)\"").Match(result).Groups[1].Value;
                //HTML表单数据
                HttpFormUrlEncodedContent postData = new HttpFormUrlEncodedContent(
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("__EVENTARGUMENT",""),
                        new KeyValuePair<string, string>("__EVENTTARGET",""),
                        new KeyValuePair<string, string>("__EVENTVALIDATION",__EVENTVALIDATION),
                        new KeyValuePair<string, string>("__VIEWSTATE",__VIEWSTATE),   
                        new KeyValuePair<string, string>("__VIEWSTATEGENERATOR","C2EE9ABB"),                                                                   
                        new KeyValuePair<string, string>("tbUserName",UserName),
                        new KeyValuePair<string, string>("tbPassword", UserPassword),
                        new KeyValuePair<string, string>("LBD_VCID_c_login_logincaptcha",LBD_VCID_c_login_logincaptcha),
                        new KeyValuePair<string, string>("LBD_BackWorkaround_c_login_logincaptcha","0"),
                        new KeyValuePair<string, string>("CaptchaCodeTextBox",CheckCode),
                        //如果需要以后自动登录，也就是此次得到的cookie永久有效，那么就可以下面的键值对也POST给服务器
                        //new KeyValuePair<string, string>("chkRemember", "on"),
                        new KeyValuePair<string, string>("btnLogin","登  录"),
                        new KeyValuePair<string, string>("txtReturnUrl", "http://home.cnblogs.com/")
                        
                    }
                );
                request.Content = postData;

                response = await filter.SendRequestAsync(request);
                responseString = await response.Content.ReadAsStringAsync();             
                //获取cookie
                HttpCookie CurrentCookie;
                HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(new Uri(loginUrl));
                foreach (HttpCookie cookie in cookieCollection)
                {
                    if (cookie.Name == "SERVERID")
                    {
                        CurrentCookie = cookie;
                        list.Add(CurrentCookie);
                    }
                    else if (cookie.Name == ".DottextCookie")
                    {
                        CurrentCookie = cookie;
                        this.LoginCookie = CurrentCookie;
                        list.Add(CurrentCookie);
                    }
                    else if (cookie.Name == "ASP.NET_SessionId")
                    {
                        CurrentCookie = cookie;
                        list.Add(CurrentCookie);
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message.ToString();
            }
            this.CookieList = list;
            return responseString;
        }

        public async Task<string> MsgGet()
        {
            string messageUrl = "http://msg.cnblogs.com/msg/inbox";
            string responseString = null;
            HttpRequestMessage request;
            HttpResponseMessage response;
            HttpBaseProtocolFilter filter;
            HttpClient httpClient;
            List<HttpCookie> list = new List<HttpCookie>();

            try
            {
                filter = new HttpBaseProtocolFilter();
                httpClient = new HttpClient(filter);
                filter.CookieManager.SetCookie(LoginCookie);
                request = new HttpRequestMessage(HttpMethod.Get, new Uri(messageUrl));

                response = await filter.SendRequestAsync(request);
                responseString = await response.Content.ReadAsStringAsync();
                HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(new Uri(messageUrl));
                foreach(var cookie in cookieCollection)
                {
                    list.Add(cookie);
                }
            }
            catch(Exception ex)
            {
                message = ex.Message.ToString();
            }
            return responseString;
        }
    }
}
