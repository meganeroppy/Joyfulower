using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlowerBaseGroup : MonoBehaviour {

	[SerializeField]
	private List<FlowerBase> child;

	/// <summary>
	/// 情報取得頻度
	/// </summary>
	[SerializeField]
	float checkInterval = 15f;
	float timer = 0;

	// Use this for initialization
	void Start () {
	//	SetAllFlower();
	}
	
	// Update is called once per frame
	void Update () {
		if( UpdateCheckTimer() )
		{
			CheckData();
		}

		GetInput();
	}

	/// <summary>
	/// デバッグ用
	/// </summary>
	void SetAllFlower()
	{
		child.ForEach( f => 
			{
				f.Bloom();
			} );
	}

	/// <summary>
	/// ランダム位置に花を一輪咲かせる
	/// </summary>
	void SetRandomFlower()
	{
		int idx = Random.Range(0, child.Count);
		child[idx].Bloom();
	}

	/// <summary>
	/// サーバーチェックタイマー更新
	/// </summary>
	/// <returns><c>true</c>, if check timer was updated, <c>false</c> otherwise.</returns>
	bool UpdateCheckTimer()
	{
		timer += Time.deltaTime;
		if( timer >= checkInterval )
		{
			timer = 0;
			return true;
		}
		return false;
	}

	/// <summary>
	/// サーバーからデータを取得する
	/// </summary>
	void CheckData()
	{
		
	}

	/// <summary>
	/// デバッグ用
	/// </summary>
	void GetInput()
	{
		if( Input.GetKeyDown(KeyCode.B) )
		{
			SetRandomFlower();
		}
	}
}
