namespace ChinarUi.ScrollView
{
    /// <summary>
    /// 所有处理滚动块回调的脚本都应该继承自这个接口
    /// </summary>
    public interface IChinarScrollDelegate
    {
        /// <summary>
        /// 获取数据列表中的单元数量
        /// </summary>
        /// <param name="cScrollView">滚动视图组件</param>
        /// <returns>单元格数量</returns>
        int GetUnitCount(CScrollView cScrollView);


        /// <summary>
        /// 获取对应下标的单元格视图的大小, 允许有不同大小的单元格
        /// </summary>
        /// <param name="cScrollView">滚动视图组件</param>
        /// <param name="dataIndex">单元下标</param>
        /// <returns>单元格大小</returns>
        float GetUnitUiSize(CScrollView cScrollView, int dataIndex);


        /// <summary>
        /// 获取应用于数据索引的单元格视图。这个函数的实现应该从滚动块请求一个新的单元格，以便它可以正确地回收旧单元格。
        /// </summary>
        /// <param name="cScrollView">滚动视图组件</param>
        /// <param name="dataIndex">数据下标</param>
        /// <param name="unitIndex">单元下标</param>
        /// <returns>CScrollUnitUi 对象</returns>
        CScrollUnitUi GetUnitUi(CScrollView cScrollView, int dataIndex, int unitIndex);
    }
}