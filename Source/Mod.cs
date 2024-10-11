using UnityEngine;
using Verse;

namespace MarketValueOverlay
{
    public class MarketValueOverlayMod : Mod
    {
        // Reference to the settings
        public static MarketValueOverlaySettings Settings;
        
        // Variables
        public static bool OverlayEnabled;
        public static Overlay Overlay;

        public MarketValueOverlayMod(ModContentPack content) : base(content)
        {
            // Initialize the settings
            Settings = GetSettings<MarketValueOverlaySettings>();
            
            // Initialize the variables
            OverlayEnabled = false;
            Overlay = new Overlay();
        }

        // This is where the settings window UI is drawn
        public override void DoSettingsWindowContents(Rect inRect)
        {
            // Create a listing for UI elements
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);

            // Checkboxes for the settings
            listingStandard.CheckboxLabeled("Enable Mod", ref Settings.EnableMod, "Enables the mod. Won't remove the toggle button.");

            listingStandard.End();
        }

        // The name displayed in the mod settings menu
        public override string SettingsCategory()
        {
            return "MarketValueOverlay";
        }
    }
}