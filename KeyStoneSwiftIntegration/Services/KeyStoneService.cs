using KeyStoneSwiftIntegration.Models;
using RestSharp;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace KeyStoneSwiftIntegration.Services
{
    public class KeyStoneService
    {
        /// <summary>
        /// Your openstack address.
        /// </summary>
        string swiftUrl = "http://172.24.47.22:5000";

        /// <summary>
        /// Openstack user id that will use the Swift.
        /// </summary>
        string userId = "9b1d82d5970f41448ade75921e4ca6ed";

        /// <summary>
        /// Openstack user password that will use the Swift.
        /// </summary>
        string userPassword = "Password1234";

        /// <summary>
        /// Openstack project id that will use the Swift.
        /// </summary>
        string projectId = "637371a2fdae40ad9604fdd74616181b";

        /// <summary>
        /// Authenticate with the Key Stone v3 (Project Scope).
        /// </summary>
        /// <returns>SwiftConfig Model</returns>
        public SwiftConfig Authenticate()
        {
            SwiftConfig swiftConfig = new SwiftConfig();

            RestClient restClient = GetRestClient();

            // Keystone v3 POST authentication URL.
            RestRequest request = new RestRequest("v3/auth/tokens", Method.POST);
            StringBuilder auth = new StringBuilder();

            // This JSON Request will authenticate on Project Scope. See more on: https://developer.openstack.org/api-ref/identity/v3/index.html?expanded=password-authentication-with-scoped-authorization-detail
            auth.Append("{");
            auth.Append("\"auth\": {");
            auth.Append("\"identity\": {");
            auth.Append("\"methods\": [");
            auth.Append("\"password\"");
            auth.Append("],");
            auth.Append("\"password\": {");
            auth.Append("\"user\": { ");
            auth.Append("\"id\": \"" + userId +"\", ");
            auth.Append("\"password\": \"" + userPassword + "\"");
            auth.Append("}");
            auth.Append("}");
            auth.Append("},");
            auth.Append("\"scope\": {");
            auth.Append("\"project\": {");
            auth.Append("\"id\": \"" + projectId + "\"");
            auth.Append("}");
            auth.Append("}");
            auth.Append("}");
            auth.Append("}");

            request.RequestFormat = DataFormat.Json;

            request.AddParameter("application/json", auth, ParameterType.RequestBody);

            IRestResponse response = restClient.Execute(request);

            swiftConfig.StorageUrl = swiftUrl + projectId;

            // Make REST API requests to other OpenStack services. You supply the ID of your authentication token in the X-Subject-Token (v3) request header.
            if (response.StatusCode == HttpStatusCode.Created)
            {
                foreach (Parameter hdr in response.Headers)
                {
                    if (hdr.Name.Equals("X-Subject-Token"))
                    {
                        swiftConfig.AuthToken = hdr.Value.ToString();
                    }
                }
            }
            else
            {
                Debug.Print("Authentication Failed. Error: " + response.ToString());
                throw new Exception("Authentication Failed. Error: " + response.ToString());
            }

            Debug.Print("Storage URL:" + swiftConfig.StorageUrl + "; " + "Auth Token: " + swiftConfig.AuthToken);

            return swiftConfig;
        }

        /// <summary>
        /// Get the REST Client.
        /// </summary>
        /// <returns></returns>
        private RestClient GetRestClient()
        {
            return new RestClient(swiftUrl);
        }
    }
}
