using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public Fighter player;
    public Fighter enemy;

    public enum CombatState { PlayerTurn, EnemyTurn, Busy }
    public CombatState state = CombatState.PlayerTurn;

    public DiceRoller diceRoller;

    private bool quickStrikeUsedThisTurn = false;

    [SerializeField] private GameObject diceResult;
    [SerializeField] private GameObject WinMenu;
    [SerializeField] private GameObject LooseMenu;




    void Start()
    {
        state = CombatState.PlayerTurn;
        player = GameObject.Find("Player").GetComponent<Fighter>();
        GameObject.Find("PlayerHPBar").GetComponent<HealthBar>().target = player;
        GameObject.Find("PlayerHPBar").GetComponent<HealthBar>().Reset();
        WinMenu.SetActive(false);
        LooseMenu.SetActive(false);
    }

    [ContextMenu("TEST PlayerTurn")]
    public void PlayerTurnAction()
    {
        if (state != CombatState.PlayerTurn) return;
        StartCoroutine(PlayerTurnRoutine());
    }

    IEnumerator PlayerTurnRoutine()
    {
        state = CombatState.Busy;

        DiceFace[] results = null;
        yield return StartCoroutine(diceRoller.RollDiceWithAnimation(r => results = r));

        Debug.Log("Roll results:");
        foreach (var r in results)
            Debug.Log($" - {r.type}");


        ApplyDiceResults(results);

        // Brief pause before enemy turn
        yield return new WaitForSeconds(1.0f);
        diceRoller.SetDiceResultVisibility(false);
        diceRoller.SetDiceEnabled(false);

        if (enemy.stats.GetCurrentHP() <= 0)
        {
            EndCombat(true);
            yield break;
        }

        state = CombatState.EnemyTurn;
        StartCoroutine(EnemyAttackRoutine());
    }

    IEnumerator EnemyAttackRoutine()
    {
        // Timer to simulate enemy thinking/animation
        float enemyAnimationTime = 1.5f;
        yield return new WaitForSeconds(enemyAnimationTime);

        enemy.Attack(player);

        if (player.stats.GetCurrentHP() <= 0)
        {
            EndCombat(false);
            yield break;
        }
        
        // Brief pause before returning to player turn
        // yield return new WaitForSeconds(0.5f);

        state = CombatState.PlayerTurn;
    }

    void EndCombat(bool playerWon)
    {
        Debug.Log("Combat termin�. Player won: " + playerWon);

        if (playerWon)
        {
            WinMenu.SetActive(true);
        } else
        {
            LooseMenu.SetActive(true);
        }

    }

    void ApplyDiceResults(DiceFace[] results)
    {
        Dictionary<DiceFaceType, int> counts = new Dictionary<DiceFaceType, int>();
        Dictionary<DiceFaceType, DiceFace> exampleFace = new Dictionary<DiceFaceType, DiceFace>();

        foreach (var f in results)
        {
            if (f == null) continue;
            if (!counts.ContainsKey(f.type)) { counts[f.type] = 0; exampleFace[f.type] = f; }
            counts[f.type] += 1;
        }

        Debug.Log("Applying faces:");
        foreach (var kv in counts)
        {
            DiceFaceType type = kv.Key;
            int tier = kv.Value;
            DiceFace face = exampleFace[type];

            Debug.Log($" - Action: {type}, Tier: {tier}");

            ApplyFaceEffect(face, tier);
        }
    }

    void ApplyFaceEffect(DiceFace face, int tier)
    {
        switch (face.type)
        {
            case DiceFaceType.Attack:
                ApplyAttack(tier);
                break;

            case DiceFaceType.HeavyAttack:
                ApplyHeavyAttack(tier);
                break;

            case DiceFaceType.Shield:
                ApplyShield(tier);
                break;

            case DiceFaceType.ShieldBash:
                ApplyShieldBash(tier);
                break;

            case DiceFaceType.Bleed:
                ApplyBleed(tier);
                break;

            case DiceFaceType.QuickStrike:
                ApplyQuickStrike(tier);
                break;

            case DiceFaceType.ShadowStep:
                ApplyShadowStep(tier);
                break;

            case DiceFaceType.Heal:
                ApplyHeal(tier);
                break;

            case DiceFaceType.Overgrowth:
                ApplyOvergrowth(tier);
                break;

            case DiceFaceType.LifeDrain:
                ApplyLifeDrain(tier);
                break;

            case DiceFaceType.BloodSacrifice:
                ApplyBloodSacrifice(tier);
                break;

            default:
                Debug.LogWarning("Face non g�r�e: " + face.type);
                break;
        }
    }

    void ApplyAttack(int tier)
    {
        Debug.Log($"Applying ATTACK tier {tier}");
        float[] mul = { 1f, 1f, 1.5f, 3f };
        int dmg = Mathf.RoundToInt(player.stats.GetATK() * mul[Mathf.Clamp(tier, 1, 3)]);
        player.Attack(enemy, dmg);
    }

    void ApplyHeavyAttack(int tier)
    {
        Debug.Log($"Applying HEAVY ATTACK tier {tier}");
        float[] mul = { 1f, 1.5f, 3f, 5f };
        int dmg = Mathf.RoundToInt(player.stats.GetATK() * mul[Mathf.Clamp(tier, 1, 3)]);
        player.Attack(enemy, dmg);
    }

    void ApplyShield(int tier)
    {
        Debug.Log($"Applying SHIELD tier {tier}");
        float[] reduction = { 0f, 0.10f, 0.50f, 1.00f };
        player.AddDamageReduction(reduction[tier]);
        Debug.Log($"reduction: {reduction}");
    }

    void ApplyShieldBash(int tier)
    {
        Debug.Log($"Applying SHIELD BASH tier {tier}");
        float[] reduce = { 0f, 0.05f, 0.25f, 0.50f };
        float[] dmgMul = { 1f, 0.5f, 0.75f, 1.5f };

        player.AddDamageReduction(reduce[tier]);

        int dmg = Mathf.RoundToInt(player.stats.GetATK() * dmgMul[tier]);
        player.Attack(enemy, dmg);
    }

    void ApplyBleed(int tier)
    {
        Debug.Log($"Applying BLEED tier {tier}");
        float[] dmgMul = { 1f, 0.8f, 1.2f, 2.0f };
        float[] bleedPct = { 0f, 0.03f, 0.05f, 0.08f };
        int dmg = Mathf.RoundToInt(player.stats.GetATK() * dmgMul[Mathf.Clamp(tier, 1, 3)]);
        player.Attack(enemy, dmg);

        enemy.ApplyBleed(bleedPct[Mathf.Clamp(tier, 1, 3)], 3, 1f);
    }

    void ApplyQuickStrike(int tier)
    {
        Debug.Log($"Applying QUICK STRIKE tier {tier}");
        float[] dmgMul = { 1f, 0.7f, 1.2f, 1.8f };
        int dmg = Mathf.RoundToInt(player.stats.GetATK() * dmgMul[tier]);
        player.Attack(enemy, dmg);

        if (!quickStrikeUsedThisTurn)
        {
            Debug.Log("QuickStrike triggered an extra roll:");
            quickStrikeUsedThisTurn = true;

            StartCoroutine(RollExtraDice());
        }
        else
        {
            Debug.Log("QuickStrike ignoré : déjà utilisé ce tour.");
        }
    }

    IEnumerator RollExtraDice()
    {
        DiceFace[] extra = null;
        yield return StartCoroutine(diceRoller.RollDiceWithAnimation(r => extra = r));
        ApplyDiceResults(extra);
    }

    void ApplyShadowStep(int tier)
    {
        Debug.Log($"Applying SHADOW STEP tier {tier}");
    }

    void ApplyHeal(int tier)
    {
        Debug.Log($"Applying HEAL tier {tier}");
        float[] pct = { 0f, 0.10f, 0.20f, 0.35f };
        player.HealPercent(pct[Mathf.Clamp(tier, 1, 3)]);
    }

    void ApplyOvergrowth(int tier)
    {
        Debug.Log($"Applying OVERGROWTH tier {tier}");
        float[] pct = { 0f, 0.10f, 0.25f, 0.50f };
        int shieldAmount = Mathf.RoundToInt(player.stats.GetMaxHP() * pct[Mathf.Clamp(tier, 1, 3)]);
        player.AddShield(shieldAmount);
    }

    void ApplyLifeDrain(int tier)
    {
        Debug.Log($"Applying LIFE DRAIN tier {tier}");
        float[] dmgMul = { 1f, 0.90f, 1.50f, 2.50f };
        float[] healPct = { 0f, 0.30f, 0.50f, 1.00f };
        int dmg = Mathf.RoundToInt(player.stats.GetATK() * dmgMul[Mathf.Clamp(tier, 1, 3)]);

        int enemyHPBefore = (int)enemy.stats.GetCurrentHP();
        enemy.TakeDamage(dmg);
        int actualDealt = enemyHPBefore - (int)enemy.stats.GetCurrentHP();
        int healAmount = Mathf.RoundToInt(actualDealt * healPct[Mathf.Clamp(tier, 1, 3)]);
        player.HealFlat(healAmount);
    }

    void ApplyBloodSacrifice(int tier)
    {
        Debug.Log($"Applying BLOOD SACRIFICE tier {tier}");
        float[] sacrificePct = { 0f, 0.05f, 0.10f, 0.15f };
        float[] dmgMul = { 1f, 2.00f, 4.00f, 7.00f };

        int lost = Mathf.RoundToInt(player.stats.GetMaxHP() * sacrificePct[Mathf.Clamp(tier, 1, 3)]);
        player.TakeDamage(lost);

        int dmg = Mathf.RoundToInt(player.stats.GetATK() * dmgMul[Mathf.Clamp(tier, 1, 3)]);
        if (tier >= 3) dmg *= 2;
        player.Attack(enemy, dmg);
    }
}
