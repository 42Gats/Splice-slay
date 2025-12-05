using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Configuration")]
    public SO_Race currentRace;
    public SO_Class currentClass;

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
}