using UnityEngine;
using UnityEngine.UI;
using ChinarUi.ScrollView;

namespace EnhancedCScrollViewDemos.PullDownRefresh
{
    /// <summary>
    /// This is the view of our unit which handles how the unit looks.
    /// </summary>
    public class UnitView : CScrollUnitUi
    {
        /// <summary>
        /// A reference to the UI Text element to display the unit data
        /// </summary>
        public Text someTextText;

        /// <summary>
        /// This function just takes the Demo data and displays it
        /// </summary>
        /// <param name="data"></param>
        public void SetData(Data data)
        {
            // update the UI text with the unit data
            someTextText.text = data.someText;
        }
    }
}