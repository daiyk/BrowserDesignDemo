using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BrowserDesign.Extension;

namespace Utility
{
    /// <summary>
    /// Utilities functions/methods for retrieving remote resources
    /// </summary>
    class Utilities : MonoBehaviour
    {
        /// <summary>
        /// Get service directory content, should not have any token requirements
        /// </summary>
        /// <param name="serviceDirectoryURL"></param>
        /// <returns></returns>
        public async static Task<ServiceDirectory> GetServiceDirectory(string serviceDirectoryURL)
        {
            //send request for the service directory
            if (!serviceDirectoryURL.StartsWith("http://") && !serviceDirectoryURL.StartsWith("https://"))
            {
                serviceDirectoryURL = "https://" + serviceDirectoryURL;
            }
            Dictionary<string, string> bodyContent = new Dictionary<string, string>();
            bodyContent.Add("f", "json");

            var serviceDirectoryMessage = await PostRequest(serviceDirectoryURL, bodyContent);
            var serviceDirectoryMessageString = await serviceDirectoryMessage.Content.ReadAsStringAsync();

            ServiceDirectory sd = JsonConvert.DeserializeObject<ServiceDirectory>(serviceDirectoryMessageString);
            if (sd == null)
            {
                //parsing failed
                Debug.LogError("Request to get service directory failed, parsing failed");
                return null;
            }

            return sd;
        }

        public async static Task<HttpResponseMessage> PostRequest(string uri, Dictionary<string, string> content = null)
        {
            HttpClient client = new HttpClient();
            //TODO: set up headers

            if (!uri.StartsWith("http://") && !uri.StartsWith("https://"))
            {
                uri = "https://" + uri;
            }
            //set up body
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            if (content != null)
            {
                //encoding x-www-form-urlencoded
                requestMessage.Content = new FormUrlEncodedContent(content);
            }

            //execute request
            try
            {
                var res = await client.SendAsync(requestMessage);

                if (res.IsSuccessStatusCode)
                {
                    var resMessage = await res.Content.ReadAsStringAsync();
                    if (resMessage.Contains("error"))
                    {
                        //TODO: call the message panel
                        string message = ParseArcGISErrorCode(JsonConvert.DeserializeObject<ErrorReponse>(resMessage).error);

                        Debug.LogError(message);
                        res.Content.Headers.Add("error", message);
                    }
                    return res;
                }
                Debug.LogError($"PostRequest: post request with {uri} is Not a success code");
                return null;
            }
            catch (InvalidOperationException)
            {
                //The request message is already sent, and cannot be sent again
                var errorMess = LanguageManager.Translate("The request message was already sent. Please wait and don't send it repeatedly");
               
                Debug.LogError("Repeat Operation: " + errorMess);
            }
            catch (HttpRequestException)
            {
                var errorMess = LanguageManager.Translate("Request failed! an potential issue can be network connectivity, DNS failure, server validation or timeout.");
                
                Debug.LogError("Failed Operation:" + errorMess);
            }

            catch (TaskCanceledException)
            {
                var errorMess = LanguageManager.Translate("Request timeout, please check your network!");
                
                Debug.LogError("TimeOut: " + errorMess);
            }
            catch (Exception ex)
            {
                
                Debug.LogError(ex.ToString());
            }
            //return default(HttpResponseMessage);
            return null;
        }
        private static string ParseArcGISErrorCode(ErrorBody error)
        {

            StringBuilder message = new StringBuilder();
            //TODO: error code specified operation
            switch (error.code)
            {
                //code list from https://developers.arcgis.com/net/reference/platform-error-codes/#http-network-and-rest-errors
                case 400:
                    message.AppendLine("ArcGIS Bad Request: " + error.message);
                    break;
                case 401:
                case 403:
                    message.AppendLine("ArcGIS Authorization required: " + error.message);
                    break;
                case 404:
                    message.AppendLine("ArcGIS Resource Not Found: " + error.message);
                    break;
                case 413:
                    message.AppendLine("ArcGIS Attachment/request too large: " + error.message);
                    break;
                case 498:
                    message.AppendLine("ArcGIS Wrong/expired token: " + error.message);
                    break;
                case 499:
                    message.AppendLine("ArcGIS Token required: " + error.message);
                    break;
                case 500:
                    message.AppendLine("ArcGIS server can't response: " + error.message);
                    break;
                case 501:
                    message.AppendLine("ArcGIS service Not implemented: " + error.message);
                    break;
                default:
                    message.AppendLine("ArcGIS unknown error: " + error.message);
                    break;
            }
            //parse details of the error
            if (error.details != null && error.details.Count > 0)
            {
                /*foreach (var mess in error.details)
                {
                    message.AppendLine(mess);
                }*/
                message.AppendLine(error.details.Last());
            }
            return message.ToString();
        }

        /// <summary>
        /// Gets the description of a layer
        /// </summary>
        public async static Task<string> GetLayerDescription(string descriptionURL)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(descriptionURL);


            var postData = "f=json";
            var data = Encoding.ASCII.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            // send the get request
            HttpWebResponse resp = (HttpWebResponse)(await request.GetResponseAsync());
            //HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
            // read the response
            StreamReader reader = new StreamReader(resp.GetResponseStream());
            // put it into a string
            string resp2string = reader.ReadToEnd();

