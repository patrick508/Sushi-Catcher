using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {
    // Use this for initialization
    public Sprite UnmutedButton;
    public Sprite MutedButton;
    public GameObject StartSoundButton;
    void Awake() {
        DontDestroyOnLoad(gameObject);
        if (FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
    }
	void Start () {
        AudioListener.volume = PlayerPrefs.GetFloat("SoundVolume", AudioListener.volume);
        if (AudioListener.volume == 1) {
            StartSoundButton.GetComponent<Image>().sprite = UnmutedButton;
        }
        if (AudioListener.volume == 0) {
            StartSoundButton.GetComponent<Image>().sprite = MutedButton;
        }
    }
	
	// Update is called once per frame
	void Update () {

	}

}
