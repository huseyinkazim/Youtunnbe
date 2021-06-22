using System.Collections.Generic;
using YoutubeDownloader.Entity.Enums;

namespace YoutubeDownloader.Model
{
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
