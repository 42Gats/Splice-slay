using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    [Header("The dice used for this battle")]
    public Dice playerDice;

    public DiceFace[] RollDice()
    {
        DiceFace[] result = new DiceFace[3];

        for (int i = 0; i < 3; i++)
        {
            int index = Random.Range(0, 6);
            result[i] = playerDice.faces[index];
        }

        return result;
    }
}
