using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour
{
    [Header("Dice Settings")]
    [SerializeField] private Dice playerDice;
    [SerializeField] private GameObject dicePrefab;
    [SerializeField] private int diceCount = 3;
    [SerializeField] private Vector3 spacing = new Vector3(50, 0, 0);

    [Header("UI References")]
    [SerializeField] private Image[] diceImages = new Image[3];
    [SerializeField] private GameObject diceResult;

    private List<Animator> diceAnimators = new List<Animator>();
    private List<GameObject> diceInstances = new List<GameObject>();

    private void Start()
    {
        InstantiateDice();
    }

    private void InstantiateDice()
    {
        ClearDice();
        SetDiceResultVisibility(false);

        float distanceBetweenDice = spacing.magnitude;
        float totalDistance = (diceCount - 1) * distanceBetweenDice;
        float startPosition = -totalDistance / 2f;

        for (int i = 0; i < diceCount; i++)
        {
            float offset = startPosition + (i * distanceBetweenDice);
            Vector3 position = transform.position + (spacing.normalized * offset);
            
            GameObject dice = Instantiate(dicePrefab, position, Quaternion.identity, transform);
            diceInstances.Add(dice);

            Animator animator = dice.GetComponent<Animator>();
            if (animator != null)
                diceAnimators.Add(animator);
        }
    }

    private void ClearDice()
    {
        diceAnimators.Clear();
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        diceInstances.Clear();
    }

    public void SetDiceResultVisibility(bool showResults)
    {
        if (diceResult != null)
            diceResult.SetActive(showResults);

        foreach (GameObject dice in diceInstances)
        {
            if (dice != null)
                dice.SetActive(!showResults);
        }
    }

    public void SetDiceEnabled(bool enabled)
    {
        if (diceResult != null)
            if (enabled == false)
                diceResult.SetActive(enabled);
    
        foreach (GameObject dice in diceInstances)
        {
            if (dice != null)
                dice.SetActive(enabled);
        }
    }

    public bool IsDiceEnabled()
    {
        if (diceInstances.Count == 0) return false;
        return diceInstances[0].activeSelf;
    }

    private DiceFace[] RollDice()
    {
        DiceFace[] results = new DiceFace[diceCount];
        SetDiceResultVisibility(true);

        for (int i = 0; i < diceCount; i++)
        {
            int randomIndex = Random.Range(0, playerDice.faces.Length);
            results[i] = playerDice.faces[randomIndex];
            
            if (i < diceImages.Length && diceImages[i] != null)
                diceImages[i].sprite = results[i].icon;
        }

        return results;
    }

    public IEnumerator RollDiceWithAnimation(System.Action<DiceFace[]> onComplete)
    {
        TriggerAllAnimations();
        yield return null;

        yield return StartCoroutine(WaitForAllAnimationsComplete());

        DiceFace[] results = RollDice();
        onComplete?.Invoke(results);
    }

    private void TriggerAllAnimations()
    {
        foreach (Animator animator in diceAnimators)
        {
            if (animator != null)
                animator.SetTrigger("Rolling");
        }
    }

    private IEnumerator WaitForAllAnimationsComplete()
    {
        bool allDone = false;
        while (!allDone)
        {
            allDone = true;
            foreach (Animator animator in diceAnimators)
            {
                if (animator == null) continue;

                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                bool isAnimating = stateInfo.normalizedTime < 1.0f || animator.IsInTransition(0);
                
                if (isAnimating)
                {
                    allDone = false;
                    break;
                }
            }
            yield return null;
        }
    }
}
