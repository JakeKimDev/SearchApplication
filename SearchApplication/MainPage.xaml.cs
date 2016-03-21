using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SearchApplication.Commons;
using SearchApplication.Models;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿은 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 에 문서화되어 있습니다.

namespace SearchApplication
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static String ServiceKey = "f85ybkyCCEvPNrtvQhxmbxiRNdcFNzHxlz0v7Gev1nU%2B8kRcuaEKbv%2FwMRVuvU1Ah6Rv1GGFMl4W95eGhtnr7Q%3D%3D"; // Open API Service Key

        //"EngService";
        //"JpnService";
        public static String SetLanguage = "EngService"; // Service 지역
        
        public MainPage()
        {
            this.InitializeComponent();
            LoadArea();//기본 구역 정보 
            LoadService(); // 기본 서비스 정보
            btn_Search.Click += Btn_Search_Click;
            btn_Research.Click += Btn_Research_Click;
           
        }

        private void Umc_MapControl_eReceiveMsg(Windows.UI.Xaml.Controls.Maps.MapControl sender, object args)
        {
           LoadReLocation();
        }

        private void Btn_Research_Click(object sender, RoutedEventArgs e)
        {
            this.umc_MapControl.SetCollapsed();
            LoadReLocation();
        }
        public async void LoadReLocation()
        {
           
            umc_MapControl.MyItemClear();
            string str = await Request_LocationJson(true);
            ParseLocationJson(str,true);


            _festivalList = new List<KoreaLocation>() ;
        }
     


        private void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            tbtn_Menu.IsChecked = false;
            this.umc_MapControl.SetCollapsed();
            LoadLocation();
        }

        #region 지역 선택

        public async void LoadArea()
        {
            string str = await Request_Json();
            ParseAreaJson(str);
        }

        private async Task<string> Request_Json()
        {
           // string url = "http://www.redmine.org/issues.json";
               
               string url =
                @"http://api.visitkorea.or.kr/openapi/service/rest/"+MainPage.SetLanguage+"/areaCode?"+
                @"ServiceKey="+ ServiceKey+ "&numOfRows=100&pageNo=1&MobileOS=ETC&MobileApp=TestApp&_type=json";
            HttpClient client = new HttpClient();
            Task<string> getStringTask = client.GetStringAsync(url);
            string result = await getStringTask;
            return result;

        }
        private void ParseAreaJson(String json)
        {
            List<KoreaAreaCode> koreaAreaCodeList = new List<KoreaAreaCode>();

            JObject obj = JObject.Parse(json);
                    
            if (obj["response"]["header"]["resultMsg"].ToString() == "OK")
            {
                JArray array = JArray.Parse(obj["response"]["body"]["items"]["item"].ToString());
                foreach (JObject itemObj in array)
                {
                    KoreaAreaCode koreaAreaCode = new KoreaAreaCode();
                    koreaAreaCode.Code = itemObj["code"].ToString();
                    koreaAreaCode.Name = itemObj["name"].ToString();
                    koreaAreaCode.RNum = itemObj["rnum"].ToString();
                    koreaAreaCodeList.Add(koreaAreaCode);
                }

                cb_AreaSelect.ItemsSource = koreaAreaCodeList;
                if(koreaAreaCodeList.Count >0)
                cb_AreaSelect.SelectedIndex = 0;
            }
        }
        #endregion
        
        #region 서비스 코드
        public async void LoadService()
        {
            string str = await Request_ServiceJson();
            ParseServiceJson(str);
        }

        private async Task<string> Request_ServiceJson()
        {
            // string url = "http://www.redmine.org/issues.json";

            string url =
             @"http://api.visitkorea.or.kr/openapi/service/rest/"+MainPage.SetLanguage+"/categoryCode?" +
             @"ServiceKey=" + ServiceKey + "&numOfRows=100&pageNo=1&MobileOS=ETC&MobileApp=TestApp&_type=json";
            HttpClient client = new HttpClient();
            Task<string> getStringTask = client.GetStringAsync(url);
            string result = await getStringTask;
            return result;

        }
        private void ParseServiceJson(String json)
        {
            List<KoreaAreaCode> koreaAreaCodeList = new List<KoreaAreaCode>();

            JObject obj = JObject.Parse(json);

            if (obj["response"]["header"]["resultMsg"].ToString() == "OK")
            {
                JArray array = JArray.Parse(obj["response"]["body"]["items"]["item"].ToString());
                foreach (JObject itemObj in array)
                {
                    KoreaAreaCode koreaAreaCode = new KoreaAreaCode();
                    koreaAreaCode.Code = itemObj["code"].ToString();
                    koreaAreaCode.Name = itemObj["name"].ToString();
                    koreaAreaCode.RNum = itemObj["rnum"].ToString();
                    koreaAreaCodeList.Add(koreaAreaCode);
                }
                koreaAreaCodeList.Insert(0, new KoreaAreaCode() { Code = "", Name = "All", RNum = "" });
                
                cb_ServiceSelect.ItemsSource = koreaAreaCodeList;

                if (koreaAreaCodeList.Count > 0)
                {
                    cb_ServiceSelect.SelectedIndex = 0;
                }

            }
        }


        #endregion

        #region 위치 정보
        public static  List<KoreaLocation> _festivalList = new List<KoreaLocation>();
        public async void LoadLocation()
        {
            //st_Stack.Children.Clear();
            umc_MapControl.ItemClear();
            string str = await Request_LocationJson();
            ParseLocationJson(str);

            string str2 = await Request_PartyLocationJson();
            ParseFestivalJson(str2);
        }
        private async Task<string> Request_PartyLocationJson()
        {
            // string url = "http://www.redmine.org/issues.json";

            string url =
             @"http://api.visitkorea.or.kr/openapi/service/rest/" + MainPage.SetLanguage + "/searchFestival?" +
             @"ServiceKey=" + ServiceKey + "&numOfRows=20&pageNo=1&arrange=A&listYN=Y&MobileOS=ETC&MobileApp=TestApp&"//contentTypeId="+cb_ServiceSelect.SelectedValue
             + "&areaCode=" + cb_AreaSelect.SelectedValue + "&eventStartDate="+DateTime.Now.ToString("yyyyMMdd")+"&_type=json";
            HttpClient client = new HttpClient();
            Task<string> getStringTask = client.GetStringAsync(url);
            string result = await getStringTask;
            JObject obj = JObject.Parse(result);
            if (Convert.ToDouble(obj["response"]["body"]["totalCount"].ToString()) > 20)
            {
                url =
                             @"http://api.visitkorea.or.kr/openapi/service/rest/" + MainPage.SetLanguage + "/searchFestival?" +
                             @"ServiceKey=" + ServiceKey + "&numOfRows=" + obj["response"]["body"]["totalCount"].ToString() + "&pageNo=1&arrange=A&listYN=Y&MobileOS=ETC&MobileApp=TestApp&"//contentTypeId="+cb_ServiceSelect.SelectedValue
                             + "&areaCode=" + cb_AreaSelect.SelectedValue + "&eventStartDate=" + DateTime.Now.ToString("yyyyMMdd") + "&_type=json";
                client = new HttpClient();
                getStringTask = client.GetStringAsync(url);
                result = await getStringTask;
            }




            return result;

        }

        private void ParseFestivalJson(String json)
        {

            //      Task task = new Task(() =>
            //    {

            bool set = false;
            _festivalList = new List<KoreaLocation>();

            JObject obj = JObject.Parse(json);

            if (obj["response"]["header"]["resultMsg"].ToString() == "OK" && Convert.ToDouble(obj["response"]["body"]["totalCount"].ToString()) > 0)
            {
                if (Convert.ToDouble(obj["response"]["body"]["totalCount"].ToString()) == 1)
                {
                    JObject itemObj =  (obj["response"]["body"]["items"]["item"]) as JObject;
                   // foreach (JObject itemObj in array)
                    {
                        KoreaLocation korea = new KoreaLocation();
                        korea.Addr1 = itemObj["addr1"] == null ? "" : itemObj["addr1"].ToString();
                        korea.Addr2 = itemObj["addr2"] == null ? "" : itemObj["addr2"].ToString();
                        korea.AreaCode = itemObj["areacode"] == null ? "" : itemObj["areacode"].ToString();
                        korea.Cat1 = itemObj["cat1"] == null ? "" : itemObj["cat1"].ToString();
                        korea.Cat2 = itemObj["cat2"] == null ? "" : itemObj["cat2"].ToString();
                        korea.Cat3 = itemObj["cat3"] == null ? "" : itemObj["cat3"].ToString();
                        korea.ContentID = itemObj["contentid"] == null ? "" : itemObj["contentid"].ToString();
                        korea.ContentTypeID = itemObj["contenttypeid"] == null ? "" : itemObj["contenttypeid"].ToString();
                        korea.CreatedTime = itemObj["createdtime"] == null ? "" : itemObj["createdtime"].ToString();
                        korea.FirstImage = itemObj["firstimage"] == null ? "" : itemObj["firstimage"].ToString();
                        korea.Image2 = itemObj["firstimage2"] == null ? "" : itemObj["firstimage2"].ToString();
                        korea.MapX = itemObj["mapx"] == null ? "" : itemObj["mapx"].ToString();
                        korea.MapY = itemObj["mapy"] == null ? "" : itemObj["mapy"].ToString();
                        korea.mLevel = itemObj["mlevel"] == null ? "" : itemObj["mlevel"].ToString();
                        korea.ModifiedTime = itemObj["modifiedtime"] == null ? "" : itemObj["modifiedtime"].ToString();
                        korea.ReadCount = itemObj["readcount"] == null ? "" : itemObj["readcount"].ToString();
                        korea.SigunguCode = itemObj["sigungucode"] == null ? "" : itemObj["sigungucode"].ToString();
                        korea.Tel = itemObj["tel"] == null ? "" : itemObj["tel"].ToString();
                        korea.Title = itemObj["title"] == null ? "" : itemObj["title"].ToString();
                        korea.EventStartDate = itemObj["eventstartdate"] == null ? "" : itemObj["eventstartdate"].ToString();
                        korea.EventEndDate = itemObj["eventenddate"] == null ? "" : itemObj["eventenddate"].ToString();
                        _festivalList.Add(korea);


                    }
                }
                else
                {
                    JArray array = JArray.Parse(obj["response"]["body"]["items"]["item"].ToString());
                    foreach (JObject itemObj in array)
                    {
                        KoreaLocation korea = new KoreaLocation();
                        korea.Addr1 = itemObj["addr1"] == null ? "" : itemObj["addr1"].ToString();
                        korea.Addr2 = itemObj["addr2"] == null ? "" : itemObj["addr2"].ToString();
                        korea.AreaCode = itemObj["areacode"] == null ? "" : itemObj["areacode"].ToString();
                        korea.Cat1 = itemObj["cat1"] == null ? "" : itemObj["cat1"].ToString();
                        korea.Cat2 = itemObj["cat2"] == null ? "" : itemObj["cat2"].ToString();
                        korea.Cat3 = itemObj["cat3"] == null ? "" : itemObj["cat3"].ToString();
                        korea.ContentID = itemObj["contentid"] == null ? "" : itemObj["contentid"].ToString();
                        korea.ContentTypeID = itemObj["contenttypeid"] == null ? "" : itemObj["contenttypeid"].ToString();
                        korea.CreatedTime = itemObj["createdtime"] == null ? "" : itemObj["createdtime"].ToString();
                        korea.FirstImage = itemObj["firstimage"] == null ? "" : itemObj["firstimage"].ToString();
                        korea.Image2 = itemObj["firstimage2"] == null ? "" : itemObj["firstimage2"].ToString();
                        korea.MapX = itemObj["mapx"] == null ? "" : itemObj["mapx"].ToString();
                        korea.MapY = itemObj["mapy"] == null ? "" : itemObj["mapy"].ToString();
                        korea.mLevel = itemObj["mlevel"] == null ? "" : itemObj["mlevel"].ToString();
                        korea.ModifiedTime = itemObj["modifiedtime"] == null ? "" : itemObj["modifiedtime"].ToString();
                        korea.ReadCount = itemObj["readcount"] == null ? "" : itemObj["readcount"].ToString();
                        korea.SigunguCode = itemObj["sigungucode"] == null ? "" : itemObj["sigungucode"].ToString();
                        korea.Tel = itemObj["tel"] == null ? "" : itemObj["tel"].ToString();
                        korea.Title = itemObj["title"] == null ? "" : itemObj["title"].ToString();
                        korea.EventStartDate = itemObj["eventstartdate"] == null ? "" : itemObj["eventstartdate"].ToString();
                        korea.EventEndDate = itemObj["eventenddate"] == null ? "" : itemObj["eventenddate"].ToString();
                        _festivalList.Add(korea);


                    }
                }

                //cb_ServiceSelect.ItemsSource = koreaList;
            }

            //   });

            // task.Start();
        }
        private async Task<string> Request_LocationJson(bool myLocation = false)
        {
            // string url = "http://www.redmine.org/issues.json";

            if (myLocation)
            {
                Geopoint point =
                umc_MapControl.GetCenter();
                string url =
                @"http://api.visitkorea.or.kr/openapi/service/rest/" + MainPage.SetLanguage + "/locationBasedList?" +
                @"ServiceKey=" + ServiceKey + "&mapY="+point.Position.Latitude.ToString()
                + "&mapX=" + point.Position.Longitude.ToString() +"&radius="+txt_dist.Text
                + "&numOfRows=20&pageNo=1&arrange=A&listYN=Y&MobileOS=ETC&MobileApp=TestApp"//contentTypeId="+cb_ServiceSelect.SelectedValue
                + "&_type=json";
                HttpClient client = new HttpClient();
                Task<string> getStringTask = client.GetStringAsync(url);
                string result = await getStringTask;
                JObject obj = JObject.Parse(result);
                if (Convert.ToDouble(obj["response"]["body"]["totalCount"].ToString()) > 20)
                {
                    url =
                                 @"http://api.visitkorea.or.kr/openapi/service/rest/" + MainPage.SetLanguage + "/locationBasedList?" +
                                 @"ServiceKey=" + ServiceKey +"&mapY=" + point.Position.Latitude.ToString()
                                 + "&mapX=" + point.Position.Longitude.ToString() + "&radius=" + txt_dist.Text
                                  +"&numOfRows=" + obj["response"]["body"]["totalCount"].ToString()
                                 + "&pageNo=1&arrange=A&listYN=Y"
                                 + "&MobileOS=ETC&MobileApp=TestApp"
                                 + "&_type=json";
                    client = new HttpClient();
                    getStringTask = client.GetStringAsync(url);
                    result = await getStringTask;
                }
                return result;
            }
            else {
                string url =
                 @"http://api.visitkorea.or.kr/openapi/service/rest/" + MainPage.SetLanguage + "/areaBasedList?" +
                 @"ServiceKey=" + ServiceKey + "&numOfRows=20&pageNo=1&arrange=A&listYN=Y&MobileOS=ETC&MobileApp=TestApp&"//contentTypeId="+cb_ServiceSelect.SelectedValue
                 + "&areaCode=" + cb_AreaSelect.SelectedValue + "&_type=json";
                HttpClient client = new HttpClient();
                Task<string> getStringTask = client.GetStringAsync(url);
                string result = await getStringTask;
                JObject obj = JObject.Parse(result);
                if (Convert.ToDouble(obj["response"]["body"]["totalCount"].ToString()) > 20)
                {
                    url =
                                 @"http://api.visitkorea.or.kr/openapi/service/rest/" + MainPage.SetLanguage + "/areaBasedList?" +
                                 @"ServiceKey=" + ServiceKey + "&numOfRows=" + obj["response"]["body"]["totalCount"].ToString() + "&pageNo=1&arrange=A&listYN=Y&MobileOS=ETC&MobileApp=TestApp&"//contentTypeId="+cb_ServiceSelect.SelectedValue
                                 + "&areaCode=" + cb_AreaSelect.SelectedValue + "&_type=json";
                    client = new HttpClient();
                    getStringTask = client.GetStringAsync(url);
                    result = await getStringTask;
                }




                return result;
            }

        }


        private void ParseLocationJson(String json, bool set=false)
        {
            var dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;

            Task task = new Task(() =>
                {
                   
                    dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                         
                        List<KoreaLocation> koreaList = new List<KoreaLocation>();

                        JObject obj = JObject.Parse(json);

                        if (obj["response"]["header"]["resultMsg"].ToString() == "OK" && Convert.ToDouble(obj["response"]["body"]["totalCount"].ToString()) > 0)
                        {

                            if (Convert.ToDouble(obj["response"]["body"]["totalCount"].ToString()) > 1){
                                JArray array = JArray.Parse(obj["response"]["body"]["items"]["item"].ToString());
                                foreach (JObject itemObj in array)
                                {
                                    set = SettingItem(set, koreaList, itemObj);
                                }

                            }
                            else
                            {
                                SettingItem(set, koreaList, obj["response"]["body"]["items"]["item"] as JObject);
                            }
                            //cb_ServiceSelect.ItemsSource = koreaList;
                        }
                    });

               });

             task.Start();
        }

        private bool SettingItem(bool set, List<KoreaLocation> koreaList, JObject itemObj)
        {
            KoreaLocation korea = new KoreaLocation();
            korea.Addr1 = itemObj["addr1"] == null ? "" : itemObj["addr1"].ToString();
            korea.Addr2 = itemObj["addr2"] == null ? "" : itemObj["addr2"].ToString();
            korea.AreaCode = itemObj["areacode"] == null ? "" : itemObj["areacode"].ToString();
            korea.Cat1 = itemObj["cat1"] == null ? "" : itemObj["cat1"].ToString();
            korea.Cat2 = itemObj["cat2"] == null ? "" : itemObj["cat2"].ToString();
            korea.Cat3 = itemObj["cat3"] == null ? "" : itemObj["cat3"].ToString();
            korea.ContentID = itemObj["contentid"] == null ? "" : itemObj["contentid"].ToString();
            korea.ContentTypeID = itemObj["contenttypeid"] == null ? "" : itemObj["contenttypeid"].ToString();
            korea.CreatedTime = itemObj["createdtime"] == null ? "" : itemObj["createdtime"].ToString();
            korea.FirstImage = itemObj["firstimage"] == null ? "" : itemObj["firstimage"].ToString();
            korea.Image2 = itemObj["firstimage2"] == null ? "" : itemObj["firstimage2"].ToString();
            korea.MapX = itemObj["mapx"] == null ? "" : itemObj["mapx"].ToString();
            korea.MapY = itemObj["mapy"] == null ? "" : itemObj["mapy"].ToString();
            korea.mLevel = itemObj["mlevel"] == null ? "" : itemObj["mlevel"].ToString();
            korea.ModifiedTime = itemObj["modifiedtime"] == null ? "" : itemObj["modifiedtime"].ToString();
            korea.ReadCount = itemObj["readcount"] == null ? "" : itemObj["readcount"].ToString();
            korea.SigunguCode = itemObj["sigungucode"] == null ? "" : itemObj["sigungucode"].ToString();
            korea.Tel = itemObj["tel"] == null ? "" : itemObj["tel"].ToString();
            korea.Title = itemObj["title"] == null ? "" : itemObj["title"].ToString();
            korea.Dist = itemObj["dist"] == null ? "" : itemObj["dist"].ToString();
            koreaList.Add(korea);

            if (cb_ServiceSelect.SelectedIndex > -1 && korea.Cat1 == cb_ServiceSelect.SelectedValue.ToString())
            {

                if (String.IsNullOrEmpty(korea.MapX) == false && String.IsNullOrEmpty(korea.MapY) == false)
                {

                    double lng = Convert.ToDouble(korea.MapX); ;
                    double lat = Convert.ToDouble(korea.MapY);
                    string path = "ms-appx:///Assets/";
                    //"ms-appx:///Assets/customicon.png"
                    switch (korea.Cat1)
                    {
                        case "A01":
                            path += "img1.png";
                            break;
                        case "A02":
                            path += "img2.png";
                            break;
                        case "A03":
                            path += "img3.png";
                            break;
                        case "A04":
                            path += "img4.png";
                            break;
                        case "A05":
                            path += "img5.png";
                            break;
                        case "B01":
                            path += "img6.png";
                            break;
                        case "B02":
                            path += "img7.png";
                            break;
                    }

                    umc_MapControl.SetPOI(lat, lng, korea.Title, path, korea.FirstImage, korea);
                    if (!set)
                    {
                        umc_MapControl.SetLocation(lat, lng);
                        set = true;
                    }

                }



            }
            else if (cb_ServiceSelect.SelectedIndex == -1 || cb_ServiceSelect.SelectedIndex == 0)
            {
                if (String.IsNullOrEmpty(korea.MapX) == false && String.IsNullOrEmpty(korea.MapY) == false)
                {

                    double lng = Convert.ToDouble(korea.MapX); ;
                    double lat = Convert.ToDouble(korea.MapY);
                    string path = "ms-appx:///Assets/";
                    //"ms-appx:///Assets/customicon.png"
                    switch (korea.Cat1)
                    {
                        case "A01":
                            path += "img1.png";
                            break;
                        case "A02":
                            path += "img2.png";
                            break;
                        case "A03":
                            path += "img3.png";
                            break;
                        case "A04":
                            path += "img4.png";
                            break;
                        case "A05":
                            path += "img5.png";
                            break;
                        case "B01":
                            path += "img6.png";
                            break;
                        case "B02":
                            path += "img7.png";
                            break;
                    }

                    umc_MapControl.SetPOI(lat, lng, korea.Title, path, korea.FirstImage, korea);
                    if (!set)
                    {
                        umc_MapControl.SetLocation(lat, lng);
                        set = true;
                    }

                }
            }

            return set;
        }

        private void Item_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
           if(sender is ScreenItem)
            {

                ScreenItem item = sender as ScreenItem;


                if (item.Tag != null)
                {

                    KoreaLocation itemData = item.Tag as KoreaLocation;

                    if (String.IsNullOrEmpty(itemData.MapX) == false && String.IsNullOrEmpty(itemData.MapY) == false)
                    {

                        double lng = Convert.ToDouble(itemData.MapX); ;
                        double lat = Convert.ToDouble(itemData.MapY);
                        string path = "ms-appx:///Assets/";
                        //"ms-appx:///Assets/customicon.png"
                        switch (itemData.Cat1)
                        {
                            case "A01":
                                path += "img1.png";
                                break;
                            case "A02":
                                path += "img2.png";
                                break;
                            case "A03":
                                path += "img3.png";
                                break;
                            case "A04":
                                path += "img4.png";
                                break;
                            case "A05":
                                path += "img5.png";
                                break;
                            case "B01":
                                path += "img6.png";
                                break;
                            case "B02":
                                path += "img7.png";
                                break;
                        }
                         
                        umc_MapControl.SetLocation(lat,lng,itemData.Title, path);
                    }

                }


            }
        }


        #endregion
    }
}
