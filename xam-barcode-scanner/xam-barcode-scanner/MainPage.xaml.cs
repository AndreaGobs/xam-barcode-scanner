using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace xam_barcode_scanner
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (cameraStatus != PermissionStatus.Granted)
                cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BarcodeScanPage());
        }
    }
}