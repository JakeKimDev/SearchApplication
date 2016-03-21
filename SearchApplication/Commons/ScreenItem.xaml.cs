using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using SearchApplication.Models;
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
    public sealed partial class ScreenItem : UserControl
    {
        public ScreenItem()
        {
            this.InitializeComponent();
        }

        public void SetItem(string imgPath, string title, string servPath , KoreaLocation location)
        {

            if (String.IsNullOrEmpty(imgPath) == false)
            {
                BitmapImage img = new BitmapImage();



                img.UriSource = new Uri(imgPath);

                img_Main.Source = img;
            }

            txt_Title.Text = title;


            if (String.IsNullOrEmpty(servPath) == false)
            {
                BitmapImage img2 = new BitmapImage();

                img2.DecodePixelWidth = 100;

                img2.UriSource = new Uri(servPath);

                img_Serv.Source = img2;
            }


            this.Tag = location;
        }
    }
}
