using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ChinarUi.ScrollView;
using ChinarUi;

namespace EnhancedCScrollViewDemos.RemoteResourcesDemo
{
    /// <summary>
    /// This demo shows how you can remotely load resources, calling the set data function when
    /// the unit's visibility changes to true. When the unit is hidden, we set the image back to
    /// a default loading sprite.
    /// </summary>
    public class Controller : MonoBehaviour, IChinarScrollDelegate
    {
        /// <summary>
        /// The data for the CScrollView
        /// </summary>
        private CList<Data> _data;

        /// <summary>
        /// The CScrollView to control
        /// </summary>
        public CScrollView CScrollView;

        /// <summary>
        /// The prefab of the unit view
        /// </summary>
        public CScrollUnitUi unitUiPrefab;

        void Start()
        {
            // set the CScrollView's delegate to this controller
            CScrollView.Delegate = this;

            // set the CScrollView's unit view visbility changed delegate to a method in this controller
            CScrollView.unitUiVisibilityChanged = UnitViewVisibilityChanged;

            // set up some simple data
            _data = new CList<Data>();

            // set up a list of images with their dimensions
            for (var i = 0; i <= 12; i++)
            {
                _data.Add(new Data() { imageUrl = string.Format("http://qiniu.chinar.xin/18-12-10/5552072.jpg", i), imageDimensions = new Vector2(200f, 200f) });
            }

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
            // return a fixed unit size of 200 pixels
            return (260f);
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

            // In this example, we do not set the data here since the unit is not visibile yet. Use a coroutine
            // before the unit is visibile will result in errors, so we defer loading until the unit has
            // become visible. We can trap this in the unitUiVisibilityChanged delegate handled below

            // return the unit to the CScrollView
            return unitUi;
        }

        /// <summary>
        /// This handler will be called any time a unit view is shown or hidden
        /// </summary>
        /// <param name="unitUi">The unit view that was shown or hidden</param>
        private void UnitViewVisibilityChanged(CScrollUnitUi unitUi)
        {
            // cast the unit view to our custom view
            UnitView view = unitUi as UnitView;

            // if the unit is active, we set its data, 
            // otherwise we will clear the image back to 
            // its default state

            if (unitUi.Active)
                view.SetData(_data[unitUi.DataIndex]);
            else
                view.ClearImage();
        }

        #endregion
    }
}