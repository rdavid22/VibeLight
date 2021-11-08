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
    public partial class Rocket : ContentPage
    {
        public Rocket()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            PageAnimation.Load(MainLayout, 30);
        }
        // !-- lucky rocket: -1,halvanyulas,szin,szineltolasnagysaga,tényerő,kesleltetes-->

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

        private async void OnFadeChanged(object sender, ValueChangedEventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice,"param:-1,"+ Convert.ToString(Convert.ToInt32(Fade.Value)) + ",-1,-1,-1,-1");
        }
        private async void OnHueChanged(object sender, ValueChangedEventArgs e)
        {
        
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1,"+ Convert.ToString(Convert.ToInt32(Hue.Value)) + ",-1,-1,-1");
            Hue.ThumbColor = Hue.MinimumTrackColor = ColorConverters.FromHsla(Convert.ToSingle(Hue.Value * 1.41), 80, 80, 200);

        }
        private async void OnDeltaHueChanged(object sender, ValueChangedEventArgs e)
        {

          await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1,-1," + Convert.ToString(Convert.ToInt32(DeltaHue.Value)) + ",-1,-1");

        }
        private async void OnBrightnessChanged(object sender, ValueChangedEventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1,-1,-1," + Convert.ToString(Convert.ToInt32(Brightness.Value)) + ",-1");

        }
        private async void OnDelayChanged(object sender, ValueChangedEventArgs e)
        {

            await App.Message.MessageSendAsync(App.SelectedDevice, "param:-1,-1,-1,-1,-1," + Convert.ToString(Convert.ToInt32(Delay.Value)));

        }


    }
}