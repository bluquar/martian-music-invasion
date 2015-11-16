using UnityEngine;
using System.Collections;

public class CutsceneControl : MonoBehaviour {

	public string nextScene;
	string[] ButtonScenes = {"TitleScene", "LetsGoScene"};
	string[] TimerScenes = {"IntroCutscene1", "IntroCutscene2", "IntroCutscene3", "IntroCutscene4"}; 
	
	// Use this for initialization
	void Start () {
		if ((Application.loadedLevelName != "TitleScene") || (Application.loadedLevelName != "LetsGoScene")) {
			TimerChangeScene ();
		}
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
		Application.LoadLevel(nextScene);
	}
}
