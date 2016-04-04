using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SearchApplication.Commons
{
    public sealed partial class WeatherControl : UserControl
    {
        public WeatherControl()
        {
            this.InitializeComponent();
        }

        public void SetItem(String Sky, String Pty, String Rn1, String S06, String Lgt, String Uuu, String Vvv, String title = "")
        {
            string path = "ms-appx:///Assets/";

            String path_Else = "1";

            switch (Sky)
            {
                case "1":
                    path_Else = "1";
                    break;
                case "2":
                    path_Else = "2";
                    break;
                case "3":
                    path_Else = "3";
                    break;
                case "4":
                    path_Else = "4";
                    break;
                default:
                    break;

            }

            switch (Pty)
            {
                case "0":
                    break;
                case "1":
                    path_Else += "_1";
                    break;
                case "2":
                    path_Else += "_2";
                    break;
                case "3":
                    path_Else += "_3";
                    break;
                default:
                    break;
            }
            path += "sky" + path_Else + ".png";


            if (String.IsNullOrEmpty(path) == false)
            {
                BitmapImage img = new BitmapImage();



                img.UriSource = new Uri(path);

                img_Weather.Source = img;
            }

            if(Lgt != "0")
            {
                img_Thunder.Visibility = Visibility.Visible;
            }


            if (S06 != null)
            {
                txt_Water.Text = " 적설 : " + S06;
            }
            else
                txt_Water.Text = "강수량 : " + Rn1;
            txt_Thunder.Text = title;

            txt_Wind.Text = Uuu + " / " + Vvv;


        }
    }
}
