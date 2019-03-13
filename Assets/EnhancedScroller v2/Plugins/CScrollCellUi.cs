using UnityEngine;
using System;



namespace ChinarUi.ScrollView
{
    /// <summary>
    /// 这是UI单元元素的基类，所有用于显示的UI单元都应该继承自此类
    /// </summary>
    public class CScrollUnitUi : MonoBehaviour
    {
        /// <summary>
        /// UnitMark 是一个独特的字符串，它允许滚动块在一个列表中处理不同类型的单元。每种类型的单元格都应该有自己的标记
        /// </summary>
        public string UnitMark;

        /// <summary>
        /// Ui单元的下标，如果列表是循环的，UnitIndex将与dataIndex不同
        /// </summary>
        [NonSerialized] public int UnitIndex;

        /// <summary>
        /// 单元Ui的数据下标
        /// </summary>
        [NonSerialized] public int DataIndex;

        /// <summary>
        /// 单元Ui显示是正常滚动，还是可循环的
        /// </summary>
        [NonSerialized] public bool Active;


        /// <summary>
        /// 在 CScrollView 调用  RefreshActiveUnitViews 时，此函数被 RefreshUnitUi 调用
        /// 你可以重写它去更新你的单元Ui
        /// </summary>
        public virtual void RefreshUnitUi()
        {
        }
    }
}