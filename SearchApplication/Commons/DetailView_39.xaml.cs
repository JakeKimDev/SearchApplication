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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SearchApplication.Commons
{
    public sealed partial class DetailView_39 : UserControl
    {
        public DetailView_39()
        {
            this.InitializeComponent();
        }

        public void SetItem(String str1, String str2, String str3, String str4, String str5, String str6, String str7, String str8,
                           String str9, String str10, String str11 , String str12)
        {
            txt_1.Text = str1;
            txt_2.Text = str2;
            txt_3.Text = str3;
            txt_4.Text = str4;
            txt_5.Text = str5;
            txt_6.Text = str6;
            txt_7.Text = str7;
            txt_8.Text = str8;
            txt_9.Text = str9;
            txt_10.Text = str10;
            txt_11.Text = str11;
            txt_12.Text = str12;
            //txt_12.Text = str12;
            //txt_13.Text = str13;
            //txt_14.Text = str14;
            //txt_15.Text = str15;
            //txt_16.Text = str16;
            //txt_17.Text = str17;
            //txt_18.Text = str18;
            //txt_19.Text = str19;
            //txt_20.Text = str20;

        }
    }
}
