using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] TextMeshProUGUI scoreText;

    int score = 0;

    void Awake()
    {
        Instance = this;
    }
    public void AddScore(int point)
    {
        score += point;
        scoreText.text = "Score : " + score;

        Debug.Log(score);
    }
}