using UnityEngine;
using System.Collections;

public class CutsceneControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (TimerChangeScene ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ChangeScene(string levelName) 
	{
		Application.LoadLevel(levelName);
	}

	IEnumerator TimerChangeScene() {
		yield return new WaitForSeconds(3);
		Application.LoadLevel("IntroCutscene2");
	}
}
