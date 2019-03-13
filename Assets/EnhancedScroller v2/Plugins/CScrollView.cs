using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;



namespace ChinarUi.ScrollView
{
    /// <summary>
    /// 此委托操控单元UI的可见性
    /// </summary>
    /// <param name="cScrollUnitUi"> CScrollUnitUi </param>
    public delegate void UnitViewVisibilityChangedDelegate(CScrollUnitUi cScrollUnitUi);


    /// <summary>
    /// 这个委托将在单元Ui被回收之前 触发
    /// </summary>
    /// <param name="cScrollUnitUi"> CScrollUnitUi </param>
    public delegate void UnitViewWillRecycleDelegate(CScrollUnitUi cScrollUnitUi);


    /// <summary>
    /// 这个委托处理 ScrollRect 滚动时的回调
    /// </summary>
    /// <param name="cScrollView">调用委托的 CScrollView</param>
    /// <param name="vector2">滚动的值</param>
    /// <param name="startScrollPosition">滚动开始的位置（以像素为单位）</param>
    public delegate void CScrollViewScrolledDelegate(CScrollView cScrollView, Vector2 vector2, float startScrollPosition);


    /// <summary>
    /// 这个委托处理 CScrollView 所在的UI。
    /// </summary>
    /// <param name="cScrollView">调用委托的CScrollView</param>
    /// <param name="unitIndex">单元下标 (循环模式下可能与数据下标不同)</param>
    /// <param name="dataIndex">视图所在数据的索引</param>
    /// <param name="cScrollUnitUi">单元UI CScrollUnitUi</param>
    public delegate void CScrollViewSnappedDelegate(CScrollView cScrollView, int unitIndex, int dataIndex, CScrollUnitUi cScrollUnitUi);


    /// <summary>
    /// 此委托处理 CScrollView 的滚动/不滚动状态
    /// </summary>
    /// <param name="cScrollView">改变状态 CScrollView</param>
    /// <param name="scrolling">是否滚动</param>
    public delegate void CScrollViewScrollingChangedDelegate(CScrollView cScrollView, bool scrolling);


    /// <summary>
    /// This delegate handles the change in state of the CScrollView (jumping or not jumping)
    /// </summary>
    /// <param name="CScrollView">The CScrollView that changed state</param>
    /// <param name="tweening">Whether or not the CScrollView is tweening</param>
    public delegate void CScrollViewTweeningChangedDelegate(CScrollView CScrollView, bool tweening);


