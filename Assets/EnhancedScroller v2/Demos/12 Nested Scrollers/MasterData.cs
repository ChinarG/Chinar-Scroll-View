using System.Collections.Generic;

namespace EnhancedCScrollViewDemos.NestedCScrollViews
{
    /// <summary>
    /// Main unit view data
    /// </summary>
    public class MasterData
    {
        // This value will store the position of the detail CScrollView to be used 
        // when the CScrollView's unit view is recycled
        public float normalizedScrollPosition;

        public List<DetailData> childData;
    }
}
