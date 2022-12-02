using Playa.Audio.Agora;
using Playa.Avatars;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.Audio
{
    public class SamplesReadyEvent : EventArgs
    {
        public SamplesReadyEvent(float[] samples, int length, float startTimestamp)
        {
            Samples = samples;
            Length = length;
            StartTimestamp = startTimestamp;
        }

        /// <summary>
        ///     Audio samples array. Only mono 16-bit PCM supported
        /// </summary>
        public float[] Samples { get; }

        /// <summary>
        ///     Audio samples array payload length
        /// </summary>
        public int Length { get; }

        public float StartTimestamp = 0.0f;
    }

    abstract public class AvatarAudioSource : MonoBehaviour
    {
        //protected AudioClip _Clip;
        public AudioClip _Clip;
        public AudioClip Clip => _Clip;

        //comment: true play source, not record. used in AgoraSource
        public AudioSource audioSource;

        protected bool _IsPlaying;
        public bool IsPlaying => _IsPlaying;

        public Agora.AudioSourceType audioSourceType;
        //public RingBuffer<float> _audioBuffer;
        public float[] _audioBuffer;

        abstract public void StartAudio();
        abstract public void StopAudio();

        abstract public int GetClipPosition();
        abstract public IEnumerator WaitForClipReady();

        virtual public int SampleRate { get; private set; }

        /// <summary>
        ///     Event signaling the arrival of new samples. The submitted samples will be added to the recognition queue
        /// </summary>
        /// public event?
        public EventHandler<SamplesReadyEvent> SamplesReady;

        public AvatarBrain _AvatarBrain;


        public void SetClipData(AudioSourceType sourceType, float[] audioBuffer, AudioClip clip)
        {
            //Debug.Assert(_Clip != null, "clip模式输入音频不能为空");

            audioSourceType = sourceType;
            _audioBuffer = audioBuffer;
            _Clip = clip;
            audioSource.clip = _Clip;
        }

        virtual public void Update()
        {
            CheckSpaceReleasedAction();
        }

        virtual public float GetElapsedRecordTime()
        {
            return 0;
        }

        private void CheckSpaceReleasedAction()
        {
            var spaceReleased = Input.GetKeyUp(KeyCode.Space);
            var touchEnded = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended;

            if (!spaceReleased && !touchEnded)
            {
                return;
            }

            if (IsPlaying)
            {
                Debug.Log("start audio is playing" + audioSourceType);
                StopAudio();
            }
            else
            {
                Debug.Log("start audio " + audioSourceType);
                StartAudio();
            }
        }
    }

}