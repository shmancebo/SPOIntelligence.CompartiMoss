using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using Newtonsoft.Json;
using Encamina.SPO.Intelligence.Webhooks;
using SPOManager;
using System.Text;

namespace Encamina.SPO.Intelligence.AzureFunctions
{
    public static class WebhooksClient
    {
        [FunctionName("WebhooksClient")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            string url = "https://encamina.sharepoint.com/sites/shernandez/SPOIntelligence";
            log.Info("C# HTTP trigger function processed a request.");
            string validationToken = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "validationtoken", true) == 0)
        .Value;


            if (validationToken != null)
            {
                log.Info($"Validation token {validationToken} received");
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(validationToken);
                return response;
            }

            log.Info($"SharePoint triggered our webhook...great :-)");
            var content = await req.Content.ReadAsStringAsync();
            log.Info($"Received following payload: {content}");

            var notifications = JsonConvert.DeserializeObject<ResponseModel<NotificationModel>>(content).Value;
            log.Info($"Found {notifications.Count} notifications");

            if (notifications.Count > 0)
            {
                var notification = notifications[0];

                log.Info($"Found {notification.SiteUrl} notifications");
                SPOService spService = new SPOService(url);
                var eventData = spService.GetEvent(notification.Resource, url);

                HttpClient client = new HttpClient();

                log.Info("JSON:" + JsonConvert.SerializeObject(eventData));
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://spointelligence.azurewebsites.net/api/News?code=1OFts7dd2OaMT2z/bKr7KDHKL/q3Yf0OiN9iRSfemeWPaJIM14e8nQ==");
                request.Content = new StringContent(JsonConvert.SerializeObject(eventData), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.SendAsync(request).Result;
                if (response.IsSuccessStatusCode)
                {
                    string requestResult = response.Content.ReadAsStringAsync().Result;
                    if (JsonConvert.DeserializeObject<bool>(requestResult))
                    {
                        return req.CreateResponse(HttpStatusCode.OK);
                    }
                }
            }

            return req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body");

        }
    }
}
