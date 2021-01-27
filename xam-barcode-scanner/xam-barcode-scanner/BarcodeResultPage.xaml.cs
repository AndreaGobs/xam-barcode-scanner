using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace xam_barcode_scanner
{
    public partial class BarcodeResultPage : ContentPage
    {
        public BarcodeResultPage(string scanResult) :
            base()
        {
            InitializeComponent();

            barcodeResultText.Text = scanResult;
        }
    }
}