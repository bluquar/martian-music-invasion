using UnityEngine;
using System.Collections;

public class UIControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ChangeScene(string levelName) 
	{
		Application.LoadLevel(levelName);
	}
}
