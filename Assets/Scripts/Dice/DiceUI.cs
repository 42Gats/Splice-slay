using UnityEngine;
using UnityEngine.UI;

public class DiceUI : MonoBehaviour
{
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private Button rollButton;
    [SerializeField] private CanvasGroup rollGroup;
    [SerializeField] private DiceRoller diceRoller;

    private void Start()
    {

        rollButton.onClick.AddListener(OnRollButtonClicked);

        if (rollGroup == null)
        {
            rollGroup = rollButton.GetComponent<CanvasGroup>();
            if (rollGroup == null)
            {
                rollGroup = rollButton.gameObject.AddComponent<CanvasGroup>();
            }
        }

        UpdateButtonVisibility();
    }
    
    private void Update()
    {
        UpdateButtonVisibility();
    }

    private void OnRollButtonClicked()
    {
        combatManager.PlayerTurnAction();
    }

    private void UpdateButtonVisibility()
    {
        bool isPlayerTurn = combatManager.state == CombatManager.CombatState.PlayerTurn;

        if (!rollButton.gameObject.activeSelf)
        {
            rollButton.gameObject.SetActive(true);
        }

        rollButton.interactable = isPlayerTurn;

        if (isPlayerTurn && !diceRoller.IsDiceEnabled())
        {
            diceRoller.SetDiceEnabled(true);
        }

        if (rollGroup != null)
        {
            rollGroup.alpha = isPlayerTurn ? 1f : 0f;
            rollGroup.blocksRaycasts = isPlayerTurn;
            rollGroup.interactable = isPlayerTurn;
        }
    }
}
