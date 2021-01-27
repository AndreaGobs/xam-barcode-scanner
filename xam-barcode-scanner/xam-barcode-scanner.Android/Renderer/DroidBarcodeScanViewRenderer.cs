using Android;
using Android.App;
using Android.Content;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using AndroidX.Core.App;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using xam_barcode_scanner.Controls;
using xam_barcode_scanner.Droid.Renderer;
using static Android.Gms.Vision.Detector;

[assembly: ExportRenderer(typeof(BarcodeScanView), typeof(DroidBarcodeScanViewRenderer))]
namespace xam_barcode_scanner.Droid.Renderer
{
    public class DroidBarcodeScanViewRenderer : ViewRenderer, ISurfaceHolderCallback, IProcessor
    {
        private Android.Views.View _View;
        private SurfaceView _SurfaceView;
        private BarcodeDetector _BarcodeDetector;
        private CameraSource _CameraSource;

        #region ViewRenderer class

        public DroidBarcodeScanViewRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            try
            {
                if (e.OldElement != null || Element == null)
                    return;

                var activity = this.Context as Activity;
                _View = activity.LayoutInflater.Inflate(Resource.Layout.BarcodeScanView, this, false);
                AddView(_View);

                _SurfaceView = FindViewById<SurfaceView>(Resource.Id.barcodeScanView);

                var view = (BarcodeScanView)this.Element;
                _BarcodeDetector = new BarcodeDetector.Builder(this.Context)
                    .SetBarcodeFormats(XamBarcodeFormatToAndroid(view.BarcodeFormat))
                    .Build();

                _CameraSource = new CameraSource
                    .Builder(this.Context, _BarcodeDetector)
                    .SetFacing(CameraFacing.Back)        //default to back
                    .SetAutoFocusEnabled(true)           //default to false
                    .SetRequestedPreviewSize(1024, 768)  //default to 1024x768
                    .SetRequestedFps(30f)                //default to 30 fps
                    .Build();

                _SurfaceView.Holder.AddCallback(this);
                _BarcodeDetector.SetProcessor(this);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DROID_SCAN | OnElementChanged error", ex);
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            try
            {
                var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
                var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);

                _View.Measure(msw, msh);
                _View.Layout(0, 0, r - l, b - t);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DROID_SCAN | OnLayout error", ex);
            }
        }
        #endregion

        #region ISurfaceHolderCallback interface
        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            try
            {
                if (ActivityCompat.CheckSelfPermission(this.Context.ApplicationContext, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
                    return;

                var view = (BarcodeScanView)this.Element;
                view.StartCamera = this.StartCamera;
                StartCamera(true);
            }
            catch (InvalidOperationException ioex)
            {
                Console.WriteLine("DROID_SCAN | SurfaceCreated invalid operation", ioex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DROID_SCAN | SurfaceCreated error", ex);
            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            try
            {
                StartCamera(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("DROID_SCAN | SurfaceDestroyed error", ex);
            }
        }
        #endregion

        #region IProcessor interface
        public void ReceiveDetections(Detections detections)
        {
            try
            {
                SparseArray barcodes = detections.DetectedItems;

                if (barcodes.Size() != 0)
                {
                    var view = (BarcodeScanView)this.Element;
                    view.OnBarcodeScanResult?.Invoke(((Barcode)barcodes.ValueAt(0)).RawValue);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DROID_SCAN | ReceiveDetections error", ex);
            }
        }

        public void Release()
        {
        }
        #endregion

        #region Private methods

        private void StartCamera(bool start)
        {
            try
            {
                if (start)
                    _CameraSource?.Start(_SurfaceView?.Holder);
                else
                    _CameraSource?.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("DROID_SCAN | error StartCamera to " + start, ex);
            }
        }

        private BarcodeFormat XamBarcodeFormatToAndroid(XamBarcodeFormat barcodeFormat)
        {
            int targetFormat = (int)XamBarcodeFormat.Deafult;

            if (((int)barcodeFormat & (int)XamBarcodeFormat.Code128) == (int)XamBarcodeFormat.Code128)
                targetFormat |= (int)BarcodeFormat.Code128;
            if (((int)barcodeFormat & (int)XamBarcodeFormat.Code39) == (int)XamBarcodeFormat.Code39)
                targetFormat |= (int)BarcodeFormat.Code39;
            if (((int)barcodeFormat & (int)XamBarcodeFormat.Code93) == (int)XamBarcodeFormat.Code93)
                targetFormat |= (int)BarcodeFormat.Code93;
            if (((int)barcodeFormat & (int)XamBarcodeFormat.Codabar) == (int)XamBarcodeFormat.Codabar)
                targetFormat |= (int)BarcodeFormat.Codabar;
            if (((int)barcodeFormat & (int)XamBarcodeFormat.DataMatrix) == (int)XamBarcodeFormat.DataMatrix)
                targetFormat |= (int)BarcodeFormat.DataMatrix;
            if (((int)barcodeFormat & (int)XamBarcodeFormat.Ean13) == (int)XamBarcodeFormat.Ean13)
                targetFormat |= (int)BarcodeFormat.Ean13;
            if (((int)barcodeFormat & (int)XamBarcodeFormat.Ean8) == (int)XamBarcodeFormat.Ean8)
                targetFormat |= (int)BarcodeFormat.Ean8;
            if (((int)barcodeFormat & (int)XamBarcodeFormat.Itf) == (int)XamBarcodeFormat.Itf)
                targetFormat |= (int)BarcodeFormat.Itf;
            if (((int)barcodeFormat & (int)XamBarcodeFormat.QrCode) == (int)XamBarcodeFormat.QrCode)
                targetFormat |= (int)BarcodeFormat.QrCode;
            if (((int)barcodeFormat & (int)XamBarcodeFormat.UpcA) == (int)XamBarcodeFormat.UpcA)
                targetFormat |= (int)BarcodeFormat.UpcA;
            if (((int)barcodeFormat & (int)XamBarcodeFormat.UpcE) == (int)XamBarcodeFormat.UpcE)
                targetFormat |= (int)BarcodeFormat.UpcE;
            if (((int)barcodeFormat & (int)XamBarcodeFormat.Pdf417) == (int)XamBarcodeFormat.Pdf417)
                targetFormat |= (int)BarcodeFormat.Pdf417;
            if (((int)barcodeFormat & (int)XamBarcodeFormat.Aztec) == (int)XamBarcodeFormat.Aztec)
                targetFormat |= (int)BarcodeFormat.Aztec;

            return (BarcodeFormat)targetFormat;
        }

        #endregion
    }
}