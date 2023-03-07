using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RepeatItemsChecker.Core.Models
{
    /// <summary>
    /// 重复项组合，多个重复项放在这个类里面
    /// </summary>
    /// <typeparam name="TKey">重复分组依据</typeparam>
    /// <typeparam name="TElement">重复项</typeparam>
    public class RepeatItemGroup<TKey, TElement> : IGrouping<TKey , TElement> ,IDisposable
    {
        private IGrouping<TKey , TElement> files;
        /// <summary>
        /// 重复项分组依据，
        /// </summary>
        public TKey Key => files.Key;
        /// <summary>
        /// 重复项可观察集合
        /// </summary>
        public ObservableCollection<TElement> Collections { get; } = new ObservableCollection<TElement>();

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="_files"></param>
        public void Initial (IGrouping<TKey , TElement> _files)
        {
            files = _files;
            foreach (var file in files)
            {
                Collections.Add(file);
            }
        }
        /// <summary>
        /// 移除重复项中的某一个
        /// </summary>
        /// <param name="element">要移除的重复项</param>
        /// <returns>剩下的项的个数</returns>
        public int TryRemoveItem (TElement element)
        {
            if (Collections.Contains(element))
            {
                Collections.Remove(element);
            }

            return Collections.Count;
        }

        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TElement> GetEnumerator () => files.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator ()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 清空资源
        /// </summary>
        public void Dispose ()
        {
            Collections.Clear();
        }
    }
}