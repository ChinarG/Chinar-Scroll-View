using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ChinarUi.ScrollView;

namespace EnhancedCScrollViewDemos.JumpToDemo
{
    /// <summary>
    /// This demo shows how to jump to an index in the CScrollView. You can jump to a position before
    /// or after the unit. You can also include the spacing before or after the unit.
    /// </summary>
    public class Controller : MonoBehaviour, IChinarScrollDelegate
    {
        /// <summary>
        /// In this example we are going to use a standard generic List. We could have used
        /// a SmallList for efficiency, but this is just a demonstration that other list
        /// types can be used.
        /// </summary>
        private List<Data> _data;

        /// <summary>
        /// Reference to the CScrollViews
        /// </summary>
        public CScrollView vCScrollView;
        public CScrollView hCScrollView;

        /// <summary>
        /// References to the UI elements
        /// </summary>
        public InputField jumpIndexInput;
        public Toggle useSpacingToggle;
        public Slider CScrollViewOffsetSlider;
        public Slider unitOffsetSlider;

        /// <summary>
        /// Reference to the unit prefab
        /// </summary>
        public CScrollUnitUi unitUiPrefab;

        public CScrollView.TweenType vCScrollViewTweenType = CScrollView.TweenType.immediate;
        public float vCScrollViewTweenTime = 0f;

        public CScrollView.TweenType hCScrollViewTweenType = CScrollView.TweenType.immediate;
        public float hCScrollViewTweenTime = 0f;

        void Start()
        {
            // set up the CScrollView delegates
            vCScrollView.Delegate = this;
            hCScrollView.Delegate = this;

            // set up some simple data
            _data = new List<Data>();
            for (var i = 0; i < 100; i++)
                _data.Add(new Data() { unitText = "Unit Data Index " + i.ToString() });

            // tell the CScrollView to reload now that we have the data
            vCScrollView.ReloadData();
            hCScrollView.ReloadData();
        }

        #region UI Handlers

        public void JumpButton_OnClick()
        {
            int jumpDataIndex;

            // extract the integer from the input text
            if (int.TryParse(jumpIndexInput.text, out jumpDataIndex))
            {
                // jump to the index
                vCScrollView.JumpToDataIndex(jumpDataIndex, CScrollViewOffsetSlider.value, unitOffsetSlider.value, useSpacingToggle.isOn, vCScrollViewTweenType, vCScrollViewTweenTime, null, CScrollView.LoopJumpDirectionEnum.Down);
                hCScrollView.JumpToDataIndex(jumpDataIndex, CScrollViewOffsetSlider.value, unitOffsetSlider.value, useSpacingToggle.isOn, hCScrollViewTweenType, hCScrollViewTweenTime);
            }
            else
            {
                Debug.LogWarning("The jump value you entered is not a number.");
            }
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
            // in this example, even numbered units are 30 pixels tall, odd numbered units are 100 pixels tall for the vertical CScrollView
            // the horizontal CScrollView has a fixed unit size of 200 pixels

            if (CScrollView == vCScrollView)
                return (dataIndex % 2 == 0 ? 30f : 100f);
            else
                return (200f);
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