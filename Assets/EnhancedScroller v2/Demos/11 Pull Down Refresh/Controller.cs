using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using ChinarUi;
using ChinarUi.ScrollView;

namespace EnhancedCScrollViewDemos.PullDownRefresh
{
    /// <summary>
    /// This example shows one way you can implement a pull down to refresh feature. 
    ///
    /// When you are near the top of the CScrollView, the instructions to pull down appear.
    /// As you drag the CScrollView down, the release to refresh instructions appear. 
    /// When you release, new data is inserted into the dataset and the CScrollView is reloaded.
    /// 
    /// Note that this example requires the controller to be on the same game object
    /// as the EnhancedCScrollView component for the OnBeginDrag and OnEndDrag functions to work.
    /// </summary>
    public class Controller : MonoBehaviour, IChinarScrollDelegate, IBeginDragHandler, IEndDragHandler
    {
        /// <summary>
        /// Internal representation of our data. Note that the CScrollView will never see
        /// this, so it separates the data from the layout using MVC principles.
        /// </summary>
        private CList<Data> _data;

        /// <summary>
        /// Whether the CScrollView is being dragged
        /// </summary>
        private bool _dragging = true;

        /// <summary>
        /// Whether we should refresh after releasing the drag
        /// </summary>
        private bool _pullToRefresh = false;

        /// <summary>
        /// This is our CScrollView we will be a delegate for
        /// </summary>
        public CScrollView CScrollView;

        /// <summary>
        /// This will be the prefab of each unit in our CScrollView. Note that you can use more
        /// than one kind of unit, but this example just has the one type.
        /// </summary>
        public CScrollUnitUi unitUiPrefab;

        /// <summary>
        /// The higher the number here, the more we have to pull down to refresh
        /// </summary>
        public float pullDownThreshold;

        /// <summary>
        /// Some text to show that the user can pull down to refresh.
        /// Only shows up when near the top of the CScrollView in this example.
        /// </summary>
        public UnityEngine.UI.Text pullDownToRefreshText;

        /// <summary>
        /// Some text to show that the user can release to refresh.
        /// </summary>
        public UnityEngine.UI.Text releaseToRefreshText;

        /// <summary>
        /// Be sure to set up your references to the CScrollView after the Awake function. The 
        /// CScrollView does some internal configuration in its own Awake function. If you need to
        /// do this in the Awake function, you can set up the script order through the Unity editor.
        /// In this case, be sure to set the EnhancedCScrollView's script before your delegate.
        /// 
        /// In this example, we are calling our initializations in the delegate's Start function,
        /// but it could have been done later, perhaps in the Update function.
        /// </summary>
        void Start()
        {
            // tell the CScrollView that this script will be its delegate
            CScrollView.Delegate = this;

            // tell our controller to monitor the CScrollView's scrolled event.
            CScrollView.CScrollViewScrolled = CScrollViewScrolled;

            // load in a large set of data
            LoadLargeData();
        }

        /// <summary>
        /// Populates the data with a lot of records
        /// </summary>
        private void LoadLargeData()
        {
            // set up some simple data
            _data = new CList<Data>();
            for (var i = 0; i < 100; i++)
                _data.Add(new Data() { someText = "Unit Data Index " + i.ToString() });

            // tell the CScrollView to reload now that we have the data
            CScrollView.ReloadData();
        }

        #region EnhancedCScrollView Handlers

        /// <summary>
        /// This tells the CScrollView the number of units that should have room allocated. This should be the length of your data array.
        /// </summary>
        /// <param name="CScrollView">The CScrollView that is requesting the data size</param>
        /// <returns>The number of units</returns>
        public int GetUnitCount(CScrollView CScrollView)
        {
            // in this example, we just pass the number of our data elements
            return _data.Count;
        }

