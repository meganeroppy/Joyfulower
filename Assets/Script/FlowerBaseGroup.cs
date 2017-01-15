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
	void SetRandomFlower()
	{
		int idx = Random.Range(0, child.Count);
		child[idx].Bloom();
	}

	/// <summary>
	/// ツイート情報を元に花の生成を行う
	/// </summary>
	public void SetFlower(string data)
	{
		// TODO: 情報を解析して花を咲かせる

		SetRandomFlower();
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
