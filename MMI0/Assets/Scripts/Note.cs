using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Note : MonoBehaviour {

	public string[] names = {"A1"};

	public uint number;

	public int toneCount {
		get {
			return this.names.Length;
		}
	}

	public IEnumerable<char> letters {
		get {
			foreach (string n in this.names) {
				yield return n [0];
			}
		}
	}

	private Hero hero;
	private LevelManager levelManager;

	public Vector3 position {
		get {
			return this.transform.position;
		}
	}

	public void Match() {
		this.levelManager.DeregisterNote (this);
		Destroy (this.gameObject);
	}

	public void Fail() {
		StartCoroutine (PulseRed (LevelManager.singleton.noteFailClip.length));
	}

	private IEnumerator PulseRed (float length) {
		SpriteRenderer rend = this.gameObject.GetComponent<SpriteRenderer> ();
		byte NRED;
		for (NRED = 0xFF; NRED != 0x0F; NRED -= 0x10) {
			rend.color = new Color32(0xFF, NRED, NRED, 0xFF);
			yield return new WaitForSeconds(length / 32);
		}
		for (NRED = 0x00; NRED != 0xF0; NRED += 0x10) {
			rend.color = new Color32(0xFF, NRED, NRED, 0xFF);
			yield return new WaitForSeconds(length / 32);
		}
		rend.color = new Color32 (0xFF, 0xFF, 0xFF, 0xFF);
	}

	private BoxCollider2D bc;

	// Use this for initialization
	void Start () {
		//this.bc = this.gameObject.AddComponent <BoxCollider2D>();
		//this.bc.enabled = true;
		//this.bc.isTrigger = true;

		this.hero = Hero.singleton;
		this.levelManager = LevelManager.singleton;
		this.levelManager.RegisterNote (this);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected void OnMouseDown() {
		this.hero.TurnInNote (this);
	}
}
