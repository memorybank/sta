using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

namespace Playa.Common.Utils
{
    public class FileUtils
    {
        public static List<string> GetAllSubDirsWithSuffix(string dirPath, string suffix)
        {
            List<string> dirs = new List<string>();

            foreach (string path in Directory.GetFiles(dirPath))
            {
                //获取所有文件夹中包含后缀为 suffix 的路径
                if (Path.GetExtension(path) == suffix)
                {
                    dirs.Add(path.Substring(path.IndexOf("Assets")));
                }
            }

            if (Directory.GetDirectories(dirPath).Length > 0)  //遍历所有文件夹
            {
                foreach (string path in Directory.GetDirectories(dirPath))
                {
                    dirs.AddRange(GetAllSubDirsWithSuffix(path, suffix));
                }
            }

            return dirs;
        }
    }

}