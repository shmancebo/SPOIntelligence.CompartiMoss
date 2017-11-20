using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SPOManager;
using CongnitveManager;
using Newtonsoft.Json;
using Encamina.SPO.Intelligence.Search;
using System.Collections.Generic;

namespace Encamina.SPO.Intelligence.AzureFunctions
{
    public static class News
    {
        [FunctionName("News")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            dynamic data = await req.Content.ReadAsAsync<object>();

            log.Info("Data:" + data.Lista + data.Id + data.site);
            // Set name to query string or body data
            string lista = data?.Lista;
            string id = data?.Id;
            string site = data?.site;



            if (lista != null && id != null)
            {

                log.Info("Lista " + lista + " id " + id + " site " + site);

                SPOService spoM = new SPOService(site);
                var image = spoM.GetPhotoInfo(lista, id);
                CelebrityService cService = new CelebrityService();
                var content = cService.MakeAnalysisCelebrity(image);
                var celebrityName = cService.GetCelebrity(content.celebrity);

                log.Info("Obteniendo el celebrity");

                var azureSearch = new AzureSearchService();
                var indexCreate = azureSearch.CreateIndexAsync<AzureSearchModel>("newsindex", false, null).Result;
                var contentIndex = new AzureSearchModel() { IdSharepoint = id, Name = celebrityName, Tags = content.tags, Id = id };
                var uploadDocument = azureSearch.UploadDocuments<AzureSearchModel>("newsindex", new List<AzureSearchModel>() { contentIndex }.ToArray());

                log.Info("Creado el search");
                spoM.SetResultNews(lista, id, JsonConvert.SerializeObject(content.tags));
                return req.CreateResponse(HttpStatusCode.OK, "Noticia categorizada");
            }

            else
                return req.CreateResponse(HttpStatusCode.BadRequest, "Faltan parametros");
        }
    }
}
