using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Minion : MonoBehaviour {

	private Hero hero;
	private BackgroundClick background;
	
	public char letter;

	/* Detach from the hero onto the scene as the parent
	 */
	public void DetachToScene(Transform scene) 
	{
		this.transform.parent = scene;
		this.SetScale (/*reversed: */ false);
		this.EnableGravity ();
		this.beingCarried = false;
		this.transform.position = new Vector3 (
			this.transform.position.x, this.transform.position.y,
			this.initialPosition.z
		);
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
	private Vector3 lastRestingPosition;
	
	private Rigidbody2D rigidBody;
	private bool beingCarried;

	// // Event Handlers // //

	protected void Awake () 
	{
		this.rigidBody = GetComponent<Rigidbody2D> ();
	}

	protected void Start () 
	{
		this.initialPosition = this.transform.position;
		this.lastRestingPosition = this.initialPosition;

		this.hero = Hero.singleton;

		this.background = BackgroundClick.singleton;
		this.backgroundCollider = background.GetComponent<Collider2D> ();

		LevelManager.singleton.RegisterMinion (this);
	}

	protected void OnMouseDown()
	{
		hero.PickUpMinion (this);
	}

	/* Called when the minion touches something with a 2D Collider
	 */
	protected void OnTriggerEnter2D(Collider2D other) 
	{
		if (this.beingCarried) {
			// Ignore collisions while being carried
			return;
		}
		if (other == this.backgroundCollider) {
			// We're inside the background
			return;
		}
		if (other.GetComponentInParent<Note> () != null) {
			// Collided with a note
			return;
		}
		this.DisableGravity();
	}
	
	/* Called when the minion stops touching something with a 2D Collider
	 * */
	protected void OnTriggerExit2D(Collider2D other) {
		if (!this.beingCarried && (other == this.backgroundCollider)) {
			// Left the screen
			this.ResetPositionToInitial();
		}
	}
	
	public void ResetPosition() 
	{
		this.transform.position = this.lastRestingPosition;
		this.DisableGravity ();
		this.rigidBody.velocity = Vector2.zero;
	}

	private void ResetPositionToInitial() {
		this.transform.position = this.initialPosition;
		this.EnableGravity ();
		this.rigidBody.velocity = Vector2.zero;
	}

	private void DisableGravity() 
	{
		this.rigidBody.isKinematic = true;
		this.lastRestingPosition = this.transform.position;
	}
	
	private void EnableGravity() 
	{
		this.rigidBody.isKinematic = false;
	}
}
