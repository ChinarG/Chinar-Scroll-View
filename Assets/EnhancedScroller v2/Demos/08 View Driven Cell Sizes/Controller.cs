using UnityEngine;
using System.Collections.Generic;
using ChinarUi;
using ChinarUi.ScrollView;

namespace EnhancedCScrollViewDemos.ViewDrivenUnitSizes
{
    /// <summary>
    /// This demo shows how you can use the calculated size of the unit view to drive the CScrollView's unit sizes.
    /// This can be good for cases where you do not know how large each unit will need to be until the contents are
    /// populated. An example of this would be text units containing unknown information.
    /// </summary>
    public class Controller : MonoBehaviour, IChinarScrollDelegate
    {
        private List<Data> _data;

        public CScrollView CScrollView;
        public CScrollUnitUi unitUiPrefab;

        void Start()
        {
            CScrollView.Delegate = this;
            LoadData();
        }

        /// <summary>
        /// Populates the data with some random Lorum Ipsum text
        /// </summary>
        private void LoadData()
        {
            _data = new List<Data>();

            // populate the CScrollView with some text
            for (var i = 0; i < 7; i++)
            {
                _data.Add(new Data() { unitSize = 0, someText = (i * 11 + 0).ToString() + " Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam augue enim, scelerisque ac diam nec, efficitur aliquam orci. Vivamus laoreet, libero ut aliquet convallis, dolor elit auctor purus, eget dapibus elit libero at lacus. Aliquam imperdiet sem ultricies ultrices vestibulum. Proin feugiat et dui sit amet ultrices. Quisque porta lacus justo, non ornare nulla eleifend at. Nunc malesuada eget neque sit amet viverra. Donec et lectus ac lorem elementum porttitor. Praesent urna felis, dapibus eu nunc varius, varius tincidunt ante. Vestibulum vitae nulla malesuada, consequat justo eu, dapibus elit. Nulla tristique enim et convallis facilisis." });
                _data.Add(new Data() { unitSize = 0, someText = (i * 11 + 1).ToString() + " Nunc convallis, ipsum a porta viverra, tortor velit feugiat est, eget consectetur ex metus vel diam." });
                _data.Add(new Data() { unitSize = 0, someText = (i * 11 + 2).ToString() + " Phasellus laoreet vitae lectus sit amet venenatis. Duis scelerisque ultricies tincidunt. Cras ullamcorper lectus sed risus porttitor, id viverra urna venenatis. Maecenas in odio sed mi tempus porta et a justo. Nullam non ullamcorper est. Nam rhoncus nulla quis commodo aliquam. Maecenas pulvinar est sed ex iaculis, eu pretium tellus placerat. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Praesent in ipsum faucibus, fringilla lectus id, congue est. " });
                _data.Add(new Data() { unitSize = 0, someText = (i * 11 + 3).ToString() + " Fusce ex lectus." });
                _data.Add(new Data() { unitSize = 0, someText = (i * 11 + 4).ToString() + " Fusce mollis elementum sem euismod malesuada. Aenean et convallis turpis. Suspendisse potenti." });
                _data.Add(new Data() { unitSize = 0, someText = (i * 11 + 5).ToString() + " Fusce nec sapien orci. Pellentesque mollis ligula vitae interdum imperdiet. Aenean ultricies velit at turpis luctus, nec lacinia ligula malesuada. Nulla facilisi. Donec at nisi lorem. Aenean vestibulum velit velit, sed eleifend dui sodales in. Nunc vulputate, nulla non facilisis hendrerit, neque dolor lacinia orci, et fermentum nunc quam vel purus. Donec gravida massa non ullamcorper consectetur. Sed pellentesque leo ac ornare egestas. " });
                _data.Add(new Data() { unitSize = 0, someText = (i * 11 + 6).ToString() + " Curabitur non dignissim turpis, vel viverra elit. Cras in sem rhoncus, gravida velit ut, consectetur erat. Proin ac aliquet nulla. Mauris quis augue nisi. Sed purus magna, mollis sed massa ac, scelerisque lobortis leo. Nullam at facilisis ex. Nullam ut accumsan orci. Integer vitae dictum felis, quis tristique sem. Suspendisse potenti. Curabitur bibendum eleifend eros at porta. Ut malesuada consectetur arcu nec lacinia. " });
                _data.Add(new Data() { unitSize = 0, someText = (i * 11 + 7).ToString() + " Pellentesque pulvinar ac arcu fermentum interdum. Pellentesque gravida faucibus ipsum at blandit. Vestibulum pharetra erat sit amet feugiat sodales. Nunc et dui viverra tellus efficitur egestas. Sed ex mauris, eleifend in nisi sed, consequat tincidunt elit. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Proin vel bibendum enim. Etiam feugiat nulla ac dui commodo, eget vehicula est scelerisque. In metus neque, congue a justo ac, consequat lacinia neque. Vivamus non velit vitae ex dictum pharetra. Aliquam blandit nisi eget libero feugiat porta. " });
                _data.Add(new Data() { unitSize = 0, someText = (i * 11 + 8).ToString() + " Proin bibendum ligula a pulvinar convallis. Mauris tincidunt tempor ipsum id viverra. Vivamus congue ipsum venenatis tellus semper, vel venenatis mauris finibus. Vivamus a nisl in lacus fermentum varius. Mauris bibendum magna placerat risus interdum, vitae facilisis nulla pellentesque. Curabitur vehicula odio quis magna pulvinar, et lacinia ante bibendum. Morbi laoreet eleifend ante, quis luctus augue luctus sit amet. Sed consectetur enim et orci posuere euismod. Curabitur sollicitudin metus eu nisl dictum suscipit. " });
                _data.Add(new Data() { unitSize = 0, someText = (i * 11 + 9).ToString() + " Sed gravida augue ligula, tempus auctor ante rutrum sit amet. Vestibulum finibus magna ut viverra rhoncus. Vestibulum rutrum eu nibh interdum imperdiet. Curabitur ac nunc a turpis ultricies dictum. Phasellus in molestie eros. Morbi porta imperdiet odio sed pharetra. Cras blandit tincidunt ultricies. " });
                _data.Add(new Data() { unitSize = 0, someText = (i * 11 + 10).ToString() + " Integer pellentesque viverra orci, sollicitudin luctus dui rhoncus sed. Duis placerat at felis vel placerat. Mauris massa urna, scelerisque vitae posuere vitae, ultrices in nibh. Mauris posuere hendrerit viverra. In lacinia urna nibh, ut lobortis lectus finibus et. Aliquam arcu dolor, suscipit eget massa id, eleifend dapibus est. Quisque eget bibendum urna. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed condimentum pulvinar ornare. Aliquam venenatis eget nunc et euismod. " });
            }

            ResizeCScrollView();
        }

