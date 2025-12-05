// Assets/Scripts/Fighter.cs
using System;
using System.Collections;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public string fighterName = "Fighter";
    public int maxHP = 100;
    public int currentHP;
    public int attackDamage = 10;

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
        int remaining = dmg;

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

        if (animator != null) animator.SetTrigger("Hit");

        if (currentHP <= 0)
        {
            currentHP = 0;
            if (animator != null) animator.SetTrigger("Death");
            OnDeath?.Invoke();
        }
    }

    public void AddShield(int amount)
    {
        shield += amount;
    }

    public void HealPercent(float percent)
    {
        int heal = Mathf.RoundToInt(maxHP * percent);
        currentHP += heal;
        currentHP = Mathf.Min(currentHP, maxHP);
    }

    public void HealFlat(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
    }

    public void Attack(Fighter target, int damageOverride = -1)
    {
        if (animator != null) animator.SetTrigger("Attack");
        int dmg = (damageOverride > -1) ? damageOverride : attackDamage;
        target.TakeDamage(dmg);
    }

    public void ApplyBleed(float percentOfMaxPerTick, int ticks, float tickIntervalSeconds = 1f)
    {
        if (bleedCoroutine != null) StopCoroutine(bleedCoroutine);
        bleedCoroutine = StartCoroutine(BleedCoroutine(percentOfMaxPerTick, ticks, tickIntervalSeconds));
    }

    private IEnumerator BleedCoroutine(float percentOfMaxPerTick, int ticks, float tickInterval)
    {
        for (int i = 0; i < ticks; i++)
        {
            int dmg = Mathf.RoundToInt(maxHP * percentOfMaxPerTick);
            TakeDamage(dmg);
            yield return new WaitForSeconds(tickInterval);
        }
        bleedCoroutine = null;
    }
}
