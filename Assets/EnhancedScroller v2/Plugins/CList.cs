using UnityEngine;



namespace ChinarUi
{
    /// <summary>
    /// 这是一个超级轻量级的数组实现，它的行为类似于一个列表，在需要时自动分配新内存，但不会将其释放到垃圾收集中。
    /// </summary>
    /// <typeparam name="T">The type of the list</typeparam>
    public class CList<T>
    {
        /// <summary>
        /// 列表数据的内部存储
        /// </summary>
        public T[] data;

        /// <summary>
        /// 列表中元素的数量
        /// </summary>
        public int Count = 0;

        /// <summary>
        /// 对列表项的索引访问
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public T this[int i]
        {
            get { return data[i]; }
            set { data[i] = value; }
        }


        /// <summary>
        /// 当需要更多内存时调整数组的大小。
        /// </summary>
        private void ResizeArray()
        {
            T[] newData;
            if (data != null)
                newData = new T[Mathf.Max(data.Length << 1, 64)];
            else
                newData = new T[64];
            if (data != null && Count > 0)
                data.CopyTo(newData, 0);
            data = newData;
        }


        /// <summary>
        /// 列表大小被设置为零，而不是释放内存到垃圾收集
        /// </summary>
        public void Clear()
        {
            Count = 0;
        }


        /// <summary>
        /// 返回列表的第一个元素
        /// </summary>
        /// <returns></returns>
        public T First()
        {
            if (data == null || Count == 0) return default(T);
            return data[0];
        }


        /// <summary>
        /// 返回列表的最后一个元素
        /// </summary>
        /// <returns></returns>
        public T Last()
        {
            if (data == null || Count == 0) return default(T);
            return data[Count - 1];
        }


        /// <summary>
        ///向数组中添加一个新元素，在必要时创建更多内存
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if (data == null || Count == data.Length)
                ResizeArray();
            data[Count] = item;
            Count++;
        }


        /// <summary>
        /// 在数组的开头添加一个新元素，在必要时创建更多内存
        /// </summary>
        /// <param name="item"></param>
        public void AddStart(T item)
        {
            Insert(item, 0);
        }


        /// <summary>
        /// 在指定的索引处向数组插入一个新元素，创建更多如果需要的话，内存
        /// </summary>
        /// <param name="item"></param>
        public void Insert(T item, int index)
        {
            if (data == null || Count == data.Length)
                ResizeArray();
            for (var i = Count; i > index; i--)
            {
                data[i] = data[i - 1];
            }

            data[index] = item;
            Count++;
        }


        /// <summary>
        /// 从数据的开始删除项
        /// </summary>
        /// <returns></returns>
        public T RemoveStart()
        {
            return RemoveAt(0);
        }


        /// <summary>
        /// 从数据索引中删除项
        /// </summary>
        /// <returns></returns>
        public T RemoveAt(int index)
        {
            if (data != null && Count != 0)
            {
                T val = data[index];
                for (var i = index; i < Count - 1; i++)
                {
                    data[i] = data[i + 1];
                }

                Count--;
                data[Count] = default(T);
                return val;
            }
            else
            {
                return default(T);
            }
        }


        /// <summary>
        /// 从数据中删除项
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public T Remove(T item)
        {
            if (data != null && Count != 0)
            {
                for (var i = 0; i < Count; i++)
                {
                    if (data[i].Equals(item))
                    {
                        return RemoveAt(i);
                    }
                }
            }

            return default(T);
        }


        /// <summary>
        /// 从数据末尾删除项
        /// </summary>
        public T RemoveEnd()
        {
            if (data != null && Count != 0)
            {
                Count--;
                T val = data[Count];
                data[Count] = default(T);
                return val;
            }
            else
            {
                return default(T);
            }
        }


        /// <summary>
        /// 确定数据是否包含该项
        /// </summary>
        /// <param name="item">项目对比</param>
        /// <returns>如果项存在于teh数据中，则为真</returns>
        public bool Contains(T item)
        {
            if (data == null)
                return false;
            for (var i = 0; i < Count; i++)
            {
                if (data[i].Equals(item))
                    return true;
            }

            return false;
        }
    }
}