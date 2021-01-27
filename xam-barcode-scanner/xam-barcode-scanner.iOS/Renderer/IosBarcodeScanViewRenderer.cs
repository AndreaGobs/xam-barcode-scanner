using AVFoundation;
using CoreFoundation;
using Foundation;
using System;
using System.Linq;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using xam_barcode_scanner.Controls;
using xam_barcode_scanner.iOS.Renderer;

[assembly: ExportRenderer(typeof(BarcodeScanView), typeof(IosBarcodeScanViewRenderer))]
namespace xam_barcode_scanner.iOS.Renderer
{
    public class IosBarcodeScanViewRenderer : ViewRenderer, IAVCaptureMetadataOutputObjectsDelegate
    {
        /// <summary>
        /// The AVCaptureSession object coordinates the recording of video input and passing the recorded information to one or more output objects
        /// </summary>
        private AVCaptureSession _AVSession;
        /// <summary>
        /// To set camera input
        /// </summary>
        private AVCaptureDeviceInput _AVDeviceImput;
        private AVCaptureVideoPreviewLayer _AVVideoPeviewLayer;
        /// <summary>
        /// To capture session output
        /// </summary>
        private AVCaptureMetadataOutput _AVMetadataOutput;
        private DispatchQueue _MetadataObjectsQueue;

        #region ViewRenderer class

        public IosBarcodeScanViewRenderer()
        {
            InitSession();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            try
            {
                if (_AVVideoPeviewLayer != null)
                    _AVVideoPeviewLayer.Frame = Bounds;

                var view = (BarcodeScanView)this.Element;
                _AVMetadataOutput.MetadataObjectTypes = XamBarcodeFormatToIos(view.BarcodeFormat);
                view.StartCamera = this.StartSession;
            }
            catch (Exception ex)
            {
                Console.WriteLine("IOS_SCAN | LayoutSubviews error", ex);
            }
        }

        #endregion

        #region AVCaptureMetadataOutputObjectsDelegate

        /// <summary>
        /// _ResetEvent is used to drop new notifications if old ones are still processing, to avoid queuing up a bunch of stale data.
        /// </summary>
        private readonly AutoResetEvent _ResetEvent = new AutoResetEvent(true);

        [Export("captureOutput:didOutputMetadataObjects:fromConnection:")]
        public void DidOutputMetadataObjects(AVCaptureMetadataOutput captureOutput, AVMetadataObject[] metadataObjects, AVCaptureConnection connection)
        {
            try
            {
                if (this._ResetEvent.WaitOne(0))
                {
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        if (metadataObjects != null)
                        {
                            foreach (var metadataObject in metadataObjects)
                            {
                                try
                                {
                                    var barcodeMetadataObject = metadataObject as AVMetadataMachineReadableCodeObject;
                                    if (barcodeMetadataObject != null && !string.IsNullOrEmpty(barcodeMetadataObject.StringValue))
                                    {
                                        var view = (BarcodeScanView)this.Element;
                                        view.OnBarcodeScanResult?.Invoke(barcodeMetadataObject.StringValue);
                                        StartSession(false);
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("IOS_SCAN | trasforming metadata error error", ex);
                                }
                            }
                        }

                        this._ResetEvent.Set();
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("IOS_SCAN | DidOutputMetadataObjects error", ex);
            }
        }

        #endregion

        #region Private methods

        private void InitSession()
        {
            try
            {
                //init capture session
                _AVSession = new AVCaptureSession();

                //check permissions
                var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);
                if (authorizationStatus != AVAuthorizationStatus.Authorized)
                    return;

                //check capture camera
                var cameras = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
                var camera = cameras.FirstOrDefault(d => d.Position == AVCaptureDevicePosition.Back);
                if (camera == null)
                    return;

                //add input to capture session
                _AVDeviceImput = new AVCaptureDeviceInput(camera, out NSError _);
                if (_AVSession.CanAddInput(_AVDeviceImput))
                    _AVSession.AddInput(_AVDeviceImput);
                else
                    return;

                //add output to camera session
                _MetadataObjectsQueue = new DispatchQueue("metadata objects queue");
                _AVMetadataOutput = new AVCaptureMetadataOutput();
                if (_AVSession.CanAddOutput(_AVMetadataOutput))
                    _AVSession.AddOutput(_AVMetadataOutput);
                else
                    return;
                _AVMetadataOutput.SetDelegate(this, _MetadataObjectsQueue);

                //init the video preview layer and add it to the current view
                _AVVideoPeviewLayer = new AVCaptureVideoPreviewLayer(_AVSession);
                _AVVideoPeviewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
                _AVVideoPeviewLayer.Frame = Bounds;
                this.Layer.AddSublayer(_AVVideoPeviewLayer);

                //start capture session
                StartSession(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("IOS_SCAN | init error", ex);
            }
        }

        private void StartSession(bool start)
        {
            try
            {
                if (start)
                {
                    _AVSession?.StartRunning();
                }
                else
                {
                    _AVSession?.StopRunning();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("IOS_SCAN | error StartCamera to " + start, ex);
            }
        }

        private AVMetadataObjectType XamBarcodeFormatToIos(XamBarcodeFormat barcodeFormat)
        {
            AVMetadataObjectType targetFormat = AVMetadataObjectType.None;

            if ((barcodeFormat & XamBarcodeFormat.Code128) == XamBarcodeFormat.Code128)
                targetFormat |= AVMetadataObjectType.Code128Code;
            if ((barcodeFormat & XamBarcodeFormat.Code39) == XamBarcodeFormat.Code39)
                targetFormat |= AVMetadataObjectType.Code39Code;
            if ((barcodeFormat & XamBarcodeFormat.Code93) == XamBarcodeFormat.Code93)
                targetFormat |= AVMetadataObjectType.Code93Code;
            if ((barcodeFormat & XamBarcodeFormat.DataMatrix) == XamBarcodeFormat.DataMatrix)
                targetFormat |= AVMetadataObjectType.DataMatrixCode;
            if ((barcodeFormat & XamBarcodeFormat.Ean13) == XamBarcodeFormat.Ean13)
                targetFormat |= AVMetadataObjectType.EAN13Code;
            if ((barcodeFormat & XamBarcodeFormat.Ean8) == XamBarcodeFormat.Ean8)
                targetFormat |= AVMetadataObjectType.EAN8Code;
            if ((barcodeFormat & XamBarcodeFormat.Itf) == XamBarcodeFormat.Itf)
                targetFormat |= AVMetadataObjectType.ITF14Code;
            if ((barcodeFormat & XamBarcodeFormat.QrCode) == XamBarcodeFormat.QrCode)
                targetFormat |= AVMetadataObjectType.QRCode;
            if ((barcodeFormat & XamBarcodeFormat.UpcE) == XamBarcodeFormat.UpcE)
                targetFormat |= AVMetadataObjectType.UPCECode;
            if ((barcodeFormat & XamBarcodeFormat.Pdf417) == XamBarcodeFormat.Pdf417)
                targetFormat |= AVMetadataObjectType.PDF417Code;
            if ((barcodeFormat & XamBarcodeFormat.Aztec) == XamBarcodeFormat.Aztec)
                targetFormat |= AVMetadataObjectType.AztecCode;

            return targetFormat;
        }

        #endregion
    }
}