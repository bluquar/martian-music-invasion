using UnityEngine;
using System.Collections;

public class LevelSelectionGrid : MonoBehaviour {

	// unlockTiles is set equal to the version of the game
	private GameObject[] unlockTiles;
	private GameObject playLevelButton;
	private int[] tutorialLevels = new int[] {1, 4, 7, 10, 13, 16};

	// The two arrays dependent on version
	public GameObject[] musicUnlockTiles;
	public GameObject[] comicUnlockTiles;
	
	// The two play buttons dependent on verison
	public GameObject musicPlayButton;
	public GameObject comicPlayButton;

	public static LevelSelectionGrid singleton;

	protected void Awake () {
		LevelSelectionGrid.singleton = this;
	}

	// Use this for initialization
	void Start () {
		VersionSetup();
		RemoveUnlockedLevelTiles ();
		DisableTilesFor ("tutorial");

		// Move the play button when the player returns to the level select screen after completing a level
		playLevelButton.transform.position = unlockTiles [GameManager.currentLevel - 1].transform.position;
	
		if (GameManager.currentLevel == GameManager.numOfLevels) {
			Application.LoadLevel("OutroCutscene1");
		}
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

			//Debug.Log (string.Format ("{0}: musicPlayButton", GameManager.currentLevel));
			//playLevelButton.transform.position = comicUnlockTiles[GameManager.currentLevel-1].transform.position;

			comicPlayButton.SetActive (false);
		} else {
			// disable the tiles from other version
			unlockTiles = comicUnlockTiles;
			DisableTilesFor("musicLocks");
			// disable music play button
			playLevelButton = comicPlayButton;

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
		for (int i = 0; i < GameManager.currentLevel; i++) {
			unlockTiles[i].GetComponent<SpriteRenderer>().enabled = false;
		}
	}

	public void PlayLevel() {
		playLevelButton.transform.Translate (playLevelButton.transform.position.x + 150, playLevelButton.transform.position.y, playLevelButton.transform.position.z);
		Application.LoadLevel("Level" + GameManager.currentLevel);
	}
}





