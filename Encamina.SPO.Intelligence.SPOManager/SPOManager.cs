using Encamina.SPO.Intelligence.Webhooks;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SPOManager
{
    public class SPOService
    {
        private readonly string siteWebhook = "URL Sharepoint";
        private readonly string userName;
        private readonly string passWord;
        private readonly string siteUrl;



        public SPOService(string url)
        {
            userName = "******";
            passWord = "******";
            siteUrl = url;
        }
        public Stream GetPhotoInfo(string library, string idItem)
        {

            using (ClientContext client = new ClientContext(siteUrl))
            {
                client.Credentials = new SharePointOnlineCredentials(userName, GetPassword(passWord));
                var list = client.Web.Lists.GetByTitle(library);
                client.Load(list);
                client.ExecuteQuery();

                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><Query><Where><Geq><FieldRef Name='ID'/>" +
                "<Value Type='Number'>" + idItem + "</Value></Geq></Where></Query><RowLimit>100</RowLimit></View>";

                ListItemCollection collListItem = list.GetItems(camlQuery);
                client.Load(collListItem);
                client.ExecuteQuery();

                if (collListItem != null)
                {
                    var item = collListItem.FirstOrDefault();
                    client.Load(item, i => i.File);
                    client.Load(item, i => i.File.Length);
                    client.ExecuteQuery();
                    var fileRef = item.File.ServerRelativeUrl;
                    client.ExecuteQuery();
                    var fileInfo = client.Web.GetFileByServerRelativeUrl(fileRef);
                    var clientFile = fileInfo.OpenBinaryStream();
                    client.Load(fileInfo);
                    client.ExecuteQuery();
                    var stream = clientFile.Value;
                    var lg = clientFile.Value.Length;
                    return stream;
                }
                return null;
            }
        }


        public void SetResultNews(string library, string idItem, string categoria)
        {

            using (ClientContext client = new ClientContext(siteUrl))
            {
                client.Credentials = new SharePointOnlineCredentials(userName, GetPassword(passWord));
                var list = client.Web.Lists.GetByTitle(library);
                client.Load(list);
                client.ExecuteQuery();

                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><Query><Where><Geq><FieldRef Name='ID'/>" +
                "<Value Type='Number'>" + idItem + "</Value></Geq></Where></Query><RowLimit>100</RowLimit></View>";

                ListItemCollection collListItem = list.GetItems(camlQuery);
                client.Load(collListItem);
                client.ExecuteQuery();

                if (collListItem != null)
                {
                    var item = collListItem.FirstOrDefault();
                    item["Categoria"] = categoria;
                    item.Update();
                    client.ExecuteQuery();
                }
            }
        }


        public string GetData(string library)
        {
            using (ClientContext client = new ClientContext(siteUrl))
            {
                client.Credentials = new SharePointOnlineCredentials(userName, GetPassword(passWord));
                var list = client.Web.Lists.GetByTitle(library);
                client.Load(list);
                client.ExecuteQuery();

                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><Query></Query><RowLimit>100</RowLimit></View>";
                ListItemCollection collListItem = list.GetItems(camlQuery);
                client.Load(collListItem,i=>i.Include(
                    item=>item["Id"],
                    item => item["Body"],
                    item => item["Titular"],
                    item => item["FileRef"]

                ));
                client.ExecuteQuery();

                var result = new List<NewsModel>();
                foreach (var item in collListItem)
                {

                    result.Add(new NewsModel() {
                        Body = item["Body"]?.ToString(),
                        Titular = item["Titular"]?.ToString(),
                        Image = item["FileRef"]?.ToString(),
                        Id = item.Id.ToString()
                    });
                   
                    
                }
                return JsonConvert.SerializeObject(result);
            }
        }
        public void SetResultEmotion(string library, string idItem, string result)
        {

            using (ClientContext client = new ClientContext(siteUrl))
            {
                client.Credentials = new SharePointOnlineCredentials(userName, GetPassword(passWord));
                var list = client.Web.Lists.GetByTitle(library);
                client.Load(list);
                client.ExecuteQuery();

                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = "<View><Query><Where><Geq><FieldRef Name='ID'/>" +
                "<Value Type='Number'>" + idItem + "</Value></Geq></Where></Query><RowLimit>100</RowLimit></View>";

                ListItemCollection collListItem = list.GetItems(camlQuery);
                client.Load(collListItem);
                client.ExecuteQuery();

                if (collListItem != null)
                {
                    var item = collListItem.FirstOrDefault();
                    item["Animo"] = result;
                    item.Update();
                    client.ExecuteQuery();
                }
            }
        }

        public static SecureString GetPassword(string pass)
        {
            SecureString result = new SecureString();
            pass.ToCharArray().ToList().ForEach(p => result.AppendChar(p));
            return result;
        }


        public EventChange GetEvent(string listId, string web)
        {
            using (ClientContext client = new ClientContext(siteUrl))
            {
                client.Credentials = new SharePointOnlineCredentials(userName, GetPassword(passWord));
                var list = client.Web.Lists.GetById(new Guid(listId));
                client.Load(list);
                client.ExecuteQuery();
                ChangeQuery changes = new ChangeQuery(true, true);
                changes.Item = true;
                changes.RecursiveAll = true;
                changes.Add = true;
                changes.User = true;
                changes.Update = true;
                changes.List = true;
                changes.Field = true;
                if (list != null)
                {
                    var listChages = list.GetChanges(changes);
                    client.Load(listChages);
                    client.ExecuteQuery();
                    var result = listChages.LastOrDefault();
                    if (result != null)
                    {
                        var itemChange = (ChangeItem)result;
                        var type = result.ChangeType;
                        if (type == ChangeType.Add)
                        {
                            var changeInfo = new SPChangeInfo()
                            {
                                ItemId = itemChange?.ItemId.ToString(),
                                ItemTitle = string.Format("{0}_{1}", itemChange.ListId.ToString(), itemChange.ItemId.ToString()),
                                ListName = itemChange.ListId.ToString(),
                                TypeChange = type.ToString()
                            };

                            return new EventChange() { Id = changeInfo.ItemId, Lista = list.Title, site = siteUrl };
                        }
                    }
                }
            }

            return null;
        }

    }

    public class SPChangeInfo
    {
        public string ItemId { get; set; }

        public string ItemTitle { get; set; }
        public string ListName { get; set; }
        public string TypeChange { get; set; }
    }

    public class NewsModel
    {
        public string Id { get; set; }
        public string Body { get; set; }
        public string Titular { get; set; }
        public string Image { get; set; }
    }
}
