using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class VideoInfo
    {
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
        public int Resolution { get; private set; }

       // public bool RequiresDecryption { get; set; }
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
       // public string HtmlPlayerVersion { get; set; }
        public override string ToString()
        {
            return string.Format("Full Title: {0}, Type: {1}, Resolution: {2}p", this.Title + this.VideoExtension, this.VideoType, this.Resolution);
        }
        //private string jsPath { set; get; }
    }
    public class ExtractionInfo
    {
        public bool RequiresDecryption { get; set; }

        public Uri Uri { get; set; }
    }
    public enum VideoType
    {
        Mobile,
        Flash,
        Mp4,
        WebM,
        M4a,
        Unknown
    }
    public enum AudioType
    {
        Aac,
        Mp3,
        Vorbis,
        m4a,
        weba,
        Unknown
    }
    public enum AdaptiveType
    {
        None,
        Audio,
        Video
    }
    public class Cache
    {
        public static IEnumerable<VideoInfo> Defaults = new List<VideoInfo>
        {
            /* Non-adaptive */
            new VideoInfo(5, VideoType.Flash, 240, false, AudioType.Mp3, 64, AdaptiveType.None),
            new VideoInfo(6, VideoType.Flash, 270, false, AudioType.Mp3, 64, AdaptiveType.None),
            new VideoInfo(13, VideoType.Mobile, 0, false, AudioType.Aac, 0, AdaptiveType.None),
            new VideoInfo(17, VideoType.Mobile, 144, false, AudioType.Aac, 24, AdaptiveType.None),
            new VideoInfo(18, VideoType.Mp4, 360, false, AudioType.Aac, 96, AdaptiveType.None),
            new VideoInfo(22, VideoType.Mp4, 720, false, AudioType.Aac, 192, AdaptiveType.None),
            new VideoInfo(34, VideoType.Flash, 360, false, AudioType.Aac, 128, AdaptiveType.None),
            new VideoInfo(35, VideoType.Flash, 480, false, AudioType.Aac, 128, AdaptiveType.None),
            new VideoInfo(36, VideoType.Mobile, 240, false, AudioType.Aac, 38, AdaptiveType.None),
            new VideoInfo(37, VideoType.Mp4, 1080, false, AudioType.Aac, 192, AdaptiveType.None),
            new VideoInfo(38, VideoType.Mp4, 3072, false, AudioType.Aac, 192, AdaptiveType.None),
            new VideoInfo(43, VideoType.WebM, 360, false, AudioType.Vorbis, 128, AdaptiveType.None),
            new VideoInfo(44, VideoType.WebM, 480, false, AudioType.Vorbis, 128, AdaptiveType.None),
            new VideoInfo(45, VideoType.WebM, 720, false, AudioType.Vorbis, 192, AdaptiveType.None),
            new VideoInfo(46, VideoType.WebM, 1080, false, AudioType.Vorbis, 192, AdaptiveType.None),

            /* 3d */
            new VideoInfo(82, VideoType.Mp4, 360, true, AudioType.Aac, 96, AdaptiveType.None),
            new VideoInfo(83, VideoType.Mp4, 240, true, AudioType.Aac, 96, AdaptiveType.None),
            new VideoInfo(84, VideoType.Mp4, 720, true, AudioType.Aac, 152, AdaptiveType.None),
            new VideoInfo(85, VideoType.Mp4, 520, true, AudioType.Aac, 152, AdaptiveType.None),
            new VideoInfo(100, VideoType.WebM, 360, true, AudioType.Vorbis, 128, AdaptiveType.None),
            new VideoInfo(101, VideoType.WebM, 360, true, AudioType.Vorbis, 192, AdaptiveType.None),
            new VideoInfo(102, VideoType.WebM, 720, true, AudioType.Vorbis, 192, AdaptiveType.None),

            /* Adaptive (aka DASH) - Video */
            new VideoInfo(133, VideoType.Mp4, 240, false, AudioType.Unknown, 0, AdaptiveType.Video),
            new VideoInfo(134, VideoType.Mp4, 360, false, AudioType.Unknown, 0, AdaptiveType.Video),
            new VideoInfo(135, VideoType.Mp4, 480, false, AudioType.Unknown, 0, AdaptiveType.Video),
            new VideoInfo(136, VideoType.Mp4, 720, false, AudioType.Unknown, 0, AdaptiveType.Video),
            new VideoInfo(137, VideoType.Mp4, 1080, false, AudioType.Unknown, 0, AdaptiveType.Video),
            new VideoInfo(138, VideoType.Mp4, 2160, false, AudioType.Unknown, 0, AdaptiveType.Video),
            new VideoInfo(160, VideoType.Mp4, 144, false, AudioType.Unknown, 0, AdaptiveType.Video),
            new VideoInfo(242, VideoType.WebM, 240, false, AudioType.Unknown, 0, AdaptiveType.Video),
            new VideoInfo(243, VideoType.WebM, 360, false, AudioType.Unknown, 0, AdaptiveType.Video),
            new VideoInfo(244, VideoType.WebM, 480, false, AudioType.Unknown, 0, AdaptiveType.Video),
            new VideoInfo(247, VideoType.WebM, 720, false, AudioType.Unknown, 0, AdaptiveType.Video),
            new VideoInfo(248, VideoType.WebM, 1080, false, AudioType.Unknown, 0, AdaptiveType.Video),
            new VideoInfo(264, VideoType.Mp4, 1440, false, AudioType.Unknown, 0, AdaptiveType.Video),
            new VideoInfo(271, VideoType.WebM, 1440, false, AudioType.Unknown, 0, AdaptiveType.Video),
            new VideoInfo(272, VideoType.WebM, 2160, false, AudioType.Unknown, 0, AdaptiveType.Video),
            new VideoInfo(278, VideoType.WebM, 144, false, AudioType.Unknown, 0, AdaptiveType.Video),

            /* Adaptive (aka DASH) - Audio */
            new VideoInfo(139, VideoType.Mp4, 0, false, AudioType.Aac, 48, AdaptiveType.Audio),
            new VideoInfo(140, VideoType.M4a, 0, false, AudioType.m4a, 128, AdaptiveType.Audio),
            new VideoInfo(141, VideoType.Mp4, 0, false, AudioType.Aac, 256, AdaptiveType.Audio),
            new VideoInfo(171, VideoType.WebM, 0, false, AudioType.Vorbis, 128, AdaptiveType.Audio),
            new VideoInfo(172, VideoType.WebM, 0, false, AudioType.Vorbis, 192, AdaptiveType.Audio)
        };
    }
}
