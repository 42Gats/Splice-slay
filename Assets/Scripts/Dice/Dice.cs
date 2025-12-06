using UnityEngine;

[CreateAssetMenu(fileName = "NewDice", menuName = "Dice/Dice")]
public class Dice : ScriptableObject
{
    public DiceFace[] faces = new DiceFace[6];
}
