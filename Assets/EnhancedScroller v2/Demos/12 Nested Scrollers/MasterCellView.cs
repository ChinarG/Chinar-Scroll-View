using UnityEngine;
using UnityEngine.UI;
using ChinarUi.ScrollView;
using System.Collections.Generic;
using System;

namespace EnhancedCScrollViewDemos.NestedCScrollViews
{
    /// <summary>
    /// The master unit view contains a detail EnhancedCScrollView
    /// </summary>
    public class MasterUnitView : CScrollUnitUi, IChinarScrollDelegate
    {
        private bool reloadDataNextFrame = false;

        /// <summary>
        /// The detail CScrollView containing our detail units
        /// </summary>
        public CScrollView detailCScrollView;

        /// <summary>
        /// The list of detail units for this master unit
        /// </summary>
        private MasterData _data;

        /// <summary>
        /// Detail unit prefab to instantiate
        /// </summary>
        public CScrollUnitUi detailUnitViewPrefab;

        /// <summary>
        /// Sets the detail CScrollView delegate and data
        /// </summary>
        /// <param name="data"></param>
        public void SetData(MasterData data)
        {
            // set up delegates and callbacks
            detailCScrollView.Delegate = this;
            detailCScrollView.CScrollViewScrolled = CScrollViewScrolled;

            // assign data and flag that the detail CScrollView needs to be reloaded.
            // we have to reload on the next frame through the update so that the 
            // main CScrollView has time to set up the master unit views first.
            _data = data;
            reloadDataNextFrame = true;
        }

        /// <summary>
        /// Check to see if the CScrollView needs to be reloaded
        /// </summary>
        void Update()
        {
            if (reloadDataNextFrame)
            {
                // CScrollView needs reloaded, so we unflag and reload the detail data

                reloadDataNextFrame = false;
                detailCScrollView.ReloadData(_data.normalizedScrollPosition);
            }
        }

        #region EnhancedCScrollView Handlers

        /// <summary>
        /// This tells the CScrollView the number of units that should have room allocated. This should be the length of your data array.
        /// </summary>
        /// <param name="CScrollView">The CScrollView that is requesting the data size</param>
        /// <returns>The number of units</returns>
        public int GetUnitCount(CScrollView CScrollView)
        {
            // in this example, we just pass the number of our detail data elements
            return _data.childData.Count;
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
            // in this example, we set the units at 100 pixels wide
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
            DetailUnitView detailUnitView = CScrollView.GetUnitView(detailUnitViewPrefab) as DetailUnitView;

            // set the name of the game object to the unit's data index.
            // this is optional, but it helps up debug the objects in 
            // the scene hierarchy.
            detailUnitView.name = "Detail Unit " + dataIndex.ToString();

            // in this example, we just pass the data to our unit's view which will update its UI
            detailUnitView.SetData(_data.childData[dataIndex]);

            // return the unit to the CScrollView
            return detailUnitView;
        }

        /// <summary>
        /// Capture the scroll position to use when the CScrollView is recycled
        /// </summary>
        private void CScrollViewScrolled(CScrollView CScrollView, Vector2 val, float scrollPosition)
        {
            _data.normalizedScrollPosition = CScrollView.NormalizedScrollPosition;
        }

        #endregion
    }
}
