using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Text.Format;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VibelightApp.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidWifi))]

namespace VibelightApp.Droid
{
    class AndroidWifi : IAndroidWifi
    {
        readonly WifiManager WifiManage = (WifiManager)Android.App.Application.Context.GetSystemService(Context.WifiService);

        public string GetIp()
        {
            string ipAddress = "";

            WifiInfo wifiInfo = WifiManage.ConnectionInfo;
            int ip = wifiInfo.IpAddress;
            ipAddress = Formatter.FormatIpAddress(ip);
            return ipAddress;
        }
    }
}