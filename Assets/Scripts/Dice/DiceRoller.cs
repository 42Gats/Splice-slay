using System.Collections;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    [Header("The dice used for this battle")]
    public Dice playerDice;

    [Header("Animation")]
    [SerializeField] private Animator diceAnimator;
    [SerializeField] private string rollTrigger = "Rolling";
    [SerializeField] private string rollAnimationStateName = "Rolling"; // The name of the animation state

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
            
            // Wait one frame for the animator to process the trigger
            yield return null;
            
            // Get the current animation state info
            AnimatorStateInfo stateInfo = diceAnimator.GetCurrentAnimatorStateInfo(0);
            
            // Wait for the animation to complete
            while (stateInfo.normalizedTime < 1.0f || diceAnimator.IsInTransition(0))
            {
                yield return null;
                stateInfo = diceAnimator.GetCurrentAnimatorStateInfo(0);
            }
        }

        onComplete?.Invoke(RollDice());
    }
}
