using UnityEngine;
using System.Collections;

public class Minion : MonoBehaviour {

	public Hero hero;
	public BackgroundClick background;

	private Collider2D backgroundCollider;
	private Vector3 initialPosition;

	public Rigidbody2D rb2d;

	protected void Start () {
		this.backgroundCollider = background.GetComponent<Collider2D> ();
		this.initialPosition = this.transform.position;

		//Physics2D.IgnoreCollision (this.GetComponent<Collider2D> (), this.backgroundCollider);
		this.rb2d = GetComponent<Rigidbody2D> ();
	}

	private void ResetPosition() {
		this.transform.position = this.initialPosition;
		this.rb2d.velocity = Vector2.zero;
	}

	public void DisableGravity() {
		this.rb2d.isKinematic = true;
	}

	public void EnableGravity() {
		this.rb2d.isKinematic = false;
	}

	public void DetachToScene(Transform scene) {
		this.transform.parent = scene;
		this.transform.localScale = Constants.ForwardScale;
		this.EnableGravity ();
	}
	
	protected void OnMouseDown()
	{
		hero.PickUpMinion (this);
	}

	private static class Constants
	{
		public static readonly Vector3 ForwardScale = new Vector3 (1, 1, 1);
		public static readonly Vector3 BackwardScale = new Vector3 (-1, 1, 1);
	}

	/* Set the Minion's local scale based on whether supergirl is reversed.
	 * Minions should never appear reversed
	 */
	public void SetScale(bool reversed) {
		this.transform.localScale = reversed ? Constants.BackwardScale : Constants.ForwardScale;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other != this.backgroundCollider) {
			// Landed on a platform
			this.DisableGravity();
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other == this.backgroundCollider) {
			// Left the screen
			this.ResetPosition();
			this.EnableGravity();
		}
	}
}
