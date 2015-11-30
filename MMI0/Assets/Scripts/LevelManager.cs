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

	private Transform measureTransform;

	private Dictionary<char, uint> numNeeded, numRemaining;

	// Source to play clips from
	private AudioSource audioSource;

	// Individual note AudioClips
	public AudioClip[] noteClips;
	private Dictionary<string, AudioSource> noteNameToSource;

	// Final measure clip
	public AudioClip measureClip;


	private static class Constants
	{
		public static readonly uint firstChordLevel = 16;

		// The amount of time that it takes an audio clip to load before playing it
		// probably machine dependent, but fuck it for now
		// need a better way to avoid this delay...
		public static readonly float audioDelay = 0.2f;

		public static readonly float measureCenterTime = 1f;

		public static readonly Color32 semiTransparent = new Color32(0xFF, 0xFF, 0xFF, 0x80);
	}

	public static float audioDelay {
		get { return Constants.audioDelay; }
	}

	public bool ChordsAllowed () {
		return this.levelNumber >= Constants.firstChordLevel;
	}

	public void PrePlayNote(Note note, float delay) {
		StartCoroutine (NoteMatchDelayed (note, delay));
	}

	private IEnumerator NoteMatchDelayed(Note note, float delay) {
		// Set up the playing of the sounds
		float maxClipTime = 0f;
		foreach (string name in note.names) {
			AudioSource src = this.noteNameToSource[name];
			this.noteNameToSource[name].PlayDelayed(delay - Constants.audioDelay);
			maxClipTime = Mathf.Max (maxClipTime, src.clip.length);
		}

		// Wait for the note to finish playing
		yield return new WaitForSeconds (delay);

		// Correct Match
		this.notesRemaining--;

		if (this.notesRemaining == 0)
			this.CompleteLevel ();
	}

	public bool StillNeedsMinion(Minion m) {
		uint needed = 0, remaining = 0;

		this.numNeeded.TryGetValue (m.letter, out needed);
		this.numRemaining.TryGetValue (m.letter, out remaining);

		return (remaining <= needed);
	}

	public void DoneWithMinion(Minion m) {
		if (this.minionsAutoclicked.Contains (m))
			this.minionsAutoclicked.Remove (m);
	}

	public void CompleteLevel() {
		StartCoroutine (CompleteLevelAsync ());
	}

	private IEnumerator CompleteLevelAsync() {
		GameManager.currentLevel = (int)(this.levelNumber + 1);

		// Move the measure to the center of the screen
		float moveTime = LevelManager.Constants.measureCenterTime;
		float currentTime = 0f;

		Vector3 measureStart = this.measureTransform.position;
		Vector3 measureEnd = new Vector3 (0, 0, -9);

		Vector3 measureScaleStart = this.measureTransform.localScale;
		Vector3 measureScaleEnd = measureScaleStart * 1.5f;

		foreach (SpriteRenderer rend in this.gameObject.GetComponentsInChildren<SpriteRenderer>()) {
			if (rend.gameObject.transform == this.measureTransform ||
			    rend.gameObject.transform.parent == this.measureTransform)
				continue; // Skip notes and measure
			rend.color = Constants.semiTransparent;
		}

		float t;

		while (currentTime <= moveTime) {
			t = currentTime / moveTime;
			t = t * t * t * (t * (6f * t - 15f) + 10f);

			this.measureTransform.position = Vector3.Lerp (measureStart, measureEnd, t);
			this.measureTransform.localScale = Vector3.Lerp (measureScaleStart, measureScaleEnd, t);

			yield return new WaitForEndOfFrame();
			currentTime += Time.deltaTime;
		}
		this.measureTransform.position = measureEnd;

		// Play the final measure
		AudioClip clip = this.measureClip;
		this.audioSource.clip = clip;
		this.audioSource.volume = 1f;
		this.audioSource.Play ();

		yield return new WaitForSeconds (clip.length + 0.2f);

		Application.LoadLevel ("LevelSelection");
	}

	public void IncorrectMatch(Note note) {
		// TODO
	}

	public void RegisterNote(Note note) {
		uint charCount;
		foreach (char c in note.letters) {
			charCount = 0;
			this.numNeeded.TryGetValue(c, out charCount);
			this.numNeeded[c] = charCount + 1;
		}
		this.notesRemaining++;
		this.notes.Add (note);
		this.measureTransform = note.transform.parent;
	}

	public void RegisterMinion(Minion minion) {
		uint charCount = 0;
		this.numRemaining.TryGetValue (minion.letter, out charCount);
		this.numRemaining [minion.letter] = charCount + 1;

		this.minions.Add (minion);
	}

	public void DeregisterNote(Note note) {
		foreach (char c in note.letters) {
			this.numNeeded[c]--;
		}

		this.notes.Remove(note);
	}

	public void DeregisterMinion(Minion minion) {
		this.numRemaining [minion.letter]--;
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

		this.numNeeded = new Dictionary<char, uint> ();
		this.numRemaining = new Dictionary<char, uint> ();
	}

	// Use this for initialization
	void Start () {
		this.audioSource = this.GetComponent<AudioSource> ();

		this.noteNameToSource = new Dictionary<string, AudioSource> ();

		foreach (AudioClip clip in this.noteClips) {
			AudioSource src = this.gameObject.AddComponent<AudioSource>();
			src.clip = clip;
			clip.LoadAudioData();
			this.noteNameToSource[clip.name] = src;
		}
	}

	private bool autoplay = false;

	private void AutoMatch() {
		List<Note> notes = new List<Note>();
		foreach (Note n in this.notes) {
			if (!this.notesAutoclicked.Contains(n))
				notes.Add(n);
		}
		if (notes.Count == 0)
			return;
		int i = Random.Range (0, notes.Count);
		Note note = notes[i];

		List<Minion> toPickUp = new List<Minion> ();
	
		foreach (char letter in note.letters) {	
			List<Minion> minions = new List<Minion>();
			foreach (Minion m in this.minions) {
				if (!this.minionsAutoclicked.Contains(m) 
				    && !toPickUp.Contains(m)
				    && (m.letter == letter))
					minions.Add(m);
			}
			if (minions.Count == 0)
				return;
			
			i = Random.Range (0, minions.Count);
			Minion minion = minions[i];
			toPickUp.Add(minion);
		}

		foreach (Minion m in toPickUp) {
			Hero.singleton.PickUpMinion(m);
			this.minionsAutoclicked.Add(m);
		}

		this.notesAutoclicked.Add(note);
		Hero.singleton.TurnInNote(note);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			this.CompleteLevel();
		} 

		if (Input.GetKeyDown (KeyCode.Return)) {
			Hero.singleton.Caffeinate(5f);
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
