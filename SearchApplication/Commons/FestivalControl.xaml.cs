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
    public sealed partial class FestivalControl : UserControl
    {
        public FestivalControl()
        {
            this.InitializeComponent();
        }

        public void SetItem(String img, String start, String end,String title)
        {
            txt_End.Text = end;
            txt_Start.Text = start;

            txt_Title.Text = title;
            if (String.IsNullOrEmpty(img) == false)
            {
                BitmapImage imgs = new BitmapImage();



                imgs.UriSource = new Uri(img);
                img_Festival.Source = imgs;
            }
        }
    }
}
