using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

// Created with RW Mod Structure Builder
namespace MarketValueOverlay
{
    [StaticConstructorOnStartup]
    public static class MarketValueOverlay
    {
        static MarketValueOverlay()
        {
            Log.Message($"[Market Value Overlay v0.0.0.1] Initialized");

            var harmony = new Harmony("com.peko.rimworld.mod.MarketValueOverlay");
            harmony.PatchAll();
        }
    }

    /// <summary>
    /// Add the toggle button for the Market Value Overlay in the Global Controls.
    /// </summary>
    [HarmonyPatch(typeof(PlaySettings), nameof(PlaySettings.DoPlaySettingsGlobalControls))]
    public static class AddTogglePatch
    {
        [HarmonyPostfix]
        public static void PostFix(WidgetRow row, bool worldView)
        {
            if (worldView)
                return;

            if (row == null || Resources.Icon == null)
                return;

            row.ToggleableIcon(ref MarketValueOverlayMod.OverlayEnabled, Resources.Icon, "Toggle the Market Value Overlay. Shows the Market Value of buildings.\n\nRed - High Market Value\nGreen - Low Market Value", SoundDefOf.Mouseover_ButtonToggle);
        }
    }
    
    /// <summary>
    /// Update the Market Value Overlay every 60 frames.
    /// </summary>
    [HarmonyPatch(typeof(MapInterface), nameof(MapInterface.MapInterfaceUpdate))]
    public static class MapInterfaceMapInterfaceUpdateDetour
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!MarketValueOverlayMod.Settings.EnableMod || Find.CurrentMap == null || WorldRendererUtility.WorldRenderedNow)
                return;

            MarketValueOverlayMod.Overlay.Update(60);
        }
    }
    
    /// <summary>
    /// Reset the Market Value Overlay when the map changes.
    /// </summary>
    [HarmonyPatch(typeof(MapInterface), nameof(MapInterface.Notify_SwitchedMap))]
    internal static class MapInterfaceNotifySwitchedMapDetour
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            MarketValueOverlayMod.Overlay = new Overlay();
        }
    }
}