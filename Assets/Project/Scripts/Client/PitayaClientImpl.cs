using System;
using UnityEngine;
using Pitaya;
using Protos;
using ClusterProtos;
using Playa.Common.Utils;
using Google.Protobuf;

namespace Playa.Client
{
    public class PitayaClientImpl : MonoBehaviour
    {
        //comment: dll import not smart
        public IPitayaClient client;
        public string JoinUUID="1001";
        void Awake()
        {
            Debug.Log("pitaya client awake");
            client = new PitayaClient();
            Debug.Log("pitaya client init");
            client.NetWorkStateChangedEvent += (networkState, error) =>
            {
                switch (networkState)
                {
                    case PitayaNetWorkState.Connected:
                        break;
                    case PitayaNetWorkState.Disconnected:
                        break;
                    case PitayaNetWorkState.FailToConnect:
                        break;
                    case PitayaNetWorkState.Kicked:
                        break;
                    case PitayaNetWorkState.Closed:
                        break;
                    case PitayaNetWorkState.Connecting:
                        break;
                    case PitayaNetWorkState.Timeout:
                        break;
                    case PitayaNetWorkState.Error:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(networkState), networkState, null);
                }
            };
        }

        public void Connect(string ip, int port)
        {
            client.Connect(ip, port);
            UserJoin u = new UserJoin();
            u.Uuid = JoinUUID;
            Request<Response>("connector.userjoin", u, (Response data) => {
                Debug.Log($"pitaya [connector.userjoin] - response={data}");
            });
        }

        private void OnApplicationQuit()
        {
            if (client != null)
            {
                client.Dispose();
                client = null;
            }
        }

        public void SubscribeRoute<IMessage>(string route, Action<IMessage> msgfunc)
        {
            client.OnRoute<IMessage>(route, (IMessage data) => {
                msgfunc(data);
            });
        }

        public void Request<IMessage>(string route, object message, Action<IMessage> msgfunc)
        {
            client.Request<IMessage>(route, message,
            (IMessage res) =>
            {
                msgfunc(res);
            },
            error =>
            {
                Debug.Log($"pitaya pb [{route}] ERROR - error-code={error.Code} metadata={error.Metadata}");
            });
        }

    }
}