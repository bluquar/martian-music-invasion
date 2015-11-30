using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	static public int currentLevel = 1;
	static public bool integratedVersion = false;
	static public int numOfLevels = 18;

	void Awake() {
		// Do not destory this GameManager instance when changing scenes in order to store data
		DontDestroyOnLoad (this.gameObject);


	}

}
