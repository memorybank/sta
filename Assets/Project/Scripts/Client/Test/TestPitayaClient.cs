using System;
using UnityEngine;
using Pitaya;
using Protos;
using ClusterProtos;
using Playa.Common.Utils;

namespace Playa.Client
{
    class TestPitayaClient : MonoBehaviour
    {
        //comment: dll import not smart
        public IPitayaClient client;
        void Start()
        {
            Debug.Log("pitaya client start");
            clientTest1();
            Debug.Log("pitaya client test complete");
        }

        private void OnApplicationQuit()
        {
            if (client != null)
            {
                client.Dispose();
                client = null;
            }
        }

        private void clientTest1()
        {
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
            //comment: port depends on server tcp
            client.Connect("127.0.0.1", 3351);

            SessionData a = new SessionData();
            client.Request<SessionData>("connector.getsessiondata", a, 
            (SessionData res) => {
                Debug.Log($"pitaya pb [connector.getsessiondata] - response={res}");
            },
            error => {
                Debug.Log($"pitaya pb [connector.getsessiondata] ERROR - error-code={error.Code} metadata={error.Metadata}");
            });

            BigMessage d = new BigMessage();
            client.Request<BigMessage>("connector.getbigmessage", d,
            (BigMessage res) =>
            {
                Debug.Log($"pitaya pb [connector.getbigmessage] - response={res}");
            },
            error =>
            {
                Debug.Log($"pitaya pb [connector.getbigmessage] ERROR - error-code={error.Code} metadata={error.Metadata}");
            });

            client.OnRoute<NewUser>("onNewUser", (NewUser data) => {
                Debug.Log("pitaya [connector.room.onnewuser] PUSH RESPONSE = " + data);
            });

            client.OnRoute<UserUseItem>("onUseItem", (UserUseItem data) =>
            {
                Debug.Log("pitaya [connector.room.onuseitem] PUSH RESPONSE = " + data);
            });

            UserJoin uj = new UserJoin();
            uj.Uuid = 11.ToString();
            client.Request<Response>("connector.userjoin", uj,
            (Response res) =>
            {
                Debug.Log($"pitaya [connector.userjoin] - response={res}");
            },
            error =>
            {
                Debug.Log($"pitaya [connector.userjoin] ERROR - error-code={error.Code} metadata={error.Metadata}");
            });

            UserUseItem item = new UserUseItem();
            item.Uuid = 11.ToString();
            item.Name = "guitar";
            item.Timestamp = (long)TimeUtils.GetMSTimestamp();
            client.Request<Response>("connector.useitem", item,
            (Response res) =>
            {
                Debug.Log($"pitaya [connector.useitem] - response={res}");
            },
            error =>
            {
                Debug.Log($"pitaya [connector.useitem] ERROR - error-code={error.Code} metadata={error.Metadata}");
            });

        }

    }
}