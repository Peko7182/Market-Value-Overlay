using Verse;

namespace MarketValueOverlay
{
    public class MarketValueOverlaySettings : ModSettings
    {
        // Setting to enable/disable mod
        public bool EnableMod = true;

        // Method to save and load the settings
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref EnableMod, "enableMod", true);
        }
    }
}