using UnityEngine;
using System.Collections;

public class CutsceneAudio : MonoBehaviour {

	public AudioClip orchestralMusic;
	public AudioClip alienMusic;

	public AudioSource audioSource;

	public static CutsceneAudio singleton;

	// Use this for initialization
	void Start () {
		CutsceneAudio.singleton = this;
		DontDestroyOnLoad (this);
		this.audioSource = this.gameObject.GetComponent<AudioSource> ();
		this.audioSource.clip = orchestralMusic;
		this.audioSource.Play ();
	}

	public static void ChangeScene (string sceneName) {
		CutsceneAudio ca = CutsceneAudio.singleton;
		if (sceneName == "IntroCutscene1") {
			ca.audioSource.clip = ca.alienMusic;
			ca.audioSource.Play ();
		} else if (sceneName == "LevelSelection") {
			Destroy(ca.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
}
