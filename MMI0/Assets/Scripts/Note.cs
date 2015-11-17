using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour {

	public string letter;

	private Hero hero;

	public Vector3 position {
		get {
			return this.transform.position;
		}
	}

	private BoxCollider2D bc;

	// Use this for initialization
	void Start () {
		this.bc = this.gameObject.AddComponent <BoxCollider2D>();

		this.bc.enabled = true;
		this.bc.isTrigger = true;

		this.hero = Hero.singleton;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected void OnMouseDown() {
		this.hero.TurnInNote (this);
	}
}
