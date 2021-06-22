using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using YoutubeDownloader.Model;

namespace YoutubeDownloader.Business
{
    public abstract class Downloader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Downloader"/> class.
        /// </summary>
        /// <param name="video">The video to download/convert.</param>
        /// <param name="savePath">The path to save the video/audio.</param>
        /// /// <param name="bytesToDownload">An optional value to limit the number of bytes to download.</param>
        /// <exception cref="ArgumentNullException"><paramref name="video"/> or <paramref name="savePath"/> is <c>null</c>.</exception>
        protected Downloader(VideoInfo video, string savePath, int? bytesToDownload = null)
        {
            if (video == null)
                throw new ArgumentNullException("video");

            if (savePath == null)
                throw new ArgumentNullException("savePath");

            this.Video = video;
            this.SavePath = savePath;
            this.BytesToDownload = bytesToDownload;
            this.filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Youtube";
        }

        /// <summary>
        /// Occurs when the download finished.
        /// </summary>
        public event EventHandler DownloadFinished;

        /// <summary>
        /// Occurs when the download is starts.
        /// </summary>
        public event EventHandler DownloadStarted;

        /// <summary>
        /// Gets the number of bytes to download. <c>null</c>, if everything is downloaded.
        /// </summary>
        public int? BytesToDownload { get; private set; }

        /// <summary>
        /// Gets the path to save the video/audio.
        /// </summary>
        public string SavePath { get; private set; }
        public string filePath { get; private set; }

        /// <summary>
        /// Gets the video to download/convert.
        /// </summary>
        public VideoInfo Video { get; private set; }

        /// <summary>
        /// Starts the work of the <see cref="Downloader"/>.
        /// </summary>
        public abstract void Execute();

        protected void OnDownloadFinished(EventArgs e)
        {
            if (this.DownloadFinished != null)
            {
                this.DownloadFinished(this, e);
            }
        }

        protected void OnDownloadStarted(EventArgs e)
        {
            if (this.DownloadStarted != null)
            {
                this.DownloadStarted(this, e);
            }
        }
    }
    public class VideoDownloader : Downloader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoDownloader"/> class.
        /// </summary>
        /// <param name="video">The video to download.</param>
        /// <param name="savePath">The path to save the video.</param>
        /// <param name="bytesToDownload">An optional value to limit the number of bytes to download.</param>
        /// <exception cref="ArgumentNullException"><paramref name="video"/> or <paramref name="savePath"/> is <c>null</c>.</exception>
        public VideoDownloader(VideoInfo video, string savePath, int? bytesToDownload = null)
            : base(video, savePath, bytesToDownload)
        { }

        /// <summary>
        /// Occurs when the downlaod progress of the video file has changed.
        /// </summary>
        public event EventHandler<ProgressEventArgs> DownloadProgressChanged;

        public void DownloadLinkAsync()
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(bittiMesaji);


                client.DownloadFileAsync(new Uri(this.Video.DownloadUrl), this.SavePath);
            }

        }

        public void bittiMesaji(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled == false)
                Console.WriteLine($"{this.Video.Title} indirilmiştir");
            else
                IndirmeHatasi(e.Error);

        }

        public void IndirmeHatasi(Exception ex)
        {

            Console.WriteLine(ex.Message);
        }

        /// <summary>
        /// Starts the video download.
        /// </summary>
        /// <exception cref="IOException">The video file could not be saved.</exception>
        /// <exception cref="WebException">An error occured while downloading the video.</exception>
        public override void Execute()
        {
            this.OnDownloadStarted(EventArgs.Empty);

            var request = (HttpWebRequest)WebRequest.Create(this.Video.DownloadUrl);

            if (this.BytesToDownload.HasValue)
            {
                request.AddRange(0, this.BytesToDownload.Value - 1);
            }

            // the following code is alternative, you may implement the function after your needs
            using (WebResponse response = request.GetResponse())
            {
                using (Stream source = response.GetResponseStream())
                {
                    if (!Directory.Exists(this.filePath))
                        Directory.CreateDirectory(this.filePath);
                    using (FileStream target = File.Open(this.SavePath, FileMode.Create, FileAccess.Write))
                    {
                        var buffer = new byte[4096];
                        bool cancel = false;
                        int bytes;
                        int copiedBytes = 0;

                        while (!cancel && (bytes = source.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            target.Write(buffer, 0, bytes);

                            copiedBytes += bytes;
                            ProgressEventArgs eventArgs;
                            //if (((copiedBytes * 1.0 / response.ContentLength) * 100) % 10 == 0)
                            //{
                            eventArgs = new ProgressEventArgs((copiedBytes * 1.0 / response.ContentLength) * 100);

                            if (this.DownloadProgressChanged != null)
                            {
                                this.DownloadProgressChanged(this, eventArgs);

                                if (eventArgs.Cancel)
                                {
                                    cancel = true;
                                }
                                // }
                            }
                        }
                    }
                }
            }

            this.OnDownloadFinished(EventArgs.Empty);
        }
    }
    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(double progressPercentage)
        {
            this.ProgressPercentage = progressPercentage;
        }

        /// <summary>
        /// Gets or sets a token whether the operation that reports the progress should be canceled.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets the progress percentage in a range from 0.0 to 100.0.
        /// </summary>
        public double ProgressPercentage { get; private set; }
    }
}