    /// <summary>
    /// The EnhancedCScrollView allows you to easily set up a dynamic CScrollView that will recycle views for you. This means
    /// that using only a handful of views, you can display thousands of rows. This will save memory and processing
    /// power in your application.
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class CScrollView : MonoBehaviour
    {
        #region Public

        /// <summary>
        /// The direction this CScrollView is handling
        /// </summary>
        public enum ScrollDirectionEnum
        {
            Vertical,
            Horizontal
        }


        /// <summary>
        /// Which side of a unit to reference.
        /// For vertical CScrollViews, before means above, after means below.
        /// For horizontal CScrollViews, before means to left of, after means to the right of.
        /// </summary>
        public enum UnitViewPositionEnum
        {
            Before,
            After
        }


        /// <summary>
        /// This will set how the scroll bar should be shown based on the data. If no scrollbar
        /// is attached, then this is ignored. OnlyIfNeeded will hide the scrollbar based on whether
        /// the CScrollView is looping or there aren't enough items to scroll.
        /// </summary>
        public enum ScrollbarVisibilityEnum
        {
            OnlyIfNeeded,
            Always,
            Never
        }


        /// <summary>
        /// The direction the CScrollView is handling
        /// </summary>
        public ScrollDirectionEnum scrollDirection;

        /// <summary>
        /// The number of pixels between unit views, starting after the first unit view
        /// </summary>
        public float spacing;

        /// <summary>
        /// The padding inside of the CScrollView: top, bottom, left, right.
        /// </summary>
        public RectOffset padding;

        /// <summary>
        /// Whether the CScrollView should loop the unit views
        /// </summary>
        [SerializeField] private bool loop;

        /// <summary>
        /// Whether the scollbar should be shown
        /// </summary>
        [SerializeField] private ScrollbarVisibilityEnum scrollbarVisibility;

        /// <summary>
        /// Whether snapping is turned on
        /// </summary>
        public bool snapping;

        /// <summary>
        /// This is the speed that will initiate the snap. When the
        /// CScrollView slows down to this speed it will snap to the location
        /// specified.
        /// </summary>
        public float snapVelocityThreshold;

        /// <summary>
        /// The snap offset to watch for. When the snap occurs, this
        /// location in the CScrollView will be how which unit to snap to 
        /// is determined.
        /// Typically, the offset is in the range 0..1, with 0 being
        /// the top / left of the CScrollView and 1 being the bottom / right.
        /// In most situations the watch offset and the jump offset 
        /// will be the same, they are just separated in case you need
        /// that added functionality.
        /// </summary>
        public float snapWatchOffset;

        /// <summary>
        /// The snap location to move the unit to. When the snap occurs,
        /// this location in the CScrollView will be where the snapped unit
        /// is moved to.
        /// Typically, the offset is in the range 0..1, with 0 being
        /// the top / left of the CScrollView and 1 being the bottom / right.
        /// In most situations the watch offset and the jump offset 
        /// will be the same, they are just separated in case you need
        /// that added functionality.
        /// </summary>
        public float snapJumpToOffset;

        /// <summary>
        /// Once the unit has been snapped to the CScrollView location, this
        /// value will determine how the unit is centered on that CScrollView
        /// location. 
        /// Typically, the offset is in the range 0..1, with 0 being
        /// the top / left of the unit and 1 being the bottom / right.
        /// </summary>
        public float snapUnitCenterOffset;

        /// <summary>
        /// Whether to include the spacing between units when determining the
        /// unit offset centering.
        /// </summary>
        public bool snapUseUnitSpacing;

        /// <summary>
        /// What function to use when interpolating between the current 
        /// scroll position and the snap location. This is also known as easing. 
        /// If you want to go immediately to the snap location you can either 
        /// set the snapTweenType to immediate or set the snapTweenTime to zero.
        /// </summary>
        public TweenType snapTweenType;

        /// <summary>
        /// The time it takes to interpolate between the current scroll 
        /// position and the snap location.
        /// If you want to go immediately to the snap location you can either 
        /// set the snapTweenType to immediate or set the snapTweenTime to zero.
        /// </summary>
        public float snapTweenTime;

        /// <summary>
        /// This delegate is called when a unit view is hidden or shown
        /// </summary>
        public UnitViewVisibilityChangedDelegate unitUiVisibilityChanged;

        /// <summary>
        /// This delegate is called just before a unit view is hidden by recycling
        /// </summary>
        public UnitViewWillRecycleDelegate unitUiWillRecycle;

        /// <summary>
        /// This delegate is called when the scroll rect scrolls
        /// </summary>
        public CScrollViewScrolledDelegate CScrollViewScrolled;

        /// <summary>
        /// This delegate is called when the CScrollView has snapped to a position
        /// </summary>
        public CScrollViewSnappedDelegate CScrollViewSnapped;

        /// <summary>
        /// This delegate is called when the CScrollView has started or stopped scrolling
        /// </summary>
        public CScrollViewScrollingChangedDelegate CScrollViewScrollingChanged;

        /// <summary>
        /// This delegate is called when the CScrollView has started or stopped tweening
        /// </summary>
        public CScrollViewTweeningChangedDelegate CScrollViewTweeningChanged;

        /// <summary>
        /// The Delegate is what the CScrollView will call when it needs to know information about
        /// the underlying data or views. This allows a true MVC process.
        /// </summary>
        public IChinarScrollDelegate Delegate
        {
            get { return _delegate; }
            set
            {
                _delegate   = value;
                _reloadData = true;
            }
        }

        /// <summary>
        /// The absolute position in pixels from the start of the CScrollView
        /// </summary>
        public float ScrollPosition
        {
            get { return _scrollPosition; }
            set
            {
                // make sure the position is in the bounds of the current set of views
                value = Mathf.Clamp(value, 0, GetScrollPositionForUnitViewIndex(_unitUiSizeArray.Count - 1, UnitViewPositionEnum.Before));

                // only if the value has changed
                if (_scrollPosition != value)
                {
                    _scrollPosition = value;
                    if (scrollDirection == ScrollDirectionEnum.Vertical)
                    {
                        // set the vertical position
                        _scrollRect.verticalNormalizedPosition = 1f - (_scrollPosition / ScrollSize);
                    }
                    else
                    {
                        // set the horizontal position
                        _scrollRect.horizontalNormalizedPosition = (_scrollPosition / ScrollSize);
                    }

                    // flag that we need to refresh
                    //_refreshActive = true;
                }
            }
        }

        /// <summary>
        /// The size of the active unit view container minus the visibile portion
        /// of the CScrollView
        /// </summary>
        public float ScrollSize
        {
            get
            {
                if (scrollDirection == ScrollDirectionEnum.Vertical)
                    return Mathf.Max(_container.rect.height - _scrollRectTransform.rect.height, 0);
                else
                    return Mathf.Max(_container.rect.width - _scrollRectTransform.rect.width, 0);
            }
        }

        /// <summary>
        /// The normalized position of the CScrollView between 0 and 1
        /// </summary>
        public float NormalizedScrollPosition
        {
            get
            {
                var scrollPosition = ScrollPosition;
                return (scrollPosition <= 0 ? 0 : _scrollPosition / ScrollSize);
            }
        }

        /// <summary>
        /// Whether the CScrollView should loop the resulting unit views.
        /// Looping creates three sets of internal size data, attempting
        /// to keep the CScrollView in the middle set. If the CScrollView goes
        /// outside of this set, it will jump back into the middle set,
        /// giving the illusion of an infinite set of data.
        /// </summary>
        public bool Loop
        {
            get { return loop; }
            set
            {
                // only if the value has changed
                if (loop != value)
                {
                    // get the original position so that when we turn looping on
                    // we can jump back to this position
                    var originalScrollPosition = _scrollPosition;
                    loop = value;

                    // call resize to generate more internal elements if loop is on,
                    // remove the elements if loop is off
                    _Resize(false);
                    if (loop)
                    {
                        // set the new scroll position based on the middle set of data + the original position
                        ScrollPosition = _loopFirstScrollPosition + originalScrollPosition;
                    }
                    else
                    {
                        // set the new scroll position based on the original position and the first loop position
                        ScrollPosition = originalScrollPosition - _loopFirstScrollPosition;
                    }

                    // update the scrollbars
                    ScrollbarVisibility = scrollbarVisibility;
                }
            }
        }

        /// <summary>
        /// Sets how the visibility of the scrollbars should be handled
        /// </summary>
        public ScrollbarVisibilityEnum ScrollbarVisibility
        {
            get { return scrollbarVisibility; }
            set
            {
                scrollbarVisibility = value;

                // only if the scrollbar exists
                if (_scrollbar != null)
                {
                    // make sure we actually have some unit views
                    if (_unitUiOffsetArray != null && _unitUiOffsetArray.Count > 0)
                    {
                        if (_unitUiOffsetArray.Last() < ScrollRectSize || loop)
                        {
                            // if the size of the scrollable area is smaller than the CScrollView
                            // or if we have looping on, hide the scrollbar unless the visibility
                            // is set to Always.
                            _scrollbar.gameObject.SetActive(scrollbarVisibility == ScrollbarVisibilityEnum.Always);
                        }
                        else
                        {
                            // if the size of the scrollable areas is larger than the CScrollView
                            // or looping is off, then show the scrollbars unless visibility
                            // is set to Never.
                            _scrollbar.gameObject.SetActive(scrollbarVisibility != ScrollbarVisibilityEnum.Never);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This is the velocity of the CScrollView.
        /// </summary>
        public Vector2 Velocity
        {
            get { return _scrollRect.velocity; }
            set { _scrollRect.velocity = value; }
        }

        /// <summary>
        /// The linear velocity is the velocity on one axis.
        /// The CScrollView should only be moving one one axix.
        /// </summary>
        public float LinearVelocity
        {
            get
            {
                // return the velocity component depending on which direction this is scrolling
                return (scrollDirection == ScrollDirectionEnum.Vertical ? _scrollRect.velocity.y : _scrollRect.velocity.x);
            }
            set
            {
                // set the appropriate component of the velocity
                if (scrollDirection == ScrollDirectionEnum.Vertical)
                {
                    _scrollRect.velocity = new Vector2(0, value);
                }
                else
                {
                    _scrollRect.velocity = new Vector2(value, 0);
                }
            }
        }

        /// <summary>
        /// Whether the CScrollView is scrolling or not
        /// </summary>
        public bool IsScrolling { get; private set; }

        /// <summary>
        /// Whether the CScrollView is tweening or not
        /// </summary>
        public bool IsTweening { get; private set; }

        /// <summary>
        /// This is the first unit view index showing in the CScrollView's visible area
        /// </summary>
        public int StartUnitViewIndex
        {
            get { return _activeUnitViewsStartIndex; }
        }

        /// <summary>
        /// This is the last unit view index showing in the CScrollView's visible area
        /// </summary>
        public int EndUnitViewIndex
        {
            get { return _activeUnitViewsEndIndex; }
        }

        /// <summary>
        /// This is the first data index showing in the CScrollView's visible area
        /// </summary>
        public int StartDataIndex
        {
            get { return _activeUnitViewsStartIndex % NumberOfUnits; }
        }

        /// <summary>
        /// This is the last data index showing in the CScrollView's visible area
        /// </summary>
        public int EndDataIndex
        {
            get { return _activeUnitViewsEndIndex % NumberOfUnits; }
        }

        /// <summary>
        /// This is the number of units in the CScrollView
        /// </summary>
        public int NumberOfUnits
        {
            get { return (_delegate != null ? _delegate.GetUnitCount(this) : 0); }
        }

        /// <summary>
        /// This is a convenience link to the CScrollView's scroll rect
        /// </summary>
        public ScrollRect ScrollRect
        {
            get { return _scrollRect; }
        }

        /// <summary>
        /// The size of the visible portion of the CScrollView
        /// </summary>
        public float ScrollRectSize
        {
            get
            {
                if (scrollDirection == ScrollDirectionEnum.Vertical)
                    return _scrollRectTransform.rect.height;
                else
                    return _scrollRectTransform.rect.width;
            }
        }


        /// <summary>
        /// Create a unit view, or recycle one if it already exists
        /// </summary>
        /// <param name="unitPrefab">The prefab to use to create the unit view</param>
        /// <returns></returns>
        public CScrollUnitUi GetUnitView(CScrollUnitUi unitPrefab)
        {
            // see if there is a view to recycle
            var unitUi = _GetRecycledUnitView(unitPrefab);
            if (unitUi == null)
            {
                // no recyleable unit found, so we create a new view
                // and attach it to our container
                var go = Instantiate(unitPrefab.gameObject);
                unitUi = go.GetComponent<CScrollUnitUi>();
                unitUi.transform.SetParent(_container);
                unitUi.transform.localPosition = Vector3.zero;
                unitUi.transform.localRotation = Quaternion.identity;
            }

            return unitUi;
        }


        /// <summary>
        /// 这将重置内部大小列表并刷新单元格视图
        /// </summary>
        /// <param name="scrollPositionFactor">从0到1,0之间开始的滚动块的百分比是滚动块的开始</param>
        public void ReloadData(float scrollPositionFactor = 0)
        {
            _reloadData = false;

            // recycle all the active units so
            // that we are sure to get fresh views
            _RecycleAllUnits();

            // if we have a delegate handling our data, then
            // call the resize
            if (_delegate != null)
                _Resize(false);
            if (_scrollRect == null || _scrollRectTransform == null || _container == null)
            {
                _scrollPosition = 0f;
                return;
            }

            _scrollPosition = Mathf.Clamp(scrollPositionFactor * ScrollSize, 0, GetScrollPositionForUnitViewIndex(_unitUiSizeArray.Count - 1, UnitViewPositionEnum.Before));
            if (scrollDirection == ScrollDirectionEnum.Vertical)
            {
                // set the vertical position
                _scrollRect.verticalNormalizedPosition = 1f - scrollPositionFactor;
            }
            else
            {
                // set the horizontal position
                _scrollRect.horizontalNormalizedPosition = scrollPositionFactor;
            }
        }


        /// <summary>
        /// This calls the RefreshUnitView method on each active unit.
        /// If you override the RefreshUnitView method in your units
        /// then you can update the UI without having to reload the data.
        /// Note: this will not change the unit sizes, you will need
        /// to call ReloadData for that to work.
        /// </summary>
        public void RefreshActiveUnitViews()
        {
            for (var i = 0; i < _activeUnitViews.Count; i++)
            {
                _activeUnitViews[i].RefreshUnitUi();
            }
        }


        /// <summary>
        /// Removes all units, both active and recycled from the CScrollView.
        /// This will call garbage collection.
        /// </summary>
        public void ClearAll()
        {
            ClearActive();
            ClearRecycled();
        }


        /// <summary>
        /// Removes all the active unit views. This should only be used if you want
        /// to get rid of units because of settings set by Unity that cannot be
        /// changed at runtime. This will call garbage collection.
        /// </summary>
        public void ClearActive()
        {
            for (var i = 0; i < _activeUnitViews.Count; i++)
            {
                DestroyImmediate(_activeUnitViews[i].gameObject);
            }

            _activeUnitViews.Clear();
        }


        /// <summary>
        /// Removes all the recycled unit views. This should only be used after you
        /// load in a completely different set of unit views that will not use the 
        /// recycled views. This will call garbage collection.
        /// </summary>
        public void ClearRecycled()
        {
            for (var i = 0; i < _recycledUnitViews.Count; i++)
            {
                DestroyImmediate(_recycledUnitViews[i].gameObject);
            }

            _recycledUnitViews.Clear();
        }


        /// <summary>
        /// Turn looping on or off. This is just a helper function so 
        /// you don't have to keep track of the state of the looping
        /// in your own scripts.
        /// </summary>
        public void ToggleLoop()
        {
            Loop = !loop;
        }


        public enum LoopJumpDirectionEnum
        {
            Closest,
            Up,
            Down
        }


        /// <summary>
        /// Jump to a position in the CScrollView based on a dataIndex. This overload allows you
        /// to specify a specific offset within a unit as well.
        /// </summary>
        /// <param name="dataIndex">he data index to jump to</param>
        /// <param name="CScrollViewOffset">The offset from the start (top / left) of the CScrollView in the range 0..1.
        /// Outside this range will jump to the location before or after the CScrollView's viewable area</param>
        /// <param name="unitOffset">The offset from the start (top / left) of the unit in the range 0..1</param>
        /// <param name="useSpacing">Whether to calculate in the spacing of the CScrollView in the jump</param>
        /// <param name="tweenType">What easing to use for the jump</param>
        /// <param name="tweenTime">How long to interpolate to the jump point</param>
        /// <param name="jumpComplete">This delegate is fired when the jump completes</param>
        public void JumpToDataIndex(int                   dataIndex,
                                    float                 CScrollViewOffset = 0,
                                    float                 unitOffset        = 0,
                                    bool                  useSpacing        = true,
                                    TweenType             tweenType         = TweenType.immediate,
                                    float                 tweenTime         = 0f,
                                    Action                jumpComplete      = null,
                                    LoopJumpDirectionEnum loopJumpDirection = LoopJumpDirectionEnum.Closest
        )
        {
            var unitOffsetPosition = 0f;
            if (unitOffset != 0)
            {
                // calculate the unit offset position

                // get the unit's size
                var unitSize = (_delegate != null ? _delegate.GetUnitUiSize(this, dataIndex) : 0);
                if (useSpacing)
                {
                    // if using spacing add spacing from one side
                    unitSize += spacing;

                    // if this is not a bounday unit, then add spacing from the other side
                    if (dataIndex > 0 && dataIndex < (NumberOfUnits - 1)) unitSize += spacing;
                }

                // calculate the position based on the size of the unit and the offset within that unit
                unitOffsetPosition = unitSize * unitOffset;
            }

            if (CScrollViewOffset == 1f)
            {
                unitOffsetPosition += padding.bottom;
            }

            // cache the offset for quicker calculation
            var offset            = -(CScrollViewOffset * ScrollRectSize) + unitOffsetPosition;
            var newScrollPosition = 0f;
            if (loop)
            {
                // if looping, then we need to determine the closest jump position.
                // we do that by checking all three sets of data locations, and returning the closest one

                // get the scroll positions for each data set.
                // Note: we are calculating the position based on the unit view index, not the data index here
                var set1Position = GetScrollPositionForUnitViewIndex(dataIndex,                       UnitViewPositionEnum.Before) + offset;
                var set2Position = GetScrollPositionForUnitViewIndex(dataIndex + NumberOfUnits,       UnitViewPositionEnum.Before) + offset;
                var set3Position = GetScrollPositionForUnitViewIndex(dataIndex + (NumberOfUnits * 2), UnitViewPositionEnum.Before) + offset;

                // get the offsets of each scroll position from the current scroll position
                var set1Diff = (Mathf.Abs(_scrollPosition - set1Position));
                var set2Diff = (Mathf.Abs(_scrollPosition - set2Position));
                var set3Diff = (Mathf.Abs(_scrollPosition - set3Position));
                switch (loopJumpDirection)
                {
                    case LoopJumpDirectionEnum.Closest:

                        // choose the smallest offset from the current position (the closest position)
                        if (set1Diff < set2Diff)
                        {
                            if (set1Diff < set3Diff)
                            {
                                newScrollPosition = set1Position;
                            }
                            else
                            {
                                newScrollPosition = set3Position;
                            }
                        }
                        else
                        {
                            if (set2Diff < set3Diff)
                            {
                                newScrollPosition = set2Position;
                            }
                            else
                            {
                                newScrollPosition = set3Position;
                            }
                        }

                        break;
                    case LoopJumpDirectionEnum.Up:
                        newScrollPosition = set1Position;
                        break;
                    case LoopJumpDirectionEnum.Down:
                        newScrollPosition = set3Position;
                        break;
                }
            }
            else
            {
                // not looping, so just get the scroll position from the dataIndex
                newScrollPosition = GetScrollPositionForDataIndex(dataIndex, UnitViewPositionEnum.Before) + offset;
            }

            // clamp the scroll position to a valid location
            newScrollPosition = Mathf.Clamp(newScrollPosition, 0, GetScrollPositionForUnitViewIndex(_unitUiSizeArray.Count - 1, UnitViewPositionEnum.Before));

            // if spacing is used, adjust the final position
            if (useSpacing)
            {
                // move back by the spacing if necessary
                newScrollPosition = Mathf.Clamp(newScrollPosition - spacing, 0, GetScrollPositionForUnitViewIndex(_unitUiSizeArray.Count - 1, UnitViewPositionEnum.Before));
            }

            // ignore the jump if the scroll position hasn't changed
            if (newScrollPosition == _scrollPosition)
            {
                if (jumpComplete != null) jumpComplete();
                return;
            }

            // start tweening
            StartCoroutine(TweenPosition(tweenType, tweenTime, ScrollPosition, newScrollPosition, jumpComplete));
        }


        /// <summary>
        /// Snaps the CScrollView on command. This is called internally when snapping is set to true and the velocity
        /// has dropped below the threshold. You can use this to manually snap whenever you like.
        /// </summary>
        public void Snap()
        {
            if (NumberOfUnits == 0) return;

            // set snap jumping to true so other events won't process while tweening
            _snapJumping = true;

            // stop the CScrollView
            LinearVelocity = 0;

            // cache the current inertia state and turn off inertia
            _snapInertia        = _scrollRect.inertia;
            _scrollRect.inertia = false;

            // calculate the snap position
            var snapPosition = ScrollPosition + (ScrollRectSize * Mathf.Clamp01(snapWatchOffset));

            // get the unit view index of unit at the watch location
            _snapUnitViewIndex = GetUnitViewIndexAtPosition(snapPosition);

            // get the data index of the unit at the watch location
            _snapDataIndex = _snapUnitViewIndex % NumberOfUnits;

            // jump the snapped unit to the jump offset location and center it on the unit offset
            JumpToDataIndex(_snapDataIndex, snapJumpToOffset, snapUnitCenterOffset, snapUseUnitSpacing, snapTweenType, snapTweenTime, SnapJumpComplete);
        }


        /// <summary>
        /// Gets the scroll position in pixels from the start of the CScrollView based on the unitUiIndex
        /// </summary>
        /// <param name="unitUiIndex">The unit index to look for. This is used instead of dataIndex in case of looping</param>
        /// <param name="insertPosition">Do we want the start or end of the unit view's position</param>
        /// <returns></returns>
        public float GetScrollPositionForUnitViewIndex(int unitUiIndex, UnitViewPositionEnum insertPosition)
        {
            if (NumberOfUnits == 0) return 0;
            if (unitUiIndex   < 0) unitUiIndex = 0;
            if (unitUiIndex == 0 && insertPosition == UnitViewPositionEnum.Before)
            {
                return 0;
            }
            else
            {
                if (unitUiIndex < _unitUiOffsetArray.Count)
                {
                    // the index is in the range of unit view offsets
                    if (insertPosition == UnitViewPositionEnum.Before)
                    {
                        // return the previous unit view's offset + the spacing between unit views
                        return _unitUiOffsetArray[unitUiIndex - 1] + spacing + (scrollDirection == ScrollDirectionEnum.Vertical ? padding.top : padding.left);
                    }
                    else
                    {
                        // return the offset of the unit view (offset is after the unit)
                        return _unitUiOffsetArray[unitUiIndex] + (scrollDirection == ScrollDirectionEnum.Vertical ? padding.top : padding.left);
                    }
                }
                else
                {
                    // get the start position of the last unit (the offset of the second to last unit)
                    return _unitUiOffsetArray[_unitUiOffsetArray.Count - 2];
                }
            }
        }


        /// <summary>
        /// Gets the scroll position in pixels from the start of the CScrollView based on the dataIndex
        /// </summary>
        /// <param name="dataIndex">The data index to look for</param>
        /// <param name="insertPosition">Do we want the start or end of the unit view's position</param>
        /// <returns></returns>
        public float GetScrollPositionForDataIndex(int dataIndex, UnitViewPositionEnum insertPosition)
        {
            return GetScrollPositionForUnitViewIndex(loop ? _delegate.GetUnitCount(this) + dataIndex : dataIndex, insertPosition);
        }


        /// <summary>
        /// Gets the index of a unit view at a given position
        /// </summary>
        /// <param name="position">The pixel offset from the start of the CScrollView</param>
        /// <returns></returns>
        public int GetUnitViewIndexAtPosition(float position)
        {
            // call the overrloaded method on the entire range of the list
            return _GetUnitIndexAtPosition(position, 0, _unitUiOffsetArray.Count - 1);
        }

        #endregion

        #region Private

        /// <summary>
        /// Cached reference to the scrollRect
        /// </summary>
        private ScrollRect _scrollRect;

        /// <summary>
        /// Cached reference to the scrollRect's transform
        /// </summary>
        private RectTransform _scrollRectTransform;

        /// <summary>
        /// Cached reference to the scrollbar if it exists
        /// </summary>
        private Scrollbar _scrollbar;

        /// <summary>
        /// Cached reference to the active unit view container
        /// </summary>
        private RectTransform _container;

        /// <summary>
        /// Cached reference to the layout group that handles view positioning
        /// </summary>
        private HorizontalOrVerticalLayoutGroup _layoutGroup;

        /// <summary>
        /// Reference to the delegate that will tell this CScrollView information
        /// about the underlying data
        /// </summary>
        private IChinarScrollDelegate _delegate;

        /// <summary>
        /// Flag to tell the CScrollView to reload the data
        /// </summary>
        private bool _reloadData;

        /// <summary>
        /// Flag to tell the CScrollView to refresh the active list of unit views
        /// </summary>
        private bool _refreshActive;

        /// <summary>
        /// List of views that have been recycled
        /// </summary>
        private CList<CScrollUnitUi> _recycledUnitViews = new CList<CScrollUnitUi>();

        /// <summary>
        /// Cached reference to the element used to offset the first visible unit view
        /// </summary>
        private LayoutElement _firstPadder;

        /// <summary>
        /// Cached reference to the element used to keep the unit views at the correct size
        /// </summary>
        private LayoutElement _lastPadder;

        /// <summary>
        /// Cached reference to the container that holds the recycled unit views
        /// </summary>
        private RectTransform _recycledUnitViewContainer;

        /// <summary>
        /// Internal list of unit view sizes. This is created when the data is reloaded 
        /// to speed up processing.
        /// </summary>
        private CList<float> _unitUiSizeArray = new CList<float>();

        /// <summary>
        /// Internal list of unit view offsets. Each unit view offset is an accumulation 
        /// of the offsets previous to it.
        /// This is created when the data is reloaded to speed up processing.
        /// </summary>
        private CList<float> _unitUiOffsetArray = new CList<float>();

        /// <summary>
        /// The CScrollViews position
        /// </summary>
        private float _scrollPosition;

        /// <summary>
        /// The list of unit views that are currently being displayed
        /// </summary>
        private CList<CScrollUnitUi> _activeUnitViews = new CList<CScrollUnitUi>();

        /// <summary>
        /// The index of the first unit view that is being displayed
        /// </summary>
        private int _activeUnitViewsStartIndex;

        /// <summary>
        /// The index of the last unit view that is being displayed
        /// </summary>
        private int _activeUnitViewsEndIndex;

        /// <summary>
        /// The index of the first element of the middle section of unit view sizes.
        /// Used only when looping
        /// </summary>
        private int _loopFirstUnitIndex;

        /// <summary>
        /// The index of the last element of the middle seciton of unit view sizes.
        /// used only when looping
        /// </summary>
        private int _loopLastUnitIndex;

        /// <summary>
        /// The scroll position of the first element of the middle seciotn of unit views.
        /// Used only when looping
        /// </summary>
        private float _loopFirstScrollPosition;

        /// <summary>
        /// The scroll position of the last element of the middle section of unit views.
        /// Used only when looping
        /// </summary>
        private float _loopLastScrollPosition;

        /// <summary>
        /// The position that triggers the CScrollView to jump to the end of the middle section
        /// of unit views. This keeps the CScrollView in the middle section as much as possible.
        /// </summary>
        private float _loopFirstJumpTrigger;

        /// <summary>
        /// The position that triggers the CScrollView to jump to the start of the middle section
        /// of unit views. This keeps the CScrollView in the middle section as much as possible.
        /// </summary>
        private float _loopLastJumpTrigger;

        /// <summary>
        /// The cached value of the last scroll rect size. This is checked every frame to see
        /// if the scroll rect has resized. If so, it will refresh.
        /// </summary>
        private float _lastScrollRectSize;

        /// <summary>
        /// The cached value of the last loop setting. This is checked every frame to see
        /// if looping was toggled. If so, it will refresh.
        /// </summary>
        private bool _lastLoop;

        /// <summary>
        /// The unit view index we are snapping to
        /// </summary>
        private int _snapUnitViewIndex;

        /// <summary>
        /// The data index we are snapping to
        /// </summary>
        private int _snapDataIndex;

        /// <summary>
        /// Whether we are currently jumping due to a snap
        /// </summary>
        private bool _snapJumping;

        /// <summary>
        /// What the previous inertia setting was before the snap jump.
        /// We cache it here because we need to turn off inertia while
        /// manually tweeing.
        /// </summary>
        private bool _snapInertia;

        /// <summary>
        /// The cached value of the last scrollbar visibility setting. This is checked every
        /// frame to see if the scrollbar visibility needs to be changed.
        /// </summary>
        private ScrollbarVisibilityEnum _lastScrollbarVisibility;


        /// <summary>
        /// Where in the list we are
        /// </summary>
        private enum ListPositionEnum
        {
            First,
            Last
        }


        /// <summary>
        /// This function will create an internal list of sizes and offsets to be used in all calculations.
        /// It also sets up the loop triggers and positions and initializes the unit views.
        /// </summary>
        /// <param name="keepPosition">If true, then the CScrollView will try to go back to the position it was at before the resize</param>
        private void _Resize(bool keepPosition)
        {
            // cache the original position
            var originalScrollPosition = _scrollPosition;

            // clear out the list of unit view sizes and create a new list
            _unitUiSizeArray.Clear();
            var offset = _AddUnitViewSizes();

            // if looping, we need to create three sets of size data
            if (loop)
            {
                // if the units don't entirely fill up the scroll area, 
                // make some more size entries to fill it up
                if (offset < ScrollRectSize)
                {
                    int additionalRounds = Mathf.CeilToInt(ScrollRectSize / offset);
                    _DuplicateUnitViewSizes(additionalRounds, _unitUiSizeArray.Count);
                }

                // set up the loop indices
                _loopFirstUnitIndex = _unitUiSizeArray.Count;
                _loopLastUnitIndex  = _loopFirstUnitIndex + _unitUiSizeArray.Count - 1;

                // create two more copies of the unit sizes
                _DuplicateUnitViewSizes(2, _unitUiSizeArray.Count);
            }

            // calculate the offsets of each unit view
            _CalculateUnitViewOffsets();

            // set the size of the active unit view container based on the number of unit views there are and each of their sizes
            if (scrollDirection == ScrollDirectionEnum.Vertical)
                _container.sizeDelta = new Vector2(_container.sizeDelta.x, _unitUiOffsetArray.Last() + padding.top + padding.bottom);
            else
                _container.sizeDelta = new Vector2(_unitUiOffsetArray.Last() + padding.left + padding.right, _container.sizeDelta.y);

            // if looping, set up the loop positions and triggers
            if (loop)
            {
                _loopFirstScrollPosition = GetScrollPositionForUnitViewIndex(_loopFirstUnitIndex, UnitViewPositionEnum.Before)                + (spacing * 0.5f);
                _loopLastScrollPosition  = GetScrollPositionForUnitViewIndex(_loopLastUnitIndex, UnitViewPositionEnum.After) - ScrollRectSize + (spacing * 0.5f);
                _loopFirstJumpTrigger    = _loopFirstScrollPosition                                                                           - ScrollRectSize;
                _loopLastJumpTrigger     = _loopLastScrollPosition                                                                            + ScrollRectSize;
            }

            // create the visibile units
            _ResetVisibleUnitViews();

            // if we need to maintain our original position
            if (keepPosition)
            {
                ScrollPosition = originalScrollPosition;
            }
            else
            {
                if (loop)
                {
                    ScrollPosition = _loopFirstScrollPosition;
                }
                else
                {
                    ScrollPosition = 0;
                }
            }

            // set up the visibility of the scrollbar
            ScrollbarVisibility = scrollbarVisibility;
        }


        /// <summary>
        /// Creates a list of unit view sizes for faster access
        /// </summary>
        /// <returns></returns>
        private float _AddUnitViewSizes()
        {
            var offset = 0f;
            // add a size for each row in our data based on how many the delegate tells us to create
            for (var i = 0; i < NumberOfUnits; i++)
            {
                // add the size of this unit based on what the delegate tells us to use. Also add spacing if this unit isn't the first one
                _unitUiSizeArray.Add(_delegate.GetUnitUiSize(this, i) + (i == 0 ? 0 : _layoutGroup.spacing));
                offset += _unitUiSizeArray[_unitUiSizeArray.Count - 1];
            }

            return offset;
        }


        /// <summary>
        /// Create a copy of the unit view sizes. This is only used in looping
        /// </summary>
        /// <param name="numberOfTimes">How many times the copy should be made</param>
        /// <param name="unitCount">How many units to copy</param>
        private void _DuplicateUnitViewSizes(int numberOfTimes, int unitCount)
        {
            for (var i = 0; i < numberOfTimes; i++)
            {
                for (var j = 0; j < unitCount; j++)
                {
                    _unitUiSizeArray.Add(_unitUiSizeArray[j] + (j == 0 ? _layoutGroup.spacing : 0));
                }
            }
        }


        /// <summary>
        /// Calculates the offset of each unit, accumulating the values from previous units
        /// </summary>
        private void _CalculateUnitViewOffsets()
        {
            _unitUiOffsetArray.Clear();
            var offset = 0f;
            for (var i = 0; i < _unitUiSizeArray.Count; i++)
            {
                offset += _unitUiSizeArray[i];
                _unitUiOffsetArray.Add(offset);
            }
        }


        /// <summary>
        /// Get a recycled unit with a given identifier if available
        /// </summary>
        /// <param name="unitPrefab">The prefab to check for</param>
        /// <returns></returns>
        private CScrollUnitUi _GetRecycledUnitView(CScrollUnitUi unitPrefab)
        {
            for (var i = 0; i < _recycledUnitViews.Count; i++)
            {
                if (_recycledUnitViews[i].UnitMark == unitPrefab.UnitMark)
                {
                    // the unit view was found, so we use this recycled one.
                    // we also remove it from the recycled list
                    var unitUi = _recycledUnitViews.RemoveAt(i);
                    return unitUi;
                }
            }

            return null;
        }


        /// <summary>
        /// This sets up the visible units, adding and recycling as necessary
        /// </summary>
        private void _ResetVisibleUnitViews()
        {
            int startIndex;
            int endIndex;

            // calculate the range of the visible units
            _CalculateCurrentActiveUnitRange(out startIndex, out endIndex);

            // go through each previous active unit and recycle it if it no longer falls in the range
            var        i                    = 0;
            CList<int> remainingUnitIndices = new CList<int>();
            while (i < _activeUnitViews.Count)
            {
                if (_activeUnitViews[i].UnitIndex < startIndex || _activeUnitViews[i].UnitIndex > endIndex)
                {
                    _RecycleUnit(_activeUnitViews[i]);
                }
                else
                {
                    // this unit index falls in the new range, so we add its
                    // index to the reusable list
                    remainingUnitIndices.Add(_activeUnitViews[i].UnitIndex);
                    i++;
                }
            }

            if (remainingUnitIndices.Count == 0)
            {
                // there were no previous active units remaining, 
                // this list is either brand new, or we jumped to 
                // an entirely different part of the list.
                // just add all the new unit views
                for (i = startIndex; i <= endIndex; i++)
                {
                    _AddUnitView(i, ListPositionEnum.Last);
                }
            }
            else
            {
                // we are able to reuse some of the previous
                // unit views

                // first add the views that come before the 
                // previous list, going backward so that the
                // new views get added to the front
                for (i = endIndex; i >= startIndex; i--)
                {
                    if (i < remainingUnitIndices.First())
                    {
                        _AddUnitView(i, ListPositionEnum.First);
                    }
                }

                // next add teh views that come after the
                // previous list, going forward and adding
                // at the end of the list
                for (i = startIndex; i <= endIndex; i++)
                {
                    if (i > remainingUnitIndices.Last())
                    {
                        _AddUnitView(i, ListPositionEnum.Last);
                    }
                }
            }

            // update the start and end indices
            _activeUnitViewsStartIndex = startIndex;
            _activeUnitViewsEndIndex   = endIndex;

            // adjust the padding elements to offset the unit views correctly
            _SetPadders();
        }


        /// <summary>
        /// Recycles all the active units
        /// </summary>
        private void _RecycleAllUnits()
        {
            while (_activeUnitViews.Count > 0) _RecycleUnit(_activeUnitViews[0]);
            _activeUnitViewsStartIndex = 0;
            _activeUnitViewsEndIndex   = 0;
        }


        /// <summary>
        /// Recycles one unit view
        /// </summary>
        /// <param name="unitUi"></param>
        private void _RecycleUnit(CScrollUnitUi unitUi)
        {
            if (unitUiWillRecycle != null) unitUiWillRecycle(unitUi);

            // remove the unit view from the active list
            _activeUnitViews.Remove(unitUi);

            // add the unit view to the recycled list
            _recycledUnitViews.Add(unitUi);

            // move the GameObject to the recycled container
            unitUi.transform.SetParent(_recycledUnitViewContainer);

            // reset the unitUi's properties
            unitUi.DataIndex = 0;
            unitUi.UnitIndex = 0;
            unitUi.Active    = false;
            if (unitUiVisibilityChanged != null) unitUiVisibilityChanged(unitUi);
        }


        /// <summary>
        /// Creates a unit view, or recycles if it can
        /// </summary>
        /// <param name="unitIndex">The index of the unit view</param>
        /// <param name="listPosition">Whether to add the unit to the beginning or the end</param>
        private void _AddUnitView(int unitIndex, ListPositionEnum listPosition)
        {
            if (NumberOfUnits == 0) return;

            // get the dataIndex. Modulus is used in case of looping so that the first set of units are ignored
            var dataIndex = unitIndex % NumberOfUnits;
            // request a unit view from the delegate
            var unitUi = _delegate.GetUnitUi(this, dataIndex, unitIndex);

            // set the unit's properties
            unitUi.UnitIndex = unitIndex;
            unitUi.DataIndex = dataIndex;
            unitUi.Active    = true;

            // add the unit view to the active container
            unitUi.transform.SetParent(_container, false);
            unitUi.transform.localScale = Vector3.one;

            // add a layout element to the unitUi
            LayoutElement layoutElement              = unitUi.GetComponent<LayoutElement>();
            if (layoutElement == null) layoutElement = unitUi.gameObject.AddComponent<LayoutElement>();

            // set the size of the layout element
            if (scrollDirection == ScrollDirectionEnum.Vertical)
                layoutElement.minHeight = _unitUiSizeArray[unitIndex] - (unitIndex > 0 ? _layoutGroup.spacing : 0);
            else
                layoutElement.minWidth = _unitUiSizeArray[unitIndex] - (unitIndex > 0 ? _layoutGroup.spacing : 0);

            // add the unit to the active list
            if (listPosition == ListPositionEnum.First)
                _activeUnitViews.AddStart(unitUi);
            else
                _activeUnitViews.Add(unitUi);

            // set the hierarchy position of the unit view in the container
            if (listPosition == ListPositionEnum.Last)
                unitUi.transform.SetSiblingIndex(_container.childCount - 2);
            else if (listPosition == ListPositionEnum.First)
                unitUi.transform.SetSiblingIndex(1);

            // call the visibility change delegate if available
            if (unitUiVisibilityChanged != null) unitUiVisibilityChanged(unitUi);
        }


        /// <summary>
        /// This function adjusts the two padders that control the first unit view's
        /// offset and the overall size of each unit.
        /// </summary>
        private void _SetPadders()
        {
            if (NumberOfUnits == 0) return;

            // calculate the size of each padder
            var firstSize = _unitUiOffsetArray[_activeUnitViewsStartIndex] - _unitUiSizeArray[_activeUnitViewsStartIndex];
            var lastSize  = _unitUiOffsetArray.Last()                      - _unitUiOffsetArray[_activeUnitViewsEndIndex];
            if (scrollDirection == ScrollDirectionEnum.Vertical)
            {
                // set the first padder and toggle its visibility
                _firstPadder.minHeight = firstSize;
                _firstPadder.gameObject.SetActive(_firstPadder.minHeight > 0);

                // set the last padder and toggle its visibility
                _lastPadder.minHeight = lastSize;
                _lastPadder.gameObject.SetActive(_lastPadder.minHeight > 0);
            }
            else
            {
                // set the first padder and toggle its visibility
                _firstPadder.minWidth = firstSize;
                _firstPadder.gameObject.SetActive(_firstPadder.minWidth > 0);

                // set the last padder and toggle its visibility
                _lastPadder.minWidth = lastSize;
                _lastPadder.gameObject.SetActive(_lastPadder.minWidth > 0);
            }
        }


        /// <summary>
        /// This function is called if the CScrollView is scrolled, updating the active list of units
        /// </summary>
        private void _RefreshActive()
        {
            //_refreshActive = false;
            int startIndex;
            int endIndex;
            var velocity = Vector2.zero;

            // if looping, check to see if we scrolled past a trigger
            if (loop)
            {
                if (_scrollPosition < _loopFirstJumpTrigger)
                {
                    velocity             = _scrollRect.velocity;
                    ScrollPosition       = _loopLastScrollPosition - (_loopFirstJumpTrigger - _scrollPosition) + spacing;
                    _scrollRect.velocity = velocity;
                }
                else if (_scrollPosition > _loopLastJumpTrigger)
                {
                    velocity             = _scrollRect.velocity;
                    ScrollPosition       = _loopFirstScrollPosition + (_scrollPosition - _loopLastJumpTrigger) - spacing;
                    _scrollRect.velocity = velocity;
                }
            }

            // get the range of visibile units
            _CalculateCurrentActiveUnitRange(out startIndex, out endIndex);

            // if the index hasn't changed, ignore and return
            if (startIndex == _activeUnitViewsStartIndex && endIndex == _activeUnitViewsEndIndex) return;

            // recreate the visibile units
            _ResetVisibleUnitViews();
        }


        /// <summary>
        /// Determines which units can be seen
        /// </summary>
        /// <param name="startIndex">The index of the first unit visible</param>
        /// <param name="endIndex">The index of the last unit visible</param>
        private void _CalculateCurrentActiveUnitRange(out int startIndex, out int endIndex)
        {
            startIndex = 0;
            endIndex   = 0;

            // get the positions of the CScrollView
            var startPosition = _scrollPosition;
            var endPosition   = _scrollPosition + (scrollDirection == ScrollDirectionEnum.Vertical ? _scrollRectTransform.rect.height : _scrollRectTransform.rect.width);

            // calculate each index based on the positions
            startIndex = GetUnitViewIndexAtPosition(startPosition);
            endIndex   = GetUnitViewIndexAtPosition(endPosition);
        }


        /// <summary>
        /// Gets the index of a unit at a given position based on a subset range.
        /// This function uses a recursive binary sort to find the index faster.
        /// </summary>
        /// <param name="position">The pixel offset from the start of the CScrollView</param>
        /// <param name="startIndex">The first index of the range</param>
        /// <param name="endIndex">The last index of the rnage</param>
        /// <returns></returns>
        private int _GetUnitIndexAtPosition(float position, int startIndex, int endIndex)
        {
            // if the range is invalid, then we found our index, return the start index
            if (startIndex >= endIndex) return startIndex;

            // determine the middle point of our binary search
            var middleIndex = (startIndex + endIndex) / 2;

            // if the middle index is greater than the position, then search the last
            // half of the binary tree, else search the first half
            if ((_unitUiOffsetArray[middleIndex] + (scrollDirection == ScrollDirectionEnum.Vertical ? padding.top : padding.left)) >= position)
                return _GetUnitIndexAtPosition(position, startIndex, middleIndex);
            else
                return _GetUnitIndexAtPosition(position, middleIndex + 1, endIndex);
        }


        /// <summary>
        /// Caches and initializes the CScrollView
        /// </summary>
        void Awake()
        {
            GameObject go;

            // cache some components
            _scrollRect          = this.GetComponent<ScrollRect>();
            _scrollRectTransform = _scrollRect.GetComponent<RectTransform>();

            // destroy any content objects if they exist. Likely there will be
            // one at design time because Unity gives errors if it can't find one.
            if (_scrollRect.content != null)
            {
                DestroyImmediate(_scrollRect.content.gameObject);
            }

            // Create a new active unit view container with a layout group
            go = new GameObject("Container", typeof(RectTransform));
            go.transform.SetParent(_scrollRectTransform);
            if (scrollDirection == ScrollDirectionEnum.Vertical)
                go.AddComponent<VerticalLayoutGroup>();
            else
                go.AddComponent<HorizontalLayoutGroup>();
            _container = go.GetComponent<RectTransform>();

            // set the containers anchor and pivot
            if (scrollDirection == ScrollDirectionEnum.Vertical)
            {
                _container.anchorMin = new Vector2(0, 1);
                _container.anchorMax = Vector2.one;
                _container.pivot     = new Vector2(0.5f, 1f);
            }
            else
            {
                _container.anchorMin = Vector2.zero;
                _container.anchorMax = new Vector2(0, 1f);
                _container.pivot     = new Vector2(0, 0.5f);
            }

            _container.offsetMax     = Vector2.zero;
            _container.offsetMin     = Vector2.zero;
            _container.localPosition = Vector3.zero;
            _container.localRotation = Quaternion.identity;
            _container.localScale    = Vector3.one;
            _scrollRect.content      = _container;

            // cache the scrollbar if it exists
            if (scrollDirection == ScrollDirectionEnum.Vertical)
            {
                _scrollbar = _scrollRect.verticalScrollbar;
            }
            else
            {
                _scrollbar = _scrollRect.horizontalScrollbar;
            }

            // cache the layout group and set up its spacing and padding
            _layoutGroup                        = _container.GetComponent<HorizontalOrVerticalLayoutGroup>();
            _layoutGroup.spacing                = spacing;
            _layoutGroup.padding                = padding;
            _layoutGroup.childAlignment         = TextAnchor.UpperLeft;
            _layoutGroup.childForceExpandHeight = true;
            _layoutGroup.childForceExpandWidth  = true;

            // force the CScrollView to scroll in the direction we want
            _scrollRect.horizontal = scrollDirection == ScrollDirectionEnum.Horizontal;
            _scrollRect.vertical   = scrollDirection == ScrollDirectionEnum.Vertical;

            // create the padder objects
            go = new GameObject("First Padder", typeof(RectTransform), typeof(LayoutElement));
            go.transform.SetParent(_container, false);
            _firstPadder = go.GetComponent<LayoutElement>();
            go           = new GameObject("Last Padder", typeof(RectTransform), typeof(LayoutElement));
            go.transform.SetParent(_container, false);
            _lastPadder = go.GetComponent<LayoutElement>();

            // create the recycled unit view container
            go = new GameObject("Recycled Units", typeof(RectTransform));
            go.transform.SetParent(_scrollRect.transform, false);
            _recycledUnitViewContainer = go.GetComponent<RectTransform>();
            _recycledUnitViewContainer.gameObject.SetActive(false);

            // set up the last values for updates
            _lastScrollRectSize      = ScrollRectSize;
            _lastLoop                = loop;
            _lastScrollbarVisibility = scrollbarVisibility;
        }


        void Update()
        {
            if (_reloadData)
            {
                // if the reload flag is true, then reload the data
                ReloadData();
            }

            // if the scroll rect size has changed and looping is on,
            // or the loop setting has changed, then we need to resize
            if (
                (loop && _lastScrollRectSize != ScrollRectSize)
                ||
                (loop != _lastLoop)
            )
            {
                _Resize(true);
                _lastScrollRectSize = ScrollRectSize;
                _lastLoop           = loop;
            }

            // update the scroll bar visibility if it has changed
            if (_lastScrollbarVisibility != scrollbarVisibility)
            {
                ScrollbarVisibility      = scrollbarVisibility;
                _lastScrollbarVisibility = scrollbarVisibility;
            }

            // determine if the CScrollView has started or stopped scrolling
            // and call the delegate if so.
            if (LinearVelocity != 0 && !IsScrolling)
            {
                IsScrolling = true;
                if (CScrollViewScrollingChanged != null) CScrollViewScrollingChanged(this, true);
            }
            else if (LinearVelocity == 0 && IsScrolling)
            {
                IsScrolling = false;
                if (CScrollViewScrollingChanged != null) CScrollViewScrollingChanged(this, false);
            }
        }


        /// <summary>
        /// This method was used in Unity 5.5.1 to get around a bug in the Unity ScrollRect component.
        /// In Unity 2017+, this code is no longer needed and actually produces other bugs if left in.
        /// </summary>
        //void LateUpdate()
        //{
        //    if (_refreshActive)
        //    {
        //        // if the refresh toggle is on, then
        //        // refresh the list
        //        _RefreshActive();
        //    }
        //}
        void OnEnable()
        {
            // when the CScrollView is enabled, add a listener to the onValueChanged handler
            _scrollRect.onValueChanged.AddListener(_ScrollRect_OnValueChanged);
        }


        void OnDisable()
        {
            // when the CScrollView is disabled, remove the listener
            _scrollRect.onValueChanged.RemoveListener(_ScrollRect_OnValueChanged);
        }


        /// <summary>
        /// Handler for when the CScrollView changes value
        /// </summary>
        /// <param name="val">The scroll rect's value</param>
        private void _ScrollRect_OnValueChanged(Vector2 val)
        {
            // set the internal scroll position
            if (scrollDirection == ScrollDirectionEnum.Vertical)
                _scrollPosition = (1f - val.y) * ScrollSize;
            else
                _scrollPosition = val.x * ScrollSize;
            //_refreshActive = true;
            _scrollPosition = Mathf.Clamp(_scrollPosition, 0, GetScrollPositionForUnitViewIndex(_unitUiSizeArray.Count - 1, UnitViewPositionEnum.Before));

            // call the handler if it exists
            if (CScrollViewScrolled != null) CScrollViewScrolled(this, val, _scrollPosition);

            // if the snapping is turned on, handle it
            if (snapping && !_snapJumping)
            {
                // if the speed has dropped below the threshhold velocity
                if (Mathf.Abs(LinearVelocity) <= snapVelocityThreshold && LinearVelocity != 0)
                {
                    // Call the snap function
                    Snap();
                }
            }

            _RefreshActive();
        }


        /// <summary>
        /// This is fired by the tweener when the snap tween is completed
        /// </summary>
        private void SnapJumpComplete()
        {
            // reset the snap jump to false and restore the inertia state
            _snapJumping        = false;
            _scrollRect.inertia = _snapInertia;
            CScrollUnitUi unitUi = null;
            for (var i = 0; i < _activeUnitViews.Count; i++)
            {
                if (_activeUnitViews[i].DataIndex == _snapDataIndex)
                {
                    unitUi = _activeUnitViews[i];
                    break;
                }
            }

            // fire the CScrollView snapped delegate
            if (CScrollViewSnapped != null) CScrollViewSnapped(this, _snapUnitViewIndex, _snapDataIndex, unitUi);
        }

        #endregion

        #region Tweening

        /// <summary>
        /// The easing type
        /// </summary>
        public enum TweenType
        {
            immediate,
            linear,
            spring,
            easeInQuad,
            easeOutQuad,
            easeInOutQuad,
            easeInCubic,
            easeOutCubic,
            easeInOutCubic,
            easeInQuart,
            easeOutQuart,
            easeInOutQuart,
            easeInQuint,
            easeOutQuint,
            easeInOutQuint,
            easeInSine,
            easeOutSine,
            easeInOutSine,
            easeInExpo,
            easeOutExpo,
            easeInOutExpo,
            easeInCirc,
            easeOutCirc,
            easeInOutCirc,
            easeInBounce,
            easeOutBounce,
            easeInOutBounce,
            easeInBack,
            easeOutBack,
            easeInOutBack,
            easeInElastic,
            easeOutElastic,
            easeInOutElastic
        }


        private float _tweenTimeLeft;


        /// <summary>
        /// Moves the scroll position over time between two points given an easing function. When the
        /// tween is complete it will fire the jumpComplete delegate.
        /// </summary>
        /// <param name="tweenType">The type of easing to use</param>
        /// <param name="time">The amount of time to interpolate</param>
        /// <param name="start">The starting scroll position</param>
        /// <param name="end">The ending scroll position</param>
        /// <param name="jumpComplete">The action to fire when the tween is complete</param>
        /// <returns></returns>
        IEnumerator TweenPosition(TweenType tweenType, float time, float start, float end, Action tweenComplete)
        {
            if (tweenType == TweenType.immediate || time == 0)
            {
                // if the easing is immediate or the time is zero, just jump to the end position
                ScrollPosition = end;
            }
            else
            {
                // zero out the velocity
                _scrollRect.velocity = Vector2.zero;

                // fire the delegate for the tween start
                IsTweening = true;
                if (CScrollViewTweeningChanged != null) CScrollViewTweeningChanged(this, true);
                _tweenTimeLeft = 0;
                var newPosition = 0f;

                // while the tween has time left, use an easing function
                while (_tweenTimeLeft < time)
                {
                    switch (tweenType)
                    {
                        case TweenType.linear:
                            newPosition = linear(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.spring:
                            newPosition = spring(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInQuad:
                            newPosition = easeInQuad(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeOutQuad:
                            newPosition = easeOutQuad(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInOutQuad:
                            newPosition = easeInOutQuad(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInCubic:
                            newPosition = easeInCubic(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeOutCubic:
                            newPosition = easeOutCubic(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInOutCubic:
                            newPosition = easeInOutCubic(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInQuart:
                            newPosition = easeInQuart(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeOutQuart:
                            newPosition = easeOutQuart(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInOutQuart:
                            newPosition = easeInOutQuart(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInQuint:
                            newPosition = easeInQuint(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeOutQuint:
                            newPosition = easeOutQuint(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInOutQuint:
                            newPosition = easeInOutQuint(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInSine:
                            newPosition = easeInSine(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeOutSine:
                            newPosition = easeOutSine(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInOutSine:
                            newPosition = easeInOutSine(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInExpo:
                            newPosition = easeInExpo(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeOutExpo:
                            newPosition = easeOutExpo(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInOutExpo:
                            newPosition = easeInOutExpo(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInCirc:
                            newPosition = easeInCirc(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeOutCirc:
                            newPosition = easeOutCirc(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInOutCirc:
                            newPosition = easeInOutCirc(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInBounce:
                            newPosition = easeInBounce(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeOutBounce:
                            newPosition = easeOutBounce(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInOutBounce:
                            newPosition = easeInOutBounce(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInBack:
                            newPosition = easeInBack(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeOutBack:
                            newPosition = easeOutBack(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInOutBack:
                            newPosition = easeInOutBack(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInElastic:
                            newPosition = easeInElastic(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeOutElastic:
                            newPosition = easeOutElastic(start, end, (_tweenTimeLeft / time));
                            break;
                        case TweenType.easeInOutElastic:
                            newPosition = easeInOutElastic(start, end, (_tweenTimeLeft / time));
                            break;
                    }

                    if (loop)
                    {
                        // if we are looping, we need to make sure the new position isn't past the jump trigger.
                        // if it is we need to reset back to the jump position on the other side of the area.
                        if (end > start && newPosition > _loopLastJumpTrigger)
                        {
                            //Debug.Log("name: " + name + " went past the last jump trigger, looping back around");
                            newPosition = _loopFirstScrollPosition + (newPosition - _loopLastJumpTrigger);
                        }
                        else if (start > end && newPosition < _loopFirstJumpTrigger)
                        {
                            //Debug.Log("name: " + name + " went past the first jump trigger, looping back around");
                            newPosition = _loopLastScrollPosition - (_loopFirstJumpTrigger - newPosition);
                        }
                    }

                    // set the scroll position to the tweened position
                    ScrollPosition = newPosition;

                    // increase the time elapsed
                    _tweenTimeLeft += Time.unscaledDeltaTime;
                    yield return null;
                }

                // the time has expired, so we make sure the final scroll position
                // is the actual end position.
                ScrollPosition = end;
            }

            // the tween jump is complete, so we fire the delegate
            if (tweenComplete != null) tweenComplete();

            // fire the delegate for the tween ending
            IsTweening = false;
            if (CScrollViewTweeningChanged != null) CScrollViewTweeningChanged(this, false);
        }


        private float linear(float start, float end, float val)
        {
            return Mathf.Lerp(start, end, val);
        }


        private static float spring(float start, float end, float val)
        {
            val = Mathf.Clamp01(val);
            val = (Mathf.Sin(val * Mathf.PI * (0.2f + 2.5f * val * val * val)) * Mathf.Pow(1f - val, 2.2f) + val)   * (1f + (1.2f * (1f - val)));
            return start + (end                                                                            - start) * val;
        }


        private static float easeInQuad(float start, float end, float val)
        {
            end -= start;
            return end * val * val + start;
        }


        private static float easeOutQuad(float start, float end, float val)
        {
            end -= start;
            return -end * val * (val - 2) + start;
        }


        private static float easeInOutQuad(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val + start;
            val--;
            return -end / 2 * (val * (val - 2) - 1) + start;
        }


        private static float easeInCubic(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val + start;
        }


        private static float easeOutCubic(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * (val * val * val + 1) + start;
        }


        private static float easeInOutCubic(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val * val + start;
            val -= 2;
            return end / 2 * (val * val * val + 2) + start;
        }


        private static float easeInQuart(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val * val + start;
        }


        private static float easeOutQuart(float start, float end, float val)
        {
            val--;
            end -= start;
            return -end * (val * val * val * val - 1) + start;
        }


        private static float easeInOutQuart(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val * val * val + start;
            val -= 2;
            return -end / 2 * (val * val * val * val - 2) + start;
        }


        private static float easeInQuint(float start, float end, float val)
        {
            end -= start;
            return end * val * val * val * val * val + start;
        }


        private static float easeOutQuint(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * (val * val * val * val * val + 1) + start;
        }


        private static float easeInOutQuint(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * val * val * val * val * val + start;
            val -= 2;
            return end / 2 * (val * val * val * val * val + 2) + start;
        }


        private static float easeInSine(float start, float end, float val)
        {
            end -= start;
            return -end * Mathf.Cos(val / 1 * (Mathf.PI / 2)) + end + start;
        }


        private static float easeOutSine(float start, float end, float val)
        {
            end -= start;
            return end * Mathf.Sin(val / 1 * (Mathf.PI / 2)) + start;
        }


        private static float easeInOutSine(float start, float end, float val)
        {
            end -= start;
            return -end / 2 * (Mathf.Cos(Mathf.PI * val / 1) - 1) + start;
        }


        private static float easeInExpo(float start, float end, float val)
        {
            end -= start;
            return end * Mathf.Pow(2, 10 * (val / 1 - 1)) + start;
        }


        private static float easeOutExpo(float start, float end, float val)
        {
            end -= start;
            return end * (-Mathf.Pow(2, -10 * val / 1) + 1) + start;
        }


        private static float easeInOutExpo(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return end / 2 * Mathf.Pow(2, 10 * (val - 1)) + start;
            val--;
            return end / 2 * (-Mathf.Pow(2, -10 * val) + 2) + start;
        }


        private static float easeInCirc(float start, float end, float val)
        {
            end -= start;
            return -end * (Mathf.Sqrt(1 - val * val) - 1) + start;
        }


        private static float easeOutCirc(float start, float end, float val)
        {
            val--;
            end -= start;
            return end * Mathf.Sqrt(1 - val * val) + start;
        }


        private static float easeInOutCirc(float start, float end, float val)
        {
            val /= .5f;
            end -= start;
            if (val < 1) return -end / 2 * (Mathf.Sqrt(1 - val * val) - 1) + start;
            val -= 2;
            return end / 2 * (Mathf.Sqrt(1 - val * val) + 1) + start;
        }


        private static float easeInBounce(float start, float end, float val)
        {
            end -= start;
            float d = 1f;
            return end - easeOutBounce(0, end, d - val) + start;
        }


        private static float easeOutBounce(float start, float end, float val)
        {
            val /= 1f;
            end -= start;
            if (val < (1 / 2.75f))
            {
                return end * (7.5625f * val * val) + start;
            }
            else if (val < (2 / 2.75f))
            {
                val -= (1.5f / 2.75f);
                return end * (7.5625f * (val) * val + .75f) + start;
            }
            else if (val < (2.5 / 2.75))
            {
                val -= (2.25f / 2.75f);
                return end * (7.5625f * (val) * val + .9375f) + start;
            }
            else
            {
                val -= (2.625f / 2.75f);
                return end * (7.5625f * (val) * val + .984375f) + start;
            }
        }


        private static float easeInOutBounce(float start, float end, float val)
        {
            end -= start;
            float d = 1f;
            if (val < d / 2) return easeInBounce(0, end, val * 2) * 0.5f       + start;
            else return easeOutBounce(0, end, val * 2 - d) * 0.5f + end * 0.5f + start;
        }


        private static float easeInBack(float start, float end, float val)
        {
            end -= start;
            val /= 1;
            float s = 1.70158f;
            return end * (val) * val * ((s + 1) * val - s) + start;
        }


        private static float easeOutBack(float start, float end, float val)
        {
            float s = 1.70158f;
            end -= start;
            val =  (val / 1) - 1;
            return end * ((val) * val * ((s + 1) * val + s) + 1) + start;
        }


        private static float easeInOutBack(float start, float end, float val)
        {
            float s = 1.70158f;
            end -= start;
            val /= .5f;
            if ((val) < 1)
            {
                s *= (1.525f);
                return end / 2 * (val * val * (((s) + 1) * val - s)) + start;
            }

            val -= 2;
            s   *= (1.525f);
            return end / 2 * ((val) * val * (((s) + 1) * val + s) + 2) + start;
        }


        private static float easeInElastic(float start, float end, float val)
        {
            end -= start;
            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;
            if (val == 0) return start;
            val = val / d;
            if (val == 1) return start + end;
            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            val = val - 1;
            return -(a * Mathf.Pow(2, 10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p)) + start;
        }


        private static float easeOutElastic(float start, float end, float val)
        {
            end -= start;
            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;
            if (val == 0) return start;
            val = val / d;
            if (val == 1) return start + end;
            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return (a * Mathf.Pow(2, -10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p) + end + start);
        }


        private static float easeInOutElastic(float start, float end, float val)
        {
            end -= start;
            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;
            if (val == 0) return start;
            val = val / (d / 2);
            if (val == 2) return start + end;
            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            if (val < 1)
            {
                val = val - 1;
                return -0.5f * (a * Mathf.Pow(2, 10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p)) + start;
            }

            val = val - 1;
            return a * Mathf.Pow(2, -10 * val) * Mathf.Sin((val * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
        }

        #endregion
    }
}