using YoutubeDownloader.Entity.Enums;
using System.Linq;

namespace YoutubeDownloader.Model
{
    public class VideoInfo
    {
        #region constructor
        public VideoInfo(int formatCode)
        {
            var videoFormat = Cache.Defaults.FirstOrDefault(i => i.FormatCode == formatCode);
            if (videoFormat != null)
            {
                this.FormatCode = videoFormat.FormatCode;
                this.VideoType = videoFormat.VideoType;
                this.Resolution = videoFormat.Resolution;
                this.Is3D = videoFormat.Is3D;
                this.AudioType = videoFormat.AudioType;
                this.AudioBitrate = videoFormat.AudioBitrate;
                this.AdaptiveType = videoFormat.AdaptiveType;
            }
            else
            {
                this.FormatCode = 0;
                this.VideoType = VideoType.Unknown;
                this.Resolution = 0;
                this.Is3D = false;
                this.AudioType = AudioType.Unknown;
                this.AudioBitrate = 0;
                this.AdaptiveType = AdaptiveType.None;
            }
        }
  
        public VideoInfo(int formatCode, VideoType videoType, int resolution, bool is3D, AudioType audioType, int audioBitrate, AdaptiveType adaptiveType)
        {
            this.FormatCode = formatCode;
            this.VideoType = videoType;
            this.Resolution = resolution;
            this.Is3D = is3D;
            this.AudioType = audioType;
            this.AudioBitrate = audioBitrate;
            this.AdaptiveType = adaptiveType;

        }
        #endregion
        #region properties
        public int Resolution { get; private set; }
        public bool Is3D { get; private set; }
        public int FormatCode { get; private set; }
        public string DownloadUrl { get; set; }
        public AudioType AudioType { get; private set; }
        public int AudioBitrate { get; private set; }
        public AdaptiveType AdaptiveType { get; private set; }
        public string Title { get; set; }
        public string YoutubeLinkId{ get; set; }

        public string VideoExtension
        {
            get
            {
                switch (this.VideoType)
                {
                    case VideoType.Mp4:
                        return ".mp4";

                    case VideoType.Mobile:
                        return ".3gp";
                    case VideoType.M4a:
                        return ".m4a";
                    case VideoType.Flash:
                        return ".flv";

                    case VideoType.WebM:
                        return ".webm";
                }

                return null;
            }
        }
        public VideoType VideoType { get; private set; }
        #endregion
        #region methods
        public override string ToString()
        {
            return string.Format("Full Title: {0}, Type: {1}, Resolution: {2}p", this.Title + this.VideoExtension, this.VideoType, this.Resolution);
        }
        #endregion
    }

   
      
}
