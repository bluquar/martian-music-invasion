using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Note : MonoBehaviour {

	public string[] names;

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
		this.levelManager.CorrectMatch (this);
		this.levelManager.DeregisterNote (this);
		Destroy (this.gameObject);
	}

	public void Fail() {
		this.levelManager.IncorrectMatch (this);
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
