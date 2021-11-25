using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Xamarin.Forms;

[assembly: Dependency(typeof(TaglierinaPanoramica.UWP.PhotoLibrary))]

namespace TaglierinaPanoramica.UWP
{
    /// <summary>
    /// UWP photo library implementation
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
            StorageFolder picturesDirectory = KnownFolders.PicturesLibrary;
            StorageFolder folderDirectory = picturesDirectory;

            // Get the folder or create it if necessary
            if (!string.IsNullOrEmpty(folder))
            {
                try
                {
                    folderDirectory = await picturesDirectory.GetFolderAsync(folder);
                }
                catch (Exception)
                {
                    folderDirectory = null;
                }

                if (folderDirectory == null)
                {
                    folderDirectory = await picturesDirectory.CreateFolderAsync(folder);
                }
            }

            // Create the file.
            StorageFile storageFile = await folderDirectory.CreateFileAsync(
                filename,
                CreationCollisionOption.GenerateUniqueName);

            // Convert byte[] to Windows buffer and write it out.
            IBuffer buffer = WindowsRuntimeBuffer.Create(data, 0, data.Length, data.Length);
            await FileIO.WriteBufferAsync(storageFile, buffer);
        }
    }
}
