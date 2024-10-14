using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OK
{
    public class PauseMenu : MonoBehaviour
    {
        public GameObject pauseMenu;

        public bool isPaused;

        private void Start()
        {
            pauseMenu.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                if (isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }

        public void PauseGame()
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }

        public void ResumeGame()
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Menu");
            isPaused = false;
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}