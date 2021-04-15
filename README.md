# xam-barcode-scanner

Xamarin.Forms project implementing barcode scanner natively on Android and iOS.

QR scanning to function properly requires the Google Play Services app to be installed.
For the Huawei devices without the services of Google the best solution is probably to import [Huawei Scan Kit](https://developer.huawei.com/consumer/en/hms/huawei-scankit).
Documentation seems good also for Xamarinians.

## Supported Formats

Format | Android | iOS
-- | -- | --
Code128 | ✓| ✓
Code39 | ✓ | ✓
Code93 | ✓ | ✓
Codabar | ✓ | ✗
DataMatrix | ✓ | ✓
Ean13 | ✓ | ✓
Ean8 | ✓ | ✓
Itf | ✓ | ✓
QrCode | ✓ | ✓
UpcA | ✓ | ✗
UpcE | ✓ | ✓
Pdf417 | ✓ | ✓
Aztec | ✓ | ✓

## Permissions

The only permission required is for the *Camera* usage.

## Links

Android solution is base on [Google Mobile vision API](https://developers.google.com/vision/introduction)
- [NuGet package](https://www.nuget.org/packages/Xamarin.GooglePlayServices.Vision)

iOS solution is based on [AVFoundation framework](https://developer.apple.com/av-foundation)
- [Xamarin doc](https://docs.microsoft.com/it-it/dotnet/api/avfoundation.avcapturesession?view=xamarin-ios-sdk-12)
- [iOS tutorial](https://www.appcoda.com/intermediate-swift-tips/qrcode-reader.html)

## Installation

Installation | Version
-- | --
Xamarin.Forms | 5.0
Android | 21+
iOS | 12+