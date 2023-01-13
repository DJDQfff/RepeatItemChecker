using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Windows.Storage;

namespace RepeatItemChecker.Models
{
    public class RepeaStorageFileGroup : RepeatItems.RepeatItemGroup<ulong , StorageFile>
    {
        public RepeaStorageFileGroup (IGrouping<ulong , StorageFile> _files) : base(_files)
        {
        }
    }
}