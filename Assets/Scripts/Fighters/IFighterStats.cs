public interface IFighterStats
{
    float GetMaxHP();
    float GetCurrentHP();
    void SetCurrentHP(float value);
    float GetATK();
    float GetDEF();
    float GetSPD();
    float GetCC();
    float GetCD();
}