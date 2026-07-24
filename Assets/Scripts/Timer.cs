using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] float time = 60f;
    [SerializeField] TextMeshProUGUI timerText;

    [SerializeField] GameObject timeUpText;

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
            Time.timeScale = 0;
        }
    }
}