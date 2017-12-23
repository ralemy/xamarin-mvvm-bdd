using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Sockets;
using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Shared;
using RestSharp;

namespace Specflow.Server
{
    public class RestTestServer : RestServer
    {
        public RestTestServer(string Port) : base(new ServerSettings
        {
            Port = Port,
            Host= "0.0.0.0",
            Router = InitRouter()
        })
        {
        }
        public RestTestServer() : this("4343")
        {

        }

        private static IRouter InitRouter()
        {
            var router = new Router();
            router.Register(new Route(typeof(RestTestServer).GetMethod("IndexPoint"), "/"));
            return router;
        }

        public IHttpContext IndexPoint(IHttpContext context)
        {
            context.Response.SendResponse("Server Up");
            return context;
        }

        internal string GetUrl(string v)
        {
            if (!v.StartsWith("/", StringComparison.Ordinal))
                v = "/" + v;
            return $"http://{GetLocalIP()}:{Port}{v}";
        }

        public void Register(Func<IHttpContext, IHttpContext> endpoint, HttpMethod method, string path) => this.Router.Register(endpoint, method, path);

        public string GetLocalIP()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }

        public T Get<T>(string path) where T : new()
        {
            var client = new RestClient(GetUrl(path));
            return client.Execute<T>(new RestRequest()).Data;
        }
        public string Get(string path)
        {
            var client = new RestClient(GetUrl(path));
            return client.Execute(new RestRequest()).Content;
        }
    }
}
