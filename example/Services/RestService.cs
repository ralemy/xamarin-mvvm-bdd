using System;
using System.Threading.Tasks;
using example.Helpers;
using MVVMFramework.Statics;
using Plugin.Connectivity;
using RestSharp;

namespace example.Services
{
    public class RestService
    {
        public async Task<string> Call()
        {
            if (!CheckConnectivity())
                return "";
            var Client = new RestSharp.RestClient(Settings.ServerUrl);
            var request = new RestRequest(RestCalls.TestEndPoint);
            request.AddParameter("key1", "value1");
            request.AddParameter("key2", "value2");
            var response = await Client.ExecuteGetTaskAsync(request);
            return response.Content;
        }

        public bool CheckConnectivity() => !CrossConnectivity.IsSupported ||
                                     CrossConnectivity.Current.IsConnected;
    }
}
