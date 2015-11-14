using UnityEngine;
using System.Collections;

public class Minion : MonoBehaviour {

	public Hero hero;
	public BackgroundClick background;

	protected void Start () {
		Physics2D.IgnoreCollision (this.GetComponent<Collider2D> (), background.GetComponent<Collider2D>());
	}
	
	void OnMouseDown()
	{
		hero.PickUpMinion (this);
	}
}
