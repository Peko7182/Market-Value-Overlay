using UnityEngine;
using Verse;

namespace MarketValueOverlay
{
    [StaticConstructorOnStartup]
    public static class Resources
    {
        // The toggle icon (Bottom right corner)
        public static readonly Texture2D Icon = ContentFinder<Texture2D>.Get("OverlayIcon");
    }
}