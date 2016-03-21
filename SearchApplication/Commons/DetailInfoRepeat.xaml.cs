using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SearchApplication.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SearchApplication.Commons
{
    public delegate void DetailEventHandler(bool value);
    public sealed partial class DetailInfoRepeat : UserControl
    {
        public event DetailEventHandler returnEvent;
        public DetailInfoRepeat()
        {
            this.InitializeComponent();
        }

        public async void SetData(String contenID, String contentTypeID)
        {

            String str = await Request_Json(contenID, contentTypeID);
            ParseAreaJson(str);
        }


        private async Task<string> Request_Json(String contenID, String contentTypeID)
        {
            // string url = "http://www.redmine.org/issues.json";

            string url =
             @"http://api.visitkorea.or.kr/openapi/service/rest/"+MainPage.SetLanguage+"/detailInfo?" +
             @"ServiceKey=" + MainPage.ServiceKey
             + "&MobileOS=ETC&MobileApp=TestApp&contentId=" + contenID + "&contentTypeId=" + contentTypeID + "&_type=json";
            HttpClient client = new HttpClient();
            Task<string> getStringTask = client.GetStringAsync(url);
            string result = await getStringTask;


            return result;

        }

        private void ParseAreaJson(String json)
        {
            st_Main.Children.Clear();
            json = json.Trim();
            List<DetailInfoRepeatClass> itemList = new List<DetailInfoRepeatClass>();

            JObject obj = JObject.Parse(json);

            if (obj["response"]["header"]["resultMsg"].ToString() == "OK")
            {
                if (obj["response"]["body"]["totalCount"].ToString() == "1")
                {
                    JObject itemObj = obj["response"]["body"]["items"]["item"] as JObject;
                    {
                        DetailInfoRepeatClass item = new DetailInfoRepeatClass();

                        item.ContentID = itemObj["contentid"] != null ? itemObj["contentid"].ToString().Replace("<br>", "\r\n") : "";
                        item.ContentTypeId = itemObj["contenttypeid"] != null ? itemObj["contenttypeid"].ToString().Replace("<br>", "\r\n") : "";
                        item.Figubun = itemObj["figubun"] != null ? itemObj["figubun"].ToString().Replace("<br>", "\r\n") : "";
                        item.InfoName = itemObj["infoname"] != null ? itemObj["infoname"].ToString().Replace("<br>", "\r\n") : "";
                        item.InfoText = itemObj["infotext"] != null ? itemObj["infotext"].ToString().Replace("<br>", "\r\n") : "";
                        item.SerialNum = itemObj["serialnum"] != null ? itemObj["serialnum"].ToString().Replace("<br>", "\r\n") : "";

                        itemList.Add(item);


                        DetailItem ditem = new DetailItem();
                        ditem.SetItem(item.InfoName, item.InfoText);

                        st_Main.Children.Add(ditem);
                    }
                }
                else if(obj["response"]["body"]["totalCount"].ToString() !="0") {
                    JArray array = JArray.Parse(obj["response"]["body"]["items"]["item"].ToString());
                    foreach (JObject itemObj in array)
                    {
                        //JObject itemObj = obj["response"]["body"]["items"]["item"] as JObject;
                        {
                            DetailInfoRepeatClass item = new DetailInfoRepeatClass();

                            item.ContentID = itemObj["contentid"] != null ? itemObj["contentid"].ToString().Replace("<br>", "\r\n") : "";
                            item.ContentTypeId = itemObj["contenttypeid"] != null ? itemObj["contenttypeid"].ToString().Replace("<br>", "\r\n") : "";
                            item.Figubun = itemObj["figubun"] != null ? itemObj["figubun"].ToString().Replace("<br>", "\r\n") : "";
                            item.InfoName = itemObj["infoname"] != null ? itemObj["infoname"].ToString().Replace("<br>", "\r\n") : "";
                            item.InfoText = itemObj["infotext"] != null ? itemObj["infotext"].ToString().Replace("<br>", "\r\n") : "";
                            item.SerialNum = itemObj["serialnum"] != null ? itemObj["serialnum"].ToString().Replace("<br>", "\r\n") : "";

                            itemList.Add(item);


                            DetailItem ditem = new DetailItem();
                            ditem.SetItem(item.InfoName, item.InfoText);

                            st_Main.Children.Add(ditem);
                        }

                    }
                }

                // cb_AreaSelect.ItemsSource = koreaAreaCodeList;
            }
            if (returnEvent != null)
            {
                if(st_Main.Children.Count>0)
                returnEvent(true);
                else
                {
                    returnEvent(false);

                }
            }
        }
    }
}
