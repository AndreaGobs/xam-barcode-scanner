using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using xam_barcode_scanner.Controls;

namespace xam_barcode_scanner
{
    public partial class BarcodeScanPage : ContentPage
    {
        private View _ScanView;

        public BarcodeScanPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_ScanView == null)
            {
                if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS)
                {
                    var scanView = new BarcodeScanView(XamBarcodeFormat.QrCode);
                    scanView.OnBarcodeScanResult += OnScanResult;
                    _ScanView = scanView;
                }

                if (_ScanView == null)
                {
                    if (Device.RuntimePlatform != Device.Android && Device.RuntimePlatform != Device.iOS)
                        this.scanMessage.Text = "Barcode scan not supported in this platform!";
                    else
                    {
                        var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
                        if (cameraStatus != PermissionStatus.Granted)
                            this.scanMessage.Text = "Check the Camera permissions to scan barcodes";
                        else
                            this.scanMessage.Text = "Generic error";
                    }
                    this.scanMessage.IsVisible = true;
                }
                else
                {
                    this.scanMessage.IsVisible = false;

                    _ScanView.VerticalOptions = LayoutOptions.FillAndExpand;
                    _ScanView.HorizontalOptions = LayoutOptions.FillAndExpand;
                    this.scanView.Children.Add(_ScanView, 0, 0);
                }
            }
            else
            {
                if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS)
                {
                    ((BarcodeScanView)_ScanView).OnBarcodeScanResult -= OnScanResult;
                    ((BarcodeScanView)_ScanView).OnBarcodeScanResult += OnScanResult;
                    ((BarcodeScanView)_ScanView).StartCamera?.Invoke(true);
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (_ScanView != null)
            {
                ((BarcodeScanView)_ScanView).OnBarcodeScanResult -= OnScanResult;
                ((BarcodeScanView)_ScanView).StartCamera?.Invoke(false);
            }
        }

        public void OnScanResult(string result)
        {
            if (!string.IsNullOrEmpty(result))
            {
                try
                {
                    Vibration.Vibrate(250);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("vibration error, ex=" + ex.Message);
                }
                Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new BarcodeResultPage(result)));
            }
            else
                Console.WriteLine("scan error: null result");
        }
    }
}
