[System.Serializable]
public struct Stats
{
    public float HP;
    public float ATK;
    public float SPD;
    public float DEF;
    public float CC;
    public float CD;

    public static Stats operator +(Stats a, Stats b)
    {
        return new Stats
        {
            HP = a.HP + b.HP,
            ATK = a.ATK + b.ATK,
            SPD = a.SPD + b.SPD,
            DEF = a.DEF + b.DEF,
            CC = a.CC + b.CC,
            CD = a.CD + b.CD
        };
    }

    public static Stats operator *(Stats a, float multiplier)
    {
        return new Stats
        {
            HP = a.HP * multiplier,
            ATK = a.ATK * multiplier,
            SPD = a.SPD * multiplier,
            DEF = a.DEF * multiplier,
            CC = a.CC * multiplier,
            CD = a.CD * multiplier
        };
    }
}