using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ChinarUi;
using ChinarUi.ScrollView;

namespace EnhancedCScrollViewDemos.NestedCScrollViews
{
    /// <summary>
    /// This example scene shows one way you could set up nested CScrollViews. 
    /// Each MasterUnitView in the master CScrollView contains an EnhancedCScrollView which in turn contains DetailUnitViews.
    ///
    /// Events are passed from the detail units up to the master scroll rect so that scrolling can be done naturally 
    /// in both the horizontal and vertical directions.The detail ScrollRectEx is an extension of Unity's ScrollRect 
    /// that allows this event pass through.
    /// </summary>
    public class Controller : MonoBehaviour, IChinarScrollDelegate
    {
        /// <summary>
        /// Internal representation of our data. Note that the CScrollView will never see
        /// this, so it separates the data from the layout using MVC principles.
        /// </summary>
        private List<MasterData> _data;

        /// <summary>
        /// This is our CScrollView we will be a delegate for. The master CScrollView contains masterunitviews which in turn
        /// contain EnhancedCScrollViews
        /// </summary>
        public CScrollView masterCScrollView;

        /// <summary>
        /// This will be the prefab of each unit in our CScrollView. Note that you can use more
        /// than one kind of unit, but this example just has the one type.
        /// </summary>
        public CScrollUnitUi masterUnitViewPrefab;

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
            masterCScrollView.Delegate = this;

            // load in a large set of data
            LoadData();
        }

        /// <summary>
        /// Populates the data with a lot of records
        /// </summary>
        private void LoadData()
        {
            // set up some simple data. This will be a two-dimensional array,
            // specifically a list within a list.

            _data = new List<MasterData>();
            for (var i = 0; i < 1000; i++)
            {
                var masterData = new MasterData()
                {
                    normalizedScrollPosition = 0,
                    childData = new List<DetailData>()
                };

                _data.Add(masterData);

                for (var j = 0; j < 20; j++)
                    masterData.childData.Add(new DetailData() { someText = i.ToString() + "," + j.ToString() });
            }

            // tell the CScrollView to reload now that we have the data
            masterCScrollView.ReloadData();
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
            // in this example, our master units are 100 pixels tall
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
            MasterUnitView masterUnitView = CScrollView.GetUnitView(masterUnitViewPrefab) as MasterUnitView;

            // set the name of the game object to the unit's data index.
            // this is optional, but it helps up debug the objects in 
            // the scene hierarchy.
            masterUnitView.name = "Master Unit " + dataIndex.ToString();

            // in this example, we just pass the data to our unit's view which will update its UI
            masterUnitView.SetData(_data[dataIndex]);

            // return the unit to the CScrollView
            return masterUnitView;
        }

        #endregion
    }
}
