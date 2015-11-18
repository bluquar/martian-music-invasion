using UnityEngine;
using System.Collections;

public class LevelSelectionGrid : MonoBehaviour {

	// unlockTiles is set equal to the version of the game
	public GameObject[] unlockTiles;

	// Use this for initialization
	void Start () {
		VersionSetup();
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	// The two arrays dependent on version
	public GameObject[] musicUnlockTiles;
	public GameObject[] comicUnlockTiles;

	private void VersionSetup() {
		// set unlockTiles array equal to the right array set
		if (GameManager.integratedVersion == true) {
			unlockTiles = musicUnlockTiles;
			// disable the tiles from other version
			disableTilesFor("comicLocks");
		} else {
			unlockTiles = comicUnlockTiles;
			// disable the tiles from other version
			disableTilesFor("musicLocks");
		}
	}

	private void disableTilesFor(string version) {
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
}





