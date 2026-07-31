using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject gameOverText;

    void Awake()
    {
        Instance = this;
    }

    public void GameOver()
    {
        gameOverText.SetActive(true);

        Time.timeScale = 0;
    }
}