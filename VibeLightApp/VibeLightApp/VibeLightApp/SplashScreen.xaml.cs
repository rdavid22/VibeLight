using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VibelightApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashScreen : ContentPage
    {
        public SplashScreen()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            BindingContext = this;
            PageAnimation.Load(MainLayout,100);
        }
        protected override void OnAppearing()
        {
           
        }
        protected override void OnDisappearing()
        {

        }
        protected override bool OnBackButtonPressed()
        {

            base.OnDisappearing();
            if (Navigation.NavigationStack.Count == 1)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            else
            {
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);
            }

            GC.Collect();
            return false;
        }


        private async void Next_Clicked(object sender, EventArgs e)
        {
            await Task.WhenAll(
            PageAnimation.ButtonPress(Next, 100),
            PageAnimation.Unload(MainLayout, 10)

        );


            await Navigation.PushAsync(new ConnectionPage(), false);

            this.Navigation.RemovePage(this.Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
            GC.Collect();

        }
    }
}