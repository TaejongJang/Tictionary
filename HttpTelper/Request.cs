using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using HtmlAgilityPack;

namespace HttpTelper {
    public static class Request {
        public class WebParameter {
            public string KEY { get; set; }
            public string VALUE { get; set; }
            public WebParameter(string Key, string Value) {
                this.KEY = Key;
                this.VALUE = Value;
            }
        }
        public enum Method {
            GET = 1,
            HEAD = 2,
            POST = 3,
            PUT = 4,
            DELETE = 5,
            CONNECT = 6,
            OPTIONS = 7,
            TRACE = 8,
            PATCH = 9
        }
        public static bool TryAddCookie(this WebRequest WebRequest, Cookie Cookie) {
            HttpWebRequest httpRequest = WebRequest as HttpWebRequest;
            if (httpRequest == null)
            {
                return false;
            }

            if (httpRequest.CookieContainer == null)
            {
                httpRequest.CookieContainer = new CookieContainer();
            }

            httpRequest.CookieContainer.Add(Cookie);
            return true;
        }
        public static bool TryAddCookie(this WebRequest WebRequest, string Name, string Value) {
            HttpWebRequest httpRequest = WebRequest as HttpWebRequest;
            if (httpRequest == null) return false;
            if (httpRequest.CookieContainer == null) httpRequest.CookieContainer = new CookieContainer();

            httpRequest.CookieContainer.Add(new Cookie(Name, Value));
            return true;
        }
        public static string ResponseToString(string Url) {
            using (var webClient = new WebClient())
            {
                var uri = new Uri(Url);
                var webRequest = WebRequest.Create(uri);
                var response = webRequest.GetResponse();
                var receiveStream = response.GetResponseStream();
                var readStream = new System.IO.StreamReader(receiveStream);
                var htmlCode = readStream.ReadToEnd();
                return htmlCode;
            }
        }
        public static string ResponseToString(string Url, Encoding Encoding) {
            using (var webClient = new WebClient())
            {
                var uri = new Uri(Url);
                var webRequest = WebRequest.Create(uri);
                var response = webRequest.GetResponse();
                var receiveStream = response.GetResponseStream();
                var readStream = new System.IO.StreamReader(receiveStream, Encoding);
                var htmlCode = readStream.ReadToEnd();
                return htmlCode;
            }
        }
        public static string ResponseToString(string Url, params Cookie[] Cookies) {
            using (var webClient = new WebClient())
            {
                var uri = new Uri(Url);
                var webRequest = WebRequest.Create(uri);
                foreach (var nameValue in Cookies)
                {
                    webRequest.TryAddCookie(new Cookie(nameValue.Name, nameValue.Value, "/", uri.Host));
                }
                var response = webRequest.GetResponse();
                var receiveStream = response.GetResponseStream();
                var readStream = new System.IO.StreamReader(receiveStream);
                var htmlCode = readStream.ReadToEnd();
                return htmlCode;
            }
        }
        public static string ResponseToString(string Url, Encoding Encoding, params Cookie[] Cookies) {
            using (var webClient = new WebClient())
            {
                var uri = new Uri(Url);
                var webRequest = WebRequest.Create(uri);
                foreach (var nameValue in Cookies)
                {
                    webRequest.TryAddCookie(new Cookie(nameValue.Name, nameValue.Value, "/", uri.Host));
                }
                var response = webRequest.GetResponse();
                var receiveStream = response.GetResponseStream();
                var readStream = new System.IO.StreamReader(receiveStream, Encoding);
                var htmlCode = readStream.ReadToEnd();
                return htmlCode;
            }
        }
        public static string ResponseToString(string Url, IDictionary<string, string> CookiesNameValues) {
            using (var webClient = new WebClient())
            {
                var uri = new Uri(Url);
                var webRequest = WebRequest.Create(uri);
                foreach (var nameValue in CookiesNameValues)
                {
                    webRequest.TryAddCookie(new Cookie(nameValue.Key, nameValue.Value, "/", uri.Host));
                }
                var response = webRequest.GetResponse();
                var receiveStream = response.GetResponseStream();
                var readStream = new System.IO.StreamReader(receiveStream);
                var htmlCode = readStream.ReadToEnd();
                return htmlCode;
            }
        }
        public static string ResponseToString(string Url, Encoding Encoding, IDictionary<string, string> CookiesNameValues) {
            using (var webClient = new WebClient())
            {
                var uri = new Uri(Url);
                var webRequest = WebRequest.Create(uri);
                foreach (var nameValue in CookiesNameValues)
                {
                    webRequest.TryAddCookie(new Cookie(nameValue.Key, nameValue.Value, "/", uri.Host));
                }
                var response = webRequest.GetResponse();
                var receiveStream = response.GetResponseStream();
                var readStream = new System.IO.StreamReader(receiveStream, Encoding);
                var htmlCode = readStream.ReadToEnd();
                return htmlCode;
            }
        }
        public static string ResponseToString(Method Method, string Url, Encoding Encoding, params WebParameter[] Params) {
            using (var webClient = new WebClient())
            {
                var uri = new Uri(Url);
                var webRequest = WebRequest.Create(uri);
                webRequest.Method = Method.GetName(Method.GetType(), Method);
                webRequest.ContentType = "application/x-www-form-urlencoded";
                var parambytes = Encoding.ASCII.GetBytes(string.Join("&", Params.Select(x => x.KEY + "=" + x.VALUE)));
                webRequest.ContentLength = parambytes.Length;
                using (var stream = webRequest.GetRequestStream())
                    stream.Write(parambytes, 0, parambytes.Length);

                var response = webRequest.GetResponse();
                var receiveStream = response.GetResponseStream();
                var readStream = new System.IO.StreamReader(receiveStream, Encoding);
                var htmlCode = readStream.ReadToEnd();
                return htmlCode;
            }
        }
        public static string ResponseToString(Method Method, string Url, Encoding Encoding, Cookie[] Cookies, params WebParameter[] Params) {
            using (var webClient = new WebClient())
            {
                var uri = new Uri(Url);
                var webRequest = WebRequest.Create(uri);
                Cookies.ToList().ForEach(x => webRequest.TryAddCookie(x));
                webRequest.Method = Method.ToString();
                webRequest.ContentType = "application/x-www-form-urlencoded";
                var parambytes = Encoding.ASCII.GetBytes(string.Join("&", Params.Select(x => x.KEY + "=" + x.VALUE)));
                webRequest.ContentLength = parambytes.Length;
                using (var stream = webRequest.GetRequestStream())
                    stream.Write(parambytes, 0, parambytes.Length);
                
                var response = webRequest.GetResponse();
                var receiveStream = response.GetResponseStream();
                var readStream = new System.IO.StreamReader(receiveStream, Encoding);
                var htmlCode = readStream.ReadToEnd();
                return htmlCode;
            }
        }
        public static string ResponseToString(Method Method, string Url, Encoding Encoding,
            IDictionary<string, string> CookiesNameValues, params WebParameter[] Params) {
            using (var webClient = new WebClient())
            {
                var uri = new Uri(Url);
                var webRequest = WebRequest.Create(uri);
                CookiesNameValues.ToList().ForEach(x => webRequest.TryAddCookie(new Cookie(x.Key, x.Value, "/", uri.Host)));
                webRequest.Method = Method.ToString();
                webRequest.ContentType = "application/x-www-form-urlencoded";
                var parambytes = Encoding.ASCII.GetBytes(string.Join("&", Params.Select(x => x.KEY + "=" + x.VALUE)));
                webRequest.ContentLength = parambytes.Length;
                using (var stream = webRequest.GetRequestStream())
                    stream.Write(parambytes, 0, parambytes.Length);

                var response = webRequest.GetResponse();
                var receiveStream = response.GetResponseStream();
                var readStream = new System.IO.StreamReader(receiveStream, Encoding);
                var htmlCode = readStream.ReadToEnd();
                return htmlCode;
            }
        }
        public static string ResponseToString(Method Method, string Url, Encoding Encoding, IDictionary<string, string> CookiesNameValues, IDictionary<string, string> ParamKeyValues) {
            using (var webClient = new WebClient())
            {
                var uri = new Uri(Url);
                var webRequest = WebRequest.Create(uri);
                CookiesNameValues.Cast<KeyValuePair<string, string>>().ToList().ForEach(x => webRequest.TryAddCookie(new Cookie(x.Key, x.Value, "/", uri.Host)));
                webRequest.Method = Method.ToString();
                webRequest.ContentType = "application/x-www-form-urlencoded";
                var parambytes = Encoding.ASCII.GetBytes(string.Join("&", ParamKeyValues.ToList().Select(x => x.Key + "=" + x.Value)));
                webRequest.ContentLength = parambytes.Length;
                using (var stream = webRequest.GetRequestStream())
                    stream.Write(parambytes, 0, parambytes.Length);

                var response = webRequest.GetResponse();
                var receiveStream = response.GetResponseStream();
                var readStream = new System.IO.StreamReader(receiveStream, Encoding);
                var htmlCode = readStream.ReadToEnd();
                return htmlCode;
            }
        }
        public static HtmlDocument ResponseToDocument(string Url) {
            HtmlDocument Doc = new HtmlDocument();
            Doc.LoadHtml(ResponseToString(Url));
            return Doc;
        }
        public static HtmlDocument ResponseToDocument(string Url, Encoding Encoding) {
            HtmlDocument Doc = new HtmlDocument();
            Doc.LoadHtml(ResponseToString(Url, Encoding));
            return Doc;
        }
        public static HtmlDocument ResponseToDocument(string Url, params Cookie[] Cookies) {
            HtmlDocument Doc = new HtmlDocument();
            Doc.LoadHtml(ResponseToString(Url, Cookies));
            return Doc;
        }
        public static HtmlDocument ResponseToDocument(string Url, Encoding Encoding, params Cookie[] Cookies) {
            HtmlDocument Doc = new HtmlDocument();
            Doc.LoadHtml(ResponseToString(Url, Encoding, Cookies));
            return Doc;
        }
        public static HtmlDocument ResponseToDocument(string Url, IDictionary<string, string> CookiesNameValues) {
            HtmlDocument Doc = new HtmlDocument();
            Doc.LoadHtml(ResponseToString(Url, CookiesNameValues));
            return Doc;
        }
        public static HtmlDocument ResponseToDocument(string Url, Encoding Encoding, IDictionary<string, string> CookiesNameValues) {
            HtmlDocument Doc = new HtmlDocument();
            Doc.LoadHtml(ResponseToString(Url, Encoding, CookiesNameValues));
            return Doc;
        }
    }
}