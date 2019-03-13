using System.Collections;
using System.Collections.Generic;
using ChinarUi.ScrollView;
using UnityEngine;
using UnityEngine.UI;


public class ChinarUnitView : CScrollUnitUi
{
    public Text UnitNameText;


    public void SetData(ChinarUnitData data)
    {
        UnitNameText.text = data.UnitName;
    }
}