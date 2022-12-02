using Recognissimo.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Playa.Common;
using agora_gaming_rtc;
using Playa.Audio.Agora;
using Playa.Audio.MicrophoneEngine;
using FrostweepGames.Plugins.Native;
using System.Linq;

namespace Playa.Audio
{
    // TODO: Rewrite this
    public class AgoraSource : AvatarAudioSource
    {
        [SerializeField] private AgoraManager _AgoraManager;

        //todo: deprecate file
        //public AudioPeer audioPeer;

        private const string GreetingMessage = "Press 'Space' or tap to start/stop microphone";
        private const string RecognitionStartedMessage = "Recording...";

        private static readonly ArrayPool<float> Pool = ArrayPool<float>.Shared;

        private int _SampleRate;
        [SerializeField] private int _MaxRecordingTime;
        [SerializeField] private int _TimeSensitivity;

        private int _ClipSamplesLength;
        //private int _LastPos;
        private int _MaxSamplesNum;
        private int _MinSamplesNum;
        private float _ElapsedRecordTime = 0.0f;

        private IAudioRawDataManager _audioRawDataManager;
        private IRtcEngine _rtcEngine = null;

        private const int CHANNEL = 1;
        private const int PULL_FREQ_PER_SEC = 100;
        //comment: from audio component
        //public const int SAMPLE_RATE = 32000; // this should = CLIP_SAMPLES x PULL_FREQ_PER_SEC
        //public const int CLIP_SAMPLES = 320;

        private int _count;
        private int _writeCount;
        private int _readCount;
        private float[] _currentAudioSamples;
        private int _currentSamplePosition;
        private int _previousSamplePosition;
        //comment: there is a ring buffer

        private int AudioChunkSize => _SampleRate / 1000 * _TimeSensitivity;

        private string _channelName;
        private uint _uid;

        List<float> _currentRecordedSamples = new List<float>();

        // UI components
        // [SerializeField] private Text text;

        override public int SampleRate => _SampleRate;

        override public void StartAudio()
        {
            if (_Clip == null)
            {
                Debug.Log("Null Clip");
                return;
            }
            
            _IsPlaying = true;
            //_LastPos = 0;
            _ClipSamplesLength = _Clip.samples;

            _MinSamplesNum = _SampleRate / 1000 * _TimeSensitivity;
            _MaxSamplesNum = Math.Min(ArrayPool<float>.MaxArraySize, _ClipSamplesLength);
            Debug.Log(String.Format("MinSample {0}", _MinSamplesNum));
            Debug.Log(String.Format("MaxSample {0}", _MaxSamplesNum));

            // text.text = RecognitionStartedMessage;
        }

        override public void StopAudio()
        {
            _IsPlaying = false;

            // text.text = GreetingMessage;
        }

        override public int GetClipPosition()
        {
            return _writeCount % _audioBuffer.Length;
        }

        override public IEnumerator WaitForClipReady()
        {
            while (!_Clip)
            {
                yield return new WaitForSeconds(ThreadsConstants.WaitForReady);
            }
            yield return null;
        }

        void Awake()
        {
            // TODO: Set Agora playback audio frequency
            _SampleRate = 32000;
            // //The larger the buffer, the higher the delay
            var bufferLength = _SampleRate / PULL_FREQ_PER_SEC * CHANNEL * 100; // 1-sec-length buffer
            _audioBuffer = new float[bufferLength];
            _currentAudioSamples = new float[_audioBuffer.Length];
            if (_AgoraManager != null)
            {
                _AgoraManager.ChatRoomManager.eventLocalUserJoinedChannel += OnEventLocalChannelReady;
                _AgoraManager.ChatRoomManager.eventRemoteUserJoined += OnEventRemoteUserJoined;
            }
        }

        private void OnDestroy()
        {
            _audioRawDataManager?.UnRegisterAudioRawDataObserver();
            if (_AgoraManager != null)
            {
                _AgoraManager.ChatRoomManager.eventLocalUserJoinedChannel -= OnEventLocalChannelReady;
                _AgoraManager.ChatRoomManager.eventRemoteUserJoined -= OnEventRemoteUserJoined;
            }
        }

        private void OnEventLocalChannelReady(string channelName, uint uid, IRtcEngine channelData)
        {
            _channelName = channelName;
            _uid = uid;
            _rtcEngine = channelData;

            if (audioSourceType != AudioSourceType.Local) return;
            //_audioRawDataManager?.SetOnRecordAudioFrameCallback();
            SetClipData(audioSourceType, _audioBuffer, null);
            _currentAudioSamples = new float[_audioBuffer.Length];
        }

        private void OnEventRemoteUserJoined(uint uid)
        {
            _uid = uid;

            if (audioSourceType != AudioSourceType.Remote) return;
            SetupAudio("BackAudio");
            _audioRawDataManager?.SetOnPlaybackAudioFrameBeforeMixingCallback(OnPlaybackAudioFrameHandler);
            Debug.Log("audioClip = " + audioSource.clip + " uid = " + _uid);
        }

