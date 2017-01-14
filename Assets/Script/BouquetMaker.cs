using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 花束作成クラス
/// </summary>
public class BouquetMaker : MonoBehaviour 
{
	/// <summary>
	/// 実体
	/// </summary>
	public static BouquetMaker instance;

	/// <summary>
	/// 摑み候補
	/// </summary>
	private BouquetPart holdTarget;

	/// <summary>
	/// 掴む手
	/// </summary>
	[SerializeField]
	private Transform hand;

	/// <summary>
	/// 花束ベース
	/// </summary>
	[SerializeField]
	Transform bouquetbase;

	/// <summary>
	/// 花パーツ
	/// </summary>
	List<GameObject> parts;

	/// <summary>
	/// 現在掴んでいる花パーツ
	/// </summary>
	private BouquetPart heldPart;

	/// <summary>
	/// 花を追加できる距離メートル
	/// </summary>
	public float validRange = 1f;

	// Use this for initialization
	void Start () 
	{
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void UpdateHoldTarget()
	{
		// 摑み候補
		BouquetPart tempHoldTarget = null;

		// 摑み対象を判別
		for( int i=0 ; i < BouquetPart.bList.Count ; i++ )
		{
			var b = BouquetPart.bList[i];

			// 摑み範囲外だったらスキップ
			float distance = Mathf.Abs( ( b.transform.position - hand.position ).magnitude );
			if( distance > validRange )
			{
				continue;
			}

			// 摑み候補なければこれが摑み候補
			if( tempHoldTarget == null )
			{
				tempHoldTarget = b;
				continue;
			}

			// 現在の摑み候補とこれの距離
			float distance2 = Mathf.Abs( ( tempHoldTarget.transform.position - hand.position ).magnitude );

			// 摑み候補より近かったら摑み候補を更新
			if( distance < distance2 )
			{
				tempHoldTarget = b;
			}
		}

		// 一旦すべてのパーツの摑めるエフェクト解除
		for( int i=0 ; i < BouquetPart.bList.Count ; i++ )
		{			
			var b = BouquetPart.bList[i];
			b.Sparcle( false );
		}

		//　摑み候補があれば登録
		if( tempHoldTarget != null )
		{			
			holdTarget  = tempHoldTarget;

			// 摑み候補を光らせる
			holdTarget.Sparcle(true);
		}
		else
		{
			holdTarget = null;
		}
	}

	/// <summary>
	/// 範囲内の花を掴む
	/// </summary>
	public void TryHold()
	{
		if( hand == null )
		{
			Debug.LogError("右手オブジェクトが未定義");
			return;
		}

		if( holdTarget == null )
		{
			Debug.Log("摑み候補なし");
			return;
		}

		if( holdTarget.transform.parent.Equals( hand ) )
		{
			Debug.Log("既に掴んでいるものがある");
			return;	
		}

		// 右手の子要素にセット
		holdTarget.transform.SetParent( hand );
	}

	/// <summary>
	/// 掴んでいるものを離す
	/// </summary>
	public void Release()
	{
		if( hand == null )
		{
			Debug.LogError("右手オブジェクトが未定義");
			return;
		}

		if( holdTarget == null )
		{
			Debug.Log("摑み候補なし");
			return;
		}

		if( !holdTarget.transform.parent.Equals( hand ) )
		{
			Debug.Log("掴んでいるものなし");
			return;	
		}

		// 花束ベースとの距離が近ければ花束ベースの子要素に加える
		float distance = Mathf.Abs( ( heldPart.transform.position - bouquetbase.position ).magnitude );
		if( distance <= validRange )
		{
			heldPart.Attatch( bouquetbase );
		}
		else
		{
			heldPart.Release();
		}
	}
}
