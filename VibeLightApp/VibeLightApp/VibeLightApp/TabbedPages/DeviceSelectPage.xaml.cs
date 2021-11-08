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
    public partial class DeviceSelectPage : ContentPage
    {
        public DeviceSelectPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            BindingContext = this;
           
                
        }
       
        private void GenerateDeviceButtons()
        {
           

            DeviceButtons.Children.Clear();

            for (int i = 0; i < App.Controllers.Count; i++)
            {
                Button CurrentButton = new Button()
                {
                    Text = App.Controllers[i].Alias,
                    StyleId = App.Controllers[i].Device,
                    CornerRadius = 10,
                    BackgroundColor = Color.FromHex("a5c8cf"),
                    FontFamily = "sf",
                    FontSize = 18,
                    Margin = new Thickness(3, 3, 3, 3),
                    TextTransform = TextTransform.None,
                    TextColor = Color.FromHex("454545"),
                    HeightRequest = 45
                };
                CurrentButton.Clicked += DeviceButton_Clicked;
                DeviceButtons.Children.Add(CurrentButton);
            }
        }
        private async void DeviceButton_Clicked(object sender, EventArgs e)
        {
            Button ClickedButton = (Button)sender;

            try
            {
                for (int i = 0; i < DeviceButtons.Children.Count; i++)
                {

                    if (ClickedButton.StyleId == App.Controllers[i].Device)
                    {
                        App.SelectedDevice = App.Controllers[i];
                        break;
                    }
                }
            }
            catch (Exception) { }
          

            this.Navigation.RemovePage(this.Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);

        }

        protected async override void OnAppearing()
        {
            try
            {
                GenerateDeviceButtons();
                PageAnimation.Load(DeviceButtons, 100);
                App.Controllers = await App.Message.DiscoverDevicesAsync();
                GenerateDeviceButtons();
                PageAnimation.Load(DeviceButtons, 100);
            }
            catch (Exception) { }
          



        }
        protected override bool OnBackButtonPressed()
        {
            this.Navigation.RemovePage(this.Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);
            return false;
        }
    }
}