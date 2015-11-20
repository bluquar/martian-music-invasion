using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LevelManager : MonoBehaviour {
	
	public uint levelNumber;

	public static LevelManager singleton;

	private List<Minion> minions;
	private List<Minion> minionsAutoclicked;
	private List<Note> notes;
	private List<Note> notesAutoclicked;
	private Random random;

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

	public bool StillNeedsMinion(Minion m) {
		string letter = m.letter;
		foreach (Minion other in this.minions) {
			if (other == m)
				continue;
			if (other.letter == letter)
				return false;
		}
		foreach (Note note in this.notes) {
			if (note.letter == letter)
				return true;
		}
		return false;
	}

	public void DoneWithMinion(Minion m) {
		if (this.minionsAutoclicked.Contains (m))
			this.minionsAutoclicked.Remove (m);
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
		this.notes.Add (note);
	}

	public void RegisterMinion(Minion minion) {
		this.minions.Add (minion);
	}

	public void DeregisterNote(Note note) {
		this.notes.Remove(note);
	}

	public void DeregisterMinion(Minion minion) {
		this.minions.Remove (minion);
	}

	private uint notesRemaining;

	void Awake () {
		LevelManager.singleton = this;
		this.notesRemaining = 0;

		this.notes = new List<Note> ();
		this.notesAutoclicked = new List<Note> ();
		this.minions = new List<Minion> ();
		this.minionsAutoclicked = new List<Minion> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			this.CompleteLevel();
		
		} else if (Input.GetKeyDown (KeyCode.M)) {
			List<Note> notes = new List<Note>();
			foreach (Note n in this.notes) {
				if (!this.notesAutoclicked.Contains(n))
					notes.Add(n);
			}
			if (notes.Count == 0)
				return;
			int i = Random.Range (0, notes.Count);
			Note note = notes[i];

			string letter = note.letter;

			List<Minion> minions = new List<Minion>();
			foreach (Minion m in this.minions) {
				if (!this.minionsAutoclicked.Contains(m) && (m.letter == letter))
					minions.Add(m);
			}
			if (minions.Count == 0)
				return;

			i = Random.Range (0, minions.Count);
			Minion minion = minions[i];

			this.minionsAutoclicked.Add(minion);
			this.notesAutoclicked.Add(note);

			Hero.singleton.PickUpMinion(minion);
			Hero.singleton.TurnInNote(note);
		}
	}

}
