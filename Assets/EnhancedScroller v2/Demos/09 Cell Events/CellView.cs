using UnityEngine;
using UnityEngine.UI;
using ChinarUi.ScrollView;

namespace EnhancedCScrollViewDemos.UnitEvents
{
    /// <summary>
    /// These delegates will publish events when a button is clicked
    /// </summary>
    /// <param name="value"></param>
    public delegate void UnitButtonTextClickedDelegate(string value);
    public delegate void UnitButtonIntegerClickedDelegate(int value);

    public class UnitView : CScrollUnitUi
    {
        private Data _data;

        public Text someTextText;

        /// <summary>
        ///  These delegates will fire whenever one of the events occurs
        /// </summary>
        public UnitButtonTextClickedDelegate unitButtonTextClicked;
        public UnitButtonIntegerClickedDelegate unitButtonFixedIntegerClicked;
        public UnitButtonIntegerClickedDelegate unitButtonDataIntegerClicked;

        public void SetData(Data data)
        {
            _data = data;
            someTextText.text = (_data.hour == 0 ? "Midnight" : string.Format("{0} 'o clock", _data.hour.ToString()));
        }

        // Handle the click of the fixed text button (this is hooked up in the Unity editor in the button's click event)
        public void UnitButtonText_OnClick(string value)
        {
            // fire event if anyone has subscribed to it
            if (unitButtonTextClicked != null) unitButtonTextClicked(value);
        }

        // Handle the click of the fixed integer button (this is hooked up in the Unity editor in the button's click event)
        public void UnitButtonFixedInteger_OnClick(int value)
        {
            // fire event if anyone has subscribed to it
            if (unitButtonFixedIntegerClicked != null) unitButtonFixedIntegerClicked(value);
        }

        // Handle the click of the data integer button (this is hooked up in the Unity editor in the button's click event)
        public void UnitButtonDataInteger_OnClick()
        {
            // fire event if anyone has subscribed to it
            if (unitButtonDataIntegerClicked != null) unitButtonDataIntegerClicked(_data.hour);
        }
    }
}