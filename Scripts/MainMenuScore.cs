using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuScore : MonoBehaviour {

    public Text Score;
    public Text Distance;
    private int HighscoreMain;
    private int MeterHighScoreMain;

    bool DisplayScore = false;
    public GameObject Scorebuttons;

    void Awake() {
        HighscoreMain = PlayerPrefs.GetInt("HighScore");
        MeterHighScoreMain = PlayerPrefs.GetInt("MeterHighScore");
    }
    void Update() {
        Score.text = "" + HighscoreMain;
        Distance.text = "" + MeterHighScoreMain;
    }

    public void ClickScore() {
        DisplayScore = !DisplayScore;
        if (DisplayScore) {
            Scorebuttons.SetActive(true);
        } else if (!DisplayScore) {
            Scorebuttons.SetActive(false);
        }
    }
}
