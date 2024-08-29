using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CrabAssets.Scripts.UI
{
    public class MainMenuUI : MonoBehaviour
    {

        [SerializeField] private Button startButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button menuButton;

        private void OnEnable()
        {
            if (startButton)
            {
                startButton.onClick.AddListener(StartGame);
            }

            if (resetButton)
            {
                resetButton.onClick.AddListener(ResetGame);
            }

            if (menuButton)
            {
                menuButton.onClick.AddListener(MenuGame);
            }
        }

        private void OnDisable()
        {
            if (startButton)
            {
                startButton.onClick.RemoveListener(StartGame);
            }

            if (resetButton)
            {
                resetButton.onClick.RemoveListener(ResetGame);
            }

            if (menuButton)
            {
                menuButton.onClick.RemoveListener(MenuGame);
            }
        }

        private void MenuGame()
        {
            SceneManager.LoadScene("MenuScene");
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
