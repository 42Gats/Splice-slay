using System;
using System.Collections;
using UnityEngine;
using Spriter2UnityDX;


public abstract class Fighter : MonoBehaviour
{
    public string fighterName = "Fighter";
    
    public float damageReductionBuff = 0f;
    public float maxDamageReduction = 0.80f;
    
    public Animator animator;
    public Action OnDeath;
    
    [HideInInspector] public int shield = 0;
    
    protected Coroutine bleedCoroutine;
    
    // Interface pour accéder aux stats
    public abstract IFighterStats stats { get; }
    
    protected virtual void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }
    
    public virtual void TakeDamage(int dmg)
    {
        float reduced = dmg * (1f - Mathf.Clamp(damageReductionBuff, 0f, 1f));
        
        float defenseFactor = 100f / (100f + stats.GetDEF());
        int finalDamage = Mathf.RoundToInt(reduced * defenseFactor);
        
        int remaining = finalDamage;
        float currentHP = stats.GetCurrentHP();

        GetComponent<EntityRenderer>().PlayHurtAnimation();
        
        if (shield > 0)
        {
            int absorbed = Mathf.Min(shield, remaining);
            shield -= absorbed;
            remaining -= absorbed;
        }
        
        if (remaining > 0)
        {
            currentHP -= remaining;
            currentHP = Mathf.Max(0, currentHP);
            stats.SetCurrentHP(currentHP);
            Debug.Log($"{fighterName} took {finalDamage} damage, reduced to {remaining} after shield. Current HP: {currentHP}");
        }
        
        
        if (currentHP <= 0)
        {
            stats.SetCurrentHP(0);
            OnDeath?.Invoke();
        }
    }
    
    public void AddShield(int amount)
    {
        shield += amount;
    }
    
    public void ResetDamageReduction()
    {
        damageReductionBuff = 0f;
    }
    
    public void AddDamageReduction(float value)
    {
        damageReductionBuff = Mathf.Clamp(
            damageReductionBuff + value,
            0f,
            maxDamageReduction
        );
    }
    
    public void HealPercent(float percent)
    {
        float maxHP = stats.GetMaxHP();
        float currentHP = stats.GetCurrentHP();
        
        int heal = Mathf.RoundToInt(maxHP * percent);
        currentHP = Mathf.Min(currentHP + heal, maxHP);
        stats.SetCurrentHP(currentHP);
    }
    
    public void HealFlat(int amount)
    {
        float maxHP = stats.GetMaxHP();
        float currentHP = stats.GetCurrentHP();
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        stats.SetCurrentHP(currentHP);
    }

    public bool isActionFinished()
    {
        if (animator == null) return true;
        if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle")) return true;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime >= 1f;
    }
    
    public virtual void Attack(Fighter target, int overrideDamage = -1)
    {        
        float critChance = stats.GetCC() / 100f;
        float attackDamage = stats.GetATK();
        Debug.Log($"{fighterName} attacking {target.fighterName} with base ATK {attackDamage}, CC {critChance*100}%, CD {stats.GetCD()}%");
        float critDamageMultiplier = stats.GetCD() / 100f;
        
        int dmg = (overrideDamage > -1 ? overrideDamage : Mathf.RoundToInt(attackDamage));

        Debug.Log("Changing animation to ATTACK");

        GetComponent<EntityRenderer>().PlaySlashingAnimation();

        if (UnityEngine.Random.value < critChance)
        {
            dmg = Mathf.RoundToInt(dmg * critDamageMultiplier);
        }
        
        target.TakeDamage(dmg);
    }
    
    public void ApplyBleed(float percent, int ticks, float interval)
    {
        if (bleedCoroutine != null) StopCoroutine(bleedCoroutine);
        bleedCoroutine = StartCoroutine(BleedCoroutine(percent, ticks, interval));
    }
    
    protected IEnumerator BleedCoroutine(float percent, int ticks, float interval)
    {
        for (int i = 0; i < ticks; i++)
        {
            float maxHP = stats.GetMaxHP();
            int dmg = Mathf.RoundToInt(maxHP * percent);
            TakeDamage(dmg);
            yield return new WaitForSeconds(interval);
        }
        bleedCoroutine = null;
    }
}