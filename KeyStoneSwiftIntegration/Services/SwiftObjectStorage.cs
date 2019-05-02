using KeyStoneSwiftIntegration.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace KeyStoneSwiftIntegration.Services
{
    public class SwiftObjectStorage
    {
        /// <summary>
        /// Your container name.
        /// </summary>
        string containerName = "containerName";

        /// <summary>
        /// Your openstack address.
        /// </summary>
        string swiftUrl = "http://172.24.47.22:5000";

        /// <summary>
        /// Create a object on Swift Object Storage container.
        /// </summary>
        /// <param name="swiftConfig"></param>
        /// <param name="objectName"></param>
        /// <param name="path"></param>
        public void CreateObject(SwiftConfig swiftConfig, string objectName, string path)
        {
            RestClient restClient = GetRestClient();

            objectName = RemoveLeadingSlash(objectName);

            string fullObjectName = Path.Combine(path, objectName);
            fullObjectName = RemoveLeadingSlash(fullObjectName);

            IRestRequest request = GetRequest(swiftConfig.StorageUrl + "/" + containerName + "/" + objectName, Method.PUT,  null, swiftConfig.AuthToken);

            // In this case: a video file.
            request.AddParameter(objectName, File.ReadAllBytes(fullObjectName), "video/x-msvideo", ParameterType.RequestBody);
            
            IRestResponse response = restClient.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK &&
                response.StatusCode != HttpStatusCode.Created)
            {
                Debug.Print("Error in creating object. Error:" + response.ToString());
                throw new Exception("Error in creating object. Error:" + response.ToString());
            }
        }

        /// <summary>
        ///  Get a file from Swift Object Storage.
        /// </summary>
        /// <param name="videoName"></param>
        /// <returns></returns>
        public Stream GetVideoFile(SwiftConfig swiftConfig, string videoName)
        {
            RestClient restClient = GetRestClient();

            IRestRequest request = GetRequest(swiftConfig.StorageUrl + "/" + containerName + "/" + videoName, Method.GET, null, swiftConfig.AuthToken);

            IRestResponse response = restClient.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream fs = new MemoryStream(response.RawBytes);
                Debug.Print("Video get from Swift: " + videoName);
                return fs;
            }
            else
            {
                Debug.Print("Error in get object. Error:" + response.ToString());
                return null;
            }
        }

        /// <summary>
        /// Delete a range of videos of Swift Object Storage
        /// </summary>
        /// <param name="listVideoObject">List of videos to be deleted</param>
        public List<String> DeleteVideo(SwiftConfig swiftConfig, List<String> listVideoObject)
        {
            RestClient restClient = GetRestClient();

            List<String> succeededDeletes = new List<String>();

            foreach (String videoName in listVideoObject)
            {
                IRestRequest request = GetRequest(swiftConfig.StorageUrl + "/" + containerName + "/" + videoName, Method.DELETE, null, swiftConfig.AuthToken);

                IRestResponse response = restClient.Execute(request);

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    succeededDeletes.Add(videoName);
                    Debug.Print("Video: " + videoName + " deleted of Swift...");
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Debug.Print("Video: " + videoName + " not found...");
                }
                else
                {
                    Debug.Print("Video: " + videoName + " deletion failed...");
                }
            }

            return succeededDeletes;
        }

        /// <summary>
        /// Get the REST Client.
        /// </summary>
        /// <returns></returns>
        private RestClient GetRestClient()
        {
            return new RestClient(swiftUrl);
        }

        /// <summary>
        /// Remove slashes.
        /// </summary>
        /// <param name="tokenWord"></param>
        /// <returns></returns>
        private string RemoveLeadingSlash(string tokenWord)
        {
            while (tokenWord != null && tokenWord.Length > 0 && tokenWord.StartsWith("/"))
            {
                tokenWord = tokenWord.Substring(1);
            }

            while (tokenWord != null && tokenWord.Length > 0 && tokenWord.StartsWith("//"))
            {
                tokenWord = tokenWord.Substring(1);
            }

            return tokenWord;
        }

        /// <summary>
        /// Build a request with KeyStone auth token.
        /// </summary>
        /// <param name="resourceUrl"></param>
        /// <param name="method"></param>
        /// <param name="queryString"></param>
        /// <param name="authToken"></param>
        /// <returns></returns>
        private IRestRequest GetRequest(String resourceUrl, Method method, String queryString, String authToken)
        {
            RestRequest request = new RestRequest(resourceUrl +
                (String.IsNullOrEmpty(queryString) ? "" : "?" + queryString), method);

            request.AddHeader("X-Auth-Token", authToken);

            return request;
        }
    }
}
