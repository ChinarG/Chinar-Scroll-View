using UnityEngine;
using System.Collections;
using ChinarUi;
using ChinarUi.ScrollView;

namespace EnhancedCScrollViewDemos.GridSimulation
{
    /// <summary>
    /// This example shows how to simulate a grid with a fixed number of units per row
    /// The data is stored as normal, but the differences in this example are:
    /// 
    /// 1) The CScrollView is told the data count is the number of data elements divided by the number of units per row
    /// 2) The unit view is passed a reference to the data set with the offset index of the first unit in the row
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
        /// This will be the prefab of each unit in our CScrollView. The unit view will
        /// hold references to each row sub unit
        /// </summary>
        public CScrollUnitUi unitUiPrefab;

        public int numberOfUnitsPerRow = 3;

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
            LoadData();
        }

        /// <summary>
        /// Populates the data with a lot of records
        /// </summary>
        private void LoadData()
        {
            // set up some simple data
            _data = new CList<Data>();
            for (var i = 0; i < 1000; i ++)
            {
                _data.Add(new Data() { someText = i.ToString() });
            }

            // tell the CScrollView to reload now that we have the data
            CScrollView.ReloadData();
        }

        #region EnhancedCScrollView Handlers

        /// <summary>
        /// This tells the CScrollView the number of units that should have room allocated.
        /// For this example, the count is the number of data elements divided by the number of units per row (rounded up using Mathf.CeilToInt)
        /// </summary>
        /// <param name="CScrollView">The CScrollView that is requesting the data size</param>
        /// <returns>The number of units</returns>
        public int GetUnitCount(CScrollView CScrollView)
        {
            return Mathf.CeilToInt((float)_data.Count / (float)numberOfUnitsPerRow);
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
            return 100f;
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

            unitUi.name = "Unit " + (dataIndex * numberOfUnitsPerRow).ToString() + " to " + ((dataIndex * numberOfUnitsPerRow) + numberOfUnitsPerRow - 1).ToString();

            // pass in a reference to our data set with the offset for this unit
            unitUi.SetData(ref _data, dataIndex * numberOfUnitsPerRow);

            // return the unit to the CScrollView
            return unitUi;
        }

        #endregion
    }
}
