using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	static public int currentLevel = 1;
	static public bool integratedVersion = false;
	static public int numOfLevels = 18;

	public static GameObject measure;
	public static Transform measureTransform;

	public static void SetMeasure(GameObject measure, Transform transform) {
		GameManager.measure = measure;
		GameManager.measureTransform = transform;
	}

	void Awake() {
		// Do not destory this GameManager instance when changing scenes in order to store data
		DontDestroyOnLoad (this.gameObject);
	}

	void Start() {
		if (GameManager.measure != null) {
			StartCoroutine (this.ShrinkMeasure(GameManager.measure));
		}
	}

	private IEnumerator ShrinkMeasure (GameObject measure) {
		yield return new WaitForSeconds (1f);
		measure.transform.parent = this.gameObject.transform;
		measure.transform.position = GameManager.measureTransform.position;
		measure.transform.localScale = GameManager.measureTransform.localScale;
		yield return new WaitForSeconds (1f);
	}
}
