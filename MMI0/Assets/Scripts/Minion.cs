using UnityEngine;
using System.Collections;

public class Minion : MonoBehaviour {

	public Hero hero;
	public BackgroundClick background;

	private Transform canvasTransform;

	protected void Start () {
		Physics2D.IgnoreCollision (this.GetComponent<Collider2D> (), background.GetComponent<Collider2D>());

		this.canvasTransform = this.transform.FindChild ("minion").FindChild("Canvas");
	}
	
	void OnMouseDown()
	{
		hero.PickUpMinion (this);
	}

	private static class Constants
	{
		public static readonly Vector3 ForwardScale = new Vector3 (1, 1, 1);
		public static readonly Vector3 BackwardScale = new Vector3 (-1, 1, 1);
	}
	
	public void SetTextScale(bool reversed) {
		this.canvasTransform.localScale = reversed ? Constants.BackwardScale : Constants.ForwardScale;
	}
}
