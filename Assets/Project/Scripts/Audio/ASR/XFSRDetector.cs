//using Playa.Audio.MicrophoneEngine;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Net.WebSockets;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using UnityEngine;
//using Playa.Common;
//using Newtonsoft.Json;
//using Playa.Audio.ASR;
//using Newtonsoft.Json.Linq;
//using TMPro;
//using Playa.NLP.Event;

//namespace Playa.Audio
//{
//    public class XFSRDetector : AudioDetector
//    {
//        private string _AppId = "fa2b3a96";
//        private string _AppSecret = "ZGQ2OTBmNmQyODVhMmFhMDA2MmExMDQ3";
//        private string _Appkey = "8d1433b709223e28aa7f210021be96c8";

//        private bool _IsConnected = false;
//        private bool _IsFirst = true;

//        private string _TimeStamp;

//        private ClientWebSocket _Ws;
//        private CancellationToken _Ct;

//        // UI components
//        [SerializeField] private TextMeshProUGUI _resultText;

//        // Start is called before the first frame update
//        private void Start()
//        {
//            // Asynchronously establish connection
//            Connect();
//            _SpeechSource.SamplesReady += async (o, args) => await OnSamplesReady(o, args);
//            StartCoroutine(RunStreamingRecognition());
//        }

//        private async Task Connect()
//        {
//            try
//            {
//                _Ws = new ClientWebSocket();
//                _Ct = new CancellationToken();
//                await _Ws.ConnectAsync(GetUri(), _Ct);
//                _IsConnected = true;
//                Debug.Log("XunFei client init finished");
//            }
//            catch (Exception ex)
//            {
//                Debug.Log("Exception Msg: " + ex.Message);
//                _Ws.Dispose();
//            }
//        }

//        /// <summary>
//        /// 获得请求URI
//        /// </summary>
//        /// <returns></returns>
//        private Uri GetUri()
//        {
//            var uriStr = "wss://iat-api.xfyun.cn/v2/iat";

//            Uri uri = new Uri(uriStr);
//            string date = DateTime.Now.ToString("r");
//            string signature_origin = string.Format("host: " + uri.Host + "\ndate: " + date + "\nGET " + uri.AbsolutePath + " HTTP/1.1");
//            HMACSHA256 mac = new HMACSHA256(Encoding.UTF8.GetBytes(_AppSecret));
//            string signature = Convert.ToBase64String(mac.ComputeHash(Encoding.UTF8.GetBytes(signature_origin)));
//            string authorization_origin = string.Format("api_key=\"{0}\",algorithm=\"hmac-sha256\",headers=\"host date request-line\",signature=\"{1}\"", _Appkey, signature);
//            string authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes(authorization_origin));
//            string requestUrl = string.Format("{0}?authorization={1}&date={2}&host={3}", uri, authorization, date, uri.Host);

//            return new Uri(requestUrl);
//        }

//        /// <summary>
//        /// HMACSHA1算法加密并返回ToBase64String
//        /// </summary>
//        /// <param name="text">要加密的原串</param>
//        /// <param name="key">私钥</param>
//        /// <returns>返回一个签名值(即哈希值)</returns>
//        public string ToHmacSHA1(string text, string key)

//        {
//            //HMACSHA1加密
//            HMACSHA1 hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(key));
//            byte[] hashBytes = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(text));

//            return Convert.ToBase64String(hashBytes);
//        }

//        override public string GetDetectedResult()
//        {
//            return "";
//        }

//        private async Task OnSamplesReady(object sender, SamplesReadyEvent e)
//        {
//            if (!_IsConnected)
//            {
//                return;
//            }

//            try
//            {


//                JObject json = new JObject
//                {
//                    {
//                        "common",new JObject{{ "app_id", _AppId}}
//                    },
//                    {
//                        "business",new JObject{
//                            {"language", "zh_cn"},//识别语音
//                            {"domain", "iat"},
//                            {"accent", "mandarin"},
//                            {"vad_eos", 10000},
//                            {"dwa", "wpgs"}
//                        }
//                    },
//                    {
//                        "data",new JObject{
//                            {"status", 0},
//                            {"encoding","raw"},
//                            {"format","audio/L16;rate=16000"}
//                        }
//                    }
//                };
//                if (!_IsFirst)
//                {
//                    json["data"]["status"] = 1;
//                }
//                var buffer = AudioUtils.ConvertEventToBytes(int.MaxValue, e);
//                json["data"]["audio"] = Convert.ToBase64String(buffer);


//                // blocking wait
//                await _Ws.SendAsync(Encoding.UTF8.GetBytes(json.ToString()), WebSocketMessageType.Text, true, _Ct).ConfigureAwait(false); //发送数据

//                _IsFirst = false;
//                Debug.Log("XunFei fired recognition request " + e.Length);
//            }
//            catch (Exception ex)
//            {
//                Debug.LogException(ex);
//            }
//        }

//        private IEnumerator RunStreamingRecognition()
//        {
//            while (!_IsConnected)
//            {
//                yield return new WaitForSeconds(ThreadsConstants.WaitForReady);
//            }

//            while (true)
//            {
//                var result = new byte[2048];
//                var task = _Ws.ReceiveAsync(new ArraySegment<byte>(result), new CancellationToken()); //接受数据
//                yield return new WaitUntil(() => task.IsCompleted);

//                if (task.Exception != null)
//                {
//                    Debug.Log("Exception Msg: " + task.Exception.Message);
//                    _Ws.Dispose();
//                    break;
//                }

//                var resultStr = Encoding.UTF8.GetString(result, 0, result.Length);
//                //!!! type dynamic not found !!!
//                dynamic resultJson = JsonConvert.DeserializeObject(resultStr);

//                var ssrResult = new SpeechStreamingRecognitionResult();
//                ssrResult.WordInfos = new List<WordInfo>();
//                // ssrResult.IsFinal = (string)(dataJson["result"]["status"]) == "0" ? true : false;
//                foreach (var item in resultJson.data.result.ws)
//                {
//                    var word = (string)item["cw"][0]["w"];
//                    ssrResult.WordInfos.Add(new WordInfo
//                    {
//                        Word = word
//                    });
//                    ssrResult.Transcript += word;
//                }
//                _resultText.text = ssrResult.Transcript;
//            }
//        }
//    }
//}