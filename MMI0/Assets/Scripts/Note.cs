using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour {

	public string letter;
	public uint number;

	private Hero hero;
	private LevelManager levelManager;

	public Vector3 position {
		get {
			if (this == null)
				Debug.Log ("blah");
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
		Debug.Log ("clicked");
		this.hero.TurnInNote (this);
	}
}
