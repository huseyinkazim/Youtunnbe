using Business;
using Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YoutubeDownloader
{
    public class DownloadManager
    {
        public static VideoInfo ChooseVideo(IEnumerable<VideoInfo> videoInfos)
        {
            int i = 1, index; bool isRight = false;
            foreach (var item in videoInfos)
            {
                Console.WriteLine(i + ":" + item.ToString());
                i++;
            }

            do
            {
                Console.WriteLine("Lütfen seçeneklerden birini seçiniz:");
                var index_text = Console.ReadLine();
                isRight = Int32.TryParse(index_text, out index);
                if (isRight)
                    isRight = index <= i && index > 0;
            } while (!isRight);
            VideoInfo video = videoInfos.ToArray()[index - 1];
            return video;
        }
        public static void DownloadVideo(VideoInfo video)
        {
            Task.Run(() =>
            {
                var filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Youtube";
                VideoDownloader videoDownloader;
                if (video.Resolution != 0)
                    videoDownloader = new VideoDownloader(video,
                         Path.Combine(filePath,
                         RemoveIllegalPathCharacters(video.Title) + "_" + video.Resolution + video.VideoExtension));
                else
                    videoDownloader = new VideoDownloader(video,
                        Path.Combine(filePath,
                        RemoveIllegalPathCharacters(video.Title) + video.VideoExtension));

                videoDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage);

                videoDownloader.DownloadLinkAsync();
            });
        }
        private static string RemoveIllegalPathCharacters(string path)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("İndirilen dosyalar Belegelerim -> Youtube klasörünün içerisine kaydedilir");
            string link = "";
            do
            {
                try
                {
                    Console.WriteLine("Lütfen indermek istediğiniz linki yapıştırın: ");
                    link = Console.ReadLine();
                    //link = "https://www.youtube.com/watch?v=yCs6UmogKEg";
                    //link = "https://www.youtube.com/watch?v=seM8oqrU2qA";
                    //link = "https://www.youtube.com/watch?v=VfnefdC13w8";
                    //link = "https://www.youtube.com/watch?v=2a4Uxdy9TQY";
                    //link = "https://www.youtube.com/watch?v=w_vZom2qJQo";
                    //link = "https://www.youtube.com/watch?v=LWE79K2Ii-s";
                    //link = "https://www.youtube.com/watch?v=YQHsXMglC9A";
                    //link = "https://www.youtube.com/watch?v=7F--wQVviSI";
                    //link = "https://www.youtube.com/watch?v=YQHsXMglC9A";
                    //link = "https://www.youtube.com/embed/Rqz39wTP69o";
                    IYoutubeManager manager = new YoutubeManager();
                    IEnumerable<VideoInfo> videoInfos = manager.YoutubeMediaUrls(link);

                    var choosenVideo=DownloadManager.ChooseVideo(videoInfos);
                    DownloadManager.DownloadVideo(choosenVideo);
                    Console.WriteLine($"{videoInfos.FirstOrDefault().Title} adlı dosyanız indiriliyor.");

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Beklenmedik bir hata oluştu lütfen bizimle iletişime geçiniz");

                }
            } while (true); 
        }
    }
}
