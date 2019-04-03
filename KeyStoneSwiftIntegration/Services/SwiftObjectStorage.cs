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
            request.AddHeader("Content-Type", "multpart/form-data");

            request.AddFile(objectName, fullObjectName);
            
            IRestResponse response = restClient.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK &&
                response.StatusCode != HttpStatusCode.Created)
            {
                Debug.Print("Error in creating object. Error:" + response.ToString());
                throw new Exception("Error in creating object. Error:" + response.ToString());
            }
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
