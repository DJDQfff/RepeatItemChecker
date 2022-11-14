using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using Windows.Storage;
using static Windows.Storage.AccessCache.StorageApplicationPermissions;
using Windows.UI.Xaml.Controls;
using System;
using static System.Net.WebRequestMethods;

namespace RepeatItemChecker.Models
{
    internal class FoldersConfiguration
    {
        public string ConfigurationName { set; get; }
        public List<string> FolderTokens { set; get; }=new List<string>();

        public string Guid { set; get; }
        public FoldersConfiguration ()
        { }

        public FoldersConfiguration (string guid, string name) 
        {
            ConfigurationName = name;Guid = guid;
                }

        public void AddToken ( string token)
        {
            FolderTokens.Add(token);
        }

        public void RemoveToken (string  token)

        {
            FolderTokens.Remove(token);
        }

        internal static FoldersConfiguration Read (string path)
        {
            var lines = System.IO.File.ReadAllLines(path);
            var tokens = lines.Skip(1).ToList();

            var con = new FoldersConfiguration()
            {
                ConfigurationName = lines[0],
              

                Guid = Path.GetFileName(path),

                
            };
            con.FolderTokens.AddRange(tokens);
            return con;
        }


        internal static void SaveFile(string folder, FoldersConfiguration folderConFile)
        {
            string path = Path.Combine(folder, folderConFile.Guid);

            var lines1 = folderConFile.ConfigurationName;
            var lines2 = folderConFile.FolderTokens;
            var lines22 = string.Join('\r',lines2);
            var line = string.Join("\r", lines1, lines22);

            var bytes = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(line);
            MemoryStream stream = new MemoryStream(bytes);
            Stream d = new FileStream(path,FileMode.Open);

            stream.CopyTo(d);
            stream.Dispose();
            d.Dispose();

        }
    }
}