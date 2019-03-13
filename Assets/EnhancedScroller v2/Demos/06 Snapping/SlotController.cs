using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ChinarUi.ScrollView;
using ChinarUi;

namespace EnhancedCScrollViewDemos.SnappingDemo
{
    /// <summary>
    /// This class controls one slot CScrollView. We could have shared the slot data between the 
    /// three slot controllers, but for demonstration purposes we gave each slot controller their 
    /// own set of data.
    /// </summary>
    public class SlotController : MonoBehaviour, IChinarScrollDelegate
    {
        /// <summary>
        /// This list of slot units
        /// </summary>
        private CList<SlotData> _data;

        /// <summary>
        /// The CScrollView that will display the slot units
        /// </summary>
        public CScrollView CScrollView;

        /// <summary>
        /// The slot unit view prefab to use in the CScrollView
        /// </summary>
        public CScrollUnitUi slotUnitViewPrefab;

        void Awake()
        {
            // create a new data list for the slots
            _data = new CList<SlotData>();
        }

        void Start()
        {
            // set this controller as the CScrollView's delegate
            CScrollView.Delegate = this;
        }

        public void Reload(Sprite[] sprites)
        {
            // reset the data list
            _data.Clear();

            // at the sprites from the demo script to this CScrollView's data units
            foreach (var slotSprite in sprites)
            {
                _data.Add(new SlotData() { sprite = slotSprite });
            }

            // reload the CScrollView
            CScrollView.ReloadData();
        }

        /// <summary>
        /// This makes the CScrollView move without having an explicit touch event
        /// </summary>
        /// <param name="amount"></param>
        public void AddVelocity(float amount)
        {
            // set the CScrollView's linear velocity
            // (velocity in one direction)
            CScrollView.LinearVelocity = amount;
        }

        #region EnhancedCScrollView Callbacks

        /// <summary>
        /// This callback tells the CScrollView how many slot units to expect
        /// </summary>
        /// <param name="CScrollView">The CScrollView requesting the number of units</param>
        /// <returns>The number of units</returns>
        public int GetUnitCount(CScrollView CScrollView)
        {
            return _data.Count;
        }

        /// <summary>
        /// This callback tells the CScrollView what size each unit is.
        /// </summary>
        /// <param name="CScrollView">The CScrollView requesting the unit size</param>
        /// <param name="dataIndex">The index of the data list</param>
        /// <returns>The size of the unit (Height for vertical CScrollViews, Width for Horizontal CScrollViews)</returns>
        public float GetUnitUiSize(CScrollView CScrollView, int dataIndex)
        {
            return 150f;
        }

        /// <summary>
        /// This callback gets the unit to be displayed by the CScrollView
        /// </summary>
        /// <param name="CScrollView">The CScrollView requesting the unit</param>
        /// <param name="dataIndex">The index of the data list</param>
        /// <param name="unitIndex">The unit index (This will be different from dataindex if looping is involved)</param>
        /// <returns>The unit to display</returns>
        public CScrollUnitUi GetUnitUi(CScrollView CScrollView, int dataIndex, int unitIndex)
        {
            // get the unit view from the CScrollView, recycling if possible
            SlotUnitView unitUi = CScrollView.GetUnitView(slotUnitViewPrefab) as SlotUnitView;

            // set the data for the unit
            unitUi.SetData(_data[dataIndex]);

            // return the unit view to the CScrollView
            return unitUi;
        }

        #endregion
    }
}