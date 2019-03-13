using UnityEngine;
using UnityEngine.UI;
using ChinarUi.ScrollView;

namespace EnhancedCScrollViewDemos.ViewDrivenUnitSizes
{
    public class UnitView : CScrollUnitUi
    {
        public Text someTextText;

        /// <summary>
        /// A reference to the rect transform which will be
        /// updated by the content size fitter
        /// </summary>
        public RectTransform textRectTransform;

        /// <summary>
        /// The space around the text label so that we
        /// aren't up against the edges of the unit
        /// </summary>
        public RectOffset textBuffer;

        public void SetData(Data data)
        {
            someTextText.text = data.someText;

            // force update the canvas so that it can calculate the size needed for the text immediately
            Canvas.ForceUpdateCanvases();

            // set the data's unit size and add in some padding so the the text isn't up against the border of the unit
            data.unitSize = textRectTransform.rect.height + textBuffer.top + textBuffer.bottom;
        }
    }
}