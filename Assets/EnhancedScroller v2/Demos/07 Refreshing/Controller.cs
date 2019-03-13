using UnityEngine;
using System.Collections;
using ChinarUi;
using ChinarUi.ScrollView;

namespace EnhancedCScrollViewDemos.RefreshDemo
{
    /// <summary>
    /// Set up our demo script as a delegate for the CScrollView by inheriting from the IEnhancedCScrollViewDelegate interface
    /// 
    /// EnhancedCScrollView delegates will handle telling the CScrollView:
    ///  - How many units it should allocate room for (GetNumberOfUnits)
    ///  - What each unit size is (GetUnitSize)
    ///  - What the unit at a given index should be (GetUnit)
    /// </summary>
    public class Controller : MonoBehaviour, IChinarScrollDelegate
    {
        /// <summary>
        /// Internal representation of our data. Note that the CScrollView will never see
        /// this, so it separates the data from the layout using MVC principles.
        /// </summary>
        private CList<Data> _data;

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

            // load in a large set of data
            LoadLargeData();
        }

        void Update()
        {
            // update the data and refresh the active units
            if (Input.GetKeyDown(KeyCode.R))
            {
                // set up some new data for units zero through five

                _data[0].someText = "This unit was updated";
                _data[1].someText = "---";
                _data[2].someText = "---";
                _data[3].someText = "---";
                _data[4].someText = "---";
                _data[5].someText = "This unit was also updated";

                // refresh the active units

                CScrollView.RefreshActiveUnitViews();
            }
        }

        /// <summary>
        /// Populates the data with a lot of records
        /// </summary>
        private void LoadLargeData()
        {
            // set up some simple data
            _data = new CList<Data>();
            for (var i = 0; i < 1000; i++)
                _data.Add(new Data() { someText = "Unit Data Index " + i.ToString() });

            // tell the CScrollView to reload now that we have the data
            CScrollView.ReloadData();
        }

        /// <summary>
        /// Populates the data with a small set of records
        /// </summary>
        private void LoadSmallData()
        {
            // set up some simple data
            _data = new CList<Data>();

            _data.Add(new Data() { someText = "A" });
            _data.Add(new Data() { someText = "B" });
            _data.Add(new Data() { someText = "C" });

            // tell the CScrollView to reload now that we have the data
            CScrollView.ReloadData();
        }

        #region UI Handlers

        /// <summary>
        /// Button handler for the large data loader
        /// </summary>
        public void LoadLargeDataButton_OnClick()
        {
            LoadLargeData();
        }

        /// <summary>
        /// Button handler for the small data loader
        /// </summary>
        public void LoadSmallDataButton_OnClick()
        {
            LoadSmallData();
        }

        #endregion

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
    }
}
