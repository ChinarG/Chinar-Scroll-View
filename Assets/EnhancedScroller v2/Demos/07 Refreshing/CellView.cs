using UnityEngine;
using UnityEngine.UI;
using ChinarUi.ScrollView;

namespace EnhancedCScrollViewDemos.RefreshDemo
{
    /// <summary>
    /// This is the view of our unit which handles how the unit looks.
    /// </summary>
    public class UnitView : CScrollUnitUi
    {
        /// <summary>
        /// This is a reference to the unit's underlying data.
        /// We will store it in the SetData method, and use it
        /// in the RefreshUnitView method.
        /// </summary>
        private Data _data;

        /// <summary>
        /// A reference to the UI Text element to display the unit data
        /// </summary>
        public Text someTextText;

        public RectTransform RectTransform
        {
            get
            {
                var rt = gameObject.GetComponent<RectTransform>();
                return rt;
            }
        }

        /// <summary>
        /// This function just takes the Demo data and displays it
        /// </summary>
        /// <param name="data"></param>
        public void SetData(Data data)
        {
            // store the data so that it can be used when refreshing
            _data = data;

            // update the unit's UI
            RefreshUnitUi();
        }

        public override void RefreshUnitUi()
        {
            // update the UI text with the unit data
            someTextText.text = _data.someText;
        }
    }
}