        /// <summary>
        /// This tells the CScrollView what the size of a given unit will be. Units can be any size and do not have
        /// to be uniform. For vertical CScrollViews the unit size will be the height. For horizontal CScrollViews the
        /// unit size will be the width.
        /// </summary>
        /// <param name="CScrollView">The CScrollView requesting the unit size</param>
        /// <param name="dataIndex">The index of the data that the CScrollView is requesting</param>
        /// <returns>The size of the unit</returns>
        public float GetUnitUiSize(CScrollView CScrollView, int dataIndex)
        {
            // in this example, even numbered units are 30 pixels tall, odd numbered units are 100 pixels tall
            return (dataIndex % 2 == 0 ? 30f : 100f);
        }

        /// <summary>
        /// Gets the unit to be displayed. You can have numerous unit types, allowing variety in your list.
        /// Some examples of this would be headers, footers, and other grouping units.
        /// </summary>
        /// <param name="CScrollView">The CScrollView requesting the unit</param>
        /// <param name="dataIndex">The index of the data that the CScrollView is requesting</param>
        /// <param name="unitIndex">The index of the list. This will likely be different from the dataIndex if the CScrollView is looping</param>
        /// <returns>The unit for the CScrollView to use</returns>
        public CScrollUnitUi GetUnitUi(CScrollView CScrollView, int dataIndex, int unitIndex)
        {
            // first, we get a unit from the CScrollView by passing a prefab.
            // if the CScrollView finds one it can recycle it will do so, otherwise
            // it will create a new unit.
            UnitView unitUi = CScrollView.GetUnitView(unitUiPrefab) as UnitView;

            // set the name of the game object to the unit's data index.
            // this is optional, but it helps up debug the objects in 
            // the scene hierarchy.
            unitUi.name = "Unit " + dataIndex.ToString();

            // in this example, we just pass the data to our unit's view which will update its UI
            unitUi.SetData(_data[dataIndex]);

            // return the unit to the CScrollView
            return unitUi;
        }

        #endregion

        /// <summary>
        /// This delegate will fire when the CScrollView is scrolled
        /// </summary>
        /// <param name="CScrollView"></param>
        /// <param name="val"></param>
        /// <param name="scrollPosition"></param>
        private void CScrollViewScrolled(CScrollView CScrollView, Vector2 val, float scrollPosition)
        {
            var scrollMoved = ((scrollPosition <= -pullDownThreshold) || scrollPosition == 0);

            if (_dragging && scrollMoved)
            {
                // we are dragging and the scroll position is beyond the scroll threshold.
                // we should flag that a refresh is needed when the dragging is released.
                _pullToRefresh = true;

                // show the release text if the CScrollView is down beyond the threshold
                releaseToRefreshText.gameObject.SetActive(true);
            }

            // show the pull to refresh text if the CScrollView position is at the top
            pullDownToRefreshText.gameObject.SetActive(scrollPosition <= 0);
        }

        /// <summary>
        /// Fired by the ScrollRect
        /// </summary>
        /// <param name="data"></param>
        public void OnBeginDrag(PointerEventData data)
        {
            // we are now dragging.
            // we flag this so that refreshing won't occur if the CScrollView
            // is scrolling due to inertia. 
            // the user must drag manually in this example.
            _dragging = true;
        }

        /// <summary>
        /// Fired by the ScrollRect
        /// </summary>
        /// <param name="data"></param>
        public void OnEndDrag(PointerEventData data)
        {
            // no longer dragging
            _dragging = false;

            if (_pullToRefresh)
            {
                // we reached the scroll pull down threshold, so now we insert new data

                for (var i = 0; i < 3; i++)
                {
                    _data.Insert(new Data() { someText = "Brand New Data " + i.ToString() + "!!!" }, 0);
                }

                // reload the CScrollView to show the new data
                CScrollView.ReloadData();

                // take off the refresh now that it is handled
                _pullToRefresh = false;

                // hide the release text if the CScrollView is down beyond the threshold
                releaseToRefreshText.gameObject.SetActive(false);
            }
        }
    }
}
