using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VibelightApp.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidMicrophone))]

namespace VibelightApp.Droid
{
    class AndroidMicrophone : IAndroidMicrophone
    {
        private MediaRecorder MicrophoneRec;
        private AudioSource Source;
        private OutputFormat Output;
        private AudioEncoder Encoder;
        private bool AlreadyStarted = false;
        private bool InitializedSuccesfully = false;
        public async Task<bool> InitializeMicrophoneAsync()
        {
            bool SettingsFound = true;


            await Task.Run(() =>
            {

                MediaRecorder mRec = new MediaRecorder();

                AudioSource[] sources = new AudioSource[] { AudioSource.Default, AudioSource.Mic, AudioSource.RemoteSubmix, AudioSource.VoiceDownlink, AudioSource.VoiceCall, AudioSource.VoiceRecognition, AudioSource.Camcorder, AudioSource.RemoteSubmix, AudioSource.Unprocessed, AudioSource.VoicePerformance, AudioSource.VoiceCommunication };
                OutputFormat[] outputs = new OutputFormat[] { OutputFormat.Default, OutputFormat.ThreeGpp, OutputFormat.Mpeg4, OutputFormat.AmrNb, OutputFormat.RawAmr, OutputFormat.AmrWb, OutputFormat.AacAdts, OutputFormat.Mpeg2Ts, OutputFormat.Ogg, OutputFormat.Webm };
                AudioEncoder[] encoders = new AudioEncoder[] { AudioEncoder.Default, AudioEncoder.AmrNb, AudioEncoder.AmrWb, AudioEncoder.Aac, AudioEncoder.HeAac, AudioEncoder.AacEld, AudioEncoder.Vorbis, AudioEncoder.Opus };

                foreach (AudioSource source in sources)
                {
                    foreach (OutputFormat output in outputs)
                    {
                        foreach (AudioEncoder encoder in encoders)
                        {
                            try
                            {
                                SettingsFound = true;
                                mRec.SetAudioSource(source);
                                Console.WriteLine(source.ToString());
                                mRec.SetOutputFormat(output);
                                mRec.SetAudioEncoder(encoder);
                                mRec.SetOutputFile("/dev/null");
                                mRec.Prepare();
                                mRec.Start();

                            }
                            catch (Exception e)
                            {
                                mRec = new MediaRecorder();
                                SettingsFound = false;
                            };
                            if (SettingsFound)
                            {

                                Console.WriteLine("jobeallitas " + source + " " + encoder + " " + output);

                                Source = source;
                                Output = output;
                                Encoder = encoder;

                                mRec.Stop();
                                mRec.Dispose();

                                SettingsFound = true;
                                InitializedSuccesfully = true;

                                goto LoopEnd;
                            }
                        }

                    }

                }
                 LoopEnd:;
            });

            return SettingsFound;

        }
        public bool IsInitialized()
        {
            return InitializedSuccesfully;
        }
        public bool IsStarted()
        {
            return AlreadyStarted;
        }
        public async Task StartMicrophoneAsync()
        {
            if (AlreadyStarted || !InitializedSuccesfully) { return; }
            AlreadyStarted = true;

            await Task.Run(() =>
            {
                MicrophoneRec = new MediaRecorder();
                MicrophoneRec.SetAudioSource(Source);
                MicrophoneRec.SetOutputFormat(Output);
                MicrophoneRec.SetAudioEncoder(Encoder);
                MicrophoneRec.SetOutputFile("/dev/null");
                MicrophoneRec.Prepare();
                MicrophoneRec.Start();
            });

        }
        public async Task StopMicrophoneAsync()
        {
            AlreadyStarted = false;
            await Task.Run(() =>
            {
                MicrophoneRec.Stop();
                MicrophoneRec.Dispose();

            });

        }
        public int GetAmplitude()
        {
            if (!AlreadyStarted) { return 0; }

            int Amplitude = MicrophoneRec.MaxAmplitude;
            // 0-10000 integer value
            return Amplitude / 2;
        }

    }
}