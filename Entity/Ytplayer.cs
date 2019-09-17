using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Args
    {
        public string hl { get; set; }
        public string account_playback_token { get; set; }
        public string watermark { get; set; }
        public string loaderUrl { get; set; }
        public string innertube_context_client_version { get; set; }
        public string fflags { get; set; }
        public string host_language { get; set; }
        public string fexp { get; set; }
        public string enablecsi { get; set; }
        public string cr { get; set; }
        public string csi_page_type { get; set; }
        public string enablejsapi { get; set; }
        public string innertube_api_key { get; set; }
        public string c { get; set; }
        public string player_response { get; set; }
        public PlayerResponse responseModel  { get; set; }
        public string gapi_hint_params { get; set; }
        public string vss_host { get; set; }
        public string cver { get; set; }
        public string innertube_api_version { get; set; }
    }

    public class Assets
    {
        public string css { get; set; }
        public string js { get; set; }
    }

    public class Attrs
    {
        public string id { get; set; }
    }

    public class Ytplyer
    {
        public Args args { get; set; }
        public Assets assets { get; set; }
        public Attrs attrs { get; set; }
    }

    public class PlayabilityStatus
    {
        public string status { get; set; }
        public bool playableInEmbed { get; set; }
    }

    public class Format
    {
        public int itag { get; set; }
        public string mimeType { get; set; }
        public long bitrate { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string lastModified { get; set; }
        public string contentLength { get; set; }
        public string quality { get; set; }
        public string qualityLabel { get; set; }
        public string projectionType { get; set; }
        public int averageBitrate { get; set; }
        public string audioQuality { get; set; }
        public string approxDurationMs { get; set; }
        public string audioSampleRate { get; set; }
        public int audioChannels { get; set; }
        public string cipher { get; set; }
        public string url { set; get; }

    }

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

    public class ColorInfo
    {
        public string primaries { get; set; }
        public string transferCharacteristics { get; set; }
        public string matrixCoefficients { get; set; }
    }

    public class AdaptiveFormat
    {
        public int itag { get; set; }
        public string mimeType { get; set; }
        public long bitrate { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public InitRange initRange { get; set; }
        public IndexRange indexRange { get; set; }
        public string lastModified { get; set; }
        public string contentLength { get; set; }
        public string quality { get; set; }
        public int fps { get; set; }
        public string qualityLabel { get; set; }
        public string projectionType { get; set; }
        public int averageBitrate { get; set; }
        public string approxDurationMs { get; set; }
        public string cipher { get; set; }
        public ColorInfo colorInfo { get; set; }
        public bool? highReplication { get; set; }
        public string audioQuality { get; set; }
        public string audioSampleRate { get; set; }
        public int? audioChannels { get; set; }
        public string url { get; set; }
    }

    public class StreamingData
    {
        public string expiresInSeconds { get; set; }
        public List<Format> formats { get; set; }
        public List<AdaptiveFormat> adaptiveFormats { get; set; }
    }

    public class PlayerAdParams
    {
        public bool showContentThumbnail { get; set; }
        public string enabledEngageTypes { get; set; }
    }

    public class AfvParams
    {
        public string googleAdBlock { get; set; }
        public string googleAdChannel { get; set; }
        public string googleAdClient { get; set; }
        public string googleAdFormat { get; set; }
        public string googleAdHeight { get; set; }
        public string googleAdHost { get; set; }
        public string googleAdType { get; set; }
        public string googleLact { get; set; }
        public string googleLanguage { get; set; }
        public string googleLoeid { get; set; }
        public string googlePageUrl { get; set; }
        public string googleVideoDocId { get; set; }
        public string googleYtPt { get; set; }
        public string googleCoreDbp { get; set; }
        public string googlePucrd { get; set; }
    }

    public class AfcParams
    {
        public string adBlock { get; set; }
        public string adChannel { get; set; }
        public string adClient { get; set; }
        public string adHost { get; set; }
        public string adType { get; set; }
        public string format { get; set; }
        public string lact { get; set; }
        public string language { get; set; }
        public string videoDocId { get; set; }
        public string coreDbp { get; set; }
        public string loeid { get; set; }
        public string pucrd { get; set; }
    }

    public class GutParams
    {
        public string tag { get; set; }
    }

    public class PlayerLegacyDesktopWatchAdsRenderer
    {
        public PlayerAdParams playerAdParams { get; set; }
        public AfvParams afvParams { get; set; }
        public AfcParams afcParams { get; set; }
        public GutParams gutParams { get; set; }
        public bool showAfv { get; set; }
        public bool showCompanion { get; set; }
        public bool showInstream { get; set; }
        public bool useGut { get; set; }
    }

    public class PlayerAd
    {
        public PlayerLegacyDesktopWatchAdsRenderer playerLegacyDesktopWatchAdsRenderer { get; set; }
    }

    public class VideostatsPlaybackUrl
    {
        public string baseUrl { get; set; }
    }

    public class VideostatsDelayplayUrl
    {
        public string baseUrl { get; set; }
    }

    public class VideostatsWatchtimeUrl
    {
        public string baseUrl { get; set; }
    }

    public class PtrackingUrl
    {
        public string baseUrl { get; set; }
    }

    public class QoeUrl
    {
        public string baseUrl { get; set; }
    }

    public class SetAwesomeUrl
    {
        public string baseUrl { get; set; }
        public int elapsedMediaTimeSeconds { get; set; }
    }

    public class AtrUrl
    {
        public string baseUrl { get; set; }
        public int elapsedMediaTimeSeconds { get; set; }
    }

    public class PlaybackTracking
    {
        public VideostatsPlaybackUrl videostatsPlaybackUrl { get; set; }
        public VideostatsDelayplayUrl videostatsDelayplayUrl { get; set; }
        public VideostatsWatchtimeUrl videostatsWatchtimeUrl { get; set; }
        public PtrackingUrl ptrackingUrl { get; set; }
        public QoeUrl qoeUrl { get; set; }
        public SetAwesomeUrl setAwesomeUrl { get; set; }
        public AtrUrl atrUrl { get; set; }
    }

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

    public class VideoDetails
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

    public class PlayerAnnotationsUrlsRenderer
    {
        public string invideoUrl { get; set; }
        public string loadPolicy { get; set; }
        public bool allowInPlaceSwitch { get; set; }
    }

    public class Annotation
    {
        public PlayerAnnotationsUrlsRenderer playerAnnotationsUrlsRenderer { get; set; }
    }

    public class AudioConfig
    {
        public double loudnessDb { get; set; }
        public double perceptualLoudnessDb { get; set; }
    }

    public class StreamSelectionConfig
    {
        public string maxBitrate { get; set; }
    }

    public class DynamicReadaheadConfig
    {
        public int maxReadAheadMediaTimeMs { get; set; }
        public int minReadAheadMediaTimeMs { get; set; }
        public int readAheadGrowthRateMs { get; set; }
    }

    public class MediaCommonConfig
    {
        public DynamicReadaheadConfig dynamicReadaheadConfig { get; set; }
    }

    public class PlayerConfig
    {
        public AudioConfig audioConfig { get; set; }
        public StreamSelectionConfig streamSelectionConfig { get; set; }
        public MediaCommonConfig mediaCommonConfig { get; set; }
    }

    public class PlayerStoryboardSpecRenderer
    {
        public string spec { get; set; }
    }

    public class Storyboards
    {
        public PlayerStoryboardSpecRenderer playerStoryboardSpecRenderer { get; set; }
    }

    public class BotguardData
    {
        public string program { get; set; }
        public string interpreterUrl { get; set; }
    }

    public class PlayerAttestationRenderer
    {
        public string challenge { get; set; }
        public BotguardData botguardData { get; set; }
    }

    public class Attestation
    {
        public PlayerAttestationRenderer playerAttestationRenderer { get; set; }
    }

    public class Run
    {
        public string text { get; set; }
    }

    public class MessageText
    {
        public List<Run> runs { get; set; }
    }

    public class Run2
    {
        public string text { get; set; }
    }

    public class Text
    {
        public List<Run2> runs { get; set; }
    }

    public class BrowseEndpoint
    {
        public string browseId { get; set; }
        public string @params { get; set; }
    }

    public class NavigationEndpoint
    {
        public string clickTrackingParams { get; set; }
        public BrowseEndpoint browseEndpoint { get; set; }
    }

    public class ButtonRenderer
    {
        public string style { get; set; }
        public string size { get; set; }
        public Text text { get; set; }
        public NavigationEndpoint navigationEndpoint { get; set; }
        public string trackingParams { get; set; }
    }

    public class ActionButton
    {
        public ButtonRenderer buttonRenderer { get; set; }
    }

    public class Run3
    {
        public string text { get; set; }
    }

    public class Text2
    {
        public List<Run3> runs { get; set; }
    }

    public class UiActions
    {
        public bool hideEnclosingContainer { get; set; }
    }

    public class FeedbackEndpoint
    {
        public string feedbackToken { get; set; }
        public UiActions uiActions { get; set; }
    }

    public class ServiceEndpoint
    {
        public string clickTrackingParams { get; set; }
        public FeedbackEndpoint feedbackEndpoint { get; set; }
    }

    public class ButtonRenderer2
    {
        public string style { get; set; }
        public string size { get; set; }
        public Text2 text { get; set; }
        public ServiceEndpoint serviceEndpoint { get; set; }
        public string trackingParams { get; set; }
    }

    public class DismissButton
    {
        public ButtonRenderer2 buttonRenderer { get; set; }
    }

    public class UiActions2
    {
        public bool hideEnclosingContainer { get; set; }
    }

    public class FeedbackEndpoint2
    {
        public string feedbackToken { get; set; }
        public UiActions2 uiActions { get; set; }
    }

    public class ImpressionEndpoint
    {
        public string clickTrackingParams { get; set; }
        public FeedbackEndpoint2 feedbackEndpoint { get; set; }
    }

    public class Run4
    {
        public string text { get; set; }
    }

    public class MessageTitle
    {
        public List<Run4> runs { get; set; }
    }

    public class MealbarPromoRenderer
    {
        public List<MessageText> messageTexts { get; set; }
        public ActionButton actionButton { get; set; }
        public DismissButton dismissButton { get; set; }
        public string triggerCondition { get; set; }
        public string style { get; set; }
        public string trackingParams { get; set; }
        public List<ImpressionEndpoint> impressionEndpoints { get; set; }
        public bool isVisible { get; set; }
        public MessageTitle messageTitle { get; set; }
    }

    public class Message
    {
        public MealbarPromoRenderer mealbarPromoRenderer { get; set; }
    }

    public class AdTimeOffset
    {
        public string offsetStartMilliseconds { get; set; }
        public string offsetEndMilliseconds { get; set; }
    }

    public class AdPlacementConfig
    {
        public string kind { get; set; }
        public AdTimeOffset adTimeOffset { get; set; }
        public bool hideCueRangeMarker { get; set; }
    }

    public class Config
    {
        public AdPlacementConfig adPlacementConfig { get; set; }
    }

    public class AdBreakServiceRenderer
    {
        public string getAdBreakUrl { get; set; }
        public string prefetchMilliseconds { get; set; }
    }

    public class Renderer
    {
        public AdBreakServiceRenderer adBreakServiceRenderer { get; set; }
    }

    public class AdPlacementRenderer
    {
        public Config config { get; set; }
        public Renderer renderer { get; set; }
        public string trackingParams { get; set; }
    }

    public class AdPlacement
    {
        public AdPlacementRenderer adPlacementRenderer { get; set; }
    }

    public class ApmUserPreference
    {
    }

    public class AdSafetyReason
    {
        public ApmUserPreference apmUserPreference { get; set; }
    }

    public class PlayerResponse
    {
        public PlayabilityStatus playabilityStatus { get; set; }
        public StreamingData streamingData { get; set; }
        public List<PlayerAd> playerAds { get; set; }
        public PlaybackTracking playbackTracking { get; set; }
        public VideoDetails videoDetails { get; set; }
        public List<Annotation> annotations { get; set; }
        public PlayerConfig playerConfig { get; set; }
        public Storyboards storyboards { get; set; }
        public string trackingParams { get; set; }
        public Attestation attestation { get; set; }
        public List<Message> messages { get; set; }
        public List<AdPlacement> adPlacements { get; set; }
        public AdSafetyReason adSafetyReason { get; set; }
    }
}
