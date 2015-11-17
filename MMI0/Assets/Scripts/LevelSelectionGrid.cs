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
		if (LevelManager.integratedVersion == true) {
			unlockTiles = musicUnlockTiles;
			// disable the tiles from other version
			disableOtherVersionTiles("comic");
		} else {
			unlockTiles = comicUnlockTiles;
			// disable the tiles from other version
			disableOtherVersionTiles("music");
		}
	}

	private void disableOtherVersionTiles(string version) {
		// fill in code
	}
}





