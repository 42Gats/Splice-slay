using System.Collections;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    [Header("The dice used for this battle")]
    public Dice playerDice;

    [Header("Animation")]
    [SerializeField] private Animator diceAnimator;
    [SerializeField] private string rollTrigger = "Rolling";
    [SerializeField] private float rollDuration = 1.0f; // Seconds to wait for roll animation
    // [SerializeField] private bool isRolling = false;

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
    public IEnumerator RollDiceWithAnimation(System.Action<DiceFace[]> onComplete)
    {
        if (diceAnimator != null && !string.IsNullOrEmpty(rollTrigger))
        {
            diceAnimator.SetTrigger(rollTrigger);
            // diceAnimator.SetBool("Rolling", true);
        }

        // Wait for the animation to play
        yield return new WaitForSeconds(rollDuration);

        onComplete?.Invoke(RollDice());
    }
}
