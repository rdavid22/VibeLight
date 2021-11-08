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
    public partial class Fire : ContentPage
    {
        private bool MicrophoneToggled = false;
        private bool OrientationSensorToggled = false;
        public Fire()
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
            ToggleOffOrientationSensor();

            App.Mic.StopReading();

            Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);

            return false;
        }

        private void ToggleOffOrientationSensor()
        {
            try
            {
                if (OrientationSensor.IsMonitoring)
                {
                    OrientationSensor.ReadingChanged -= OrientationSensorChanged;

                    OrientationSensor.Stop();

                }

            }
            catch (Exception ex) { }

        }
        private void ToggleOnOrientationSensor()
        {
            try
            {
                if (!OrientationSensor.IsMonitoring)
                {
                    OrientationSensor.ReadingChanged += OrientationSensorChanged;

                    OrientationSensor.Start(SensorSpeed.Fastest);
                }
            }
            catch (Exception ex) { }
        }

        async void OrientationSensorChanged(object sender, OrientationSensorChangedEventArgs e)
        {
            var data = e.Reading;
            int Xaxis = Convert.ToInt32(data.Orientation.X  * 500);

            Xaxis = Math.Abs(Xaxis-300);          
            
            Console.WriteLine(Xaxis);

            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1," + Xaxis + ",80,-1,-1,-1,-1,-1");
        }
        // mode, cooling, sparkling, firsth, second, third, brightness, delaying
        private async void OnHue1Changed(object sender, ValueChangedEventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1,-1,-1," + Convert.ToString(Convert.ToInt32(Hue1.Value)) + ",-1,-1,-1");
            Hue1.ThumbColor = Hue1.MinimumTrackColor = ColorConverters.FromHsla(Convert.ToSingle(Hue1.Value * 1.41), 80, 80, 200);
        }

        private async void OnHue2Changed(object sender, ValueChangedEventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1,-1,-1,-1," + Convert.ToString(Convert.ToInt32(Hue2.Value)) + ",-1,"+"-1");
            Hue2.ThumbColor = Hue2.MinimumTrackColor = ColorConverters.FromHsla(Convert.ToSingle(Hue2.Value * 1.41), 80, 80, 200);
        }

        private async void OnBrightnessChanged(object sender, ValueChangedEventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:" + "-1," + "-1," + "-1," + "-1," + "-1," + "-1," + Convert.ToString(Convert.ToInt32(Brightness.Value)) + ",-1");
        }

        private async void OnSparklingChanged(object sender, ValueChangedEventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1," + Convert.ToString(Convert.ToInt32(Sparkling.Value)) + ",-1," + "-1," + "-1," + "-1," + "-1");
        }

        private async void OnCoolingChanged(object sender, ValueChangedEventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1," + Convert.ToString(Convert.ToInt32(Cooling.Value)) + ",-1,-1,-1,-1,-1,-1");
        }

        private async void OnDelayingChanged(object sender, ValueChangedEventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1,-1,-1,-1,-1,-1," + Convert.ToString(Convert.ToInt32(Delaying.Value)));
        }
        private async void Microphone_Clicked(object sender, EventArgs e)
        {

            if (OrientationSensorToggled || MicrophoneToggled) { Cooling.IsEnabled = true; Sparkling.IsEnabled = true; Hue1.IsEnabled = true; Hue2.IsEnabled = true; App.Mic.StopReading(); MicrophoneToggled = false; Microphone.BackgroundColor = Color.FromHex("414141"); return; }
            MicrophoneToggled = true;
            Microphone.BackgroundColor = Color.FromHex("645566");


            Cooling.IsEnabled = false;
            Sparkling.IsEnabled = false;
            Hue1.IsEnabled = false;
            Hue2.IsEnabled = false;

            App.Mic.StartReading("param:-1,[400_0],[0_255],-1,[0_255],[0_255],-1,-1");

        }

        private void OrientationSensor_Clicked(object sender, EventArgs e)
        {
            if (OrientationSensorToggled || MicrophoneToggled) { Cooling.IsEnabled = true; Sparkling.IsEnabled = true; OrientationSensorToggled = false; ToggleOffOrientationSensor(); OrientationSensors.BackgroundColor = Color.FromHex("414141"); return; }
            OrientationSensorToggled = true;
            OrientationSensors.BackgroundColor = Color.FromHex("645566");

            ToggleOnOrientationSensor();
            Cooling.IsEnabled = false; Sparkling.IsEnabled = false;

        }

        private async void FrontButton_Clicked(object sender, EventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:1,-1,-1,-1,-1,-1,-1,-1");
        }
        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:2,-1,-1,-1,-1,-1,-1,-1");
        }
        private async void MirrorButton_Clicked(object sender, EventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:3,-1,-1,-1,-1,-1,-1,-1");
        }
        private async void BothButton_Clicked(object sender, EventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:4,-1,-1,-1,-1,-1,-1,-1");
        }
    }
}