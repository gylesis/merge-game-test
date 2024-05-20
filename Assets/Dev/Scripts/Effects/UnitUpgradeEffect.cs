using UnityEngine;

namespace Dev.Effects
{
    public class UnitUpgradeEffect : Effect
    {
        public void SetParticlesColor(Color color)
        {
            ParticleSystem.MainModule settings = GetComponent<ParticleSystem>().main;

            settings.startColor = new ParticleSystem.MinMaxGradient(color);
        }
    }
}