using UnityEngine;
using Verse;
using RimWorld;
using System.Linq;

namespace MarketValueOverlay
{
    /// <summary>
    /// Overlay that displays buildings with market value on the map.
    /// </summary>
    public class Overlay : ICellBoolGiver
    {
        private static readonly Color MinColor = Color.green;
        private static readonly Color AvgColor = Color.yellow;
        private static readonly Color MaxColor = Color.red;

        private Color _color;
        private CellBoolDrawer _drawer;
        private int _nextUpdateTick;
        private float _minValue, _maxValue, _avgValue;

        /// <summary>
        /// Dummy color for the overlay.
        /// </summary>
        public Color Color => Color.white;

        /// <summary>
        /// Updates the overlay with the specified update delay.
        /// </summary>
        /// <param name="updateDelay">The delay in ticks between updates.</param>
        public void Update(int updateDelay)
        {
            if (!MarketValueOverlayMod.OverlayEnabled || !MarketValueOverlayMod.Settings.EnableMod) return;

            _drawer ??= new CellBoolDrawer(this, Find.CurrentMap.Size.x, Find.CurrentMap.Size.z);

            _drawer.MarkForDraw();
            if (Find.TickManager.TicksGame >= _nextUpdateTick)
            {
                UpdateMinMaxValues();
                _drawer.SetDirty();
                _nextUpdateTick = Find.TickManager.TicksGame + updateDelay;
            }
            _drawer.CellBoolDrawerUpdate();
        }

        /// <summary>
        /// Gets a boolean value indicating whether the cell at the specified index should be drawn.
        /// </summary>
        /// <param name="index">The index of the cell to check.</param>
        /// <returns><c>true</c> if the cell should be drawn, <c>false</c> otherwise.</returns>
        public bool GetCellBool(int index)
        {
            try
            {
                var map = Find.CurrentMap;
                if (map == null) return false;
                if (map.fogGrid.IsFogged(index)) return false;

                var building = map.edificeGrid?[index] as Building;

                if (building == null) return false;
                if (!building.Faction.IsPlayer) return false;

                var marketValue = building.GetStatValue(StatDefOf.MarketValue);
                if (marketValue > _avgValue)
                    _color = Color.Lerp(AvgColor, MaxColor, Mathf.InverseLerp(_avgValue, _maxValue, marketValue));
                else
                    _color = Color.Lerp(MinColor, AvgColor, Mathf.InverseLerp(_minValue, _avgValue, marketValue));

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the extra color for the specified cell.
        /// </summary>
        /// <param name="index">The index of the cell to check.</param>
        /// <returns>The extra color for the cell.</returns>
        public Color GetCellExtraColor(int index) => _color;

        /// <summary>
        /// Updates the minimum, average and maximum values of market value.
        /// </summary>
        private void UpdateMinMaxValues()
        {
            var buildings = Find.CurrentMap.listerBuildings.allBuildingsColonist
                .Where(b => b.Faction.IsPlayer && b.GetStatValue(StatDefOf.MarketValue) > 0).ToList();
            if (buildings.Count > 0)
            {
                _minValue = buildings.Min(b => b.GetStatValue(StatDefOf.MarketValue));
                _maxValue = buildings.Max(b => b.GetStatValue(StatDefOf.MarketValue));
                _avgValue = buildings.Average(b => b.GetStatValue(StatDefOf.MarketValue));
            }
            else
            {
                _minValue = 0;
                _maxValue = 2000;
                _avgValue = 1000;
            }
        }
    }
}