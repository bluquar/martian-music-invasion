using UnityEngine;
using System.Collections;

public class CutsceneControl : MonoBehaviour {
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ChangeScene(string levelName) 
	{
		CutsceneAudio.ChangeScene (levelName);
		Application.LoadLevel(levelName);
	}	
}
