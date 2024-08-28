using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CrabAssets.Scripts.UI
{
    public class MainMenuUI : MonoBehaviour
    {

        [SerializeField] private Button startButton;
        [SerializeField] private Button resetButton;

        private void OnEnable()
        {
            startButton.onClick.AddListener(StartGame);
            startButton.onClick.AddListener(ResetGame);
        }
        
        private void OnDisable()
        {
            startButton.onClick.RemoveListener(StartGame);
            startButton.onClick.RemoveListener(ResetGame);
        }

        private void ResetGame()
        {
            PlayerPrefs.DeleteAll();
        }

        private void StartGame()
        {
            SceneManager.LoadScene("LevelScene");
        }
    }
}
