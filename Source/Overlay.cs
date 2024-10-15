using UnityEngine;
using Verse;
using RimWorld;
using System.Collections.Generic;

namespace MarketValueOverlay
{
    public class Overlay : ICellBoolGiver
    {
        private static readonly Color MinColor = Color.green;
        private static readonly Color AvgColor = Color.yellow;
        private static readonly Color MaxColor = Color.red;

        private Color _color;
        private CellBoolDrawer _drawer;
        private int _nextUpdateTick;
        private float _minValue, _maxValue, _avgValue; // Market values of buildings, min/max/avg of all buildings
        private Dictionary<int, float> _buildingValues = new Dictionary<int, float>(); // Market values of each building

        public Color Color => Color.white;
        
        /// <summary>
        /// Updates the overlay with the specified update delay.
        /// </summary>
        /// <param name="updateDelay">The delay in ticks between updates.</param>
        public void Update(int updateDelay)
        {
            if (!MarketValueOverlayMod.OverlayEnabled || !MarketValueOverlayMod.Settings.EnableMod) return;

            var map = Find.CurrentMap;
            if (map == null) return;

            _drawer ??= new CellBoolDrawer(this, map.Size.x, map.Size.z);

            _drawer.MarkForDraw();
            if (Find.TickManager.TicksGame >= _nextUpdateTick)
            {
                UpdateBuildingValues(map);
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
            var map = Find.CurrentMap;
            if (map == null || map.fogGrid == null || map.edificeGrid == null) return false; // Null checks
            if (index < 0 || index >= map.cellIndices.NumGridCells) return false; // Index checks
            if (map.fogGrid.IsFogged(index)) return false; // If the cell is fogged (ex: Ancient Danger)

            var building = map.edificeGrid[index] as Building;
            if (building == null || building.Faction == null || !building.Faction.IsPlayer) return false; // If the building is owned by a player

            if (_buildingValues.TryGetValue(building.thingIDNumber, out var marketValue))
            {
                _color = GetColorForValue(marketValue);
                return true;
            }

            return false;
        }
        
        public Color GetCellExtraColor(int index) => _color;
        
        private void UpdateBuildingValues(Map map)
        {
            _buildingValues.Clear();
            float sum = 0;
            var count = 0;
            _minValue = float.MaxValue;
            _maxValue = float.MinValue;

            if (map?.listerBuildings?.allBuildingsColonist == null) return;

            foreach (var building in map.listerBuildings.allBuildingsColonist)
            {
                if (building?.Faction == null || !building.Faction.IsPlayer) continue;
                
                var value = building.GetStatValue(StatDefOf.MarketValue);
                if (value > 0)
                {
                    _buildingValues[building.thingIDNumber] = value;
                    sum += value;
                    count++;
                    _minValue = Mathf.Min(_minValue, value);
                    _maxValue = Mathf.Max(_maxValue, value);
                }
            }

            _avgValue = count > 0 ? sum / count : 0;
            if (_minValue == float.MaxValue) _minValue = 0;
            if (_maxValue == float.MinValue) _maxValue = 2000;
            if (_avgValue == 0) _avgValue = 1000;
        }
        
        private Color GetColorForValue(float value)
        {
            if (value > _avgValue)
                return Color.Lerp(AvgColor, MaxColor, Mathf.InverseLerp(_avgValue, _maxValue, value));
            else
                return Color.Lerp(MinColor, AvgColor, Mathf.InverseLerp(_minValue, _avgValue, value));
        }
    }
}