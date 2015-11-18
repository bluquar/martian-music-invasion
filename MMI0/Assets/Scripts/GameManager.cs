using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	static public int currentLevel = 2;
	static public bool integratedVersion = true;
	static public int numOfLevels = 3;

	void Awake() {
		// Do not destory this GameManager instance when changing scenes in order to store data
		DontDestroyOnLoad (this.gameObject); 
	}

}
