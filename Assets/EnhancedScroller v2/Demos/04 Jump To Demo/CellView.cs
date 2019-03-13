using UnityEngine;
using UnityEngine.UI;
using ChinarUi.ScrollView;

namespace EnhancedCScrollViewDemos.JumpToDemo
{
    public class UnitView : CScrollUnitUi
    {
        public Text unitText;

        public void SetData(Data data)
        {
            unitText.text = data.unitText;
        }
    }
}