using RestSharp.Authenticators;
using RestSharp;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uni.Scan.Shared.Exceptions;

namespace Uni.Scan.Infrastructure.ByDesign.Extentions
{
    public static class RestClientExtentions
    {
        public static async Task<string> GetToken(this RestClient client, string userName, string password)
        {

            client.Authenticator = new HttpBasicAuthenticator(userName, password);
            var request = new RestRequest("$metadata", Method.Get);
            request.AddHeader("x-csrf-token", "fetch");

            var response = await client.GetAsync(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApiException("Error while trying to get the token ,server error message : {0}", response.ErrorMessage);
            }
            var token = (string)(response.Headers.FirstOrDefault(x => x.Name == "x-csrf-token")?.Value);
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ApiException("Error while trying to get the token , token is null or not found}");
            }

            return token;
        }


        public static async Task SetTokenAsync(this RestClient client)
        {
            if (!client.DefaultParameters.Any(o => o.Name=="x-csrf-token"))
            {
                var request = new RestRequest("$metadata", Method.Get);
                request.AddHeader("x-csrf-token", "fetch");
                var response = await client.GetAsync(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new ApiException("Error while trying to get the token ,server error message : {0}", response.ErrorMessage);
                }
                var token = (string)(response.Headers.FirstOrDefault(x => x.Name == "x-csrf-token")?.Value);
                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new ApiException("Error while trying to get the token , token is null or not found}");
                }
                client.AddDefaultHeader("x-csrf-token", token);

            }


        }


        public static async Task<RestResponse> BydGetAsync(this RestClient client, string resource, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest(resource, Method.Get);
            return await client.GetAsync(request, cancellationToken);
        }

        public static async Task<RestResponse> BydPostAsync(this RestClient client, string resource, CancellationToken cancellationToken = default)
        {

            await client.SetTokenAsync();

            var request = new RestRequest(resource);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            return await client.PostAsync(request, cancellationToken);

        }
        public static async Task<RestResponse> BydPostJsonAsync(this RestClient client, string resource, object json, CancellationToken cancellationToken = default)
        {

            await client.SetTokenAsync();

            var request = new RestRequest(resource);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddBody(json, "application/json");
            return await client.PostAsync(request, cancellationToken);

        }

        public static async Task<RestResponse> BydPatchJsonAsync(this RestClient client, string resource, object json, CancellationToken cancellationToken = default)
        {

            await client.SetTokenAsync();
            var request = new RestRequest(resource);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddBody(json, "application/json");
            return await client.PatchAsync(request, cancellationToken);

        }


    }
}
