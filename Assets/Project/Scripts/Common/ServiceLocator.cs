using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Playa.Audio;
using Playa.Event;

namespace Playa.Common
{
    public class ServiceLocator : MonoBehaviour
    {
        [SerializeField] private AudioService _AudioService;

        [SerializeField] private EventSequencer _EventSequencerLocal;

        [SerializeField] private EventSequencer _EventSequencerRemote;

        public AudioService AudioService => _AudioService;

        public EventSequencer EventSequencerLocal => _EventSequencerLocal;

        public EventSequencer EventSequencerRemote => _EventSequencerRemote;

    }
}