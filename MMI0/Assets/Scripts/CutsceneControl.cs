using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneControl : MonoBehaviour {

    public Button PlayButton;
    public InputField NameInputField;
    public SessionManager Session;

    public void OnInputChange()
    {
        PlayButton.enabled = GameVersion.ValidID(NameInputField.text.ToLower());
    }

	// Use this for initialization
	void Start () {
        if (PlayButton != null)
        {
            PlayButton.enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

	public void ChangeScene(string levelName) 
	{
		CutsceneAudio.ChangeScene (levelName);
		SceneManager.LoadScene(levelName);
	}	

    public void Begin ()
    {
        string name = NameInputField.text.ToLower();
        Logger.Instance.UserID = name;
        LevelSelection.SetVersion(GameVersion.GetVersion(name));
        Session.LoadLevel("IntroCutscene1");
    }
}
