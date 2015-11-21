using UnityEngine;
using System.Collections;

public class LevelSelectionGrid : MonoBehaviour {

	// unlockTiles is set equal to the version of the game
	private GameObject[] unlockTiles;
	private GameObject playLevelButton;

	// The two arrays dependent on version
	public GameObject[] musicUnlockTiles;
	public GameObject[] comicUnlockTiles;
	
	// The two play buttons dependent on verison
	public GameObject musicPlayButton;
	public GameObject comicPlayButton;


	// Use this for initialization
	void Start () {
		VersionSetup();

		// Move the Play Level button after each level is completed
		playLevelButton.transform.position = unlockTiles [GameManager.currentLevel - 1].transform.position;

		RemoveUnlockedLevelTiles ();
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
			comicPlayButton.SetActive (false);
		} else {
			// disable the tiles from other version
			unlockTiles = comicUnlockTiles;
			DisableTilesFor("musicLocks");
			// disable music play button
			playLevelButton = musicPlayButton;
			musicPlayButton.SetActive(false);
		}

	}

	private void DisableTilesFor(string version) {
		if (version == "comicLocks") {
			// disable all of the music lock tiles 
			for (int i = 0; i < GameManager.numOfLevels; i++) {
				comicUnlockTiles[i].GetComponent<SpriteRenderer>().enabled = false;
			}
		} else if (version == "musicLocks") {
			// disable all of the comic lock tiles
			for (int i = 0; i < GameManager.numOfLevels; i++) {
				musicUnlockTiles[i].GetComponent<SpriteRenderer>().enabled = false;
			}
		}
	}

	// Function to disable all of the unlocked levels
	private void RemoveUnlockedLevelTiles() {
		print (GameManager.currentLevel);
		for (int i = 0; i < GameManager.currentLevel; i++) {
			unlockTiles[i].GetComponent<SpriteRenderer>().enabled = false;
		}
	}

	public void PlayLevel() {
		Application.LoadLevel("Level" + GameManager.currentLevel);
	}
}





