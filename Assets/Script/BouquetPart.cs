using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
/// <summary>
/// 花束を構成するパーツ
/// </summary>
public class BouquetPart : MonoBehaviour {

	public static List<BouquetPart> bList;

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
	/// 初期位置
	/// </summary>
	private Vector3 originPos;

	/// <summary>
	/// 初期回転
	/// </summary>
	private Quaternion originRot;

	/// <summary>
	/// 生成
	/// </summary>
	public void Create( FlowerBase.FlowerType type, Vector3 position )
	{
		if( (int)type >= modelList.Count )
		{
			Debug.LogError( type.ToString() + "が未定義");
			return;
		}
		
		GameObject obj = Instantiate( modelList[(int)type] );
		obj.transform.SetParent( modelBase );

		// リストに自身を追加
		if( bList == null )
		{
			bList = new List<BouquetPart>();
		}
		bList.Add(this);

		// 初期位置をセット
		originPos = position;

		CreateExp(position);
	}

	/// <summary>
	/// 作成演出
	/// 左手から花束パーツが飛び出す
	/// </summary>
	private void CreateExp( Vector3 endPos )
	{
		transform.localPosition = Vector3.zero;

		transform.DOMove( endPos, 0.75f );
	}

	/// <summary>
	/// つかめる状態にあることを見た目で表現
	/// </summary>
	public void Sparcle( bool key )
	{
		// 摑み可能状態エフェクトの切り替え
		// effect.enable = key;
	}

	/// <summary>
	/// 花束に追加
	/// </summary>
	public void Attatch( Transform parent ){
		// 花束にセット
		transform.SetParent( parent );

		Debug.Log (transform.name + "を花束にセット");
		// エフェクト
	}

	/// <summary>
	/// 離された
	/// </summary>
	public void Release()
	{
		transform.parent = null;
		// エフェクト

		//初期位置に戻る
		transform.position = originPos;

		// 初期回転に戻る
		transform.rotation = originRot;
	}
}