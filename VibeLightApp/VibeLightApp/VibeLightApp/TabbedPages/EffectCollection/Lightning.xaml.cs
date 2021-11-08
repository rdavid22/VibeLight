using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VibelightApp.TabbedPages.EffectCollection
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Lightning : ContentPage
    {
        public bool ZeusIsOn = false;
        public Lightning()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            PageAnimation.Load(MainLayout, 30);
        }

        protected override void OnAppearing()
        {

        }

        protected async override void OnDisappearing()
        {

        }

        protected override bool OnBackButtonPressed()
        {

            Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);

            return false;
        }
        void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var data = e.Reading;

            Console.WriteLine(data.Acceleration.X + data.Acceleration.Y + data.Acceleration.Z);

            double Gforce = Math.Abs(data.Acceleration.X + data.Acceleration.Y + data.Acceleration.Z);
            
            if (Gforce > 6)
            {
                App.Message.MessageSendAsync(App.SelectedDevice,"param: -1, 1");
            }

        }

       

        private void Zeus_Clicked(object sender, EventArgs e)
        {
            if (!ZeusIsOn)
            {             
                shake.IsVisible = true;
                Zeus.BackgroundColor = Color.FromHex("645566");
                Chance.Value = 0;
                ToggleOnAccelerometer();
                ZeusIsOn = true;
            }
            else
            {
                shake.IsVisible = false;
                Zeus.BackgroundColor = Color.FromHex("414141");
                ToggleOffAccelerometer();
                ZeusIsOn = false;
            }
        }
        private async void OnChanceChanged(object sender, EventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param: "+ Convert.ToString(Convert.ToInt32(100-Chance.Value)) + ", -1");
        }

        public void ToggleOnAccelerometer()
        {
            try
            {
                if (!Accelerometer.IsMonitoring)
                {
                    Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
                    Accelerometer.Start(SensorSpeed.Fastest);
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {

            }  // Feature not supported on device
           
        }
        
        public void ToggleOffAccelerometer()
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                {
                    Accelerometer.ReadingChanged -= Accelerometer_ReadingChanged;
                    Accelerometer.Stop();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {

            }  // Feature not supported on device

        }

        private void Chance_ValueChanged(object sender, ValueChangedEventArgs e)
        {

        }
    }

}