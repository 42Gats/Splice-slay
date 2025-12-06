using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Configuration")]
    public SO_Race currentRace;
    public SO_Class currentClass;
    
    [Header("Character Body")]
    [SerializeField] private SO_CharacterBody characterBody;

    [Header("Base Stats")]
    public Stats baseStats = new Stats
    {
        HP = 100,
        ATK = 10,
        SPD = 10,
        DEF = 3,
        CC = 5,
        CD = 150
    };

    [Header("Synergy Database")]
    [SerializeField] private List<SO_Synergy> allSynergies;

    private Stats finalStats;
    private Dictionary<SynergyTag, int> activeSynergies = new Dictionary<SynergyTag, int>();
    private List<SO_Synergy.SynergyTier> activeTiers = new List<SO_Synergy.SynergyTier>();

    public System.Action OnStatsChanged;

    private void Awake()
    {
        activeSynergies = new Dictionary<SynergyTag, int>();
        finalStats = baseStats;
    }

    private void Start()
    {
        CalculateStats();
    }

    public void SetRace(SO_Race race)
    {
        if (race == null) return;
        
        currentRace = race;
        CalculateStats();
        
        Debug.Log($"Race changed to: {race.raceName}");
    }

    public void SetClass(SO_Class playerClass)
    {
        if (playerClass == null) return;
        
        currentClass = playerClass;
        
        Debug.Log($"Class changed to: {playerClass.className}");
    }

    public void OnEquipmentChanged()
    {
        CalculateStats();
    }

    public void CalculateStats()
    {
        CountSynergies();
        finalStats = baseStats;
        Stats equipmentStats = GetEquipmentStats();

        if (HasSynergyTier(SynergyTag.ROYALTY, 3))
        {
            equipmentStats *= 1.3f;
        }

        finalStats += equipmentStats;

        ApplySynergies();
        ApplyRaceBonus();

        OnStatsChanged?.Invoke();
    }

    private void CountSynergies()
    {
        activeSynergies.Clear();
        activeTiers.Clear();

        if (currentRace != null)
        {
            AddSynergyCount(currentRace.synergyTag, 2);
        }

        if (characterBody != null && characterBody.characterBodyParts != null)
        {
            foreach (var bodyPart in characterBody.characterBodyParts)
            {
                if (bodyPart.bodyPart != null && bodyPart.bodyPart.synergyTags != null)
                {
                    foreach (var tag in bodyPart.bodyPart.synergyTags)
                    {
                        AddSynergyCount(tag, 1);
                    }
                }
            }
        }

        DetermineActiveTiers();
    }

    private void AddSynergyCount(SynergyTag tag, int amount)
    {
        if (!activeSynergies.ContainsKey(tag))
        {
            activeSynergies[tag] = 0;
        }
        activeSynergies[tag] += amount;
    }

    private void DetermineActiveTiers()
    {
        if (allSynergies == null) return;

        foreach (var synergy in allSynergies)
        {
            if (activeSynergies.ContainsKey(synergy.tag))
            {
                int count = activeSynergies[synergy.tag];
                
                var activeTier = synergy.tiers
                    .Where(t => count >= t.requiredCount)
                    .OrderByDescending(t => t.requiredCount)
                    .FirstOrDefault();

                if (activeTier != null)
                {
                    activeTiers.Add(activeTier);
                }
            }
        }
    }

    private Stats GetEquipmentStats()
    {
        Stats total = new Stats();

        if (characterBody != null && characterBody.characterBodyParts != null)
        {
            foreach (var bodyPart in characterBody.characterBodyParts)
            {
                if (bodyPart.bodyPart != null)
                {
                    total += bodyPart.bodyPart.stats;
                }
            }
        }

        return total;
    }

    private void ApplySynergies()
    {
        foreach (var tier in activeTiers)
        {
            finalStats += tier.statBonus;

            finalStats.HP *= tier.hpMultiplier;
            finalStats.ATK *= tier.atkMultiplier;
            finalStats.DEF *= tier.defMultiplier;
            finalStats.SPD *= tier.spdMultiplier;
            finalStats.CC *= tier.ccMultiplier;
            finalStats.CD *= tier.cdMultiplier;
        }
    }

    private void ApplyRaceBonus()
    {
        if (currentRace == null) return;

        switch (currentRace.race)
        {
            case Race.DEMON:
                if (HasSynergy(SynergyTag.DEMONIC))
                {
                    float hpLossReduction = 0.4f;
                    float demonicPenalty = 0f;
                    
                    if (HasSynergyTier(SynergyTag.DEMONIC, 6)) demonicPenalty = 0.40f;
                    else if (HasSynergyTier(SynergyTag.DEMONIC, 4)) demonicPenalty = 0.30f;
                    else if (HasSynergyTier(SynergyTag.DEMONIC, 2)) demonicPenalty = 0.20f;

                    float baseHp = baseStats.HP + GetEquipmentStats().HP;
                    float hpToRestore = baseHp * demonicPenalty * hpLossReduction;
                    finalStats.HP += hpToRestore;
                }
                break;
        }
    }

    
    public Stats GetFinalStats() => finalStats;
    public float GetMaxHP() => finalStats.HP;
    public float GetATK() => finalStats.ATK;
    public float GetDEF() => finalStats.DEF;
    public float GetSPD() => finalStats.SPD;
    public float GetCC() => finalStats.CC;
    public float GetCD() => finalStats.CD;

    public Dictionary<SynergyTag, int> GetActiveSynergies() => activeSynergies;
    public List<SO_Synergy.SynergyTier> GetActiveTiers() => activeTiers;
    
    public bool IsGobelinWithGolden()
    {
        return currentRace != null && 
               currentRace.race == Race.GOBELIN && 
               HasSynergy(SynergyTag.GOLDEN);
    }

    public bool CanElfReviveOnce()
    {
        return currentRace != null && 
               currentRace.race == Race.ELF && 
               HasSynergyTier(SynergyTag.NATURE, 3);
    }

    public float GetReflectDamage()
    {
        if (HasSynergyTier(SynergyTag.SHARP, 3)) return 10f;
        if (HasSynergyTier(SynergyTag.SHARP, 2)) return 5f;
        return 0f;
    }

    public float GetWarHeroBonusDamage()
    {
        if (currentRace != null && 
            currentRace.race == Race.HUMAN && 
            HasSynergy(SynergyTag.WAR_HERO))
        {
            return finalStats.HP * 0.02f;
        }
        return 0f;
    }

    public float GetHealMultiplier()
    {
        if (HasSynergyTier(SynergyTag.NATURE, 6)) return 2.0f;
        if (HasSynergyTier(SynergyTag.NATURE, 3)) return 1.5f;
        return 1.0f;
    }

    public float GetPostCombatHealPercent()
    {
        if (HasSynergyTier(SynergyTag.NATURE, 6)) return 0.20f;
        if (HasSynergyTier(SynergyTag.NATURE, 3)) return 0.10f;
        return 0f;
    }
    
    private bool HasSynergy(SynergyTag tag)
    {
        return activeSynergies.ContainsKey(tag) && activeSynergies[tag] > 0;
    }

    private bool HasSynergyTier(SynergyTag tag, int requiredCount)
    {
        return activeSynergies.ContainsKey(tag) && activeSynergies[tag] >= requiredCount;
    }

    public int GetSynergyCount(SynergyTag tag)
    {
        return activeSynergies.ContainsKey(tag) ? activeSynergies[tag] : 0;
    }
}