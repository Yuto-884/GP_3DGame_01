using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] float time = 60f;
    [SerializeField] TextMeshProUGUI timerText;

    [SerializeField] GameObject timeUpText;

    [SerializeField] GameObject resultText;
    [SerializeField] TextMeshProUGUI scoreResultText;

    void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;

            if (time < 0)
                time = 0;

            timerText.text = Mathf.Ceil(time).ToString();
        }
        else
        {
            timeUpText.SetActive(true);

            resultText.SetActive(true);
            scoreResultText.text = "Score : " + ScoreManager.Instance.score;

            Time.timeScale = 0;
        }
    }
}