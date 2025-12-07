using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;
    
    public void SelectScene(string sceneName)
    {
        if (sceneName == "CombatScene")
        {
            player.GetComponent<Transform>().position = new Vector3(-8, -2, 0);
            enemy.gameObject.SetActive(true);
        }
        SceneManager.LoadScene(sceneName);
    }
}
