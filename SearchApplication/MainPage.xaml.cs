using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using SearchApplication.Commons;
using SearchApplication.Models;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
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
        public static String ServiceKey = "6Gp0b%2Frx8wsUsUBLObMt54JbaM4c8npnLTK6F5Eo2vkYKQdG66lrABY3scRuoJFmUFb4%2B0kn36vNJycBPMn9yw%3D%3D"; // Open API Service Key


        //http://newsky2.kma.go.kr/service/MiddleFrcstInfoService
        //"EngService";
        //"JpnService";
        public static String SetLanguage = "EngService"; // Service 지역
        List<AreaDataModel> itemList = new List<AreaDataModel>();
        public MainPage()
        {
            this.InitializeComponent();
            LoadArea();//기본 구역 정보 
            LoadService(); // 기본 서비스 정보
            btn_Search.Click += Btn_Search_Click;
            btn_Research.Click += Btn_Research_Click;
            // Request_Weather();
            FileLoad();

            cb_Main.SelectionChanged += Cb_Main_SelectionChanged;
            cb_Serv1.SelectionChanged += Cb_Serv1_SelectionChanged;
            btn_Search1.Click += Btn_Search1_Click;
        }

        private async void Btn_Search1_Click(object sender, RoutedEventArgs e)
        {

            String str1 = cb_Main.SelectedValue == null ? "" : cb_Main.SelectedValue.ToString();
            String str2 = cb_Serv1.SelectedValue == null ?"": cb_Serv1.SelectedValue.ToString();
            String str3 = cb_Serv2.SelectedValue == null ? "" : cb_Serv2.SelectedValue.ToString();
           


            AreaDataModel item = itemList.Where(a => a.Step1 == str1 &&
            a.Step2 == str2 &&
            a.Step3 == str3).FirstOrDefault();

            if (item != null)
            {
                String buf = await Request_Weather(item.X, item.Y);


                //ParseWeatherJson(buf);
                //umc_MapControl.SetLocation(Convert.ToDouble(item.Lng), Convert.ToDouble(item.Lat));
                //String path =  "ms-appx:///Assets/img1";
                //umc_MapControl.SetPOI(Convert.ToDouble(item.Lng), Convert.ToDouble(item.Lat), "노원구", path, path, null);


                //for (int i = 0; i < itemList.Count(); i++)
                //{

                List<ResponsData> itemLists = ParseWeatherJson(buf);

                if (itemLists != null && itemLists.Count() > 0)
                {
                    String sky = itemLists.Where(a => a.Category == "SKY").Select(a => a.ObsrValue).FirstOrDefault();
                    String pty = itemLists.Where(a => a.Category == "PTY").Select(a => a.ObsrValue).FirstOrDefault();
                    String RN1 = itemLists.Where(a => a.Category == "RN1").Select(a => a.ObsrValue).FirstOrDefault();
                    String R06 = itemLists.Where(a => a.Category == "R06").Select(a => a.ObsrValue).FirstOrDefault();
                    String S06 = itemLists.Where(a => a.Category == "S06").Select(a => a.ObsrValue).FirstOrDefault();
                    String LGT = itemLists.Where(a => a.Category == "LGT").Select(a => a.ObsrValue).FirstOrDefault();
                    String UUU = itemLists.Where(a => a.Category == "UUU").Select(a => a.ObsrValue).FirstOrDefault();
                    String VVV = itemLists.Where(a => a.Category == "VVV").Select(a => a.ObsrValue).FirstOrDefault();

                    sky = String.IsNullOrEmpty(sky) == true ? "" : sky;
                    pty = String.IsNullOrEmpty(pty) == true ? "" : pty;
                    RN1 = String.IsNullOrEmpty(RN1) == true ? "" : RN1;
                    R06 = String.IsNullOrEmpty(R06) == true ? "" : R06;
                    S06 = String.IsNullOrEmpty(S06) == true ? "" : S06;
                    LGT = String.IsNullOrEmpty(LGT) == true ? "" : LGT;
                    UUU = String.IsNullOrEmpty(UUU) == true ? "" : UUU;
                    VVV = String.IsNullOrEmpty(VVV) == true ? "" : VVV;


                    umc_MapControl.SetPOI(Convert.ToDouble(item.Lng), Convert.ToDouble(item.Lat), "", sky, pty, RN1, S06, LGT, UUU, VVV);

                    umc_MapControl.SetLocation(Convert.ToDouble(item.Lng), Convert.ToDouble(item.Lat));
                    //    }
                    //}
                }
            }
        }

        private void Cb_Serv1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            
            if(cb_Serv1.SelectedValue != null && cb_Main.SelectedValue != null)
            cb_Serv2.ItemsSource = itemList.Where(a => a.Step1 == cb_Main.SelectedValue.ToString()
                                                    && a.Step2 == cb_Serv1.SelectedValue.ToString()
            ).GroupBy(a => a.Step3);
            else
            {
                cb_Serv2.ItemsSource = null;
            }
        }

        private void Cb_Main_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_Main.SelectedValue != null)

            cb_Serv1.ItemsSource = itemList.Where(a => a.Step1 == cb_Main.SelectedValue.ToString()).GroupBy(a => a.Step2 );
           
        }

        private async void FileLoad()
        {
           itemList= await XmlParser.LoadFile();

            if(itemList != null)
            cb_Main.ItemsSource = itemList.GroupBy(a => a.Step1);


            //List<AreaDataModel> subList = itemList.Where(a =>a.Step3 == "").ToList<AreaDataModel>();

            //for (int i = 0; i < subList.Count(); i++)
            //{
            //    String buf = await Request_Weather(subList[i].X, subList[i].Y);
            //    List<ResponsData> itemLists = ParseWeatherJson(buf);

            //    if (itemLists != null && itemLists.Count() > 0)
            //    {
            //        String sky = itemLists.Where(a => a.Category == "SKY").Select(a => a.ObsrValue).FirstOrDefault();
            //        String pty = itemLists.Where(a => a.Category == "PTY").Select(a => a.ObsrValue).FirstOrDefault();
            //        String RN1 = itemLists.Where(a => a.Category == "RN1").Select(a => a.ObsrValue).FirstOrDefault();
            //        String R06 = itemLists.Where(a => a.Category == "R06").Select(a => a.ObsrValue).FirstOrDefault();
            //        String S06 = itemLists.Where(a => a.Category == "S06").Select(a => a.ObsrValue).FirstOrDefault();
            //        String LGT = itemLists.Where(a => a.Category == "LGT").Select(a => a.ObsrValue).FirstOrDefault();
            //        String UUU = itemLists.Where(a => a.Category == "UUU").Select(a => a.ObsrValue).FirstOrDefault();
            //        String VVV = itemLists.Where(a => a.Category == "VVV").Select(a => a.ObsrValue).FirstOrDefault();

            //        sky = String.IsNullOrEmpty(sky) == true ? "" : sky;
            //        pty = String.IsNullOrEmpty(pty) == true ? "" : pty;
            //        RN1 = String.IsNullOrEmpty(RN1) == true ? "" : RN1;
            //        R06 = String.IsNullOrEmpty(R06) == true ? "" : R06;
            //        S06 = String.IsNullOrEmpty(S06) == true ? "" : S06;
            //        LGT = String.IsNullOrEmpty(LGT) == true ? "" : LGT;
            //        UUU = String.IsNullOrEmpty(UUU) == true ? "" : UUU;
            //        VVV = String.IsNullOrEmpty(VVV) == true ? "" : VVV;


            //        umc_MapControl.SetPOI(Convert.ToDouble(subList[i].Lng), Convert.ToDouble(subList[i].Lat), subList[i].Step2, sky, pty, RN1, S06, LGT, UUU, VVV);
            //    }
            //}
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
        private async Task<string> Request_Weather(String nx, String ny)
        {
            // string url = "http://www.redmine.org/issues.json";

            string url =
             @"http://newsky2.kma.go.kr/service/SecndSrtpdFrcstInfoService2/ForecastGrib?" +
             @"ServiceKey=" +ServiceKey + "&numOfRows=100&base_date="+DateTime.Now.ToString("yyyyMMdd")+"&base_time="+ DateTime.Now.AddHours(-1).ToString("HH00")
             +"&nx="+nx+"&ny="+ny+"&pageNo=1&_type=json";
            HttpClient client = new HttpClient();
            Task<string> getStringTask = client.GetStringAsync(url);
            string result = await getStringTask;
            return result;

        }


        private List<ResponsData> ParseWeatherJson(String json)
        {
            //List<AreaDataModel> koreaAreaCodeList = new List<AreaDataModel>();

            JObject obj = JObject.Parse(json);
            List<ResponsData> itemList = new List<ResponsData>();
            if (obj["response"]["header"]["resultMsg"].ToString() == "OK")
            {
                JArray array = JArray.Parse(obj["response"]["body"]["items"]["item"].ToString());
                foreach (JObject itemObj in array)
                {
                    ResponsData area = new ResponsData();
                    area.BaseDate = itemObj["baseDate"].ToString();
                    area.BaseTime = itemObj["baseTime"].ToString();
                    area.Category = itemObj["category"].ToString();
                    area.NX = itemObj["nx"].ToString();
                    area.NY = itemObj["ny"].ToString();
                    area.ObsrValue = itemObj["obsrValue"].ToString();



                    itemList.Add(area);

                }

                
            }
            //lb_List.Items.Add(area);
            return itemList;
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
