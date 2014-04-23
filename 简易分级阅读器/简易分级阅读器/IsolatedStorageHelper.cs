using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;

namespace 简易分级阅读器
{
    class IsolatedStorageHelper
    {
        /// <summary>
        /// 复制文件到独立存储空间
        /// </summary>
        /// <param name="sourceFileUrl">源文件</param>
        /// <param name="saveFileUrl">保存文件</param>
        public static void CopyContentToIsolatedStorage(string sourceFileUrl, string saveFileUrl = null)
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                saveFileUrl = string.IsNullOrWhiteSpace(saveFileUrl) ? sourceFileUrl : saveFileUrl;
                //if (iso.FileExists(saveFileUrl)) { iso.DeleteFile(saveFileUrl); }
                if (!iso.FileExists(saveFileUrl))
                {
                    var fullDirectory = System.IO.Path.GetDirectoryName(saveFileUrl);
                    if (!iso.DirectoryExists(fullDirectory))
                    {
                        iso.CreateDirectory(fullDirectory);
                    }

                    using (Stream input = Application.GetResourceStream(new Uri(sourceFileUrl, UriKind.Relative)).Stream)
                    {
                        using (IsolatedStorageFileStream output = iso.CreateFile(saveFileUrl))
                        {
                            byte[] readBuffer = new byte[4096];
                            int bytesRead = -1;
                            while ((bytesRead = input.Read(readBuffer, 0, readBuffer.Length)) > 0)
                            {
                                output.Write(readBuffer, 0, bytesRead);
                            }
                        }
                    }
                }
            }
        }



        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        public static void SaveFile(string fileName, string content)
        {
            using (var store = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists(fileName)) { store.DeleteFile(fileName); }
                using (var stream = store.CreateFile(fileName))
                {
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string OpenFile(string fileName)
        {
            string data = string.Empty;
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(fileName))
                {
                    using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(fileName, FileMode.Open, FileAccess.ReadWrite))
                    {
                        StreamReader sr = new StreamReader(fileStream, System.Text.Encoding.UTF8);
                        data = sr.ReadToEnd();
                        sr.Close();
                    }
                }
            }
            return data;
        }
    }
}
