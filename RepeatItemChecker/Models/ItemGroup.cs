using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;

namespace RepeatItemChecker.Models
{
    public class RepeatItemGroup : IGrouping<ulong, StorageFile>
    {
        private IGrouping<ulong, StorageFile> files;

        public ObservableCollection<StorageFile> StorageFiles;

        public RepeatItemGroup (IGrouping<ulong, StorageFile> _files)
        {
            files = _files;
            StorageFiles = new ObservableCollection<StorageFile>(files.ToArray());
        }

        public int TryRemoveItem(StorageFile storageFile)
        {
            if (StorageFiles.Contains(storageFile))
            {
                StorageFiles.Remove(storageFile);
            }

            return StorageFiles.Count;
        }
        public ulong Key => files.Key;

        public IEnumerator<StorageFile> GetEnumerator () => files.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator ()
        {
            throw new NotImplementedException();

        }
    }

}
