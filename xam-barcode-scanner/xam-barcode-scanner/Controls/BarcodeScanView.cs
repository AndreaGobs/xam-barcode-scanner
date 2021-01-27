using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace xam_barcode_scanner.Controls
{
    public enum XamBarcodeFormat
    {
        Deafult = 0, 
        Code128 = 1,
        Code39 = 2,
        Code93 = 4,
        Codabar = 8,
        DataMatrix = 16,
        Ean13 = 32,
        Ean8 = 64,
        Itf = 128,
        QrCode = 256,
        UpcA = 512,
        UpcE = 1024,
        Pdf417 = 2048,
        Aztec = 4096
    }

    public class BarcodeScanView : View
    {
        public BarcodeScanView(XamBarcodeFormat barcodeFormat = XamBarcodeFormat.Deafult)
        {
            BarcodeFormat = barcodeFormat;
        }

        public delegate void ScanResultDelegate(string result);
        public ScanResultDelegate OnBarcodeScanResult;

        /// <summary>
        /// Barcode target formst
        /// </summary>
        public XamBarcodeFormat BarcodeFormat { get; private set; }
        /// <summary>
        /// Start or stop phone's camera
        /// </summary>
        public Action<bool> StartCamera { get; set; }
    }
}
