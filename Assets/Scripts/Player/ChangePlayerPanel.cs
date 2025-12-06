using UnityEngine;

public class ChangePlayerPanel : MonoBehaviour
{
    [SerializeField] private GameObject gearPanel;
    [SerializeField] private GameObject playerTypePanel;
    [SerializeField] private GameObject startGameButton;

    public void GoToNextPanel()
    {
        this.gameObject.SetActive(false);
        playerTypePanel.SetActive(false);
        gearPanel.SetActive(true);
        startGameButton.SetActive(true);
    }
}
