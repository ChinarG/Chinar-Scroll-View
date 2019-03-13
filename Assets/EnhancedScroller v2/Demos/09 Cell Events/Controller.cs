using UnityEngine;
using System.Collections.Generic;
using ChinarUi;
using ChinarUi.ScrollView;

namespace EnhancedCScrollViewDemos.UnitEvents
{
    /// <summary>
    /// This demo shows how you can respond to events from your units views using delegates
    /// </summary>
    public class Controller : MonoBehaviour, IChinarScrollDelegate
    {
        private List<Data> _data;

        public CScrollView CScrollView;
        public CScrollUnitUi unitUiPrefab;
        public float unitSize;

        void Start()
        {
            CScrollView.Delegate = this;
            LoadData();
        }

        private void LoadData()
        {
            _data = new List<Data>();

            for (var i = 0; i < 24; i++)
                _data.Add(new Data() { hour = i });
        }

        #region EnhancedCScrollView Handlers

        public int GetUnitCount(CScrollView CScrollView)
        {
            return _data.Count;
        }

        public float GetUnitUiSize(CScrollView CScrollView, int dataIndex)
        {
            return unitSize;
        }

        public CScrollUnitUi GetUnitUi(CScrollView CScrollView, int dataIndex, int unitIndex)
        {
            UnitView unitUi = CScrollView.GetUnitView(unitUiPrefab) as UnitView;

            // Set handlers for the unit views delegates.
            // Each handler will respond to a different type of event
            unitUi.unitButtonTextClicked = UnitButtonTextClicked;
            unitUi.unitButtonFixedIntegerClicked = UnitButtonFixedIntegerClicked;
            unitUi.unitButtonDataIntegerClicked = UnitButtonDataIntegerClicked;

            unitUi.SetData(_data[dataIndex]);


            return unitUi;
        }

        #endregion

        /// <summary>
        /// Handler for when the unit view fires a fixed text button click event
        /// </summary>
        /// <param name="value">value of the text</param>
        private void UnitButtonTextClicked(string value)
        {
            Debug.Log("Unit Text Button Clicked! Value = " + value);
        }

        /// <summary>
        /// Handler for when the unit view fires a fixed integer button click event
        /// </summary>
        /// <param name="value">value of the integer</param>
        private void UnitButtonFixedIntegerClicked(int value)
        {
            Debug.Log("Unit Fixed Integer Button Clicked! Value = " + value);
        }

        /// <summary>
        /// Handler for when the unit view fires a data integer button click event
        /// </summary>
        /// <param name="value">value of the integer</param>
        private void UnitButtonDataIntegerClicked(int value)
        {
            Debug.Log("Unit Data Integer Button Clicked! Value = " + value);
        }
    }
}
