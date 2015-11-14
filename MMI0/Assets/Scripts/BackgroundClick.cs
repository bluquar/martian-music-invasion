using UnityEngine;
using System.Collections;

public class BackgroundClick : MonoBehaviour
{

    public Hero hero;

    private void FlyHero()
    {
        Ray ray;

#if (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8)
        ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
#else
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#endif

		Debug.Log ("Clicked on background");

        hero.MoveTo(new Vector3(ray.origin.x, ray.origin.y, 0));

    }

    void OnMouseDown()
    {
        this.FlyHero();
    }
}
