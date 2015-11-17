using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	public uint levelNumber;

	public static LevelManager singleton;

	private static class Constants
	{
		public static readonly uint firstChordLevel = 16;
	}

	public bool ChordsAllowed () {
		return this.levelNumber >= Constants.firstChordLevel;
	}

	private int notesRemaining;

	void Awake () {
		LevelManager.singleton = this;
		this.notesRemaining = 0;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
