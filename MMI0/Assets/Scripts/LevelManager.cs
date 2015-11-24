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

	// Source to play clips from
	private AudioSource audioSource;

	// Individual note AudioClips
	public AudioClip[] noteClips;
	public string[] noteClipNames;
	private Dictionary<string, AudioClip> noteNameToClip;


	private static class Constants
	{
		public static readonly uint firstChordLevel = 16;

		// The amount of time that it takes an audio clip to load before playing it
		// probably machine dependent, but fuck it for now
		// need a better way to avoid this delay...
		public static readonly float audioDelay = 0.25f;
	}

	public static float audioDelay {
		get {
			return Constants.audioDelay;
		}
	}

	public bool ChordsAllowed () {
		return this.levelNumber >= Constants.firstChordLevel;
	}

	public void PrePlayNote(Note note, float delay) {
		foreach (string name in note.names) {
			this.audioSource.clip = this.noteNameToClip[name];
			this.audioSource.PlayDelayed(delay - LevelManager.audioDelay);
		}
	}

	public void CorrectMatch(Note note) {
		this.notesRemaining--;

		/*foreach (string name in note.names) {
			this.audioSource.PlayOneShot (this.noteNameToClip [name]);
		}*/

		if (this.notesRemaining == 0) {
			this.CompleteLevel();
		}

		// TODO
	}

	public bool StillNeedsMinion(Minion m) {
		char letter = m.letter;
		foreach (Minion other in this.minions) {
			if (other == m)
				continue;
			if (other.letter == letter)
				return false;
		}
		foreach (Note note in this.notes) {
			foreach (char t in note.letters)
				if (t == letter)	
					return true;
		}
		return false;
	}

	public void DoneWithMinion(Minion m) {
		if (this.minionsAutoclicked.Contains (m))
			this.minionsAutoclicked.Remove (m);
	}

	public void CompleteLevel() {
		GameManager.currentLevel = (int)(this.levelNumber + 1);
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
		this.audioSource = this.GetComponent<AudioSource> ();

		this.noteNameToClip = new Dictionary<string, AudioClip> ();
		foreach (AudioClip clip in this.noteClips) {
			this.noteNameToClip[clip.name] = clip;
		}
	}

	private bool autoplay = false;

	private void AutoMatch() {
		if (this.ChordsAllowed ())
			return;

		List<Note> notes = new List<Note>();
		foreach (Note n in this.notes) {
			if (!this.notesAutoclicked.Contains(n))
				notes.Add(n);
		}
		if (notes.Count == 0)
			return;
		int i = Random.Range (0, notes.Count);
		Note note = notes[i];
	
		char letter = note.names [0] [0];
		
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

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			this.CompleteLevel();
		} 

		if (Input.GetKeyDown (KeyCode.Return)) {
			for (int i = 0; i < 12; i++)
				Hero.singleton.Caffeinate();
			this.autoplay = true;
		}

		if (Input.GetKeyDown (KeyCode.A)) {
			this.autoplay = !this.autoplay;
		} 

		if (Input.GetKeyDown (KeyCode.C)) {
			Hero.singleton.Caffeinate();
		}

		if (this.autoplay || Input.GetKeyDown (KeyCode.M)) {
			this.AutoMatch();
		}
	}

}
