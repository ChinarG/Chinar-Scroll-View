using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ChinarUi.ScrollView;
using ChinarUi;

namespace EnhancedCScrollViewDemos.SelectionDemo
{
    /// <summary>
    /// This class sets up the data for the inventory and handles the EnhancedCScrollViews'
    /// callbacks. It also allows changes to the CScrollViews through some UI interfaces.
    /// </summary>
    public class SelectionDemo : MonoBehaviour, IChinarScrollDelegate
    {
        /// <summary>
        /// The list of inventory data
        /// </summary>
        private CList<InventoryData> _data;

        /// <summary>
        /// The vertical inventory CScrollView
        /// </summary>
        public CScrollView vCScrollView;

        /// <summary>
        /// The horizontal inventory CScrollView
        /// </summary>
        public CScrollView hCScrollView;

        /// <summary>
        /// The unit view prefab for the vertical CScrollView
        /// </summary>
        public CScrollUnitUi vUnitViewPrefab;
        
        /// <summary>
        /// The unit view prefab for the horizontal CScrollView
        /// </summary>
        public CScrollUnitUi hUnitViewPrefab;

        /// <summary>
        /// The image that shows which item is selected
        /// </summary>
        public Image selectedImage;
        public Text selectedImageText;

        /// <summary>
        /// The base path to the resources folder where the inventory
        /// item sprites are located
        /// </summary>
        public string resourcePath;

        void Awake()
        {
            // turn on the mask and loop functionality for each CScrollView based
            // on the UI settings of this controller

            var maskToggle = GameObject.Find("Mask Toggle").GetComponent<Toggle>();
            MaskToggle_OnValueChanged(maskToggle.isOn);

            var loopToggle = GameObject.Find("Loop Toggle").GetComponent<Toggle>();
            LoopToggle_OnValueChanged(loopToggle.isOn);

            UnitViewSelected(null);
        }

        void Start()
        {
            // set up the delegates for each CScrollView

            vCScrollView.Delegate = this;
            hCScrollView.Delegate = this;

            // reload the data
            Reload();
        }

        /// <summary>
        /// This function sets up our inventory data and tells the CScrollViews to reload
        /// </summary>
        private void Reload()
        {
            // if the data existed previously, loop through
            // and remove the selection change handlers before
            // clearing out the data.
            if (_data != null)
            {
                for (var i = 0; i < _data.Count; i++)
                {
                    _data[i].selectedChanged = null;
                }
            }

            // set up a new inventory list
            _data = new CList<InventoryData>();

            // add inventory items to the list
            _data.Add(new InventoryData() { itemName = "Sword", itemCost = 123, itemDamage = 50, itemDefense = 0, itemWeight = 10, spritePath = resourcePath + "/sword", itemDescription = "Broadsword with a double-edged blade" });
            _data.Add(new InventoryData() { itemName = "Shield", itemCost = 80, itemDamage = 0, itemDefense = 60, itemWeight = 50, spritePath = resourcePath + "/shield", itemDescription = "Steel shield to deflect your enemy's blows" });
            _data.Add(new InventoryData() { itemName = "Amulet", itemCost = 260, itemDamage = 0, itemDefense = 0, itemWeight = 1, spritePath = resourcePath + "/amulet", itemDescription = "Magic amulet restores your health points gradually over time" });
            _data.Add(new InventoryData() { itemName = "Helmet", itemCost = 50, itemDamage = 0, itemDefense = 20, itemWeight = 20, spritePath = resourcePath + "/helmet", itemDescription = "Standard helm will decrease your vulnerability" });
            _data.Add(new InventoryData() { itemName = "Boots", itemCost = 40, itemDamage = 0, itemDefense = 10, itemWeight = 5, spritePath = resourcePath + "/boots", itemDescription = "Boots of speed will double your movement points" });
            _data.Add(new InventoryData() { itemName = "Bracers", itemCost = 30, itemDamage = 0, itemDefense = 20, itemWeight = 10, spritePath = resourcePath + "/bracers", itemDescription = "Bracers will upgrade your overall armor" });
            _data.Add(new InventoryData() { itemName = "Crossbow", itemCost = 100, itemDamage = 40, itemDefense = 0, itemWeight = 30, spritePath = resourcePath + "/crossbow", itemDescription = "Crossbow can attack from long range" });
            _data.Add(new InventoryData() { itemName = "Fire Ring", itemCost = 300, itemDamage = 100, itemDefense = 0, itemWeight = 1, spritePath = resourcePath + "/fireRing", itemDescription = "Fire ring gives you the magical ability to cast fireball spells" });
            _data.Add(new InventoryData() { itemName = "Knapsack", itemCost = 22, itemDamage = 0, itemDefense = 0, itemWeight = 0, spritePath = resourcePath + "/knapsack", itemDescription = "Knapsack will increase your carrying capacity by twofold" });

            // tell the CScrollViews to reload
            vCScrollView.ReloadData();
            hCScrollView.ReloadData();
        }

