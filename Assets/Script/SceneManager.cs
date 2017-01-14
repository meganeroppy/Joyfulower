using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour 
{
	public static SceneManager instance;

	public enum SceneType
	{
		Walk,	// 散策
		Bouquet,// 花束作成
	}

	public SceneType sceneType;

	// Use this for initialization
	void Start () {
		instance = this;
		sceneType = SceneType.Walk;
	}
}
