using Playa.Audio.MicrophoneEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Playa.Common;
using Newtonsoft.Json;
using Playa.Audio.ASR;
using Newtonsoft.Json.Linq;
using TMPro;
using System.Net.Http;
using System.Net;
using System.IO;

namespace Playa.Audio
{
    public class XFKeywordDetector : MonoBehaviour
    {
        private string _AppId = "fa2b3a96";
        private string _AppKey = "99719c268f9164fe98db87621ae3d46d";

        private HttpClient _Client;

        // Start is called before the first frame update
        private void Start()
        {
            // Asynchronously establish connection
            Connect();
        }

        private async Task Connect()
        {
            try
            {
                String Url = "http://ltpapi.xfyun.cn/v1/ke";

                var param = new JObject
                {
                    {"type", "dependent"}
                };
                var timestamp = GetTimeStamp();
                var paramStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(param)));
                var baseString = _AppKey + timestamp + paramStr;
                var body = new JObject
                {
                    {"text", "今天我要去大润发买鱼"}
                };
                var data = JsonConvert.SerializeObject(body);


                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers["X-Appid"] = _AppId;
                request.Headers["X-CurTime"] = timestamp;
                request.Headers["X-Param"] = paramStr;
                request.Headers["X-Checksum"] = ToMD5(baseString);

                request.ContentLength = Encoding.UTF8.GetByteCount(data);

                Debug.Log("wtf1");


                Stream requestStream = request.GetRequestStream();
                StreamWriter streamWriter = new StreamWriter(requestStream, Encoding.GetEncoding("gb2312"));
                streamWriter.Write(data);
                streamWriter.Close();

                string htmlStr = string.Empty;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream responseStream = response.GetResponseStream();
                Debug.Log("wtf2");
                using (StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8")))
                {
                    htmlStr = reader.ReadToEnd();
                    Debug.Log("XunFei keyword request result " + htmlStr);
                }
                responseStream.Close();

            }
            catch (Exception ex)
            {
                Debug.Log("Exception Msg: " + ex.Message);
            }
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns>返回的时间戳，精确到秒</returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// MD5字符串加密
        /// </summary>
        /// <param name="txt">需要加密的字符串</param>
        /// <returns>加密后字符串</returns>
        public static String ToMD5(string s)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();
            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }
            return ret.PadLeft(32, '0');
        }
    }
}