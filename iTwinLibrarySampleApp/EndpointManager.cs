/*--------------------------------------------------------------------------------------+
|
| Copyright (c) Bentley Systems, Incorporated. All rights reserved.
| See LICENSE.md in the catalog root for license terms and full copyright notice.
|
+--------------------------------------------------------------------------------------*/

using ItwinLibrarySampleApp.Models;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ItwinLibrarySampleApp
    {
    internal class EndpointManager
        {
        private static readonly HttpClient client = new();
        private const string API_BASE_URL = "https://api.bentley.com";
        private const string JOB_STATUS_SUCCESS = "Success";
        private const string JOB_STATUS_INPROGRESS = "InProgress";
        private const string JOB_STATUS_QUEUED = "Queued";
        private const string JOB_STATUS_ERROR = "Error";
        private Dictionary<string, string> _containerMap;

        #region Constructors
        internal EndpointManager (string token)
            {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.bentley.itwin-platform.v1+json");
            client.DefaultRequestHeaders.Add("Authorization", token);
            _containerMap = new Dictionary<string, string>();
            _containerMap.Add("uploadcomponent", "uploadComponent");
            _containerMap.Add("weblink", "webLink");
            _containerMap.Add("weblinks", "webLinks");
            }
        #endregion

        internal async Task<HttpGetResponseMessage<T>> MakeGetCall<T> (string relativeUrl, Dictionary<string, string> customHeaders = null)
            {
            // Add any additional headers if applicable
            AddCustomHeaders(client, customHeaders);

            // Construct full url and then make the GET call
            using var response = await client.GetAsync($"{API_BASE_URL}{relativeUrl}");

            if ( response.StatusCode == HttpStatusCode.TooManyRequests )
                {
                // You should implement retry logic for TooManyRequests (429) errors and possibly others like GatewayTimeout or ServiceUnavailable
                }

            // Copy/Deserialize the response into custom HttpGetResponseMessage.
            HttpGetResponseMessage<T> responseMsg = new HttpGetResponseMessage<T>();
            responseMsg.Status = response.StatusCode;
            responseMsg.Content = await response.Content.ReadAsStringAsync();
            var responsePayload = JObject.Parse(responseMsg.Content);
            if ( response.StatusCode == HttpStatusCode.OK )
                {
                // Successful response. Deserialize the list of objects returned.
                var containerName = $"{typeof(T).Name.ToLower()}s"; // The container is plural for lists
                if ( _containerMap.ContainsKey(containerName) )
                    containerName = _containerMap[containerName];

                var instances = responsePayload[containerName];
                responseMsg.Instances = new List<T>();
                responseMsg.Links = responsePayload["_links"]?.ToObject<PaginationLinks>();
                foreach ( var inst in instances )
                    {
                    responseMsg.Instances.Add(inst.ToObject<T>());
                    }
                }
            else
                {
                // There was an error. Deserialize the error details and return.
                responseMsg.ErrorDetails = responsePayload["error"]?.ToObject<ErrorDetails>();
                }
            return responseMsg;
            }

        internal async Task<HttpGetSingleResponseMessage<T>> MakeGetSingleCall<T> (string relativeUrl, Dictionary<string, string> customHeaders = null)
            {
            // Add any additional headers if applicable
            AddCustomHeaders(client, customHeaders);
            return await MakeGetSingleCall<T>($"{API_BASE_URL}{relativeUrl}");
            }

        internal async Task<HttpGetSingleResponseMessage<T>> MakeGetSingleCall<T> (string Url)
            {
            // Construct full url and then make the GET call
            using var response = await client.GetAsync(Url);

            if ( response.StatusCode == HttpStatusCode.TooManyRequests )
                {
                // You should implement retry logic for TooManyRequests (429) errors and possibly others like GatewayTimeout or ServiceUnavailable
                }

            // Copy/Deserialize the response into custom HttpGetSingleResponseMessage.
            HttpGetSingleResponseMessage<T> responseMsg = new HttpGetSingleResponseMessage<T>();
            responseMsg.Status = response.StatusCode;
            responseMsg.Content = await response.Content.ReadAsStringAsync();
            var responsePayload = JObject.Parse(responseMsg.Content);
            if ( response.StatusCode == HttpStatusCode.OK )
                {
                // Successful response. Deserialize the object returned.
                var containerName = typeof(T).Name.ToLower();
                if ( _containerMap.ContainsKey(containerName) )
                    containerName = _containerMap[containerName];

                responseMsg.Instance = responsePayload[containerName].ToObject<T>();
                }
            else
                {
                // There was an error. Deserialize the error details and return.
                responseMsg.ErrorDetails = responsePayload["error"]?.ToObject<ErrorDetails>();
                }
            return responseMsg;
            }

        internal async Task<HttpPostResponseMessage<T>> MakePostCall<T> (string relativeUrl, T propertyModel, Dictionary<string, string> customHeaders = null)
            {
            // Add any additional headers if applicable
            AddCustomHeaders(client, customHeaders);

            var body = new StringContent(JsonSerializer.Serialize(propertyModel, JsonSerializerOptions), Encoding.UTF8, "application/json");
            HttpPostResponseMessage<T> responseMsg = new HttpPostResponseMessage<T>();

            // Construct full url and then make the POST call
            using (var response = await client.PostAsync($"{API_BASE_URL}{relativeUrl}", body))
                {
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                    // You should implement retry logic for TooManyRequests (429) errors and possibly others like GatewayTimeout or ServiceUnavailable
                    }

                // Copy/Deserialize the response into custom HttpPostResponseMessage.

                responseMsg.Status = response.StatusCode;
                responseMsg.Content = await response.Content.ReadAsStringAsync();
                }

            if (!string.IsNullOrEmpty(responseMsg.Content))
                {
                var responsePayload = JObject.Parse(responseMsg.Content);
                if (responseMsg.Status == HttpStatusCode.Created)
                    {
                    // Successful response. Deserialize the object returned. This is the full representation
                    // of the new instance that was just created. It will contain the new instance Id.
                    var containerName = typeof(T).Name.ToLower();
                    if ( _containerMap.ContainsKey(containerName) )
                        containerName = _containerMap[containerName];

                    responseMsg.NewInstance = responsePayload[containerName].ToObject<T>();
                    }
                else
                    {
                    // There was an error. Deserialize the error details and return.
                    responseMsg.ErrorDetails = responsePayload["error"]?.ToObject<ErrorDetails>();
                    }
                }
            else
                responseMsg.NewInstance = propertyModel;

            return responseMsg;
            }

        internal async Task<HttpPostResponseMessage<TOut>> MakePostCall<TIn, TOut> (string relativeUrl, TIn propertyModel, Dictionary<string, string> customHeaders = null)
            {
            // Add any additional headers if applicable
            AddCustomHeaders(client, customHeaders);

            var body = new StringContent(JsonSerializer.Serialize(propertyModel, JsonSerializerOptions), Encoding.UTF8, "application/json");
            HttpPostResponseMessage<TOut> responseMsg = new HttpPostResponseMessage<TOut>();

            // Construct full url and then make the POST call
            using ( var response = await client.PostAsync($"{API_BASE_URL}{relativeUrl}", body) )
                {
                if ( response.StatusCode == HttpStatusCode.TooManyRequests )
                    {
                    // You should implement retry logic for TooManyRequests (429) errors and possibly others like GatewayTimeout or ServiceUnavailable
                    }

                // Copy/Deserialize the response into custom HttpPostResponseMessage.
                responseMsg.Headers = response.Headers;
                responseMsg.Status = response.StatusCode;
                responseMsg.Content = await response.Content.ReadAsStringAsync();
                }

            if ( !string.IsNullOrEmpty(responseMsg.Content) )
                {
                var responsePayload = JObject.Parse(responseMsg.Content);
                if ( responseMsg.Status == HttpStatusCode.Created || responseMsg.Status == HttpStatusCode.Accepted )
                    {
                    // Successful response. Deserialize the object returned. This is the full representation
                    // of the new instance that was just created. It will contain the new instance Id.
                    var containerName = typeof(TOut).Name.ToLower();
                    if ( _containerMap.ContainsKey(containerName) )
                        containerName = _containerMap[containerName];

                    responseMsg.NewInstance = responsePayload[containerName].ToObject<TOut>();
                    }
                else
                    {
                    // There was an error. Deserialize the error details and return.
                    responseMsg.ErrorDetails = responsePayload["error"]?.ToObject<ErrorDetails>();
                    }
                }
            else
                responseMsg.NewInstance = default(TOut);

            return responseMsg;
            }

        internal async Task<HttpUpdateResponseMessage<TOut>> MakePutCall<TIn, TOut> (string relativeUrl, TIn propertyModel, Dictionary<string, string> customHeaders = null)
            {
            // Add any additional headers if applicable
            AddCustomHeaders(client, customHeaders);

            var serialized = JsonSerializer.Serialize(propertyModel, JsonSerializerOptions);
            var body = new StringContent(serialized, Encoding.UTF8, "application/json");
            HttpUpdateResponseMessage<TOut> responseMsg = new HttpUpdateResponseMessage<TOut>();

            // Construct full url and then make the POST call
            using ( var response = await client.PutAsync($"{API_BASE_URL}{relativeUrl}", body) )
                {
                if ( response.StatusCode == HttpStatusCode.TooManyRequests )
                    {
                    // You should implement retry logic for TooManyRequests (429) errors and possibly others like GatewayTimeout or ServiceUnavailable
                    }

                // Copy/Deserialize the response into custom HttpPostResponseMessage.
                responseMsg.Status = response.StatusCode;
                responseMsg.Content = await response.Content.ReadAsStringAsync();
                }

            if ( !string.IsNullOrEmpty(responseMsg.Content) )
                {
                var responsePayload = JObject.Parse(responseMsg.Content);
                if ( responseMsg.Status == HttpStatusCode.OK )
                    {
                    // Successful response. Deserialize the object returned. This is the full representation
                    // of the new instance that was just created. It will contain the new instance Id.
                    var containerName = typeof(TOut).Name.ToLower();
                    if ( _containerMap.ContainsKey(containerName) )
                        containerName = _containerMap[containerName];

                    responseMsg.UpdatedInstance = responsePayload[containerName].ToObject<TOut>();
                    }
                else
                    {
                    // There was an error. Deserialize the error details and return.
                    responseMsg.ErrorDetails = responsePayload["error"]?.ToObject<ErrorDetails>();
                    }
                }
            else
                responseMsg.UpdatedInstance = default(TOut);

            return responseMsg;
            }

        internal async Task<HttpUpdateResponseMessage<T>> MakePutCall<T> (string relativeUrl, T propertyModel, Dictionary<string, string> customHeaders = null)
            {
            // Add any additional headers if applicable
            AddCustomHeaders(client, customHeaders);

            var body = new StringContent(JsonSerializer.Serialize(propertyModel, JsonSerializerOptions), Encoding.UTF8, "application/json");
            // Construct full url and then make the PATCH call
            using var response = await client.PutAsync($"{API_BASE_URL}{relativeUrl}", body);
            if ( response.StatusCode == HttpStatusCode.TooManyRequests )
                {
                // You should implement retry logic for TooManyRequests (429) errors and possibly others like GatewayTimeout or ServiceUnavailable
                }

            // Copy/Deserialize the response into custom HttpUpdateResponseMessage.
            HttpUpdateResponseMessage<T> responseMsg = new HttpUpdateResponseMessage<T>();
            responseMsg.Status = response.StatusCode;
            responseMsg.Content = await response.Content.ReadAsStringAsync();
            var responsePayload = JObject.Parse(responseMsg.Content);
            if ( response.StatusCode == HttpStatusCode.OK )
                {
                // Successful response. Deserialize the object returned. This is the full representation
                // of the instance that was just updated, including the updated values.
                var containerName = typeof(T).Name.ToLower();
                if ( _containerMap.ContainsKey(containerName) )
                    containerName = _containerMap[containerName];

                responseMsg.UpdatedInstance = responsePayload[containerName].ToObject<T>();
                }
            else
                {
                // There was an error. Deserialize the error details and return.
                responseMsg.ErrorDetails = responsePayload["error"]?.ToObject<ErrorDetails>();
                }
            return responseMsg;
            }

        internal async Task<HttpResponseMessage<T>> MakeDeleteCall<T> (string relativeUrl, Dictionary<string, string> customHeaders = null)
            {
            // Add any additional headers if applicable
            AddCustomHeaders(client, customHeaders);

            // Construct full url and then make the POST call
            using var response = await client.DeleteAsync($"{API_BASE_URL}{relativeUrl}");
            if ( response.StatusCode == HttpStatusCode.TooManyRequests )
                {
                // You should implement retry logic for TooManyRequests (429) errors and possibly others like GatewayTimeout or ServiceUnavailable
                }

            // Copy/Deserialize the response into custom HttpResponseMessage.
            HttpResponseMessage<T> responseMsg = new HttpResponseMessage<T>();
            responseMsg.Status = response.StatusCode;
            if ( response.StatusCode != HttpStatusCode.NoContent )
                {
                // There was an error. Deserialize the error details and return.
                responseMsg.Content = await response.Content.ReadAsStringAsync();
                var responsePayload = JObject.Parse(responseMsg.Content);
                responseMsg.ErrorDetails = responsePayload["error"]?.ToObject<ErrorDetails>();
                }
            return responseMsg;
            }


        internal async Task MakeFileUploadCall (string url, string path)
            {
            if ( !File.Exists(path) )
                throw new FileNotFoundException($"File not found on path {path}");

            Console.Write($"\n\n Uploading file");

            WebClient webClient = new WebClient();
            webClient.Headers.Add("x-ms-blob-type", "BlockBlob");
            webClient.UploadFileAsync( new Uri(url), "PUT", path);

            Console.Write($" (SUCCESS)");
            }

        internal async Task MakeFileDownloadCall (string url, string path)
            {
            WebClient webClient = new WebClient();
            webClient.DownloadFileAsync(new Uri(url), path);
            }

        internal async Task<HttpGetSingleResponseMessage<Job>> GetJobAfterCompletion (string jobUrl)
            {
            //Wait for job to complete (Job.Status == "Success") - normally job completes within couple of minutes but can take
            //more time depending upon load/no of parallel jobs being executed. If job doesn't complete within 5 minutes, 'maxTimeToWait' can be extended accordingly.
            Console.Write($"\n\n Getting Upload Job ({jobUrl})");
            var maxTimeToWait = DateTime.Now.Minute + 5;
            HttpGetSingleResponseMessage<Job> responseMsg = null;

            do
                {
                var response = await client.GetAsync(jobUrl);
                if ( response.StatusCode == HttpStatusCode.TooManyRequests )
                    {
                    // You should implement retry logic for TooManyRequests (429) errors and possibly others like GatewayTimeout or ServiceUnavailable
                    }

                // Copy/Deserialize the response into custom HttpGetSingleResponseMessage.
                responseMsg = new HttpGetSingleResponseMessage<Job>();
                responseMsg.Status = response.StatusCode;
                responseMsg.Content = await response.Content.ReadAsStringAsync();
                var responsePayload = JObject.Parse(responseMsg.Content);
                if ( response.StatusCode == HttpStatusCode.OK )
                    {
                    // Successful response. Deserialize the object returned.
                    responseMsg.Instance = responsePayload["job"].ToObject<Job>();
                    }
                else
                    {
                    // There was an error. Deserialize the error details and return.
                    responseMsg.ErrorDetails = responsePayload["error"]?.ToObject<ErrorDetails>();
                    break;
                    }

                Console.Write($"\n Job Status ({responseMsg.Instance.Status})");
                if ( responseMsg.Instance.Status.Equals("Error") )
                    {
                    Console.Write($"\n Upload Job Failed - Status ({responseMsg.Instance.Status})");
                    throw new  Exception("Upload job failed.");
                    }
                //If Job.Status is InProgress or Queued - keep making the request to job endpoint each after 2 seconds.
                if ( responseMsg.Instance.Status == JOB_STATUS_INPROGRESS || responseMsg.Instance.Status == JOB_STATUS_QUEUED )
                    await Task.Delay(5000);
                }
            while ( DateTime.Now.Minute < maxTimeToWait && (responseMsg.Instance.Status != JOB_STATUS_SUCCESS) );

            Console.Write($"\n (SUCCESS)");
            return responseMsg;
            }

        #region Private Methods

        private void AddCustomHeaders (HttpClient client, Dictionary<string, string> customHeaders = null)
            {
            if ( customHeaders != null )
                {
                foreach ( var ch in customHeaders )
                    {
                    client.DefaultRequestHeaders.Add(ch.Key, ch.Value);
                    }
                }
            }
        private static JsonSerializerOptions JsonSerializerOptions
            {
            get
                {
                var options = new JsonSerializerOptions
                    {
                    IgnoreNullValues = true,
                    WriteIndented = true,
                    AllowTrailingCommas = false,
                    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                    Converters = {
                        new JsonStringEnumConverter()
                        }
                    };
                return options;
                }
            }
        #endregion
        }

    #region Supporting Classes
    internal class HttpResponseMessage<T>
        {
        public HttpStatusCode Status
            {
            get; set;
            }
        public string Content
            {
            get; set;
            }
        public ErrorDetails ErrorDetails
            {
            get; set;
            }
        }

    internal class HttpPostResponseMessage<T> : HttpResponseMessage<T>
        {
        public HttpResponseHeaders Headers
            {
            get; set;
            }
        public T NewInstance
            {
            get; set;
            }
        }
    internal class HttpUpdateResponseMessage<T> : HttpResponseMessage<T>
        {
        public T UpdatedInstance
            {
            get; set;
            }
        }
    internal class HttpGetResponseMessage<T> : HttpResponseMessage<T>
        {
        public List<T> Instances
            {
            get; set;
            }

        public PaginationLinks Links
            {
            get; set;
            }
        }

    internal class HttpGetSingleResponseMessage<T> : HttpResponseMessage<T>
        {
        public T Instance
            {
            get; set;
            }
        }
    #endregion
    }
