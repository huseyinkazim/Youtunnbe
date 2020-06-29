using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    #region video formatı dokunma
    public class InitRange
    {
        public string start { get; set; }
        public string end { get; set; }
    }

    public class IndexRange
    {
        public string start { get; set; }
        public string end { get; set; }
    }

    public class AdaptiveFormat
    {
        public int itag { get; set; }
        public string mimeType { get; set; }
        public int bitrate { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public InitRange initRange { get; set; }
        public IndexRange indexRange { get; set; }
        public string lastModified { get; set; }
        public string contentLength { get; set; }
        public string quality { get; set; }
        public string xtags { get; set; }
        public int fps { get; set; }
        public string qualityLabel { get; set; }
        public string projectionType { get; set; }
        public int averageBitrate { get; set; }
        public string approxDurationMs { get; set; }
        public string cipher { get; set; }
        public string url { get; set; }

        public string cipherDecoded { get { return WebUtility.UrlDecode(WebUtility.UrlDecode(this.cipher)); } }
    }

    public partial class Format
    {
        public int itag { get; set; }
        public string mimeType { get; set; }
        public int bitrate { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string lastModified { get; set; }
        public string contentLength { get; set; }
        public string quality { get; set; }
        public string xtags { get; set; }
        public string qualityLabel { get; set; }
        public string projectionType { get; set; }
        public int averageBitrate { get; set; }
        public string audioQuality { get; set; }
        public string approxDurationMs { get; set; }
        public string audioSampleRate { get; set; }
        public int audioChannels { get; set; }
        public string cipher { get; set; }
        public string url { get; set; }

        public string cipherDecoded { get { return WebUtility.UrlDecode(WebUtility.UrlDecode(this.cipher)); } }
    }
    #endregion
}
