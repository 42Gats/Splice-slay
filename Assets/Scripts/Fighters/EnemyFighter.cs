using UnityEngine;

[System.Serializable]
public class EnemyStats : IFighterStats
{
    public float maxHP = 100f;
    public float currentHP = 100f;
    public float ATK = 10f;
    public float DEF = 5f;
    public float SPD = 10f;
    public float CC = 5f;
    public float CD = 150f;
    
    public float GetMaxHP() => maxHP;
    public float GetCurrentHP() => currentHP;
    public void SetCurrentHP(float value) => currentHP = value;
    public float GetATK() => ATK;
    public float GetDEF() => DEF;
    public float GetSPD() => SPD;
    public float GetCC() => CC;
    public float GetCD() => CD;
}

public class EnemyFighter : Fighter
{
    [SerializeField] private EnemyStats enemyStats = new EnemyStats();
    
    public override IFighterStats stats => enemyStats;
    
    protected override void Awake()
    {
        base.Awake();
        enemyStats.currentHP = enemyStats.maxHP;
    }
    
    // Vous pouvez ajouter des comportements spécifiques aux ennemis ici
    public void SetStats(float hp, float atk, float def, float spd, float cc, float cd)
    {
        enemyStats.maxHP = hp;
        enemyStats.currentHP = hp;
        enemyStats.ATK = atk;
        enemyStats.DEF = def;
        enemyStats.SPD = spd;
        enemyStats.CC = cc;
        enemyStats.CD = cd;
    }
}
