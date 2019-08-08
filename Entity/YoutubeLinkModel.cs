using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
   

    public class Attrs
    {
        public string id { get; set; }
    }

    public class Assets
    {
        public string js { get; set; }
        public string css { get; set; }
    }

    public class Params
    {
        public string allowfullscreen { get; set; }
        public string allowscriptaccess { get; set; }
        public string bgcolor { get; set; }
    }

    public class Args
    {
        public string adaptive_fmts { get; set; }
        public string watermark { get; set; }
        public string enabled_engage_types { get; set; }
        public string qoe_cat { get; set; }
        public string loaderUrl { get; set; }
        public string length_seconds { get; set; }
        public string xhr_apiary_host { get; set; }
        public string hl { get; set; }
        public string apiary_host { get; set; }
        public string no_get_video_log { get; set; }
        public string ucid { get; set; }
        public string idpj { get; set; }
        public string gapi_hint_params { get; set; }
        public string enablejsapi { get; set; }
        public string account_playback_token { get; set; }
        public string external_play_video { get; set; }
        public string player_response { get; set; }
        public string video_id { get; set; }
        public string player_error_log_fraction { get; set; }
        public string cver { get; set; }
        public string fexp { get; set; }
        public string timestamp { get; set; }
        public string ssl { get; set; }
        public string title { get; set; }
        public string innertube_api_key { get; set; }
        public string fmt_list { get; set; }
        public string csi_page_type { get; set; }
        public string host_language { get; set; }
        public string ismb { get; set; }
        public string thumbnail_url { get; set; }
        public string innertube_context_client_version { get; set; }
        public string author { get; set; }
        public string vss_host { get; set; }
        public string itct { get; set; }
        public string loudness { get; set; }
        public string ldpj { get; set; }
        public string innertube_api_version { get; set; }
        public string token { get; set; }
        public string relative_loudness { get; set; }
        public string url_encoded_fmt_stream_map { get; set; }
        public string t { get; set; }
        public string cr { get; set; }
        public string apiary_host_firstparty { get; set; }
        public string c { get; set; }
        public string enablecsi { get; set; }
        public string tmi { get; set; }
        public string fflags { get; set; }
    }

    public class YoutubeLinkModel
    {
        public int sts { get; set; }
        public Attrs attrs { get; set; }
        public Assets assets { get; set; }
        public Params @params { get; set; }
        public bool html5 { get; set; }
        public Args args { get; set; }
        public string url { get; set; }
    }
}