        /// <summary>
        /// This function adds a new record, resizing the CScrollView and calculating the sizes of all units
        /// </summary>
        public void AddNewRow()
        {
            // first, clear out the units in the CScrollView so the new text transforms will be reset
            CScrollView.ClearAll();

            // reset the CScrollView's position so that it is not outside of the new bounds
            CScrollView.ScrollPosition = 0;

            // second, reset the data's unit view sizes
            foreach (var item in _data)
            {
                item.unitSize = 0;
            }

            // now we can add the data row
            _data.Add(new Data() { unitSize = 0, someText = _data.Count.ToString() + " New Row Added!" });

            ResizeCScrollView();

            // optional: jump to the end of the CScrollView to see the new content
            CScrollView.JumpToDataIndex(_data.Count - 1, 1f, 1f);
        }

        /// <summary>
        /// This function will exand the CScrollView to accommodate the units, reload the data to calculate the unit sizes,
        /// reset the CScrollView's size back, then reload the data once more to display the units.
        /// </summary>
        private void ResizeCScrollView()
        {
            // capture the CScrollView dimensions so that we can reset them when we are done
            var rectTransform = CScrollView.GetComponent<RectTransform>();
            var size = rectTransform.sizeDelta;

            // set the dimensions to the largest size possible to acommodate all the units
            rectTransform.sizeDelta = new Vector2(size.x, float.MaxValue);

            // First Pass: reload the CScrollView so that it can populate the text UI elements in the unit view.
            // The content size fitter will determine how big the units need to be on subsequent passes
            CScrollView.ReloadData();

            // reset the CScrollView size back to what it was originally
            rectTransform.sizeDelta = size;

            // Second Pass: reload the data once more with the newly set unit view sizes and CScrollView content size
            CScrollView.ReloadData();
        }

        #region EnhancedCScrollView Handlers

        public int GetUnitCount(CScrollView CScrollView)
        {
            return _data.Count;
        }

        public float GetUnitUiSize(CScrollView CScrollView, int dataIndex)
        {
            // we pull the size of the unit from the model.
            // First pass (frame countdown 2): this size will be zero as set in the LoadData function
            // Second pass (frame countdown 1): this size will be set to the content size fitter in the unit view
            // Third pass (frmae countdown 0): this set value will be pulled here from the CScrollView
            return _data[dataIndex].unitSize;
        }

        public CScrollUnitUi GetUnitUi(CScrollView CScrollView, int dataIndex, int unitIndex)
        {
            UnitView unitUi = CScrollView.GetUnitView(unitUiPrefab) as UnitView;
            unitUi.SetData(_data[dataIndex]);
            return unitUi;
        }


        #endregion
    }
}
