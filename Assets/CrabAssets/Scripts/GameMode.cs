using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMode: MonoBehaviour {
    public GameObject player;
    public GameObject fadePanel;
    private Image fadeImage;
    public GameObject currentCheckpoint;
    public GameObject initialCheckpoint;
    public float GrowthtimeLeft = 10.0f;
    public float NakedtimeLeft = 10.0f;
    public Slider GrowtimerUI;
    public Image nakednessTimerUI;
    public GameObject endgamePanel;
    public Text endGameText;
    public GameObject backgroundPlane;
    public Color[] TopColors = new Color[4];
    public Color[] BottomColors = new Color[4];
    public float colorTransitionTime = 3.0f;
    float currentGrowthTimerValue;
    float currentNakedTimerValue;
    public Color currentColor;
    public Color currentColor2;
    public int currentColorInArray = 0;
    public Color initColor;
    public Color initColor2;
    AsyncOperation async;

    public Image neckednessPanel;
    public CanvasGroup neckednessCanvasGroup;

    public Coroutine startNakeTimer;
    public Coroutine startGrowthTimer;

    public GameObject finalprefab;


    void Start () {
        player = GameObject.FindWithTag ("Player");
        fadeImage = fadePanel.GetComponent<Image> ();
        endgamePanel.SetActive (false);
        initColor = TopColors[currentColorInArray];
        backgroundPlane.GetComponent<Renderer> ().material.SetColor ("_Color1", initColor);
        initColor2 = BottomColors[currentColorInArray];
        backgroundPlane.GetComponent<Renderer> ().material.SetColor ("_Color", initColor2);

        GrowtimerUI.maxValue = GrowthtimeLeft;
        /*currentColorInArray++;
        StartCoroutine(ColorChange(currentColorInArray));*/
        StartGame ();
    }

    public void BeginTimer () {
        startGrowthTimer = StartCoroutine (StartGrowTimer ());
    }
    public void BeginTimerNekedness () {
        startNakeTimer = StartCoroutine (StartNakedTimer ());
    }

    public void StopNakednessTimer () {
        if (startNakeTimer != null)
            StopCoroutine (startNakeTimer);
    }

    public void StopGrowthTimer () {
        StopCoroutine (startGrowthTimer);
    }

    private IEnumerator StartNakedTimer () {
        currentNakedTimerValue = NakedtimeLeft;
        while (currentNakedTimerValue > 0) {
            //Debug.Log("Timer: " + currentNakedTimerValue);
            nakednessTimerUI.fillAmount = 1 - currentNakedTimerValue / NakedtimeLeft;
            yield return new WaitForSeconds (.01f);
            currentNakedTimerValue -= .01f;
        }
        if (currentNakedTimerValue <= 0) {
            EndGame (false);
        }
    }

    private IEnumerator StartGrowTimer () {
        currentGrowthTimerValue = GrowthtimeLeft;
        while (currentGrowthTimerValue > 0) {
            GrowtimerUI.value = currentGrowthTimerValue;
            yield return new WaitForSeconds (.01f);
            currentGrowthTimerValue -= .01f;
        }
        if (currentGrowthTimerValue <= 0) {
            //player.GetComponent<PlayerController2D> ().GrowUp ();
            StopGrowthTimer ();
            startGrowthTimer = StartCoroutine (StartGrowTimer ());

        }
    }

    private IEnumerator FadeAnim (string Direction) {
        float initAlpha;
        float endAlpha;
        Color imageAlpha = fadeImage.color; ;
        switch (Direction) {
            case "inOut":
            initAlpha = 1.0f;
            endAlpha = 0;
            while (initAlpha > endAlpha) {
                initAlpha -= 0.01f;
                imageAlpha.a = initAlpha;
                fadeImage.color = imageAlpha;
                yield return new WaitForSeconds (.015f);
            }
            //BeginTimer();
            break;
            case "outIn":
            initAlpha = 0;
            endAlpha = 1.0f;
            while (initAlpha < endAlpha) {
                initAlpha += 0.01f;
                imageAlpha.a = initAlpha;
                fadeImage.color = imageAlpha;
                yield return new WaitForSeconds (.015f);
            }
            break;

        }
        StopCoroutine (FadeAnim ("inOut"));

    }

    private IEnumerator ColorChange (int toColor) {
        float ElapsedTime = 0.0f;
        //float TotalTime = 10.0f;
        while (ElapsedTime < colorTransitionTime) {
            ElapsedTime += Time.deltaTime;
            Color currentColor = Color.Lerp (initColor, TopColors[toColor], (ElapsedTime / colorTransitionTime));
            Color currentColor2 = Color.Lerp (initColor2, BottomColors[toColor], (ElapsedTime / colorTransitionTime));
            //Debug.Log(currentColor);
            backgroundPlane.GetComponent<Renderer> ().material.SetColor ("_Color1", currentColor);
            backgroundPlane.GetComponent<Renderer> ().material.SetColor ("_Color", currentColor2);
            yield return new WaitForSeconds (.01f);
        }
        initColor = TopColors[toColor];
        initColor2 = BottomColors[toColor];
    }

    private void StartGame () {
        fadePanel.SetActive (true);
        StartCoroutine (FadeAnim ("inOut"));
        if (currentCheckpoint == null)
            currentCheckpoint = initialCheckpoint;
        else {
            RestartInCheckpoint ();
        }
    }

    private void EndGame (bool winState) {
        if (!winState) {
            endgamePanel.SetActive (true);
            endGameText.text = "Perdiste!!!";
            StartCoroutine (FadeAnim ("outIn"));
        }
    }

    public void SetCheckPoint (GameObject overlappedCheckpoint) {

        CheckPoint check = overlappedCheckpoint.GetComponent<CheckPoint> ();
        if (check != null) {
            // if (check.setLevel) {
            //
            //     StopAllCoroutines ();
            //     Instantiate (finalprefab);
            //     return;
            // }

        }

        currentCheckpoint = overlappedCheckpoint;
        overlappedCheckpoint.GetComponent<CapsuleCollider2D> ().enabled = false;
        currentColorInArray++;
        StopCoroutine (StartGrowTimer ());
        GrowtimerUI.maxValue = GrowthtimeLeft;
        GrowtimerUI.value = GrowthtimeLeft;
        BeginTimer ();
        if (currentColorInArray <= TopColors.Length)
            StartCoroutine (ColorChange (currentColorInArray));
    }

    public void RestartInCheckpoint () {
        player.transform.position = currentCheckpoint.transform.position;
        fadePanel.SetActive (true);
        StartCoroutine (FadeAnim ("inOut"));
        endgamePanel.SetActive (false);
        StopCoroutine (startGrowthTimer);
        GrowtimerUI.maxValue = GrowthtimeLeft;
        GrowtimerUI.value = GrowthtimeLeft;
        BeginTimer ();
    }

    public void NewGameFromMenu () {
        StartCoroutine (load ("Dakalo test"));
    }

    IEnumerator load (string scene) {
        Debug.LogWarning ("ASYNC LOAD STARTED - " +
           "DO NOT EXIT PLAY MODE UNTIL SCENE LOADS... UNITY WILL CRASH");
        //async = Application.LoadLevelAsync (scene);
        async.allowSceneActivation = true;
        yield return async;
    }

    public void GotCollectable () {
        StopGrowthTimer ();
        currentGrowthTimerValue = GrowthtimeLeft;
        // if (player.GetComponent<PlayerController2D> ().HasShell == false) {
        //     startGrowthTimer = StartCoroutine (StartGrowTimer ());
        // }
        // else {
        // }

    }
}
