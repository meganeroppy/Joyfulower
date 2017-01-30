﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 花束作成クラス
/// </summary>
public class BouquetMaker : MonoBehaviour 
{
	enum TriggerState{
		Released,
		Pressed,
	};

	/// <summary>
	/// 現在のトリガー状態
	/// </summary>
	TriggerState currentTriggerState;

	/// <summary>
	/// 実体
	/// </summary>
	public static BouquetMaker instance;

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
	/// 花束の紙部分
	/// </summary>
	[SerializeField]
	GameObject bouquetPackagePrefab;
	GameObject bouquetPackage;

	/// <summary>
	/// 花パーツ
	/// </summary>
	List<GameObject> parts;

	/// <summary>
	/// 現在掴んでいるまたは掴み候補のパーツ
	/// </summary>
	private BouquetPart heldPart;

	/// <summary>
	/// 完成時のエフェクト
	/// </summary>
	public GameObject effect;

	/// <summary>
	/// 花を追加できる距離メートル
	/// </summary>
	public float validRange = 1f;

	[SerializeField]
	private AudioClip bouquetComp_se;

	[SerializeField]
	private AudioClip bouquetAttach_se;

	[SerializeField]
	private AudioClip fetchFlower_se;

	[SerializeField]
	private AudioClip makeBouquetParts_se;

	[SerializeField]
	private AudioClip switchBouquetMode_se;

	// Use this for initialization
	void Start () 
	{
		instance = this;

		currentTriggerState = TriggerState.Released;
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

	/// <summary>
	/// 掴む候補を更新
	/// </summary>
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
			heldPart  = tempHoldTarget;

			// 摑み候補を光らせる
			heldPart.Sparcle(true);
		}
		else
		{
			heldPart = null;
		}
	}

	/// <summary>
	/// 範囲内の花を掴む
	/// </summary>
	public void TryHold()
	{
		if (currentTriggerState.Equals (TriggerState.Pressed))
		{
			return;
		}
		currentTriggerState = TriggerState.Pressed;

		if( hand == null )
		{
			Debug.LogError("右手オブジェクトが未定義");
			return;
		}

		if( heldPart == null )
		{
			Debug.Log("摑み候補なし");
			return;
		}

		// 	掴み候補の親オブジェクトが自分の手の時
		if( heldPart.transform.parent != null && heldPart.transform.parent.Equals( hand ) )
		{
			Debug.Log("既に掴んでいるものがある");
			return;	
		}

		// 右手の子要素にセット
		heldPart.transform.SetParent( hand );
	}

	/// <summary>
	/// 掴んでいるものを離す
	/// </summary>
	public void Release()
	{
		currentTriggerState = TriggerState.Released;

		if( hand == null )
		{
			Debug.LogError("右手オブジェクトが未定義");
			return;
		}

		if( heldPart == null )
		{
			Debug.Log("摑み候補なし");
			return;
		}

		if( heldPart.transform.parent == null )
		{
			Debug.Log("掴んでいるものなし");
			return;	
		}

		// 花束ベースとの距離が近ければ花束ベースの子要素に加える
		float distance = Mathf.Abs( ( heldPart.transform.position - bouquetbase.position ).magnitude );
		if( distance <= validRange )
		{
			heldPart.Attatch( bouquetbase );

			// se
			bouquetbase.GetComponent<AudioSource>().PlayOneShot( bouquetAttach_se );
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

		// 所持している花を並べる
		for( int i=0 ; i < SceneManager.instance.fList ; i++ )
		{
			var flower = SceneManager.instance.fList[i];
			for( int j = 0 ; j < flower.count ; j++ )
			{
				var b = Instantiate( bouquetPartPrefab ).GetComponent<BouquetPart>();
				var type = flower.flowerType;
				b.Create( type );

				// オフセットを指定
				b.transform.position = transform.TransformPoint ( Vector3.forward * 1f );
			}
		}

		// TODO: 演出

		// 左手に花束の紙部分を持たせる
		if( bouquetPackage == null)
		{
			bouquetPackage = Instantiate(bouquetPackagePrefab) as GameObject;
			bouquetPackage.transform.SetParent( bouquetbase, false );
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
		BouquetPart.bList.Clear ();

		// 花束の紙を削除
		if( bouquetPackage != null )
		{
			Destroy( bouquetPackage );
		}
	}

	/// <summary>
	/// 花束完成
	/// </summary>
	public void CompleteBouquet()
	{
		Debug.Log("花束完成");

		// エフェクト
		GameObject g = GameObject.Instantiate(effect);
		g.transform.SetParent( bouquetbase );
		g.transform.localPosition = Vector3.zero;
		g.transform.localRotation = Quaternion.identity;

		// se 
		bouquetbase.GetComponent<AudioSource>().PlayOneShot(bouquetComp_se);
	}
}
