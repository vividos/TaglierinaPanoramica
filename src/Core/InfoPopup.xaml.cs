using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TaglierinaPanoramica
{
    /// <summary>
    /// Popup showing infos about the app
    /// </summary>
    public partial class InfoPopup
    {
        /// <summary>
        /// Creates a new info popup object
        /// </summary>
        public InfoPopup()
        {
            this.BindingContext = new HtmlWebViewSource
            {
                Html = this.GetInfoText()
            };

            this.InitializeComponent();
        }

        /// <summary>
        /// Returns info HTML text
        /// </summary>
        /// <returns>HTML text</returns>
        private string GetInfoText()
        {
            string markdownText;
            using (var stream = this.GetType().Assembly.GetManifestResourceStream("TaglierinaPanoramica.Credits.md"))
            using (var reader = new StreamReader(stream))
            {
                markdownText = reader.ReadToEnd();
            }

            markdownText = markdownText.Replace("${VERSION}", AppInfo.VersionString);

            string htmlText = Markdig.Markdown.ToHtml(markdownText);

            return $"<span style=\"font-family: sans-serif; font-size: 14\">{htmlText}</span>";
        }
    }
}
