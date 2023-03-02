using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using RepeatItemsChecker.Core.Models;

namespace RepeatItemsChecker.Core.ViewModels
{
    /// <summary>
    /// 重复项组合ViewModel
    /// </summary>
    /// <typeparam name="TKey">重复项分组依据</typeparam>
    /// <typeparam name="TElement">重复项类型</typeparam>
    /// <typeparam name="TRepeatGroup">重复项组合</typeparam>
    public class RepeatItemGroupViewModel<TKey, TElement, TRepeatGroup> where TRepeatGroup : RepeatItemGroup<TKey , TElement>, new()
    {
        /// <summary>
        /// 重复项组合的集合
        /// </summary>
        public ObservableCollection<TRepeatGroup> RepeatPairs { set; get; } = new ObservableCollection<TRepeatGroup>();
        /// <summary>
        /// 组合个数
        /// </summary>
        public int Count => RepeatPairs.Count;

        /// <summary>
        /// 获取所有元素项
        /// </summary>
        public IEnumerable<TElement> AllElements
        {
            get
            {
                var list=new List<TElement>();
                foreach(var  item in RepeatPairs)
                {
                    var em=item.Collections.ToArray();
                    list.AddRange(em);
                }
                return list;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements">数据源</param>
        /// <param name="func">分类依据</param>
        public RepeatItemGroupViewModel (IEnumerable<TElement> elements , Func<TElement , TKey> func)
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
        /// <summary>
        /// 删除一个项，在集合中检测删除
        /// </summary>
        /// <param name="elment"></param>
        public void DeleteStorageFileInRootObservable (TElement elment)
        {
            for (int index = Count - 1 ; index >= 0 ; index--)
            {
                var item = RepeatPairs[index];

                var count = item.TryRemoveItem(elment);

                if (count == 1)
                {
                    RepeatPairs.Remove(item);
                }
            }
        }
    }
}