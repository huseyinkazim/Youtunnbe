using System;
using System.Collections.Generic;
using System.Text;

namespace YoutubeDownloader.Model
{
    public class Language
    {
        public string code { get; set; }
        public string name { get; set; }//primary
        public string native { get; set; }
    }

    public class Location
    {
        public int geoname_id { get; set; }//primary
        public string capital { get; set; }
        public List<Language> languages { get; set; }
        public string country_flag { get; set; }
        public string country_flag_emoji { get; set; }
        public string country_flag_emoji_unicode { get; set; }
        public string calling_code { get; set; }
        public bool is_eu { get; set; }
    }

    public class IPModel
    {
        public string ip { get; set; }
        public string type { get; set; }
        public string continent_code { get; set; }
        public string continent_name { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
        public string region_code { get; set; }
        public string region_name { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public Location location { get; set; }

        public override string ToString() => string.Format("Ip:{3}:\n Ülke:{0} \n Şehir:{1} \n İlçe:{2} \n", country_name, region_name, city, ip);

    }
}
