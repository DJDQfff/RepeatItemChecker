﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GroupedItemsLibrary.Models;

namespace GroupedItemsLibrary.ViewModels
{
    /// <summary>
    /// 重复项组合ViewModel
    /// </summary>
    /// <typeparam name="TKey">重复项分组依据</typeparam>
    /// <typeparam name="TElement">重复项 Model</typeparam>
    /// <typeparam name="TRepeatGroup">重复项组合</typeparam>
    public class ItemsGroupsViewModel<TKey, TElement, TRepeatGroup> where TRepeatGroup : ItemsGroup<TKey , TElement>, new()
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
                var list = new List<TElement>();
                foreach (var item in RepeatPairs)
                {
                    var em = item.Collections.ToArray();
                    list.AddRange(em);
                }
                return list;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="elements">数据源</param>
        /// <param name="func">分类条件</param>
        /// <param name="filt">忽略条件</param>
        /// 
        public ItemsGroupsViewModel (IEnumerable<TElement> elements , Func<TElement , TKey> func , Func<TRepeatGroup , bool> filt)
        {
            var a = elements.GroupBy(func);
            foreach (var cc in a)
            {
                if (cc.Count() > 1)
                {
                    var item = new TRepeatGroup();
                    item.Initial(cc);
                    var can = filt.Invoke(item);
                    if (can)
                    {
                        RepeatPairs.Add(item);
                    }
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
                var group = RepeatPairs[index];

                var count = group.TryRemoveItem(elment);

                if (count == 1)
                {
                    RepeatPairs.RemoveAt(index);
                }
            }
        }
    }
}