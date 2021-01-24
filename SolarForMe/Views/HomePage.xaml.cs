using System;
using System.Collections.Generic;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace SolarForMe.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            //SolarPanel.Source = ImageSource.FromResource("SolarForMe.Images.GettyImages-871613394.jpg");
            Insta.Source = ImageSource.FromResource("SolarForMe.Images.InstaIcon.png");
            Facebook.Source = ImageSource.FromResource("SolarForMe.Images.FacebookIcon.png");
            wwwWeb.Source = ImageSource.FromResource("SolarForMe.Images.wwwIcon.png");
            SolarLogo.Source = ImageSource.FromResource("SolarForMe.Images.microcarelogo.png");


        }

        async void wwwWeb_Clicked(System.Object sender, System.EventArgs e)
        {
            await Browser.OpenAsync("http://microcare.co.za/", BrowserLaunchMode.SystemPreferred);

        }

        async void Facebook_Clicked(System.Object sender, System.EventArgs e)
        {

            await Browser.OpenAsync("https://www.facebook.com/MicrocareSolar/", BrowserLaunchMode.SystemPreferred);
        }

        async void Insta_Clicked(System.Object sender, System.EventArgs e)
        {
            await Browser.OpenAsync("https://www.instagram.com/microcaresolar/", BrowserLaunchMode.SystemPreferred);

        }

        async void Login_Clicked(System.Object sender, System.EventArgs e)
        {
             await Navigation.PushAsync(new EnterData());
           
        }

        async void SignUp_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }
    }
}