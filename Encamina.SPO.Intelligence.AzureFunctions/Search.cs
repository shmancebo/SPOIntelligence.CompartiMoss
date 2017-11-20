using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Encamina.SPO.Intelligence.Search;
using Newtonsoft.Json;

namespace Encamina.SPO.Intelligence.AzureFunctions
{
    public static class Search
    {
        [FunctionName("Search")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string text = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "text", true) == 0)
                .Value;

            string site = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "site", true) == 0)
                .Value;

            string library = req.GetQueryNameValuePairs()
               .FirstOrDefault(q => string.Compare(q.Key, "library", true) == 0)
               .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            text = text ?? data?.text;
            site = site ?? data?.site;
            library = library ?? data.library;



            var azureSearch = new AzureSearchService();
            var result = azureSearch.RunQuery<AzureSearchModel>("newsindex", new Microsoft.Azure.Search.Models.SearchParameters(), text, null).Result;
            var searched = JsonConvert.SerializeObject(result);

            var spoS = new SPOManager.SPOService(site);
            var resultSPO = spoS.GetData(library);


            return text == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Inserte texto de busqueda")
                : req.CreateResponse(HttpStatusCode.OK,new FullResult() { SearchResult = searched, SharepointResult = resultSPO });
        }
    }
}
