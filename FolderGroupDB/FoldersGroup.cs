using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FolderGroupDB
{
    public class FoldersGroup
    {
        public int Id { get; set; }
        public string GroupName { set; get; }

        /// <summary>
        /// 查询token请不要使用这个，请使用tokens字段
        /// </summary>
        public string FolderTokens { set; get; } = string.Empty;
        [NotMapped]      
        public List<string> Tokens
        {
            get
            {
                return FolderTokens.Split('\r').ToList();
            }
        }
        public FoldersGroup ()
        { }

        public FoldersGroup (string name) : this()
        {
            this.GroupName = name;
        }

        public void AddToken (string token)
        {
            var tokens = Tokens;
            tokens.Add(token); 
            var t = string.Join("\r" , tokens);
            FolderTokens = t;
        }

        public void RemoveToken (string token)
        {
            var tokens = Tokens;
            tokens.Remove(token);
            var t = string.Join("\r" , tokens);
            FolderTokens = t;
        }

        public void ClearAllTokens ()
        {

            FolderTokens = string.Empty;
        }
    }
}