﻿using UnityEngine;
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

	private static class Constants
	{
		public static readonly float measureMoveTime = 1.2f;
		public static readonly float measureShrinkTime = 2.2f;
	}

	private IEnumerator ShrinkMeasure (GameObject measure) {
		//yield return new WaitForSeconds (1f);
		measure.transform.parent = this.gameObject.transform;
		measure.transform.position = GameManager.measureTransform.position;
		measure.transform.localScale = GameManager.measureTransform.localScale * (5f / 4.25f);

		yield return new WaitForSeconds (0.2f);

		LevelSelectionGrid grid = LevelSelectionGrid.singleton;
		GameObject measureTile = grid.musicUnlockTiles [currentLevel - 2];

		Vector3 startPosition = measure.transform.position;
		Vector3 destPosition = new Vector3(measureTile.transform.position.x, measureTile.transform.position.y, -9);

		Vector3 startScale = measure.transform.localScale;
		Vector3 destScale = new Vector3 (0f, 0f, startScale.z);

		float t, s;
		float currentTime = 0f;
		float moveTime = Constants.measureMoveTime;
		float shrinkTime = Constants.measureShrinkTime;

		Transform transform = measure.transform;

		while (currentTime <= moveTime) {
			t = currentTime / moveTime;
			t = t * t * t * (t * (6f * t - 15f) + 10f);

			s = currentTime / shrinkTime;
			s = s * s * s * (s * (6f * s - 15f) + 10f);
			
			transform.position = Vector3.Lerp (startPosition, destPosition, t);
			transform.localScale = Vector3.Lerp (startScale, destScale, s);
			
			yield return new WaitForEndOfFrame();
			currentTime += Time.deltaTime;
		}
		transform.position = destPosition;

		while (currentTime <= shrinkTime) {
			s = currentTime / shrinkTime;
			s = s * s * s * (s * (6f * s - 15f) + 10f);

			transform.localScale = Vector3.Lerp(startScale, destScale, s);

			yield return new WaitForEndOfFrame();
			currentTime += Time.deltaTime;
		}

		Destroy (transform.gameObject);
		GameManager.measure = null;
	}
}
