using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 花束を構成するパーツ
/// </summary>
public class BouquetPart : MonoBehaviour {

	/// <summary>
	/// つかめる範囲
	/// </summary>
	public float range;

	/// <summary>
	/// 花の種類（仮）
	/// </summary>
	public enum FlowerType{
		Red,
		Green,
		Blue,
		Count,
	};

	/// <summary>
	/// モデル候補
	/// </summary>
	[SerializeField]
	private List<GameObject> modelList;

	/// <summary>
	/// モデルベース
	/// </summary>
	[SerializeField]
	private Transform modelBase;

	/// <summary>
	/// 生成
	/// </summary>
	/// <param name="type">Type.</param>
	public void Create( FlowerType type )
	{
		if( (int)type >= modelList.Count )
		{
			Debug.LogError( type.ToString() + "が未定義");
		}
		
		GameObject g = Instantiate( modelList[(int)type] );
		g.transform.SetParent( modelBase );
	}

	/// <summary>
	/// 花束に追加
	/// </summary>
	public void Attatch(){

		// 花束にセット
		// エフェクト
	}

	/// <summary>
	/// 選択を解除
	/// </summary>
	public void Remove()
	{
		// オブジェクト削除
		// エフェクト
	}
}