using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;

namespace RepeatItems
{
    public class RepeatItemGroup<TKey, TElment> : IGrouping<TKey , TElment>
    {
        private IGrouping<TKey , TElment> files;
        public TKey Key => files.Key;

        public ObservableCollection<TElment> Collections;

        public RepeatItemGroup (IGrouping<TKey , TElment> _files)
        {
            files = _files;
            Collections = new ObservableCollection<TElment>(files.ToArray());
        }

        public int TryRemoveItem (TElment storageFile)
        {
            if (Collections.Contains(storageFile))
            {
                Collections.Remove(storageFile);
            }

            return Collections.Count;
        }


        public IEnumerator<TElment> GetEnumerator () => files.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator ()
        {
            throw new NotImplementedException();
        }

    }
}
