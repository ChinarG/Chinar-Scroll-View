using UnityEngine;
using System.Collections;

namespace EnhancedCScrollViewDemos.SnappingDemo
{
    /// <summary>
    /// This class represents a slot unit
    /// </summary>
    public class SlotData
    {
        /// <summary>
        /// The preloaded sprite for the slot unit. 
        /// We could have loaded the sprite while scrolling,
        /// but since there are so few slot unit types, we'll
        /// just preload them to speed up the in-game processing.
        /// </summary>
        public Sprite sprite;
    }
}