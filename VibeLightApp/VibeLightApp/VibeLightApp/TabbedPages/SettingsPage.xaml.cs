using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VibelightApp.TabbedPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private int MaxChars = 20;
        public SettingsPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            InitializeComponent();


            if (App.SelectedDevice != null)
            {
                DeviceName.Text = "Device name: "+App.SelectedDevice.Device;
                IP.Text = "IP adress: " + App.SelectedDevice.IP;
                Led.Text = "Number of led: " + App.SelectedDevice.Led;
                Alias.Text = "Alias name: " + App.SelectedDevice.Alias;
            }

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
           
            this.Navigation.RemovePage(this.Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);
            
            return false;
        }

        private void Rename_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RenameEntry.Text.Length > MaxChars)
            {
                Warning.IsVisible = true;
            }
            else
            {
                Warning.IsVisible = false;

            }
        }
        private async void Rename_Clicked(object sender, EventArgs e)
        {
            if (!Warning.IsVisible)
            {
                if (await App.Message.MessageSendAsync(App.SelectedDevice, "rename:" + RenameEntry.Text))
                {
                    await DisplayAlert("Message", "Device renamed succesfuly", "Got it");

                    App.SelectedDevice.Alias = RenameEntry.Text;
                    Alias.Text = RenameEntry.Text;

                }
                else
                {
                    await DisplayAlert("Message", "Device rename error", "Got it");
                }
            }
        }

        private async void Reset_Clicked(object sender, EventArgs e)
        {
            if (await App.Message.MessageSendAsync(App.SelectedDevice, "reset"))
            {
                await DisplayAlert("Message", "Device reseted succesfuly", "Got it");
                App.SelectedDevice = null;
                this.Navigation.RemovePage(this.Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);

            }
            else
            {
                await DisplayAlert("Message", "Device reset error", "Got it");
            }
        }

        private async void EcoButton_Clicked(object sender, EventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "brightness:51");
        }

        private async void BalancedButton_Clicked(object sender, EventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "brightness:102");
        }

        private async void BrightButton_Clicked(object sender, EventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "brightness:153");
        }

        private async void UltraButton_Clicked(object sender, EventArgs e)
        {
            await App.Message.MessageSendAsync(App.SelectedDevice, "brightness:255");
        }
    }
}