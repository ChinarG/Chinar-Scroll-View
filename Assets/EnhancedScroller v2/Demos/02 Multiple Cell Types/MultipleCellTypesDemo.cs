using ChinarUi;
using ChinarUi.ScrollView;
using UnityEngine;



namespace EnhancedCScrollViewDemos.MultipleUnitTypesDemo
{
    /// <summary>
    /// 多单元类型例子 —— 02
    /// </summary>
    public class MultipleUnitTypesDemo : MonoBehaviour, IChinarScrollDelegate
    {
        /// <summary>
        /// 内部 表现
        /// Internal representation of our data.
        /// 注意
        /// Note that the CScrollView will never see this,
        ///          分离                                         原则
        ///  so it separates the data from the layout using MVC principles.
        /// </summary>
        private CList<Data> _data;

        /// <summary>
        /// 这是我们的 滚动条，我们将它交给委托
        /// </summary>
        public CScrollView CScrollView;

        public CScrollUnitUi headerUnitViewPrefab; //页眉
        public CScrollUnitUi rowUnitViewPrefab;    //行
        public CScrollUnitUi footerUnitViewPrefab; //页脚

        /// <summary>
        ///                                                          处于,位于
        /// The base path to the resources folder where sprites are located
        /// </summary>
        public string resourcePath;


        void Start()
        {
            // 注册委托，并加载数据类
            CScrollView.Delegate = this;
            LoadData();
        }


        /// <summary>
        /// 填充，居住于                          记录
        /// Populates the data with a lot of records
        /// </summary>
        private void LoadData()
        {
            // note we are using different data class fields for the header, row, and footer rows.
            //       到期，应得          due to 由于   多态性       
            // This works due to polymorphism.
            _data = new CList<Data>();
            _data.Add(new HeaderData {category = "玩家"});                                                                                  // 种类
            _data.Add(new RowData {userName    = "甲", userAvatarSpritePath = resourcePath + "/avatar_male", userHighScore   = 21323199}); //阿凡达，男性
            _data.Add(new RowData {userName    = "乙", userAvatarSpritePath = resourcePath + "/avatar_female", userHighScore = 20793219}); //女性
            _data.Add(new RowData {userName    = "丙", userAvatarSpritePath = resourcePath + "/avatar_female", userHighScore = 19932132});
            _data.Add(new FooterData());
            _data.Add(new HeaderData {category = "丁"});
            _data.Add(new RowData {userName    = "戊", userAvatarSpritePath = resourcePath + "/avatar_male", userHighScore   = 1002132});
            _data.Add(new RowData {userName    = "己", userAvatarSpritePath = resourcePath + "/avatar_female", userHighScore = 991234});
            _data.Add(new FooterData());
            _data.Add(new HeaderData {category = "庚"});
            _data.Add(new RowData {userName    = "辛", userAvatarSpritePath    = resourcePath + "/avatar_male", userHighScore   = 905723});
            _data.Add(new RowData {userName    = "壬", userAvatarSpritePath    = resourcePath + "/avatar_male", userHighScore   = 702318});
            _data.Add(new RowData {userName    = "癸", userAvatarSpritePath    = resourcePath + "/avatar_female", userHighScore = 697767});
            _data.Add(new RowData {userName    = "青铜", userAvatarSpritePath   = resourcePath + "/avatar_male", userHighScore   = 409393});
            _data.Add(new RowData {userName    = "白银", userAvatarSpritePath   = resourcePath + "/avatar_female", userHighScore = 104352});
            _data.Add(new RowData {userName    = "黄金", userAvatarSpritePath   = resourcePath + "/avatar_male", userHighScore   = 88321});
            _data.Add(new RowData {userName    = "钻石", userAvatarSpritePath   = resourcePath + "/avatar_female", userHighScore = 20826});
            _data.Add(new RowData {userName    = "大师", userAvatarSpritePath   = resourcePath + "/avatar_female", userHighScore = 17389});
            _data.Add(new RowData {userName    = "最强王者", userAvatarSpritePath = resourcePath + "/avatar_male", userHighScore   = 2918});
            _data.Add(new FooterData());

            // tell the CScrollView to reload now that we have the data
            CScrollView.ReloadData();
        }


