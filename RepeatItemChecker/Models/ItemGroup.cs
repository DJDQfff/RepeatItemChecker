using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;

namespace RepeatItemChecker.Models
{
    public class RepeatItemGroup : IGrouping<ulong, StorageFile>
    {
        private IGrouping<ulong, StorageFile> files;

        public RepeatItemGroup (IGrouping<ulong, StorageFile> _files)
        {
            files = _files;
            StorageFiles = _files.ToArray();
        }

        public ulong Key => files.Key;
        public StorageFile[] StorageFiles { set; get; }

        public IEnumerator<StorageFile> GetEnumerator () => files.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator ()
        {
            throw new NotImplementedException();
        }
    }

}
