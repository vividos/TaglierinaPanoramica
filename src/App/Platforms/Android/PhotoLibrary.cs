using Android.Media;
using Java.IO;
using Environment = Android.OS.Environment;
using File = Java.IO.File;

namespace TaglierinaPanoramica.Droid
{
    /// <summary>
    /// Android photo library implementation
    /// https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/saving
    /// </summary>
    internal class PhotoLibrary : IPhotoLibrary
    {
        /// <summary>
        /// Saves photo to photo library
        /// </summary>
        /// <param name="data">image data to save</param>
        /// <param name="folder">folder name in photo libray to use</param>
        /// <param name="filename">filename of image to save</param>
        /// <returns>task to wait on</returns>
        public async Task SavePhotoAsync(byte[] data, string folder, string filename)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            File? picturesDirectory = Environment.GetExternalStoragePublicDirectory(
                Environment.DirectoryPictures);
#pragma warning restore CS0618 // Type or member is obsolete

            File? folderDirectory = picturesDirectory;

            if (!string.IsNullOrEmpty(folder))
            {
                folderDirectory = new File(picturesDirectory, folder);
                folderDirectory.Mkdirs();
            }

            using File bitmapFile = new File(folderDirectory, filename);
            bitmapFile.CreateNewFile();

            using (FileOutputStream outputStream = new FileOutputStream(bitmapFile))
            {
                await outputStream.WriteAsync(data);
            }

            // Make sure it shows up in the Photos gallery promptly.
            MediaScannerConnection.ScanFile(
                Platform.CurrentActivity,
                [bitmapFile.Path],
                ["image/png", "image/jpeg"],
                null);
        }
    }
}