        /// <summary>
        /// This function handles the unit view's button click event
        /// </summary>
        /// <param name="unitUi">The unit view that had the button clicked</param>
        private void UnitViewSelected(CScrollUnitUi unitUi)
        {
            if (unitUi == null)
            {
                // nothing was selected
                selectedImage.gameObject.SetActive(false);
                selectedImageText.text = "None";
            }
            else
            {
                // get the selected data index of the unit view
                var selectedDataIndex = (unitUi as InventoryUnitView).DataIndex;

                // loop through each item in the data list and turn
                // on or off the selection state. This is done so that
                // any previous selection states are removed and new
                // ones are added.
                for (var i = 0; i < _data.Count; i++)
                {
                    _data[i].Selected = (selectedDataIndex == i);
                }

                selectedImage.gameObject.SetActive(true);
                selectedImage.sprite = Resources.Load<Sprite>(_data[selectedDataIndex].spritePath + "_v");

                selectedImageText.text = _data[selectedDataIndex].itemName;
            }
        }

        #region Controller UI Handlers

        /// <summary>
        /// This handles the toggle for the masks
        /// </summary>
        /// <param name="val">Is the mask on?</param>
        public void MaskToggle_OnValueChanged(bool val)
        {
            // set the mask component of each CScrollView
            vCScrollView.GetComponent<Mask>().enabled = val;
            hCScrollView.GetComponent<Mask>().enabled = val;
        }

        /// <summary>
        /// This handles the toggle fof the looping
        /// </summary>
        /// <param name="val">Is the looping on?</param>
        public void LoopToggle_OnValueChanged(bool val)
        {
            // set the loop property of each CScrollView
            vCScrollView.Loop = val;
            hCScrollView.Loop = val;
        }

        #endregion

        #region EnhancedCScrollView Callbacks

        /// <summary>
        /// This callback tells the CScrollView how many inventory items to expect
        /// </summary>
        /// <param name="CScrollView">The CScrollView requesting the number of units</param>
        /// <returns>The number of units</returns>
        public int GetUnitCount(CScrollView CScrollView)
        {
            return _data.Count;
        }

        /// <summary>
        /// This callback tells the CScrollView what size each unit is.
        /// </summary>
        /// <param name="CScrollView">The CScrollView requesting the unit size</param>
        /// <param name="dataIndex">The index of the data list</param>
        /// <returns>The size of the unit (Height for vertical CScrollViews, Width for Horizontal CScrollViews)</returns>
        public float GetUnitUiSize(CScrollView CScrollView, int dataIndex)
        {
            if (CScrollView == vCScrollView)
            {
                // return a static height for all vertical CScrollView units
                return 320f;
            }
            else
            {
                // return a static width for all horizontal CScrollView units
                return 150f;
            }
        }

        /// <summary>
        /// This callback gets the unit to be displayed by the CScrollView
        /// </summary>
        /// <param name="CScrollView">The CScrollView requesting the unit</param>
        /// <param name="dataIndex">The index of the data list</param>
        /// <param name="unitIndex">The unit index (This will be different from dataindex if looping is involved)</param>
        /// <returns>The unit to display</returns>
        public CScrollUnitUi GetUnitUi(CScrollView CScrollView, int dataIndex, int unitIndex)
        {
            // first get a unit from the CScrollView. The CScrollView will recycle if it can.
            // We want a vertical unit prefab for the vertical CScrollView and a horizontal
            // prefab for the horizontal CScrollView.
            InventoryUnitView unitUi = CScrollView.GetUnitView(CScrollView == vCScrollView ? vUnitViewPrefab : hUnitViewPrefab) as InventoryUnitView;

            // set the name of the unit. This just makes it easier to see in our
            // hierarchy what the unit is
            unitUi.name = (CScrollView == vCScrollView ? "Vertical" : "Horizontal") + " " + _data[dataIndex].itemName;

            // set the selected callback to the UnitViewSelected function of this controller. 
            // this will be fired when the unit's button is clicked
            unitUi.selected = UnitViewSelected;

            // set the data for the unit
            unitUi.SetData(dataIndex, _data[dataIndex], (CScrollView == vCScrollView));

            // return the unit view to the CScrollView
            return unitUi;
        }

        #endregion
    }
}