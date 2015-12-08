using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSelectionGrid : MonoBehaviour {

	// unlockTiles is set equal to the version of the game
	private GameObject[] unlockTiles;
	private GameObject playLevelButton;
	private int[] tutorialLevels = new int[] {1, 4, 7, 10, 13, 16};

	// The two arrays of unlock tiles dependent on version
	public GameObject[] musicUnlockTiles;
	public GameObject[] comicUnlockTiles;
	
	// The two play buttons dependent on verison
	public GameObject musicPlayButton;
	public GameObject comicPlayButton;

	// Audio
	private AudioSource audioSource;
	public AudioClip[] songClips;
	// Set this to either the individual comic tiles or the song measure tiles to follow along
	private GameObject[] audioPopUpTiles;
	private List<int> audioFullLevels = new List<int> {1, 7, 13, 18};

	// First time on Level Selection Page dialogue box items
	public GameObject outlinedBox;
	public GameObject startPlayingButton;
	public GameObject DialogueText;

	// Array of individual comic tiles or individual measure song tiles for audio following along
	public GameObject[] songMeasureTiles;

	public static LevelSelectionGrid singleton;

	protected void Awake () {
		LevelSelectionGrid.singleton = this;
	}


	// Use this for initialization
	void Start () {
		VersionSetup();
		RemoveUnlockedLevelTiles ();
		DisableTilesFor ("tutorial");

		// Dialogue box set up for first time on level selection page
		if (GameManager.currentLevel != 1) {
			DisableDialogueBoxItems();
		}

		// Move the play button when the player returns to the level select screen after completing a level
		if (GameManager.currentLevel != GameManager.numOfLevels + 1) {
			playLevelButton.transform.position = unlockTiles [GameManager.currentLevel - 1].transform.position;
		} else if (GameManager.currentLevel == GameManager.numOfLevels + 1) {
			playLevelButton.gameObject.SetActive(false);
		}

		// set up audio files
		this.audioSource = this.GetComponent<AudioSource> ();

		// For each return to the level selection page, play the unlocked song
		StartCoroutine(playUnlockedSongAudio() );
		// As the song plays, the comics or the measures enlarge along with the music
		StartCoroutine (followAlongWithTiles ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void VersionSetup() {
		// set unlockTiles array equal to the right array set
		if (GameManager.integratedVersion == true) {
			// disable the tiles from other version
			unlockTiles = musicUnlockTiles;
			DisableTilesFor("comicLocks");
			// disable comic play button
			playLevelButton = musicPlayButton;

			// set popping up tiles array equal to the songMeasureTiles array
			audioPopUpTiles = songMeasureTiles;

			//Debug.Log (string.Format ("{0}: musicPlayButton", GameManager.currentLevel));
			//playLevelButton.transform.position = comicUnlockTiles[GameManager.currentLevel-1].transform.position;

			comicPlayButton.SetActive (false);
		} else {
			// disable the tiles from other version
			unlockTiles = comicUnlockTiles;
			DisableTilesFor("musicLocks");
			// disable music play button
			playLevelButton = comicPlayButton;

			// set popping up tiles array equal to the individual Comic Tiles array
			// TODO !!!
			
			//Debug.Log (string.Format ("{0}: comicPlayButton", GameManager.currentLevel));
			//playLevelButton.transform.position = musicUnlockTiles[GameManager.currentLevel-1].transform.position;

			musicPlayButton.SetActive(false);
		}

	}

	private void DisableTilesFor(string version) {
		if (version == "comicLocks") {
			// disable all of the music lock tiles 
			for (int i = 0; i < GameManager.numOfLevels; i++) {
				comicUnlockTiles [i].GetComponent<SpriteRenderer> ().enabled = false;
			}
		} else if (version == "musicLocks") {
			// disable all of the comic lock tiles
			for (int i = 0; i < GameManager.numOfLevels; i++) {
				musicUnlockTiles [i].GetComponent<SpriteRenderer> ().enabled = false;
			}
		} else if (version == "tutorial") {
			for (int i = 0; i < tutorialLevels.Length; i++) {
				unlockTiles[tutorialLevels[i]-1].GetComponent<SpriteRenderer> ().enabled = false;
			}
		}
	}

	// Function to disable all of the unlocked levels
	private void RemoveUnlockedLevelTiles() {
		for (int i = 0; i < GameManager.currentLevel-1; i++) {
			unlockTiles[i].GetComponent<SpriteRenderer>().enabled = false;
		}
	}

	// function to load the level play 
	public void PlayLevel() {
		playLevelButton.transform.Translate (playLevelButton.transform.position.x + 150, playLevelButton.transform.position.y, playLevelButton.transform.position.z);
		Application.LoadLevel("Level" + GameManager.currentLevel);
	}

	// plays the unlocked song according to the unlocked levels
	private IEnumerator playUnlockedSongAudio () {
		AudioClip levelClip = songClips[GameManager.currentLevel-1];
		this.audioSource.clip = levelClip;
		this.audioSource.Play ();
		yield return new WaitForSeconds(levelClip.length);

		// When the all levels have been unlocked, transition to outro cutscenes
		if (GameManager.currentLevel == GameManager.numOfLevels + 1) {
			Application.LoadLevel("OutroCutscene1");
		}
	}

	// Disables the dialogue box items after button clicked or on all levels beyond level 1
	public void DisableDialogueBoxItems() {
		outlinedBox.GetComponent<SpriteRenderer> ().enabled = false;
		DialogueText.gameObject.SetActive(false);
		startPlayingButton.gameObject.SetActive(false);
	}

	// Follows along with the song audio with either comic tiles or measure tiles
	private IEnumerator followAlongWithTiles() {
		if (audioFullLevels.Contains(GameManager.currentLevel))
		for (int i = 0; i < audioPopUpTiles.Length; i++) {
			popOut(audioPopUpTiles[i]);
			yield return new WaitForSeconds(2);
			popIn(audioPopUpTiles[i]);
		}
	}

	private void popOut(GameObject tile) {
		tile.transform.localScale *= 1.5f;
		tile.transform.position -= Vector3.forward * 2;

	}

	private void popIn(GameObject tile) {
		tile.transform.localScale *= 0.66f;
		tile.transform.position -= Vector3.back * 2;
	}

}







