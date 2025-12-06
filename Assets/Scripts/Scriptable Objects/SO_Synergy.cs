using UnityEngine;

[CreateAssetMenu(fileName = "New Synergy", menuName = "Synergy")]
public class SO_Synergy : ScriptableObject
{
    public SynergyTag tag;
    public string synergyName;
    public SynergyTier[] tiers;

    [System.Serializable]
    public class SynergyTier
    {
        public int requiredCount;
        [TextArea(2, 4)]
        public string description;
        public Stats statBonus;
        public float hpMultiplier = 1f;
        public float atkMultiplier = 1f;
        public float defMultiplier = 1f;
        public float spdMultiplier = 1f;
        public float ccMultiplier = 1f;
        public float cdMultiplier = 1f;
    }
}