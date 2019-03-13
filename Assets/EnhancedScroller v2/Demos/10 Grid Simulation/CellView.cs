using UnityEngine;
using UnityEngine.UI;
using ChinarUi.ScrollView;
using ChinarUi;
using System;

namespace EnhancedCScrollViewDemos.GridSimulation
{
    /// <summary>
    /// This is the view of our unit which handles how the unit looks.
    /// It stores references to sub units
    /// </summary>
    public class UnitView : CScrollUnitUi
    {
        public RowUnitView[] rowUnitViews;

        /// <summary>
        /// This function just takes the Demo data and displays it
        /// </summary>
        /// <param name="data"></param>
        public void SetData(ref CList<Data> data, int startingIndex)
        {
            // loop through the sub units to display their data (or disable them if they are outside the bounds of the data)
            for (var i = 0; i < rowUnitViews.Length; i++)
            {
                // if the sub unit is outside the bounds of the data, we pass null to the sub unit
                rowUnitViews[i].SetData(startingIndex + i < data.Count ? data[startingIndex + i] : null);
            }
        }
    }
}