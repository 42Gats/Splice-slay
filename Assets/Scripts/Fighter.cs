using System;
using System.Collections;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public string fighterName = "Fighter";

    public int maxHP = 100;
    public int currentHP;
    public int attackDamage = 10;

    public float defense = 0f;
    public float critChance = 0.1f;
    public float critDamageMultiplier = 2f;

    public float damageReductionBuff = 0f;
    public float maxDamageReduction = 0.80f;

    public Animator animator;
    public Action OnDeath;

    [HideInInspector] public int shield = 0;

    private Coroutine bleedCoroutine;

    void Awake()
    {
        currentHP = maxHP;
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void TakeDamage(int dmg)
    {
        float reduced = dmg * (1f - Mathf.Clamp(damageReductionBuff, 0f, 1f));

        float defenseFactor = 100f / (100f + defense);
        int finalDamage = Mathf.RoundToInt(reduced * defenseFactor);

        int remaining = finalDamage;

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
        }

        if (animator) animator.SetTrigger("Hit");

        if (currentHP <= 0)
        {
            currentHP = 0;
            if (animator) animator.SetTrigger("Death");
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
        int heal = Mathf.RoundToInt(maxHP * percent);
        currentHP = Mathf.Min(currentHP + heal, maxHP);
    }

    public void HealFlat(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
    }

    public void Attack(Fighter target, int overrideDamage = -1)
    {
        int dmg = (overrideDamage > -1 ? overrideDamage : attackDamage);

        if (UnityEngine.Random.value < critChance)
        {
            dmg = Mathf.RoundToInt(dmg * critDamageMultiplier);
        }

        if (animator) animator.SetTrigger("Attack");
        target.TakeDamage(dmg);
    }

    public void ApplyBleed(float percent, int ticks, float interval)
    {
        if (bleedCoroutine != null) StopCoroutine(bleedCoroutine);
        bleedCoroutine = StartCoroutine(BleedCoroutine(percent, ticks, interval));
    }

    IEnumerator BleedCoroutine(float percent, int ticks, float interval)
    {
        for (int i = 0; i < ticks; i++)
        {
            int dmg = Mathf.RoundToInt(maxHP * percent);
            TakeDamage(dmg);
            yield return new WaitForSeconds(interval);
        }
        bleedCoroutine = null;
    }
}
