using ChinarUi;
using ChinarUi.ScrollView;
using UnityEngine;



namespace EnhancedCScrollViewDemos.SuperSimpleDemo
{
    /// <summary>
    /// 通过继承IEnhancedCScrollViewDelegate接口，将演示脚本设置为CScrollView的委托
    /// 
    /// 经过增强的CScrollView委托将处理告诉CScrollView:
    /// 它应该为多少单元分配空间(GetNumberOfUnits)
    /// -每个单元格大小是多少(GetUnitSize)
    /// -给定索引的单元格应该是什么(GetUnit)
    /// </summary>
    public class SimpleDemo : MonoBehaviour, IChinarScrollDelegate
    {
        /// <summary>
        /// 数据的内部表示。注意，滚动器永远不会看到
        /// 因此，它使用MVC原则将数据从布局中分离出来。
        /// </summary>
        private CList<Data> _data;

        /// <summary>
        /// 这是我们的CScrollView
        /// </summary>
        public CScrollView CScrollView;

        /// <summary>
        /// 这将是我们的滚动块中每个单元格的预置。注意，您可以使用多种单元格，但是本例只有一种类型。
        /// </summary>
        public CScrollUnitUi unitUiPrefab;


        /// <summary>
        ///务必在唤醒功能之后设置对滚动条的引用。
        ///    CScrollView在它自己的唤醒函数中执行一些内部配置。如果你需要
        ///    在Awake函数中执行此操作，您可以通过Unity编辑器设置脚本顺序。
        ///在这种情况下，请确保在委托之前设置EnhancedCScrollView的脚本。
        ///在这个例子中，我们调用委托的Start函数中的初始化，
        ///但是它可以稍后再做，也许在Update函数中。
        /// </summary>
        void Start()
        {
            // 告诉滚动块这个脚本将是它的委托
            CScrollView.Delegate = this;

            //加载大量数据
            LoadLargeData();
        }


        /// <summary>
        /// 用大量记录填充数据
        /// </summary>
        private void LoadLargeData()
        {
            // 设置一些简单的数据
            _data = new CList<Data>();
            for (var i = 0; i < 1000; i++)
                _data.Add(new Data() {someText = "单元数据下标: " + i});

            // 告诉滚动块重新加载，现在我们有了数据
            CScrollView.ReloadData();
        }


        /// <summary>
        /// 用一个小组记录填充数据
        /// </summary>
        private void LoadSmallData()
        {
            // 设置一些简单的数据
            _data = new CList<Data>();
            _data.Add(new Data() {someText = "A"});
            _data.Add(new Data() {someText = "B"});
            _data.Add(new Data() {someText = "C"});

            // 告诉滚动块重新加载，现在我们已有的数据
            CScrollView.ReloadData();
        }


        #region UI Handlers

        /// <summary>
        /// 点击加载 大量数据
        /// </summary>
        public void LoadLargeDataButton_OnClick()
        {
            LoadLargeData();
        }


        /// <summary>
        /// 点击加载 少量数据
        /// </summary>
        public void LoadSmallDataButton_OnClick()
        {
            LoadSmallData();
        }

        #endregion

        #region EnhancedCScrollView Handlers

        /// <summary>
        /// 这告诉滚动块应该分配空间的单元格数量。这应该是数据数组的长度。
        /// </summary>
        /// <param name="CScrollView">请求数据尺寸的滚动块</param>
        /// <returns>单元数量</returns>
        public int GetUnitCount(CScrollView CScrollView)
        {
            //在本例中，我们只传递数据元素的数量
            return _data.Count;
        }


        /// <summary>
        /// 这告诉滚动块给定单元格的大小。单元格可以是任何大小，不必是均匀的。
        /// 对于垂直滚动块，单元格大小将为高度。
        /// 对于水平滚动块，单元格大小为宽度。
        /// </summary>
        /// <param name="CScrollView">请求数据尺寸的滚动块</param>
        /// <param name="dataIndex">滚动块请求的数据的索引</param>
        /// <returns>单元格大小</returns>
        public float GetUnitUiSize(CScrollView CScrollView, int dataIndex)
        {
            // 在本例中，偶数单元格为30像素高，奇数单元格为100像素高
            return (dataIndex % 2 == 0 ? 30f : 100f);
        }


        /// <summary>
        /// 获取要显示的单元格。您可以有多种单元格类型，允许列表中有多种。例如页眉、页脚和其他分组单元格
        /// </summary>
        /// <param name="CScrollView">请求单元格的滚动程序</param>
        /// <param name="dataIndex">滚动块请求的数据的索引</param>
        /// <param name="unitIndex">列表的索引。如果滚动块是循环的，那么这可能与dataIndex不同</param>
        /// <returns>>滚动块使用的单元格</returns>
        public CScrollUnitUi GetUnitUi(CScrollView CScrollView, int dataIndex, int unitIndex)
        {
            // 首先，我们通过一个预制块从滚动块获得一个单元格。
            // 如果滚动块找到一个可以回收的，它就直接使用，否则
            // 它将创建一个新的单元格。
            UnitView unitUi = CScrollView.GetUnitView(unitUiPrefab) as UnitView;

            // 将游戏对象的名称设置为单元格的数据索引。
            // 这是可选的，但它有助于调试场景层次结构中的对象。
            unitUi.name = "单元格 - " + dataIndex;

            //在本例中，我们只是将数据传递给单元格的视图，该视图将更新其UI
            unitUi.SetData(_data[dataIndex]);

            //将单元格返回到滚动块
            return unitUi;
        }

        #endregion
    }
}