using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Thumbnail2
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Thumbnail
    {
        public List<Thumbnail2> thumbnails { get; set; }
    }

    public class VideoDetail
    {
        public string videoId { get; set; }
        public string title { get; set; }
        public string lengthSeconds { get; set; }
        public List<string> keywords { get; set; }
        public string channelId { get; set; }
        public bool isOwnerViewing { get; set; }
        public string shortDescription { get; set; }
        public bool isCrawlable { get; set; }
        public Thumbnail thumbnail { get; set; }
        public bool useCipher { get; set; }
        public double averageRating { get; set; }
        public bool allowRatings { get; set; }
        public string viewCount { get; set; }
        public string author { get; set; }
        public bool isPrivate { get; set; }
        public bool isUnpluggedCorpus { get; set; }
        public bool isLiveContent { get; set; }
    }

    public class VideoDownloadParameter
    {
        public JObject json { set; get; }
        public string videoTitle { set; get; }
        public string jsPath { set; get; }
        public string[] splitByUrls { set; get; }
        public string youtubeLinkId { set; get; }
    }
}
