using UnityEngine;

namespace AirResistance
{
    [CreateAssetMenu(fileName = "Air Resistance Settings", menuName = "Air Resistance/Air Resistance Settings")]
    public class AirResistanceSettings : ScriptableObject
    {
        [Tooltip("Density of the air in the simulation, measured in kg/m^3")]
        public float airDensity = 1.225f;

        void OnValidate()
        {
            if (airDensity < 0) airDensity = 0;
        }

    }
}