            //print(resp2string);

            return resp2string;
        }

        /// <summary>
        /// Returns the FeatureLayer from the url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async static Task<FeatureLayer> GetFeatureLayer(string layerURL, bool requireToken = false)
        {
            print("URL: " + layerURL);
            Dictionary<string, string> bodyContent = new Dictionary<string, string>();
            //add token, and other stuff
            bodyContent.Add("f", "json");
            //add token if possible
            if (requireToken)
            {
                bodyContent.Add("token", await EsriManager.GetToken());
            }
            var response = await PostRequest(layerURL, bodyContent);
            if (response == null)
            {
                return null;
            }
            if (response.Content.Headers.Contains("error"))
            {
                var messages = response.Content.Headers.GetValues("error");
                StringBuilder totalMess = new StringBuilder();
                foreach (var mess in messages)
                {
                    totalMess.Append(mess);
                }
                Debug.LogError(totalMess.ToString());
                return null;
            }
            StreamReader reader = new StreamReader(await response.Content.ReadAsStreamAsync());
            string resp2string = reader.ReadToEnd();
            print(resp2string);
            try
            {
                FeatureLayer fl = JsonConvert.DeserializeObject<FeatureLayer>(resp2string);
                fl.layerUrl = layerURL;
                fl.requireToken = requireToken;

                return fl;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Feature Layer: target {layerURL} can't be loaded. {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Gets a feature service from url
        /// </summary>
        public async static Task<FeatureService> GetFeatureService(string uri, bool requireToken = false)
        {
            Dictionary<string, string> bodyContent = new Dictionary<string, string>();
            //add token, and other stuff
            bodyContent.Add("f", "json");
            //add token if possible
            if (requireToken)
            {
                bodyContent.Add("token", await EsriManager.GetToken());
            }
            var response = await PostRequest(uri, bodyContent);
            //network request failed, return null
            if (response == null)
            {
                return null;
            }
            if (response.Content.Headers.Contains("error"))
            {
                var messages = response.Content.Headers.GetValues("error");
                StringBuilder totalMess = new StringBuilder();
                foreach (var mess in messages)
                {
                    totalMess.Append(mess);
                }
                Debug.LogError(totalMess.ToString());
                return null;
            }
            StreamReader reader = new StreamReader(await response.Content.ReadAsStreamAsync());
            string resp2string = reader.ReadToEnd();

            try
            {
                FeatureService fs = JsonConvert.DeserializeObject<FeatureService>(resp2string);
                fs.url = uri;
                fs.requireToken = requireToken;

                fs.authority = fs.capabilities.Split(',').ToList();
                return fs;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Feature Service: {uri} can't be loaded. {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Gets a map service from url
        /// </summary>
        public async static Task<MapService> GetMapService(string uri, bool requireToken = false)
        {
            Dictionary<string, string> bodyContent = new Dictionary<string, string>();
            //add token, and other stuff
            bodyContent.Add("f", "json");
            //add token if possible
            if (requireToken)
            {
                bodyContent.Add("token", await EsriManager.GetToken());
            }
            var response = await PostRequest(uri, bodyContent);
            //network request failed, return null
            if (response == null)
            {
                return null;
            }
            if (response.Content.Headers.Contains("error"))
            {
                var messages = response.Content.Headers.GetValues("error");
                StringBuilder totalMess = new StringBuilder();
                foreach (var mess in messages)
                {
                    totalMess.Append(mess);
                }
                Debug.LogError(totalMess.ToString());
                return null;
            }
            StreamReader reader = new StreamReader(await response.Content.ReadAsStreamAsync());
            string resp2string = reader.ReadToEnd();

            try
            {
                MapService ms = JsonConvert.DeserializeObject<MapService>(resp2string);
                ms.url = uri;
                ms.requireToken = requireToken;

                ms.authority = ms.capabilities.Split(',').ToList();
                return ms;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Map Service: {uri} can't be loaded. {ex.Message}");
                return null;
            }
        }
        public async static Task<Texture2D> GetImage(string uri, string token = null, bool requireToken = false)
        {
            Dictionary<string, string> bodyContent = new Dictionary<string, string>();
            //add token, and other stuff
            bodyContent.Add("f", "json");
            //add token if possible
            if (requireToken && token != null)
            {
                bodyContent.Add("token", await EsriManager.GetToken());
            }
            var response = await PostRequest(uri, bodyContent);
            //if the request failed for some reason.
            if (response == null)
            {
                return null;
            }
            if (response.Content.Headers.Contains("error"))
            {
                var messages = response.Content.Headers.GetValues("error");
                StringBuilder totalMess = new StringBuilder();
                foreach (var mess in messages)
                {
                    totalMess.Append(mess);
                }
                Debug.LogError(totalMess.ToString());
                return null;
            }
            var bitImage = await response.Content.ReadAsByteArrayAsync();
            Texture2D tex = new Texture2D(1, 1);
            ImageConversion.LoadImage(tex, bitImage);
            return tex;
        }
    }
}
