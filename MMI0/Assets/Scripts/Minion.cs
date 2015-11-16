using UnityEngine;
using System.Collections;

public class Minion : MonoBehaviour {

	public Hero hero;
	public BackgroundClick background;

	// // Public Methods // //


	/* Detach from the hero onto the scene as the parent
	 */
	public void DetachToScene(Transform scene) 
	{
		this.transform.parent = scene;
		this.SetScale (reversed: false);
		this.EnableGravity ();
		this.beingCarried = false;
	}
	
	/* Attach to the hero
	 */
	public void AttachToHero(Hero hero) 
	{
		this.beingCarried = true;
		this.transform.parent = hero.transform;
		this.DisableGravity();
	}

	/* Set the Minion's local scale based on whether supergirl is reversed.
	 * Minions should never appear reversed
	 */
	public void SetScale(bool reversed) 
	{
		this.transform.localScale = reversed ? Constants.BackwardScale : Constants.ForwardScale;
	}
	private static class Constants
	{
		public static readonly Vector3 ForwardScale = new Vector3 (1, 1, 1);
		public static readonly Vector3 BackwardScale = new Vector3 (-1, 1, 1);
	}
	
	// // Private Members // //
	private Collider2D backgroundCollider; // The 2D Collider of the Background sprite
	private Vector3 initialPosition;
	
	private Rigidbody2D rigidBody;
	private bool beingCarried;

	// // Event Handlers // //

	protected void Awake () 
	{
		this.backgroundCollider = background.GetComponent<Collider2D> ();
		this.rigidBody = GetComponent<Rigidbody2D> ();
	}

	protected void Start () 
	{
		this.initialPosition = this.transform.position;
	}

	protected void OnMouseDown()
	{
		hero.PickUpMinion (this);
	}

	/* Called when the minion touches something with a 2D Collider
	 */
	protected void OnTriggerEnter2D(Collider2D other) 
	{
		if (!this.beingCarried && (other != this.backgroundCollider)) {
			// Landed on a platform
			this.DisableGravity();
		}
	}
	
	/* Called when the minion stops touching something with a 2D Collider
	 * */
	protected void OnTriggerExit2D(Collider2D other) {
		if (!this.beingCarried && (other == this.backgroundCollider)) {
			// Left the screen
			this.ResetPosition();
		}
	}

	// // Private Helper Methods // //
	private void ResetPosition() 
	{
		this.transform.position = this.initialPosition;
		this.rigidBody.velocity = Vector2.zero;
	}
	
	private void DisableGravity() 
	{
		this.rigidBody.isKinematic = true;
	}
	
	private void EnableGravity() 
	{
		this.rigidBody.isKinematic = false;
	}
}
