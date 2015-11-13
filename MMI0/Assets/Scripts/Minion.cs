using UnityEngine;
using System.Collections;

public class Minion : MonoBehaviour {

	public Hero hero;
	
	private void FlyHero()
	{
		Ray ray;
		
		#if (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
		ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
		#else
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		#endif
		
		hero.PickUpMinion(new Vector2(ray.origin.x, ray.origin.y), this);
		
	}
	
	void OnMouseDown()
	{
		this.FlyHero();
	}
}
