using UnityEngine;

public class PlayerFighter : Fighter
{
    [SerializeField] private PlayerStats playerStats;
    
    private int gobelinGoldenCountDice = 0;
    
    public override IFighterStats stats => new PlayerStatsAdapter(playerStats);
    
    protected override void Awake()
    {
        base.Awake();
    }
    
    public override void Attack(Fighter target, int overrideDamage = -1)
    {        
        float critChance = CalculateCritChance();
        
        float attackDamage = playerStats.GetATK();
        
        // War Hero bonus damage (Human)
        attackDamage += playerStats.GetWarHeroBonusDamage();
        
        float critDamageMultiplier = playerStats.GetCD() / 100f;
        int dmg = overrideDamage > -1 ? overrideDamage : Mathf.RoundToInt(attackDamage);
        
        if (UnityEngine.Random.value < critChance)
        {
            dmg = Mathf.RoundToInt(dmg * critDamageMultiplier);
        }
        
        if (animator) animator.SetTrigger("Attack");
        target.TakeDamage(dmg);
        
        // Reflect damage (Sharp synergy)
        float reflectDmg = playerStats.GetReflectDamage();
        if (reflectDmg > 0)
        {
            target.TakeDamage(Mathf.RoundToInt(reflectDmg));
        }
    }
    
    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        
        // Elf revive (Nature synergy tier 3)
        if (playerStats.GetCurrentHP() <= 0 && playerStats.CanElfReviveOnce())
        {
            HealPercent(0.15f);
            playerStats.SetElfRevived(true);
            Debug.Log($"{fighterName} revived as Elf!");
        }
    }
    
    public void PostCombatHeal()
    {
        float healPercent = playerStats.GetPostCombatHealPercent();
        if (healPercent > 0)
        {
            HealPercent(healPercent);
        }
    }

    private float CalculateCritChance()
    {
        float critChance = playerStats.GetCC() / 100f;
        
        // Gobelin avec synergy Golden : crit garanti tous les 5 attacks
        if (playerStats.IsGobelinWithGolden())
        {
            Debug.Log("+1 with gobelin dice");
            gobelinGoldenCountDice++;
            if (gobelinGoldenCountDice >= 5)
            {
                Debug.Log("Gobelin Golden crit activated!");
                critChance = 1.0f;
                gobelinGoldenCountDice = 0;
            }
        }

        return critChance;
    }    
    // Adapter pour PlayerStats -> IFighterStats
    private class PlayerStatsAdapter : IFighterStats
    {
        private PlayerStats stats;
        
        public PlayerStatsAdapter(PlayerStats stats)
        {
            this.stats = stats;
        }
        
        public float GetMaxHP() => stats.GetMaxHP();
        public float GetCurrentHP() => stats.GetCurrentHP();
        public void SetCurrentHP(float value) => stats.SetCurrentHP(value);
        public float GetATK() => stats.GetATK();
        public float GetDEF() => stats.GetDEF();
        public float GetSPD() => stats.GetSPD();
        public float GetCC() => stats.GetCC();
        public float GetCD() => stats.GetCD();
    }
}