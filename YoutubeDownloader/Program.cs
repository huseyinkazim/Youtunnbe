using Business;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace YoutubeDownloader
{
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
                    //link = "https://www.youtube.com/watch?v=LWE79K2Ii-s";
                    //link = "https://www.youtube.com/watch?v=YQHsXMglC9A";
                    //link = "https://www.youtube.com/watch?v=7F--wQVviSI";
                    //link = "https://www.youtube.com/watch?v=YQHsXMglC9A";
                    IEnumerable<VideoInfo> videoInfos = YoutubeManager.YoutubeMediaUrls(link);

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
