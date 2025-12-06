using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    [Header("The dice used for this battle")]
    public Dice playerDice;

    [Header("Dice Prefab")]
    [SerializeField] private GameObject dicePrefab;
    [SerializeField] private int diceCount = 3;
    [SerializeField] private Vector3 spacing = new Vector3(50, 0, 0);

    private List<Animator> diceAnimators = new List<Animator>();
    private ArrayList<GameObject> diceInstances = new ArrayList<GameObject>();

    void Start()
    {
        InstantiateDice();
    }

    void InstantiateDice()
    {
        // Clear any existing dice
        diceAnimators.Clear();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        diceInstances.Clear();

        // Calculate total width needed
        float totalWidth = (diceCount - 1) * spacing.magnitude;
        float startOffset = -totalWidth / 2f;

        // Spawn new dice centered
        for (int i = 0; i < diceCount; i++)
        {
            Vector3 position = transform.position + (spacing.normalized * (startOffset + i * spacing.magnitude));
            GameObject dice = Instantiate(dicePrefab, position, Quaternion.identity, transform);

            diceInstances.Add(dice);
            
            Animator animator = dice.GetComponent<Animator>();
            if (animator != null)
            {
                diceAnimators.Add(animator);
            }
        }
    }

    public DiceFace[] RollDice()
    {
        DiceFace[] result = new DiceFace[3];

        for (int i = 0; i < diceInstances.Count; i++)
        {
            int index = Random.Range(0, 6);
            result[i] = playerDice.faces[index];
            // diceInstances[i].GetComponent<Animator>().SetTrigger(result[i].type.ToString());
            // Debug.Log($"Dice {i} rolled {result[i].type}");
        }

        return result;
    }

    // Plays the roll animation on all dice, waits for them to finish, then returns the rolled faces via callback.
    public IEnumerator RollDiceWithAnimation(System.Action<DiceFace[]> onComplete)
    {
        // Trigger all dice animations
        foreach (var animator in diceAnimators)
        {
            if (animator != null)
            {
                animator.SetTrigger("Rolling");
            }
        }
        
        // Wait one frame for animators to process
        yield return null;
        
        // Wait for all animations to complete
        bool allDoneAnimating = false;
        while (!allDoneAnimating)
        {
            allDoneAnimating = true;
            foreach (var animator in diceAnimators)
            {
                if (animator != null)
                {
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    if (stateInfo.normalizedTime < 1.0f || animator.IsInTransition(0))
                    {
                        allDoneAnimating = false;
                        break;
                    }
                }
            }
            yield return null;
        }

        onComplete?.Invoke(RollDice());
    }
}
