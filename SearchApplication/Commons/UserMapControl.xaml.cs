using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SearchApplication.Models;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SearchApplication.Commons
{
    public delegate void EventHandler(MapControl sender, object args);
    public sealed partial class UserMapControl : UserControl
    {

        string _contentID = "";
        string _contentTypeID = "";
        KoreaLocation _address = new KoreaLocation();
        public void SetCollapsed()
        {

            bd_InfoControl.Visibility = Visibility.Collapsed;
        }


        public event EventHandler eReceiveMsg;

        public UserMapControl()
        {
            this.InitializeComponent();

            this.Loaded += UserMapControl_Loaded;
           // mc_MapControl.PointerPressed += Mc_MapControl_PointerPressed;
           
            btn_Close.Click += Btn_Close_Click;
            tbtn_Info.Checked += Tbtn_Info_Checked;
            tbtn_Info.Unchecked += Tbtn_Info_Unchecked;
            this.SizeChanged += UserMapControl_SizeChanged;
            this.tbtn_Detail.Checked += Tbtn_Detail_Checked;
            this.tbtn_Detail.Unchecked += Tbtn_Detail_Unchecked;
            this.tbtn_Festival.Checked += Tbtn_Festival_Checked;
            tbtn_Festival.Unchecked += Tbtn_Festival_Unchecked;

            mc_MapControl.CenterChanged += Mc_MapControl_CenterChanged;
            dr_Info.returnEvent += Dr_Info_returnEvent;
            
        }

        private void Dr_Info_returnEvent(bool value)
        {
            if (value)
            {
                tbtn_Detail.Visibility = Visibility.Visible;
            }
            else
            {
                tbtn_Detail.IsChecked = false;
                tbtn_Detail.Visibility = Visibility.Collapsed;
            }
        }

        private void Mc_MapControl_CenterChanged(MapControl sender, object args)
        {
            if (eReceiveMsg != null) eReceiveMsg(sender, args);
        }

        private void Tbtn_Festival_Unchecked(object sender, RoutedEventArgs e)
        {
            sc_Festival.Visibility = Visibility.Collapsed;

        }

        private void Tbtn_Festival_Checked(object sender, RoutedEventArgs e)
        {
            tbtn_Detail.IsChecked = false;
            tbtn_Info.IsChecked = false;
            sc_Festival.Visibility = Visibility.Visible;
            sc_Viewer.Visibility = Visibility.Collapsed;
            dr_Info.Visibility = Visibility.Collapsed;
        }

        private void Tbtn_Detail_Unchecked(object sender, RoutedEventArgs e)
        {
            dr_Info.Visibility = Visibility.Collapsed;
        }

        private void Tbtn_Detail_Checked(object sender, RoutedEventArgs e)
        {
            tbtn_Info.IsChecked = false;
            tbtn_Festival.IsChecked = false;
            dr_Info.Visibility = Visibility.Visible;
            sc_Viewer.Visibility = Visibility.Collapsed;
            sc_Festival.Visibility = Visibility.Collapsed;

        }

        private void UserMapControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
             
        }

        private void Tbtn_Info_Unchecked(object sender, RoutedEventArgs e)
        {
            sc_Viewer.Visibility = Visibility.Collapsed ;
        }

        private void Tbtn_Info_Checked(object sender, RoutedEventArgs e)
        {
            tbtn_Detail.IsChecked = false;
            tbtn_Festival.IsChecked = false;
            sc_Viewer.Visibility = Visibility.Visible;
            dr_Info.Visibility = Visibility.Collapsed;
            sc_Festival.Visibility = Visibility.Collapsed;
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            if (bd_InfoControl.Visibility == Visibility.Visible) bd_InfoControl.Visibility = Visibility.Collapsed;
            sc_Viewer.Visibility = Visibility.Collapsed;
            tbtn_Info.IsChecked = false;
        }

         

        private void UserMapControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetLocation();

            
            mc_MapControl.Style = MapStyle.AerialWithRoads;



        }
        private async void CustomDataSource_BitmapRequested(
           CustomMapTileDataSource sender,
           MapTileBitmapRequestedEventArgs args)
        {
            var deferral = args.Request.GetDeferral();
            args.Request.PixelData = await CreateBitmapAsStreamAsync();
            deferral.Complete();
        }
        private async Task<RandomAccessStreamReference> CreateBitmapAsStreamAsync()
        {
            int pixelHeight = 256;
            int pixelWidth = 256;
            int bpp = 4;

            byte[] bytes = new byte[pixelHeight * pixelWidth * bpp];

            for (int y = 0; y < pixelHeight; y++)
            {
                for (int x = 0; x < pixelWidth; x++)
                {
                    int pixelIndex = y * pixelWidth + x;
                    int byteIndex = pixelIndex * bpp;

                    // Set the current pixel bytes.
                    bytes[byteIndex] = 0xff;        // Red
                    bytes[byteIndex + 1] = 0x00;    // Green
                    bytes[byteIndex + 2] = 0x00;    // Blue
                    bytes[byteIndex + 3] = 0x80;    // Alpha (0xff = fully opaque)
                }
            }

            // Create RandomAccessStream from byte array.
            InMemoryRandomAccessStream randomAccessStream =
                new InMemoryRandomAccessStream();
            IOutputStream outputStream = randomAccessStream.GetOutputStreamAt(0);
            DataWriter writer = new DataWriter(outputStream);
            writer.WriteBytes(bytes);
            await writer.StoreAsync();
            await writer.FlushAsync();
            return RandomAccessStreamReference.CreateFromStream(randomAccessStream);
        }

         

        public async void SetLocation()
        {

            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    // Get the current location.
                    Geolocator geolocator = new Geolocator();
                    Geoposition pos = await geolocator.GetGeopositionAsync();
                    Geopoint myLocation = pos.Coordinate.Point;

                    // Set the map location.
                    mc_MapControl.Center = myLocation;
                    mc_MapControl.ZoomLevel = 12;
                    mc_MapControl.LandmarksVisible = true;
                    break;

                case GeolocationAccessStatus.Denied:

                    BasicGeoposition cityPosition = new BasicGeoposition() { Latitude = 37.541, Longitude = 126.986 };
                    Geopoint cityCenter = new Geopoint(cityPosition);
                    mc_MapControl.Center = cityCenter;
                    mc_MapControl.ZoomLevel = 12;
                    mc_MapControl.LandmarksVisible = true;

                    // Handle the case  if access to location is denied.
                    break;

                case GeolocationAccessStatus.Unspecified:
                    // Handle the case if  an unspecified error occurs.
                    break;
            }
        }

        public void SetLocation(double latitude, double longitude, String title ="", String imgPath="")
        {
            BasicGeoposition cityPosition = new BasicGeoposition() { Latitude = latitude, Longitude = longitude };
            Geopoint cityCenter = new Geopoint(cityPosition);
            mc_MapControl.Center = cityCenter;
 
        }
        public void ItemClear()
        {
            mc_MapControl.Children.Clear();
            mc_MapControl.MapElements.Clear();
            mc_MapControl.ZoomLevel =12;
        }
        public void MyItemClear()
        {
            mc_MapControl.Children.Clear();
            mc_MapControl.MapElements.Clear();
            //mc_MapControl.ZoomLevel = 12;
        }
        public void SetPOI(double latitude, double longitude, String title, String imgPath, String mainPath, KoreaLocation location)
        {
            AddPointItem(latitude, longitude, mainPath,title,imgPath,location);
            
         

        }
        public void AddPointItem(double latitude, double longitude,String imgPath, String title, string servPath, KoreaLocation location)
        {

            if (latitude > 100) return;
            BasicGeoposition snPosition = new BasicGeoposition() { Latitude = latitude, Longitude = longitude };
            Geopoint snPoint = new Geopoint(snPosition);


            UserPOI item = new UserPOI();
            item.SetItem(imgPath, title, servPath, location);
            
            // Create a XAML border.
            item.PointerReleased += Item_PointerReleased;

            // Center the map over the POI.
            //mc_MapControl.Center = snPoint;
            //mc_MapControl.ZoomLevel = 12;

            // Add XAML to the map.
            
            mc_MapControl.Children.Add(item);
            MapControl.SetLocation(item, snPoint);
           // MapControl.SetNormalizedAnchorPoint(item, new Point(0.5, 0.5));

        }
        private async Task<string> Request_Json(String contenID, String contentTypeID)
        {
            // string url = "http://www.redmine.org/issues.json";

            string url =
             @"http://api.visitkorea.or.kr/openapi/service/rest/"+MainPage.SetLanguage+"/detailIntro?" +
             @"ServiceKey=" + MainPage.ServiceKey 
             + "&MobileOS=ETC&MobileApp=TestApp&contentId="+ contenID +"&contentTypeId="+ contentTypeID + "&introYN=Y&_type=json";
            HttpClient client = new HttpClient();
            Task<string> getStringTask = client.GetStringAsync(url);
            string result = await getStringTask;


            return result;

        }
        private async Task<string> Request_Images_Json(String contenID, String contentTypeID)
        {
            // string url = "http://www.redmine.org/issues.json";
            String imageValue = "Y";
          
            string url =
             @"http://api.visitkorea.or.kr/openapi/service/rest/"+MainPage.SetLanguage+"/detailImage?" +
             @"ServiceKey=" + MainPage.ServiceKey
             + "&MobileOS=ETC&MobileApp=TestApp&contentId=" + contenID + "&contentTypeId=" + contentTypeID + "&imageYN="+imageValue+"&_type=json";
            HttpClient client = new HttpClient();
            Task<string> getStringTask = client.GetStringAsync(url);
            string result = await getStringTask;
           

            return result;

        }
        private async Task<string> Request_FoodImages_Json(String contenID, String contentTypeID)
        {
            // string url = "http://www.redmine.org/issues.json";
          
            string url =
             @"http://api.visitkorea.or.kr/openapi/service/rest/"+MainPage.SetLanguage+"/detailImage?" +
             @"ServiceKey=" + MainPage.ServiceKey
             + "&MobileOS=ETC&MobileApp=TestApp&contentId=" + contenID + "&contentTypeId=" + contentTypeID + "&imageYN=N&_type=json";
            HttpClient client = new HttpClient();
            Task<string> getStringTask = client.GetStringAsync(url);
            string result = await getStringTask;
         
            return result;

        }
        private void ParseImagesJson(String json)
        {

            List<String> ImagePath = new List<string>();
            List<KoreaAreaCode> koreaAreaCodeList = new List<KoreaAreaCode>();
            st_Items.Children.Clear();
            JObject obj = JObject.Parse(json);
            if (img_IMage.Fill != null)
            {
                Ellipse el = new Ellipse();

                el.Width = 150;
                el.Height = 150;
                el.PointerReleased += El_PointerReleased;
                el.Fill = img_IMage.Fill;
                el.Margin = new Thickness(5, 0, 5, 0);
                st_Items.Children.Add(el);
                
            }
            if (obj["response"]["header"]["resultMsg"].ToString() == "OK")
            {
                try {
                    JArray array = JArray.Parse(obj["response"]["body"]["items"]["item"].ToString());
                    foreach (JObject itemObj in array)
                    {


                        if (itemObj["originimgurl"] != null) {
                            Image imgItem = new Image();
                            BitmapImage img = new BitmapImage();



                            img.UriSource = new Uri(itemObj["originimgurl"].ToString());
                           
                            imgItem.Source = img;
                            //imgItem.Width = 380;
                            //imgItem.Height = 380;
                            imgItem.Stretch = Stretch.Uniform;

                            Ellipse el1 = new Ellipse();

                            el1.Width = 150;
                            el1.Height = 150;
                            el1.PointerReleased += El_PointerReleased;
                            el1.Fill = new ImageBrush() { ImageSource = img, Stretch = Stretch.UniformToFill };
                            el1.Margin = new Thickness(5, 0, 5, 0);
                            st_Items.Children.Add(el1);
                            //st_Items.Children.Add(imgItem);
                           // ImagePath.Add(itemObj["originimgurl"].ToString());
                        }
                    }
                }
                catch
                {

                }
                //cb_ServiceSelect.ItemsSource = koreaAreaCodeList;
            }

            //el_Control.SetData(ImagePath);
        }
        private void ParseFoodImagesJson(String json)
        {
            List<KoreaAreaCode> koreaAreaCodeList = new List<KoreaAreaCode>();
            st_Serv.Children.Clear();

            if (img_IMage.Fill != null)
            {
                Ellipse el = new Ellipse();

                el.Width = 150;
                el.Height = 150;
                el.PointerReleased += El_PointerReleased;
                el.Fill = img_IMage.Fill;
                el.Margin = new Thickness(5, 0, 5, 0);
                st_Items.Children.Add(el);
            }
            JObject obj = JObject.Parse(json);

            if (obj["response"]["header"]["resultMsg"].ToString() == "OK")
            {
                try
                {
                    JArray array = JArray.Parse(obj["response"]["body"]["items"]["item"].ToString());
                    foreach (JObject itemObj in array)
                    {


                        if (itemObj["originimgurl"] != null)
                        {
                            Image imgItem = new Image();
                            BitmapImage img = new BitmapImage();



                            img.UriSource = new Uri(itemObj["originimgurl"].ToString());
                          
                            imgItem.Source = img;
                            //imgItem.Width = 380;
                            //imgItem.Height = 380;
                            imgItem.Stretch = Stretch.Uniform;


                            Ellipse el1 = new Ellipse();

                            el1.Width = 150;
                            el1.Height = 150;
                            el1.PointerReleased += El_PointerReleased;
                            el1.Fill = new ImageBrush() { ImageSource = img, Stretch = Stretch.UniformToFill };
                            el1.Margin = new Thickness(5, 0, 5, 0);
                            st_Serv.Children.Add(el1);
                        }
                    }
                }
                catch
                {

                }

                if(st_Serv.Children.Count == 0)
                {
                    cd_Width.Width = new GridLength(0);
                }
                else
                {
                    cd_Width.Width = new GridLength(400);
                }
                //cb_ServiceSelect.ItemsSource = koreaAreaCodeList;
            }
        }

        private void El_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if(img_IMage.Visibility == Visibility.Collapsed)
            {
                img_IMage.Visibility = Visibility.Visible;
            }
            img_IMage.Fill = (sender as Ellipse).Fill;
        }

        //http://api.visitkorea.or.kr/openapi/service/rest/"+MainPage.SetLanguage+"/detailImage
        private void ParseAreaJson(String json)
        {
            sc_Main.ScrollToHorizontalOffset(0);
            sc_Serv.ScrollToHorizontalOffset(0);

            List<KoreaAreaCode> koreaAreaCodeList = new List<KoreaAreaCode>();

            JObject obj = JObject.Parse(json);

            if (obj["response"]["header"]["resultMsg"].ToString() == "OK")
            {
                //  JArray array = JArray.Parse(obj["response"]["body"]["items"]["item"].ToString());
                JObject itemObj = obj["response"]["body"]["items"]["item"] as JObject;
                {
                    switch (itemObj["contenttypeid"].ToString() )
                    {
                        case "76":
                            DetailInfo_76 item = new DetailInfo_76();
                            //Fill="#7F726F6F" Stroke="#FF535353"
                            item.ContentID = itemObj["contentid"] != null ? itemObj["contentid"].ToString().Replace("<br>","\r\n") : "";
                            item.ContentTypeId = itemObj["contenttypeid"] != null ? itemObj["contenttypeid"].ToString().Replace("<br>", "\r\n") : ""; 
                            item.AccomCount = itemObj["accomcount"] != null ? itemObj["accomcount"].ToString().Replace("<br>", "\r\n") : ""; 
                            item.Expagerange = itemObj["expagerange"] != null ? itemObj["expagerange"].ToString().Replace("<br>", "\r\n") : "";
                            item.ExpGuide = itemObj["expguide"] != null ? itemObj["expguide"].ToString().Replace("<br>", "\r\n") : ""; 
                            item.HeriTage1 = itemObj["heritage1"] != null ? itemObj["heritage1"].ToString().Replace("<br>", "\r\n") : "";
                            item.InfoCenter = itemObj["infocenter"] != null ? itemObj["infocenter"].ToString().Replace("<br>", "\r\n") : "";
                            item.OpenDate = itemObj["opendate"] != null ? itemObj["opendate"].ToString().Replace("<br>", "\r\n") : ""; 
                            item.Parking = itemObj["parking"] != null ? itemObj["parking"].ToString().Replace("<br>", "\r\n") : ""; 
                            item.RestDate = itemObj["restdate"] != null ? itemObj["restdate"].ToString().Replace("<br>", "\r\n") : "";
                            item.UseSeason = itemObj["useseason"] != null ? itemObj["useseason"].ToString().Replace("<br>", "\r\n") : "";
                            item.UseTime = itemObj["usetime"] != null ? itemObj["usetime"].ToString().Replace("<br>", "\r\n") : "";
                            DetailView_76 itemview = new DetailView_76();
                            itemview.SetItem(item.AccomCount, item.Expagerange, item.ExpGuide, item.HeriTage1, item.InfoCenter
                                , item.OpenDate, item.Parking, item.RestDate, item.UseSeason, item.UseTime, _address.Addr1 + "\r\n" + _address.Addr2);
                            bd_Child.Child = itemview;
                            
                            //txt_Note.Text = str;
                            break;
                        case "78":
                            DetailInfo_78 item78 = new DetailInfo_78();
                            //Fill="#7F726F6F" Stroke="#FF535353"
                            item78.ContentID = itemObj["contentid"] != null ? itemObj["contentid"].ToString().Replace("<br>", "\r\n") : "";
                            item78.ContentTypeId = itemObj["contenttypeid"] != null ? itemObj["contenttypeid"].ToString().Replace("<br>", "\r\n") : "";
                            item78.Accomcountculture = itemObj["accomcountculture"] != null ? itemObj["accomcountculture"].ToString().Replace("<br>", "\r\n") : "";
                            item78.INfocenterCulture = itemObj["infocenterculture"] != null ? itemObj["infocenterculture"].ToString().Replace("<br>", "\r\n") : "";
                            item78.ParkingCulture = itemObj["parkingculture"] != null ? itemObj["parkingculture"].ToString().Replace("<br>", "\r\n") : "";
                            item78.ParkingFree = itemObj["parkingfree"] != null ? itemObj["parkingfree"].ToString().Replace("<br>", "\r\n") : "";
                            item78.RestDateCulture = itemObj["restdateculture"] != null ? itemObj["restdateculture"].ToString().Replace("<br>", "\r\n") : "";
                            item78.UserFree = itemObj["usefee"] != null ? itemObj["usefee"].ToString().Replace("<br>", "\r\n") : "";
                            item78.UseTimeCulture = itemObj["usetimeculture"] != null ? itemObj["usetimeculture"].ToString().Replace("<br>", "\r\n") : "";
                            item78.Scale = itemObj["scale"] != null ? itemObj["scale"].ToString().Replace("<br>", "\r\n") : "";
                            item78.SpendTime = itemObj["spendtime"] != null ? itemObj["spendtime"].ToString().Replace("<br>", "\r\n") : "";
                            
                            DetailView_78 itemview78 = new DetailView_78();
                            itemview78.SetItem(item78.Accomcountculture, item78.INfocenterCulture, item78.ParkingCulture, item78.ParkingFree, item78.RestDateCulture
                                , item78.UserFree, item78.UseTimeCulture, item78.Scale, item78.SpendTime, _address.Addr1 + "\r\n" + _address.Addr2);
                            bd_Child.Child = itemview78;

                            
                            break;

                        case "85":
                            DetailInfo_85 item85 = new DetailInfo_85();
                            //Fill="#7F726F6F" Stroke="#FF535353"
                            item85.ContentID = itemObj["contentid"] != null ? itemObj["contentid"].ToString().Replace("<br>", "\r\n") : "";
                            item85.ContentTypeId = itemObj["contenttypeid"] != null ? itemObj["contenttypeid"].ToString().Replace("<br>", "\r\n") : "";
                            item85.AgeLimit = itemObj["agelimit"] != null ? itemObj["agelimit"].ToString().Replace("<br>", "\r\n") : "";
                            item85.BookingPlace = itemObj["bookingplace"] != null ? itemObj["bookingplace"].ToString().Replace("<br>", "\r\n") : "";
                            item85.DiscountInfoFestival = itemObj["discountinfofestival"] != null ? itemObj["discountinfofestival"].ToString().Replace("<br>", "\r\n") : "";
                            item85.EventEndDate = itemObj["eventenddate"] != null ? itemObj["eventenddate"].ToString().Replace("<br>", "\r\n") : "";
                            item85.EventHomepage = itemObj["eventhomepage"] != null ? itemObj["eventhomepage"].ToString().Replace("<br>", "\r\n") : "";
                            item85.EventPlace = itemObj["eventplace"] != null ? itemObj["eventplace"].ToString().Replace("<br>", "\r\n") : "";
                            item85.EventStartDate = itemObj["eventstartdate"] != null ? itemObj["eventstartdate"].ToString().Replace("<br>", "\r\n") : "";
                            item85.PlaceInfo = itemObj["placeinfo"] != null ? itemObj["placeinfo"].ToString().Replace("<br>", "\r\n") : "";
                            item85.PlayTime = itemObj["playtime"] != null ? itemObj["playtime"].ToString().Replace("<br>", "\r\n") : "";
                             
                            item85.Program = itemObj["program"] != null ? itemObj["program"].ToString().Replace("<br>", "\r\n") : "";
                            item85.SpendTimeFestival = itemObj["spendtimefestival"] != null ? itemObj["spendtimefestival"].ToString().Replace("<br>", "\r\n") : "";
                            item85.Sponsor1= itemObj["sponsor1"] != null ? itemObj["sponsor1"].ToString().Replace("<br>", "\r\n") : "";
                            item85.Sponsor1Tel = itemObj["sponsor1tel"] != null ? itemObj["sponsor1tel"].ToString().Replace("<br>", "\r\n") : "";
                            item85.Sponsor2 = itemObj["sponsor2"] != null ? itemObj["sponsor2"].ToString().Replace("<br>", "\r\n") : "";
                            item85.Sponsor2Tel = itemObj["sponsor2tel"] != null ? itemObj["sponsor2tel"].ToString().Replace("<br>", "\r\n") : "";
                            item85.SubEvent = itemObj["subevent"] != null ? itemObj["subevent"].ToString().Replace("<br>", "\r\n") : "";
                            item85.UseTimeFestival = itemObj["usetimefestival"] != null ? itemObj["usetimefestival"].ToString().Replace("<br>", "\r\n") : "";
                             
                            DetailView_85 itemview85 = new DetailView_85();
                            itemview85.SetItem(item85.AgeLimit, item85.BookingPlace, item85.DiscountInfoFestival, item85.EventEndDate, item85.EventHomepage
                                , item85.EventPlace, item85.EventStartDate, item85.PlaceInfo, item85.PlayTime, item85.Program, 
                                item85.SpendTimeFestival, item85.Sponsor1, item85.Sponsor1Tel, item85.Sponsor2, item85.Sponsor2Tel, item85.SubEvent, item85.UseTimeFestival
                                , _address.Addr1 + "\r\n" + _address.Addr2
                                );
                            bd_Child.Child = itemview85;


                            break;
                        case "75":
                            DetailInfo_75 item75 = new DetailInfo_75();
                            //Fill="#7F726F6F" Stroke="#FF535353"
                            item75.ContentID = itemObj["contentid"] != null ? itemObj["contentid"].ToString().Replace("<br>", "\r\n") : "";
                            item75.ContentTypeId = itemObj["contenttypeid"] != null ? itemObj["contenttypeid"].ToString().Replace("<br>", "\r\n") : "";
                            item75.Accomcountleports = itemObj["accomcountleports"] != null ? itemObj["accomcountleports"].ToString().Replace("<br>", "\r\n") : "";
                            item75.Expagerangeleports = itemObj["expagerangeleports"] != null ? itemObj["expagerangeleports"].ToString().Replace("<br>", "\r\n") : "";
                            item75.Infocenterleports = itemObj["infocenterleports"] != null ? itemObj["infocenterleports"].ToString().Replace("<br>", "\r\n") : "";
                            item75.Openperiod = itemObj["openperiod"] != null ? itemObj["openperiod"].ToString().Replace("<br>", "\r\n") : "";
                            item75.Parkingfeeleports = itemObj["parkingfeeleports"] != null ? itemObj["parkingfeeleports"].ToString().Replace("<br>", "\r\n") : "";
                            item75.Parking = itemObj["parkingleports"] != null ? itemObj["parkingleports"].ToString().Replace("<br>", "\r\n") : "";
                            item75.Resevation = itemObj["reservation"] != null ? itemObj["reservation"].ToString().Replace("<br>", "\r\n") : "";
                            item75.Restdateleports = itemObj["restdateleports"] != null ? itemObj["restdateleports"].ToString().Replace("<br>", "\r\n") : "";
                            item75.Scale = itemObj["scaleleports"] != null ? itemObj["scaleleports"].ToString().Replace("<br>", "\r\n") : "";

                            item75.UseFree = itemObj["usefeeleports"] != null ? itemObj["usefeeleports"].ToString().Replace("<br>", "\r\n") : "";
                            item75.UseTime = itemObj["usetimeleports"] != null ? itemObj["usetimeleports"].ToString().Replace("<br>", "\r\n") : "";
                             

                            DetailView_75 itemview75 = new DetailView_75();
                            itemview75.SetItem(item75.Accomcountleports,item75.Expagerangeleports, item75.Infocenterleports, item75.Openperiod, item75.Parkingfeeleports, item75.Parking,
                                item75.Resevation, item75.Restdateleports, item75.Scale, item75.UseFree, item75.UseTime, _address.Addr1 + "\r\n" + _address.Addr2);
                            bd_Child.Child = itemview75;


                            break;

                        case "80":
                            DetailInfo_80 item80 = new DetailInfo_80();
                            //Fill="#7F726F6F" Stroke="#FF535353"
                            item80.ContentID = itemObj["contentid"] != null ? itemObj["contentid"].ToString().Replace("<br>", "\r\n") : "";
                            item80.ContentTypeId = itemObj["contenttypeid"] != null ? itemObj["contenttypeid"].ToString().Replace("<br>", "\r\n") : "";
                            item80.Accomcount = itemObj["accomcountlodging"] != null ? itemObj["accomcountlodging"].ToString().Replace("<br>", "\r\n") : "";
                            item80.Benika = itemObj["benika"] != null ? itemObj["benika"].ToString().Replace("<br>", "\r\n") : "";
                            item80.Checkintime = itemObj["checkintime"] != null ? itemObj["checkintime"].ToString().Replace("<br>", "\r\n") : "";
                            item80.CheckOutTime = itemObj["checkouttime"] != null ? itemObj["checkouttime"].ToString().Replace("<br>", "\r\n") : "";
                            item80.ChkCooking = itemObj["chkcooking"] != null ? itemObj["chkcooking"].ToString().Replace("<br>", "\r\n") : "";
                            item80.Foodplace = itemObj["foodplace"] != null ? itemObj["foodplace"].ToString().Replace("<br>", "\r\n") : "";
                            item80.GoodStay = itemObj["goodstay"] != null ? itemObj["goodstay"].ToString().Replace("<br>", "\r\n") : "";
                            item80.Hanok = itemObj["hanok"] != null ? itemObj["hanok"].ToString().Replace("<br>", "\r\n") : "";
                            item80.Infocenter = itemObj["infocenterlodging"] != null ? itemObj["infocenterlodging"].ToString().Replace("<br>", "\r\n") : "";
                            item80.Parking = itemObj["parkinglodging"] != null ? itemObj["parkinglodging"].ToString().Replace("<br>", "\r\n") : "";
                            item80.PickUp = itemObj["pickup"] != null ? itemObj["pickup"].ToString().Replace("<br>", "\r\n") : "";

                            item80.RoomCount = itemObj["roomcount"] != null ? itemObj["roomcount"].ToString().Replace("<br>", "\r\n") : "";
                            item80.Reservation = itemObj["reservationlodging"] != null ? itemObj["reservationlodging"].ToString().Replace("<br>", "\r\n") : "";
                            item80.ReservationUrl = itemObj["reservationurl"] != null ? itemObj["reservationurl"].ToString().Replace("<br>", "\r\n") : "";
                            item80.RoomType = itemObj["roomtype"] != null ? itemObj["roomtype"].ToString().Replace("<br>", "\r\n") : "";
                            item80.Scale = itemObj["scalelodging"] != null ? itemObj["scalelodging"].ToString().Replace("<br>", "\r\n") : "";
                            item80.Subfacility = itemObj["subfacility"] != null ? itemObj["subfacility"].ToString().Replace("<br>", "\r\n") : "";
                          
                            DetailView_80 itemview80 = new DetailView_80();
                            itemview80.SetItem(item80.Accomcount, item80.Benika, item80.Checkintime, item80.CheckOutTime, item80.ChkCooking, item80.Foodplace,
                                item80.GoodStay, item80.Hanok, item80.Infocenter, item80.Parking, item80.PickUp, item80.RoomCount, item80.Reservation, item80.ReservationUrl,
                                item80.RoomType, item80.Scale, item80.Subfacility, _address.Addr1 + "\r\n" + _address.Addr2);
                            bd_Child.Child = itemview80;


                            break;


                        case "79":
                            DetailInfo_79 item79 = new DetailInfo_79();
                            //Fill="#7F726F6F" Stroke="#FF535353"
                            item79.ContentID = itemObj["contentid"] != null ? itemObj["contentid"].ToString().Replace("<br>", "\r\n") : "";
                            item79.ContentTypeId = itemObj["contenttypeid"] != null ? itemObj["contenttypeid"].ToString().Replace("<br>", "\r\n") : "";
                            item79.FairDay = itemObj["fairday"] != null ? itemObj["fairday"].ToString().Replace("<br>", "\r\n") : "";
                            item79.Infocenter = itemObj["infocentershopping"] != null ? itemObj["infocentershopping"].ToString().Replace("<br>", "\r\n") : "";
                            item79.OpenDate = itemObj["opendateshopping"] != null ? itemObj["opendateshopping"].ToString().Replace("<br>", "\r\n") : "";
                            item79.OpenTime = itemObj["opentime"] != null ? itemObj["opentime"].ToString().Replace("<br>", "\r\n") : "";
                            item79.Parking = itemObj["parkingshopping"] != null ? itemObj["parkingshopping"].ToString().Replace("<br>", "\r\n") : "";
                            item79.RestDate = itemObj["restdateshopping"] != null ? itemObj["restdateshopping"].ToString().Replace("<br>", "\r\n") : "";
                            item79.RestRoom = itemObj["restroom"] != null ? itemObj["restroom"].ToString().Replace("<br>", "\r\n") : "";
                            item79.Saleitem = itemObj["saleitem"] != null ? itemObj["saleitem"].ToString().Replace("<br>", "\r\n") : "";
                            item79.Scale = itemObj["scaleshopping"] != null ? itemObj["scaleshopping"].ToString().Replace("<br>", "\r\n") : "";

                            item79.ShopGuide = itemObj["shopquide"] != null ? itemObj["shopquide"].ToString().Replace("<br>", "\r\n") : "";
                          

                            DetailView_79 itemview79 = new DetailView_79();
                            itemview79.SetItem(item79.FairDay, item79.Infocenter, item79.OpenDate, item79.OpenTime, item79.Parking, item79.RestDate,
                                item79.RestRoom, item79.Saleitem, item79.Scale,  item79.ShopGuide, _address.Addr1 + "\r\n" + _address.Addr2);
                            bd_Child.Child = itemview79;


                            break;

                        case "82":
                        case "39":
                            DetailInfo_39 item39 = new DetailInfo_39();
                            //Fill="#7F726F6F" Stroke="#FF535353"
                            item39.ContentID = itemObj["contentid"] != null ? itemObj["contentid"].ToString().Replace("<br>", "\r\n") : "";
                            item39.ContentTypeId = itemObj["contenttypeid"] != null ? itemObj["contenttypeid"].ToString().Replace("<br>", "\r\n") : "";
                            item39.FirstMenu = itemObj["firstmenu"] != null ? itemObj["firstmenu"].ToString().Replace("<br>", "\r\n") : "";
                            item39.Infocenter = itemObj["infocenterfood"] != null ? itemObj["infocenterfood"].ToString().Replace("<br>", "\r\n") : "";
                            item39.OpenDate = itemObj["opendatefood"] != null ? itemObj["opendatefood"].ToString().Replace("<br>", "\r\n") : "";
                            item39.OpenTime = itemObj["opentimefood"] != null ? itemObj["opentimefood"].ToString().Replace("<br>", "\r\n") : "";
                            item39.Parking = itemObj["parkingfood"] != null ? itemObj["parkingfood"].ToString().Replace("<br>", "\r\n") : "";
                            item39.Resrvation = itemObj["reservationfood"] != null ? itemObj["reservationfood"].ToString().Replace("<br>", "\r\n") : "";
                            item39.RestDate = itemObj["restdatefood"] != null ? itemObj["restdatefood"].ToString().Replace("<br>", "\r\n") : "";
                          
                           
                            item39.Scale = itemObj["scalefood"] != null ? itemObj["scalefood"].ToString().Replace("<br>", "\r\n") : "";
                            item39.Seat = itemObj["seat"] != null ? itemObj["seat"].ToString().Replace("<br>", "\r\n") : "";
                            item39.Smoking = itemObj["smoking"] != null ? itemObj["smoking"].ToString().Replace("<br>", "\r\n") : "";
                            item39.TreatMenu = itemObj["treatmenu"] != null ? itemObj["treatmenu"].ToString().Replace("<br>", "\r\n") : "";


                            DetailView_39 itemview39 = new DetailView_39();
                            itemview39.SetItem(item39.FirstMenu, item39.Infocenter, item39.OpenDate, item39.OpenTime, item39.Parking, item39.Resrvation,
                                item39.RestDate, item39.Scale, item39.Seat, item39.Smoking, item39.TreatMenu, _address.Addr1+ "\r\n"+_address.Addr2);
                            bd_Child.Child = itemview39;


                            break;


                        case "77":
                            DetailInfo_77 item77 = new DetailInfo_77();
                            //Fill="#7F726F6F" Stroke="#FF535353"
                            item77.ContentID = itemObj["contentid"] != null ? itemObj["contentid"].ToString().Replace("<br>", "\r\n") : "";
                            item77.ContentTypeId = itemObj["contenttypeid"] != null ? itemObj["contenttypeid"].ToString().Replace("<br>", "\r\n") : "";
                            item77.ChkCreditCard = itemObj["chkcreditcardtraffic"] != null ? itemObj["chkcreditcardtraffic"].ToString().Replace("<br>", "\r\n") : "";
                            item77.Conven = itemObj["conven"] != null ? itemObj["conven"].ToString().Replace("<br>", "\r\n") : "";
                            item77.Disablefacility = itemObj["disablefacility"] != null ? itemObj["disablefacility"].ToString().Replace("<br>", "\r\n") : "";
                            item77.ForeignerInfocenter = itemObj["foreignerinfocenter"] != null ? itemObj["foreignerinfocenter"].ToString().Replace("<br>", "\r\n") : "";
                            item77.Infocenter = itemObj["infocentertraffic"] != null ? itemObj["infocentertraffic"].ToString().Replace("<br>", "\r\n") : "";
                            item77.MainRoute = itemObj["mainroute"] != null ? itemObj["mainroute"].ToString().Replace("<br>", "\r\n") : "";
                            item77.operationTime = itemObj["operationtraffic"] != null ? itemObj["operationtraffic"].ToString().Replace("<br>", "\r\n") : "";


                            item77.Parking = itemObj["parkingtraffic"] != null ? itemObj["parkingtraffic"].ToString().Replace("<br>", "\r\n") : "";
                            item77.RestRoom = itemObj["restroomtraffic"] != null ? itemObj["restroomtraffic"].ToString().Replace("<br>", "\r\n") : "";
                            item77.Shipinfo = itemObj["shipinfo"] != null ? itemObj["shipinfo"].ToString().Replace("<br>", "\r\n") : "";
                            

                            DetailView_77 itemview77 = new DetailView_77();
                            itemview77.SetItem(item77.ChkCreditCard,item77.Conven, item77.Disablefacility, item77.ForeignerInfocenter, item77.Infocenter,
                                item77.MainRoute, item77.operationTime, item77.Parking, item77.RestRoom, item77.Shipinfo, _address.Addr1 + "\r\n" + _address.Addr2);
                            bd_Child.Child = itemview77;


                            break;
                        default:

                            bd_Child.Child = new Grid();
                            break;

                    }
                }

               // cb_AreaSelect.ItemsSource = koreaAreaCodeList;
            }
        }

       
        private async void Item_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (sender is UserPOI)
            {
                UserPOI item = sender as UserPOI;

                _address = item.Tag as KoreaLocation;
                if (String.IsNullOrEmpty((item.Tag as KoreaLocation).FirstImage) == false)
                {
                    BitmapImage img = new BitmapImage();

                    ImageBrush imgBr = new ImageBrush();

                    img.UriSource = new Uri((item.Tag as KoreaLocation).FirstImage);
                    imgBr.ImageSource = img;
                    imgBr.Stretch = Stretch.UniformToFill;
                    img_IMage.Fill = imgBr;

                    img_IMage.Visibility = Visibility.Visible;
                }
                else
                {
                    img_IMage.Fill = null;
                    img_IMage.Visibility = Visibility.Collapsed;

                }
                txt_Title.Text = (item.Tag as KoreaLocation).Title;

                dr_Info.SetData((item.Tag as KoreaLocation).ContentID, (item.Tag as KoreaLocation).ContentTypeID);
                String data = await Request_Json((item.Tag as KoreaLocation).ContentID, (item.Tag as KoreaLocation).ContentTypeID);
                ParseAreaJson(data);
                String data2 = await Request_Images_Json((item.Tag as KoreaLocation).ContentID, (item.Tag as KoreaLocation).ContentTypeID);
                ParseImagesJson(data2);
               // SetLocation(Convert.ToDouble((item.Tag as KoreaLocation).MapY), Convert.ToDouble((item.Tag as KoreaLocation).MapX));
                if ((item.Tag as KoreaLocation).ContentTypeID == "39" || (item.Tag as KoreaLocation).ContentTypeID == "82")
                {
                    String data3 = await Request_FoodImages_Json((item.Tag as KoreaLocation).ContentID, (item.Tag as KoreaLocation).ContentTypeID);
                    ParseFoodImagesJson(data3);
                }
                else
                {
                    cd_Width.Width = new GridLength(0);
                    st_Serv.Children.Clear();
                }
                st_Festival.Children.Clear();
                List<KoreaLocation> itemObject =
                MainPage._festivalList.Where(a => a.ContentID == (item.Tag as KoreaLocation).ContentID && a.ContentTypeID == (item.Tag as KoreaLocation).ContentTypeID).ToList();
                if(itemObject != null && itemObject.Count() > 0)
                {
                    tbtn_Festival.Visibility = Visibility.Visible;
                    foreach(KoreaLocation itemin in itemObject)
                    {
                        FestivalControl festy = new FestivalControl();
                        festy.SetItem(itemin.FirstImage,itemin.EventStartDate
                            , itemin.EventEndDate,itemin.Title);
                        st_Festival.Children.Add(festy);
                    }

                    
                }
                else
                {
                    tbtn_Festival.IsChecked = false;
                    tbtn_Festival.Visibility = Visibility.Collapsed;
                }
                if (bd_InfoControl.Visibility == Visibility.Collapsed) bd_InfoControl.Visibility = Visibility.Visible;
            }
        }
        public Geopoint GetCenter()
        {
            return mc_MapControl.Center;

        }
        private async void ShowRouteOnMap(double latitude, double longitude)
        {
            // Start at Microsoft in Redmond, Washington.
            BasicGeoposition startLocation = new BasicGeoposition() { Latitude = 37.541, Longitude = 126.986 };

            // End at the city of Seattle, Washington.
            BasicGeoposition endLocation = new BasicGeoposition() { Latitude = latitude, Longitude = longitude };


            // Get the route between the points.
            MapRouteFinderResult routeResult =
                  await MapRouteFinder.GetWalkingRouteAsync(
                  new Geopoint(startLocation),
                  new Geopoint(endLocation));
            
            //,
            //      MapRouteOptimization.Time,
            //      MapRouteRestrictions.None);

            if (routeResult.Status == MapRouteFinderStatus.Success)
            {
                // Use the route to initialize a MapRouteView.
                MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                viewOfRoute.RouteColor = Colors.Yellow;
                viewOfRoute.OutlineColor = Colors.Black;

                // Add the new MapRouteView to the Routes collection
                // of the MapControl.
                mc_MapControl.Routes.Add(viewOfRoute);

                // Fit the MapControl to the route.
                await mc_MapControl.TrySetViewBoundsAsync(
                      routeResult.Route.BoundingBox,
                      null,
                      Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);
            }
        }
    }
}
