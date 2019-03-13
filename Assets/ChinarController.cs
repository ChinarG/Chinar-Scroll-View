using System.Collections.Generic;
using ChinarUi.ScrollView;
using UnityEngine;


/// <summary>
/// 滚动条控制器
/// </summary>
public class ChinarController : MonoBehaviour, IChinarScrollDelegate
{
    private List<ChinarUnitData> data;                 //单元数据
    public  CScrollView     ChinarCScrollView;       //增强滚动条
    public  ChinarUnitView       ChinarUnitViewPrefab; //单元预设


    void Start()
    {
        data = new List<ChinarUnitData>
        {
            new ChinarUnitData {UnitName = "狮子"},
            new ChinarUnitData {UnitName = "小熊"},
            new ChinarUnitData {UnitName = "老鹰"},
            new ChinarUnitData {UnitName = "海豚"},
            new ChinarUnitData {UnitName = "蚂蚁"},
            new ChinarUnitData {UnitName = "猫"},
            new ChinarUnitData {UnitName = "麻雀"},
            new ChinarUnitData {UnitName = "狗"},
            new ChinarUnitData {UnitName = "蜘蛛"},
            new ChinarUnitData {UnitName = "大象"},
            new ChinarUnitData {UnitName = "猎鹰"},
            new ChinarUnitData {UnitName = "老鼠"},
            new ChinarUnitData {UnitName = "狮子"},
            new ChinarUnitData {UnitName = "小熊"},
            new ChinarUnitData {UnitName = "老鹰"},
            new ChinarUnitData {UnitName = "海豚"},
            new ChinarUnitData {UnitName = "蚂蚁"},
            new ChinarUnitData {UnitName = "猫"},
            new ChinarUnitData {UnitName = "麻雀"},
            new ChinarUnitData {UnitName = "狗"},
            new ChinarUnitData {UnitName = "蜘蛛"},
            new ChinarUnitData {UnitName = "大象"},
            new ChinarUnitData {UnitName = "猎鹰"},
            new ChinarUnitData {UnitName = "老鼠"}
        };
        ChinarCScrollView.Delegate = this;
        ChinarCScrollView.ReloadData();
    }


    /// <summary>
    /// 获取单元格数量
    /// </summary>
    public int GetUnitCount(CScrollView CScrollView)
    {
        return data.Count;
    }


    /// <summary>
    /// 获取单元格尺寸
    /// </summary>
    public float GetUnitUiSize(CScrollView CScrollView, int dataIndex)
    {
        return 100f;
    }


    /// <summary>
    /// 获取单元格视图
    /// </summary>
    public CScrollUnitUi GetUnitUi(CScrollView CScrollView, int dataIndex, int unitIndex)
    {
        ChinarUnitView unitUi = CScrollView.GetUnitView(ChinarUnitViewPrefab) as ChinarUnitView;
        unitUi.SetData(data[dataIndex]);
        return unitUi;
    }
}