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

        hero.MoveTo(new Vector2(ray.origin.x, ray.origin.y));

    }

    void OnMouseDown()
    {
        this.FlyHero();
    }
}
