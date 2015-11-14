using UnityEngine;
using System.Collections;

public class MoveToNextCutscene : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Move to next cutscene on click
	public void OnMouseDown() {
		Application.LoadLevel("IntroCutscene1");
	}
}
