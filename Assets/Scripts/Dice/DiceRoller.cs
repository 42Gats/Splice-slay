using System.Collections;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    [Header("The dice used for this battle")]
    public Dice playerDice;

    [Header("Animation")]
    [SerializeField] private Animator diceAnimator;
    [SerializeField] private string rollTrigger = "Roll";
    [SerializeField] private float rollDuration = 1.0f; // Seconds to wait for roll animation

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

    // Plays the roll animation, waits for it to finish, then returns the rolled faces via callback.
    {
        if (diceAnimator != null && !string.IsNullOrEmpty(rollTrigger))
        {
            diceAnimator.SetTrigger(rollTrigger);
        }

        // Wait for the animation to play
        yield return new WaitForSeconds(rollDuration);

        onComplete?.Invoke(RollDice());
    }
}
