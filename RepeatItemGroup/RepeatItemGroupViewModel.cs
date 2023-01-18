using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using RepeatItems;
namespace RepeatItems
{
    public class RepeatItemGroupViewModel<Tkey,TElement>
    
    {
        public ObservableCollection<RepeatItemGroup<Tkey , TElement>> RepeatPairs { set; get; } = new ObservableCollection<RepeatItemGroup<Tkey , TElement>>();
        public int Count => RepeatPairs.Count;
        public RepeatItemGroupViewModel(IEnumerable<TElement> elements,Func<TElement,Tkey> func)
        {
            var a = elements.GroupBy(func);
            foreach (var cc in a)
            {
                if (cc.Count() > 1)
                {
                    var item = new RepeatItemGroup<Tkey,TElement>(cc);
                    RepeatPairs.Add(item);
                }
            }

        }

        public void DeleteStorageFileInRootObservable (TElement elment)
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
