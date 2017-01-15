using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlowerBaseGroup : MonoBehaviour {

	[SerializeField]
	private List<FlowerBase> child;

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
	void SetRandomFlower(FlowerBase.FlowerType flowerType)
	{
		int idx = Random.Range(0, child.Count);
		child[idx].Bloom(flowerType);
	}

	/// <summary>
	/// ランダム位置にランダム花を一輪咲かせる
	/// </summary>
	void SetRandomFlower()
	{
		int idx = Random.Range(0, child.Count);
		child[idx].Bloom();
	}


	/// <summary>
	/// ツイート情報を元に花の生成を行う
	/// </summary>
	public void SetFlower(api.GetTweetInfoResponseParameter.TweetInfo info)
	{
		var alp = "ABCDEF";

		// TODO: 花の種類をセット
		int iType = alp.IndexOf( info.felling );
		var type = (FlowerBase.FlowerType)iType;
	
		// TODO:花の場所をセット
		var position = info.gps;

		// TODO:花が咲く時間をセット
		var date = info.date;

		SetRandomFlower(type);
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
