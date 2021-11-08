using Dyme.Services;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: ExportFont("sf.otf", Alias = "sf")]
[assembly: ExportFont("sfthin.ttf", Alias = "sfthin")]
[assembly: ExportFont("sflight.ttf", Alias = "sflight")]

namespace VibelightApp
{
    public partial class App : Application
    {

        public static List<DeviceList> Controllers;
        public static DeviceList SelectedDevice;
        public static MessageHandler Message;
        public static MicrophoneHandler Mic;
        public App()
        {

            InitializeComponent();

            Controllers = new List<DeviceList>(); // osszes VibelightController
            SelectedDevice = new DeviceList(); // Jelenleg kivalasztott VibelightController
            Message = new MessageHandler(); // Uzenet kuldes / eszkoz bekereses
            Mic = new MicrophoneHandler();
                             

            Controllers =  Message.DiscoverDevices();

            if (Controllers.Count > 0)
            {
                SelectedDevice = Controllers[0];
                MainPage = new NavigationPage(new TabbedPages.Effects()); 
            }
            else
            { MainPage = new NavigationPage(new SplashScreen()); }
        }

        protected async override void OnStart()
        {         
            await Mic.Initialize();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        
    }
}
