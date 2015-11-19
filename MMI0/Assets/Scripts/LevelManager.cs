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

	public void CorrectMatch(Note note) {
		this.notesRemaining--;

		if (this.notesRemaining == 0) {
			this.CompleteLevel();
		}

		// TODO
	}

	public void CompleteLevel() {
		GameManager.currentLevel++;
		Application.LoadLevel ("LevelSelection");
	}

	public void IncorrectMatch(Note note) {
		// TODO
	}

	public void RegisterNote(Note note) {
		this.notesRemaining++;
		// TODO
	}

	private uint notesRemaining;

	void Awake () {
		LevelManager.singleton = this;
		this.notesRemaining = 0;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Space)) {
			this.CompleteLevel();
		}
	}

}
