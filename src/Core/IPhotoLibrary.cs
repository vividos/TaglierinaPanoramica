using System.Threading.Tasks;

namespace TaglierinaPanoramica
{
    /// <summary>
    /// Photo library interface
    /// </summary>
    public interface IPhotoLibrary
    {
        /// <summary>
        /// Saves photo to photo library
        /// </summary>
        /// <param name="data">image data to save</param>
        /// <param name="folder">folder name in photo libray to use</param>
        /// <param name="filename">filename of image to save</param>
        /// <returns>task to wait on</returns>
        Task SavePhotoAsync(byte[] data, string folder, string filename);
    }
}
