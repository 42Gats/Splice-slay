using UnityEngine;

public enum DiceFaceType
{
    Shield,
    Attack,
    ShieldBash,
    HeavyAttack,
    Heal,
    Overgrowth,
    Bleed,
    QuickStrike,
    ShadowStep,
    LifeDrain,
    BloodSacrifice
}

[CreateAssetMenu(fileName = "NewDiceFace", menuName = "Dice/Dice Face")]
public class DiceFace : ScriptableObject
{
    public DiceFaceType type;

    [Header("Values for 1 / 2 / 3 matches")]
    public float valueT1;
    public float valueT2;
    public float valueT3;
}