        void SetupAudio(string clipName)
        {
            _audioRawDataManager = AudioRawDataManager.GetInstance(_rtcEngine);
            if (_audioRawDataManager == null)
            {
                Debug.Log("No back audio, uid = " + _uid);
                return;
            }

            // TODO: Check what if remote user has not joined

            var nRet = _audioRawDataManager.RegisterAudioRawDataObserver();

            _rtcEngine.SetParameter("che.audio.external_render", true);

            Debug.Log("´´½¨ audio clip = " + clipName + " uid = " + _uid);
            // _clip = AudioClip.Create(clipName,
            //    SAMPLE_RATE,
            //    CHANNEL, SAMPLE_RATE, false);

            _Clip = AudioClip.Create(clipName,
                _SampleRate,
                CHANNEL, _SampleRate, true,
                OnAudioRead);

            //_AvatarAudioSource._Clip = audioClip;
            //todo what does audio peer do?
            SetClipData(audioSourceType, _audioBuffer, _Clip);

            audioSource.loop = true;
            audioSource.Play();
        }

        private void OnAudioRead(float[] data)
        {
            for (var i = 0; i < data.Length; i++)
            {
                lock (_audioBuffer)
                {
                    if (_audioBuffer[i] - 0.0f > 1e-6)
                    {
                        data[i] = _audioBuffer[i];
                        _readCount += 1;
                        
                    }
                }
            }

            if (!_IsPlaying)
            {
                return;
            }

            _currentSamplePosition = GetClipPosition();
            Debug.Log(string.Format("VAD agora on audio read sample {0} currentAudioSamples {1}", _audioBuffer.Length, _currentAudioSamples.Length));
            var success = CustomMicrophone.GetRawData(ref _currentAudioSamples, _audioBuffer);
            Debug.Log(string.Format("VAD agora previous {0} current {1}", _previousSamplePosition, _currentSamplePosition));
            if (success)
            {
                if (_previousSamplePosition > _currentSamplePosition)
                {
                    for (int i = _previousSamplePosition; i < _currentAudioSamples.Length; i++)
                    {
                        if (_currentAudioSamples[i] > 0.1f)
                        {
                            Debug.Log("VAD webrtc remote find speaking");
                        }

                        _currentRecordedSamples.Add(_currentAudioSamples[i]);
                    }

                    _previousSamplePosition = 0;
                }

                for (int i = _previousSamplePosition; i < _currentSamplePosition; i++)
                {
                    if (_currentAudioSamples[i] > 0.1f)
                    {
                        Debug.Log("VAD webrtc remote find speaking");
                    }

                    _currentRecordedSamples.Add(_currentAudioSamples[i]);
                }

                _previousSamplePosition = _currentSamplePosition;
            }
            else
            {
                Debug.LogWarning("VAD agora source get sample fail");
            }

            Debug.LogFormat("buffer length remains: {0}, uid = {1}", _writeCount - _readCount, _uid);
        }

        private byte[] FloatToBytes(float sample)
        {
            return System.BitConverter.GetBytes((short)(sample * 32767));
        }

        void OnPlaybackAudioFrameHandler(uint uid = 0, AudioFrame audioFrame = new AudioFrame())
        {
            if (_count == 1)
            {
                Debug.LogFormat("audioFrame = {0} £¬ uid = {1}", audioFrame, _uid);
            }
            var floatArray = ConvertByteToFloat16(audioFrame.buffer);

            lock (_audioBuffer)
            {
                if (floatArray.Length > _audioBuffer.Length - GetClipPosition())
                {
                    float[] b = floatArray.Skip(_audioBuffer.Length - GetClipPosition()).ToArray();
                    b.CopyTo(_audioBuffer, 0);
                    floatArray = floatArray.Skip(0).Take(_audioBuffer.Length - GetClipPosition()).ToArray();
                }
                floatArray.CopyTo(_audioBuffer, GetClipPosition());

                _writeCount += floatArray.Length;
                //_currentSamplePosition = (_currentSamplePosition + floatArray.Length) % _audioBuffer.Capacity;
                //Debug.Log(string.Format("VAD agora write buffer read {0} write {1}", _previousSamplePosition, _currentSamplePosition));
                _count++;
            }
        }

        private static float[] ConvertByteToFloat16(byte[] byteArray)
        {
            var floatArray = new float[byteArray.Length / 2];
            for (var i = 0; i < floatArray.Length; i++)
            {
                floatArray[i] = BitConverter.ToInt16(byteArray, i * 2) / 32768f; // -Int16.MinValue
            }

            return floatArray;
        }

        void Start()
        {
            // text.text = GreetingMessage;
        }

        public float GetNextElapsedRecordTime()
        {
            return _ElapsedRecordTime;
        }

        void FixedUpdate()
        {
            if (!IsPlaying)
            {
                return;
            }

            if (_audioBuffer == null)
            {
                return;
            }

            if (_currentRecordedSamples.Count <= 0)
            {
                return;
            }

            _ElapsedRecordTime += Time.deltaTime;

            if (_currentRecordedSamples.Count >= AudioChunkSize)
            {
                var samplesChunk = _currentRecordedSamples.GetRange(0, AudioChunkSize);

                for (var i = 0; i < samplesChunk.Count; i++)
                {
                    // to mono 16-bit PCM
                    samplesChunk[i] = (short)(short.MaxValue * samplesChunk[i]);
                }

                SamplesReady?.Invoke(this, new SamplesReadyEvent(samplesChunk.ToArray(), AudioChunkSize, _ElapsedRecordTime));
                _currentRecordedSamples.RemoveRange(0, AudioChunkSize);
            }
        }

        //void Update()
        //{
        //    CheckSampleEvent();
        //}

        //private void CheckSampleEvent()
        //{
        //    if (!_IsPlaying)
        //    {
        //        return;
        //    }

        //    var samples = Pool.Rent(Clip.samples);
        //    SamplesReady?.Invoke(this, new SamplesReadyEvent(samples, Clip.samples, _ElapsedRecordTime));
        //}
    }
}