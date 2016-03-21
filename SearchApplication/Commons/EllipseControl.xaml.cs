using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using SearchApplication.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class EllipseControl : UserControl
    {
        public EllipseControl()
        {
            this.InitializeComponent();
        }

  

        public void SetData(List<String> itemList)
        {
            gd_ItemInArc.Children.Clear();

            if (itemList.Count() > 0)
            {

                BitmapImage img = new BitmapImage();

                ImageBrush imgBr = new ImageBrush();

                img.UriSource = new Uri(itemList[0]);
                imgBr.ImageSource = img;
                imgBr.Stretch = Stretch.UniformToFill;

                el_Main.Fill = imgBr;
                int totalCount = itemList.Count();
                for (int i = 0; i < totalCount; i++)
                {

                    gd_ItemInArc.Children.Add(MakeArea((360 / totalCount) * i, (360 / totalCount) * (i + 1), itemList[i]));
                }
            }
        }

        private Arc MakeArea(double startAngle, double endAngle,String image)
        {
            Arc arc = new Arc();
            arc.Width = 200;
            arc.Height = 200;


            BitmapImage img = new BitmapImage();

            ImageBrush imgBr = new ImageBrush();

            img.UriSource = new Uri(image);
            imgBr.ImageSource = img;
            imgBr.Stretch = Stretch.UniformToFill;
            
            arc.Fill = imgBr;
            arc.PercentValue = ((endAngle - startAngle) / 360) * 100;
            arc.Radius = 80;
            arc.Thickness = 40;
            arc.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
            arc.RenderTransform = new RotateTransform() { Angle = startAngle };
            arc.PointerPressed += Arc_PointerPressed;
            return arc;
        }

        private void Arc_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            el_Main.Fill = (sender as Arc).Fill;
        }
    }
}
