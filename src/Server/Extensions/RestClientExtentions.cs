using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uni.Scan.Shared.Exceptions;


namespace Uni.Scan.Server.Extensions
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
        
         


        public static async Task<RestResponse> BydPostProjectJsonAsync(this RestClient client,  object json, CancellationToken cancellationToken = default)
        {

          return  await client.BydPostJsonAsync($"ProjectCollection",json, cancellationToken);
        }





        public static async Task<RestResponse> BydPostAsync(this RestClient client, string resource,CancellationToken cancellationToken = default)
        {

            await client.SetTokenAsync();

            var request = new RestRequest(resource);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            return await client.PostAsync(request, cancellationToken);

        }

        public static async Task<RestResponse> BydPostProjectStatusAsync(this RestClient client, string projectUid,string status , CancellationToken cancellationToken = default)
        {
            var endpoint = "";
            switch (status)
            {
                case "1": endpoint ="Start";break;
                case "2": endpoint ="Release"; break;
                case "3": endpoint ="Finish"; break;

                default:
                    new ApiException("Error while trying to Get  project status {0} for project id  {1}  ",status, projectUid);
                    break;
            }

            var ressource = $"{endpoint}?ObjectID='{projectUid}'";
            return await client.BydPostAsync(ressource,cancellationToken);

        }

        
        public static async Task<RestResponse> BydPostTaskParendAsync(this RestClient client,  string Taskid, string taskPrentUid,CancellationToken cancellationToken = default)
        {
            
            var ressource = $"Move?ObjectID='{Taskid}'&TargetParentTaskUUID=guid'{taskPrentUid}'";
            return await client.BydPostAsync(ressource, cancellationToken);

        }
        public static async Task<RestResponse> BydPostTaskNextAsync(this RestClient client, string Taskid, string taskNexUid, CancellationToken cancellationToken = default)
        {

            var ressource = $"Move?ObjectID='{Taskid}'&TargetRightNeighbourTaskUUID=guid'{taskNexUid}'";
            return await client.BydPostAsync(ressource, cancellationToken);

        }
        public static async Task<RestResponse> BydPostStartTaskAsync(this RestClient client, string Taskid, CancellationToken cancellationToken = default)
        {

            var ressource = $"Release?ObjectID='{Taskid}'";
            return await client.BydPostAsync(ressource, cancellationToken);

        }


        public static async Task<RestResponse> BydGetAsync(this RestClient client, string resource, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest(resource, Method.Get);
            return await client.GetAsync(request,  cancellationToken);
        }



    }
}
