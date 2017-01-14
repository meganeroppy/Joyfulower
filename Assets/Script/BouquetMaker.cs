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
	/// 花束パーツプレハブ
	/// </summary>
	[SerializeField]
	BouquetPart bouquetPartPrefab;

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
	void Update () 
	{
		// 花束作成シーンのみ摑みターゲットの更新処理
		if( SceneManager.instance.sceneType.Equals( SceneManager.SceneType.Bouquet ) )
		{
			UpdateHoldTarget();
		}
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

	/// <summary>
	/// 所持している花束パーツを並べる
	/// </summary>
	public void CreateBouquetParts()
	{
		Debug.Log("花束関連オブジェクト生成");

		// TODO: 所持している花の種類分並べる
		for( int i=0 ; i < 10 ; i++ )
		{
			var b = Instantiate( bouquetPartPrefab ).GetComponent<BouquetPart>();
			var type = Random.Range(0, (int)BouquetPart.FlowerType.Count);
			b.Create( (BouquetPart.FlowerType)type );
		}
	}

	/// <summary>
	/// 表示中の花束関連オブジェクトを破棄
	/// </summary>
	public void RemoveBouquetParts()
	{
		Debug.Log("花束関連オブジェクト破棄");

		// 花束パーツの削除
		for( int i=BouquetPart.bList.Count-1 ; i >= 0 ; i-- )
		{
			var b = BouquetPart.bList[i];
			Destroy(b.gameObject);
		}
	}

	/// <summary>
	/// 花束完成
	/// </summary>
	public void CompleteBouquet()
	{
		Debug.Log("花束完成");
	}
}
