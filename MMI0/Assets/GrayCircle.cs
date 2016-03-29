using UnityEngine;
using System.Collections;

public class GrayCircle : MonoBehaviour {

    public bool IsCorrect;
    public AddressingController controller;
    public SpriteRenderer sr;

    private static Color HoverColor = new Color(0.8f, 0.8f, 0.8f);
    private static Color UnhoverColor = Color.white;

    protected void OnMouseDown()
    {
        if (IsCorrect)
        {
            controller.CorrectCircleClicked(this);
        } else
        {
            controller.IncorrectCircleClicked(this);
        }
    }

    protected void OnMouseEnter()
    {
        sr.color = HoverColor;
    }

    protected void OnMouseExit()
    {
        sr.color = UnhoverColor;
    }
}
