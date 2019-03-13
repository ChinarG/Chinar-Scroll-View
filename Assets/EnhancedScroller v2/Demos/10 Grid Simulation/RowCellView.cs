using UnityEngine;
using UnityEngine.UI;
using ChinarUi.ScrollView;
using ChinarUi;
using System;

namespace EnhancedCScrollViewDemos.GridSimulation
{
    /// <summary>
    /// This is the sub unit of the row unit
    /// </summary>
    public class RowUnitView : MonoBehaviour
    {
        public GameObject container;
        public Text text;

        /// <summary>
        /// This function just takes the Demo data and displays it
        /// </summary>
        /// <param name="data"></param>
        public void SetData(Data data)
        {
            // this unit was outside the range of the data, so we disable the container.
            // Note: We could have disable the unit gameobject instead of a child container,
            // but that can cause problems if you are trying to get components (disabled objects are ignored).
            container.SetActive(data != null);

            if (data != null)
            {
                // set the text if the unit is inside the data range
                text.text = data.someText;
            }
        }
    }
}