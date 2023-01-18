using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using RepeatItems;
namespace RepeatItems
{
    public class RepeatItemGroupViewModel<TKey,TElment,TRepeatGroup>where TRepeatGroup:RepeatItemGroup<TKey,TElment>,new()
    
    {
        public ObservableCollection<TRepeatGroup> RepeatPairs { set; get; } = new ObservableCollection<TRepeatGroup>();
        public int Count => RepeatPairs.Count;
        public RepeatItemGroupViewModel(IEnumerable<TElment> elements,Func<TElment,TKey> func)
        {
            var a = elements.GroupBy(func);
            foreach (var cc in a)
            {
                if (cc.Count() > 1)
                {
                    var item = new TRepeatGroup();
                    item.Initial(cc);
                    RepeatPairs.Add(item);
                }
            }

        }

        public void DeleteStorageFileInRootObservable (TElment elment)
        {
            for (int index = Count - 1 ; index >= 0 ; index--)
            {
                var item =RepeatPairs[index];

                var count = item.TryRemoveItem(elment);

                if (count == 1)
                {
                    RepeatPairs.Remove(item);
                }
            }
        }



    }
}
