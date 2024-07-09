# Taglierina Panoramica

- en: Taglierina Panoramica is an app for cropping panoramas
- it: Taglierina Panoramica è un'app per ritagliare panorami

![Screenshot](screenshot.jpg)

The app helps to split a panorama image into several images to use them in
fancy Instagram posts. The app name came into my mind while learning some
italian on Duolingo.

## Development

Most of the code for cropping images was taken from the SkiaSharp examples
found here:

https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/cropping

The rest is a fairly standard Xamarin.Forms app, using some external libraries
to keep code low.

Builds are done using GitHub actions:

[![Build Android app](https://github.com/vividos/TaglierinaPanoramica/actions/workflows/build-app-android.yml/badge.svg)](https://github.com/vividos/TaglierinaPanoramica/actions/workflows/build-app-android.yml)

## License

The app is licensed using the [Apache License 2.0](LICENSE), the same license
as the Xamarin.Forms SkiaSharp examples. See also the
[Credits](src/Core/Credits.md) of the used libraries and resources.
