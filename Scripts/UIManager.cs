using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static bool paused = false;
    public GameObject PauseMenuUI;

    public Sprite UnmutedButton;
    public Sprite MutedButton;
    public GameObject StartSoundButton;
    float SoundVolume;

    // Use this for initialization
    void Start () {
        //Set paused to false and Timescale to 1 so the game is always unpaused even if the player presses the escape key in another scene.
        paused = false;
        Time.timeScale = 1;

        //check what the sound volume is to change the sprite (usefull for loading in the right sprite when menu scene is loaded).
        if (AudioListener.volume == 1) {
            StartSoundButton.GetComponent<Image>().sprite = UnmutedButton;
        }
        if (AudioListener.volume == 0) {
            StartSoundButton.GetComponent<Image>().sprite = MutedButton;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) && MouseController.PauseMenuActive == false) {
            PauseGame();
        }
    }

    //Restart the current active scene
    public void RestartScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    //Start game
    public void StartGame() {
        SceneManager.LoadScene("Main");
    }

    //Change sprite and volume if button = clicked.
    public void VolumeController() {
        if (AudioListener.volume == 1) {
            AudioListener.volume = 0;
            SoundVolume = 0;
            PlayerPrefs.SetFloat("SoundVolume", AudioListener.volume);
            StartSoundButton.GetComponent<Image>().sprite = MutedButton;
        }
        else if (AudioListener.volume == 0) {
            AudioListener.volume = 1;
            SoundVolume = 1;
            PlayerPrefs.SetFloat("SoundVolume", AudioListener.volume);
            StartSoundButton.GetComponent<Image>().sprite = UnmutedButton;
        }
    }

    //Pause or Unpause game if pausemenuUI is not null (so it wont try to do tis while in main menu
    public void PauseGame() {
        paused = !paused;
            if (paused) {
                if (PauseMenuUI != null) {
                    Time.timeScale = 0;
                    PauseMenuUI.SetActive(true);
                }
            }
            else if (!paused) {
                if (PauseMenuUI != null) {
                    Time.timeScale = 1;
                    PauseMenuUI.SetActive(false);
                }
            }
    }

    public void MainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
    //Quit Game
    public void QuitGame() {
        Application.Quit();
    }
}
