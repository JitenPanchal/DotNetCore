using DotNetCore.Enums;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class HttpClientExtensions
    {
        public static void Post<TRequest, TResponse>(this HttpClient httpClient, string requestUri, TRequest content, DataFormat dataFormat,
            out HttpResponseMessage httpResponseMessage,
            out string responseString,
            out TResponse response) where TResponse : class where TRequest : class
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();

            switch (dataFormat)
            {
                case DataFormat.Xml:
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                        httpResponseMessage = httpClient.PostAsXmlAsync(requestUri, content).Result;
                        responseString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                        try
                        {
                            response = responseString.ToXml<TResponse>();
                        }
                        catch
                        {
                            response = null;
                        }
                    }
                    break;
                default:
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        httpResponseMessage = httpClient.PostAsJsonAsync(requestUri, content).Result;
                        responseString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                        try
                        {
                            response = JsonConvert.DeserializeObject<TResponse>(responseString);
                        }
                        catch
                        {
                            response = null;
                        }
                    }
                    break;
            }
        }
    }
}