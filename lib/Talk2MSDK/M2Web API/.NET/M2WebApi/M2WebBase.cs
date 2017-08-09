//
// Sample M2Web Library source code. Delivered as M2Web API sample code. Not supported by eWON s.a.
//
// Disclaimer: 
//    Delivered as is. eWON s.a. takes no responsibility for anything bad that can result from the use of this code.
//    Actual mileage may vary. Price does not include tax, title, and license. Some assembly required. Each sold 
//    separately. Batteries not included. Objects in mirror are closer than they appear. If conditions persist, 
//    contact a physician. Keep out of reach of children. Avoid prolonged exposure to direct sunlight. Keep in a cool 
//    dark place.
//    You've been warned!
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;

namespace M2WebLibrary
{
    public class M2WebException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public M2WebException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            this.StatusCode = statusCode;
        }

        public override string ToString()
        {
            return string.Format("Error {0}: {1}", StatusCode, Message);
        }
    }

    public abstract class M2WebBase
    {
        public IWebProxy Proxy { get; set; } // 1.0.3 #6330

        protected class HttpResponse
        {
            public HttpStatusCode Status;
            public byte[] Contents;

            public HttpResponse(byte[] contents)
            {
                this.Contents = contents;
                Status = HttpStatusCode.OK;
            }

            public HttpResponse(byte[] contents, HttpStatusCode status)
            {
                this.Contents = contents;
                Status = status;
            }
        }

        protected abstract string MakeUrl(string path);
        protected abstract string MakeUrl(string hostname, string path);

        protected WebClient CreateWebClient()
        {
            WebClient web = new WebClient();
            if (Proxy != null)
            {
                web.Proxy = Proxy;
            }

            return web;
        }

        protected HttpResponse Get(string url)
        {
            try
            {
                using (var web = CreateWebClient())
                {
                    return new HttpResponse(web.DownloadData(url));
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    var responseStream = ex.Response.GetResponseStream();
                    if (responseStream != null)
                    {
                        var buffer = new byte[10000];
                        int length = responseStream.Read(buffer, 0, buffer.Length);
                        Array.Resize(ref buffer, length);
                        return new HttpResponse(buffer, ((HttpWebResponse)ex.Response).StatusCode);
                    }
                }
                else
                {
                    throw;
                }
            }

            return new HttpResponse(null, HttpStatusCode.Unused);
        }

        public string GetAsString(string hostname, string path)
        {
            try
            {
                using (var web = CreateWebClient())
                {

                    var url = MakeUrl(hostname, path);
                    Console.WriteLine(url);
                    var response = web.DownloadString(url);
                    Console.WriteLine(response);
                    return response;
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    throw new M2WebException(((HttpWebResponse)ex.Response).StatusCode, "The eWON responded with an error");
                }

                throw;
            }
        }

        public string PostAsString(string hostname, string path, string parameters)
        {
            try
            {
                using (var web = CreateWebClient())
                {
                    var url = MakeUrl(hostname, path);
                    web.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    return web.UploadString(url, parameters);
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    throw new M2WebException(((HttpWebResponse)ex.Response).StatusCode, "The eWON responded with an error");
                }

                throw;
            }
        }

        public T GetJSonObject<T>(string url)
        {
            HttpResponse response = Get(url);
            var json = response.Contents;
            if (json == null) return default(T);
            Debug.WriteLine(Encoding.UTF8.GetString(json));

            using (var ms = new MemoryStream(json))
            {
                if (response.Status != HttpStatusCode.OK)
                {   // Error
                    var serializer = new DataContractJsonSerializer(typeof(Responses.M2WebError));
                    var error = (Responses.M2WebError)serializer.ReadObject(ms);
                    Debug.Assert(!error.success);
                    throw new M2WebException(response.Status, error.message);
                }
                else
                {   // OK
                    var serializer = new DataContractJsonSerializer(typeof(T));
                    return (T)serializer.ReadObject(ms);
                }
            }
        }

        public T GetXmlObject<T>(string hostname, string path)
        {
            var url = MakeUrl(hostname, path);
            HttpResponse response = Get(url);
            if (response.Status != HttpStatusCode.OK)
            {
                throw new M2WebException(response.Status, "");
            }

            var xml = response.Contents;
            Debug.WriteLine(Encoding.UTF8.GetString(xml));

            using (var ms = new MemoryStream(xml))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(ms);
            }
        }
    }
}
