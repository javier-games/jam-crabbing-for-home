using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMode: MonoBehaviour
{
    public GameObject fadePanel;
    private Image fadeImage;
    private GameObject currentCheckpoint;
    private GameObject initialCheckpoint;
    public float timeLeft = 10.0f;
    public Slider timerUI;
    public GameObject endgamePanel;
    public Text endGameText;
    float currentTimerValue;

    void Start()
    {
        fadeImage = fadePanel.GetComponent<Image>();
        endgamePanel.SetActive(false);
        StartGame();
    }
    
    private IEnumerator StartTimer()
    {
        currentTimerValue = timeLeft;
        while (currentTimerValue > 0)
        {
            //Debug.Log("Timer: " + currentTimerValue);
            timerUI.value = currentTimerValue;
            yield return new WaitForSeconds(.01f);
            currentTimerValue -= .01f;
        }
        if (currentTimerValue <= 0)
        {
            EndGame(false);
        }
    }

    private IEnumerator FadeAnim(string Direction)
    {
        float initAlpha;
        float endAlpha;
        Color imageAlpha = fadeImage.color; ;
        switch (Direction)
        {
            case "inOut":
                initAlpha = 1.0f;
                endAlpha = 0;
                while (initAlpha > endAlpha)
                {
                    initAlpha -= 0.01f;
                    imageAlpha.a = initAlpha;
                    fadeImage.color = imageAlpha;
                    yield return new WaitForSeconds(.01f);
                }
                break;
            case "outIn":
                initAlpha = 0;
                endAlpha = 1.0f;
                while (initAlpha < endAlpha)
                {
                    initAlpha += 0.01f;
                    imageAlpha.a = initAlpha;
                    fadeImage.color = imageAlpha;
                    yield return new WaitForSeconds(.01f);
                }
                break;

        }
        StartCoroutine(StartTimer());

    }
    private void StartGame()
    {
        fadePanel.SetActive(true);
        StartCoroutine(FadeAnim("inOut"));
    }

    private void EndGame(bool winState)
    {
        if (!winState)
        {
            endgamePanel.SetActive(true);
            endGameText.text = "Perdiste cabron!!!";
            StartCoroutine(FadeAnim("outIn"));
        }
    }

    public void SetCheckPoint()
    {

    }
}
