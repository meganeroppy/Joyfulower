using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour 
{
	/// <summary>
	/// 実体
	/// </summary>
	public static SceneManager instance;

	/// <summary>
	/// シーンタイプ定義
	/// </summary>
	public enum SceneType
	{
		Walk,	// 散策
		Bouquet,// 花束作成
	}

	/// <summary>
	/// 現在のシーンタイプ
	/// </summary>
	[HideInInspector]
	public SceneType sceneType;

	/// <summary>
	/// ツイート情報取得間隔
	/// </summary>
	private const float update_interval = 15f;

	/// <summary>
	/// APIマネージャ
	/// </summary>
	[SerializeField]
	private api.APIManager api;

	/// <summary>
	/// 花生成ポイントグループ
	/// </summary>
	[SerializeField]
	private FlowerBaseGroup flowerBaseGroup;

	// Use this for initialization
	void Start () 
	{
		instance = this;
		sceneType = SceneType.Walk;


		if( api == null )
		{
			Debug.LogError("APIマネージャが未定義");
			return;
		}

		StartCoroutine( WaitAndGetTweetInfo() );
	}

	/// <summary>
	/// 一定間隔でツイート情報の更新を行う
	/// </summary>
	/// <returns>The and get tweet info.</returns>
	IEnumerator WaitAndGetTweetInfo()
	{
		Debug.Log("ツイート情報取得中");

		yield return StartCoroutine( api.GetTweetInfo( res => 
		{
			Debug.Log("APIの返却値で花の生成を行います");
			
			
				res.tweetInfoList.ForEach( v => { flowerBaseGroup.SetFlower( v ); });
		} ) );

		// 一定時間待機
		yield return new WaitForSeconds( update_interval );

		// 更新
		StartCoroutine( WaitAndGetTweetInfo() );
	}
}