        #region EnhancedCScrollView Handlers

        /// <summary>
        ///                                                               空间 分配
        /// This tells the CScrollView the number of units that should have room allocated. This should be the length of your data array.
        /// </summary>                                     请求
        /// <param name="CScrollView">The CScrollView that is requesting the data size</param>
        /// <returns>The number of units</returns>
        public int GetUnitCount(CScrollView CScrollView)
        {
            //                          传递
            // in this example, we just pass the number of our data elements
            return _data.Count;
        }


        /// <summary>
        /// This tells the CScrollView what the size of a given unit will be. Units can be any size and do not have
        ///       统一的
        /// to be uniform. For vertical CScrollViews the unit size will be the height. For horizontal CScrollViews the
        /// unit size will be the width.
        /// </summary>
        /// <param name="CScrollView">The CScrollView requesting the unit size</param>
        /// <param name="dataIndex">The index of the data that the CScrollView is requesting</param>
        /// <returns>The size of the unit</returns>
        public float GetUnitUiSize(CScrollView CScrollView, int dataIndex)
        {
            switch (_data[dataIndex])
            {
                //         确定                              根据它是第几行
                // we will determine the unit height based on what kind of row it is
                case HeaderData _:
                    // header views
                    return 70f;
                case RowData _:
                    // row views
                    return 100f;
                default:
                    // footer views
                    return 90f;
            }
        }


        /// <summary>
        /// 获取要显示的单元格                               许多                          多种
        /// Gets the unit to be displayed. You can have numerous unit types, allowing variety in your list.
        /// Some examples of this would be headers, footers, and other grouping units.
        /// </summary>
        /// <param name="CScrollView">The CScrollView requesting the unit</param>
        /// <param name="dataIndex">The index of the data that the CScrollView is requesting</param>
        /// <param name="unitIndex">The index of the list. This will likely be different from the dataIndex if the CScrollView is looping</param>
        /// <returns>The unit for the CScrollView to use</returns>
        public CScrollUnitUi GetUnitUi(CScrollView CScrollView, int dataIndex, int unitIndex)
        {
            UnitView unitUi;
            switch (_data[dataIndex])
            {
                // 根据数据行类型决定要获取什么单元格视图
                case HeaderData _:
                    // 从滚动块中获得标题单元预制，如果可能的话回收旧单元
                    unitUi = CScrollView.GetUnitView(headerUnitViewPrefab) as UnitViewHeader;

                    // 为清晰起见                                                  表示
                    // optional for clarity: set the unit's name to something to indicate this is a header row
                    unitUi.name = "[头] " + ((HeaderData) _data[dataIndex]).category;
                    break;
                case RowData _:
                    // get a row unit prefab from the CScrollView, recycling old units if possible
                    unitUi = CScrollView.GetUnitView(rowUnitViewPrefab) as UnitViewRow;

                    // optional for clarity: set the unit's name to something to indicate this is a row
                    unitUi.name = "[行] " + (_data[dataIndex] as RowData).userName;
                    break;
                default:
                    // get a footer unit prefab from the CScrollView, recycling old units if possible
                    unitUi = CScrollView.GetUnitView(footerUnitViewPrefab) as UnitViewFooter;

                    // optional for clarity: set the unit's name to something to indicate this is a footer row
                    unitUi.name = "[脚]";
                    break;
            }

            //                                                     声明了
            // set the unit view's data. We can do this because we declared a single SetData function
            // in the UnitView base class, saving us from having to call this for each unit type
            unitUi.SetData(_data[dataIndex]);

            // return the unitUi to the CScrollView
            return unitUi;
        }

        #endregion
    }
}