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
    public GameObject backgroundPlane;
    public Color[] TopColors = new Color[4];
    public Color[] BottomColors = new Color[4];
    public float colorTransitionTime = 3.0f;
    float currentTimerValue;
    public Color currentColor;
    public Color currentColor2;
    public int currentColorInArray = 0;
    public Color initColor;
    public Color initColor2;


    void Start()
    {
        fadeImage = fadePanel.GetComponent<Image>();
        endgamePanel.SetActive(false);
        initColor = TopColors[currentColorInArray];
        backgroundPlane.GetComponent<Renderer>().material.SetColor("_Color1", initColor);
        initColor2 = BottomColors[currentColorInArray];
        backgroundPlane.GetComponent<Renderer>().material.SetColor("_Color", initColor2);


        /*currentColorInArray++;
        StartCoroutine(ColorChange(currentColorInArray));*/
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
                    yield return new WaitForSeconds(.015f);
                }
                StartCoroutine(StartTimer());
                break;
            case "outIn":
                initAlpha = 0;
                endAlpha = 1.0f;
                while (initAlpha < endAlpha)
                {
                    initAlpha += 0.01f;
                    imageAlpha.a = initAlpha;
                    fadeImage.color = imageAlpha;
                    yield return new WaitForSeconds(.015f);
                }
                break;

        }
        

    }

    private IEnumerator ColorChange(int toColor)
    {
        float ElapsedTime = 0.0f;
        //float TotalTime = 10.0f;
        while (ElapsedTime < colorTransitionTime)
        {
            ElapsedTime += Time.deltaTime;
            Color currentColor = Color.Lerp(initColor, TopColors[toColor], (ElapsedTime / colorTransitionTime));
            Color currentColor2 = Color.Lerp(initColor2, BottomColors[toColor], (ElapsedTime / colorTransitionTime));
            Debug.Log(currentColor);
            backgroundPlane.GetComponent<Renderer>().material.SetColor("_Color1", currentColor);
            backgroundPlane.GetComponent<Renderer>().material.SetColor("_Color", currentColor2);
            yield return new WaitForSeconds(.01f);
        }
        initColor = TopColors[toColor];
        initColor2 = BottomColors[toColor];
    }

    private void StartGame()
    {
        fadePanel.SetActive(true);
        StartCoroutine(FadeAnim("inOut"));
        currentCheckpoint = initialCheckpoint;
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

    public void SetCheckPoint(GameObject overlappedCheckpoint)
    {
        currentCheckpoint = overlappedCheckpoint;
        currentColorInArray++;
        StartCoroutine(ColorChange(currentColorInArray));
    }
}
