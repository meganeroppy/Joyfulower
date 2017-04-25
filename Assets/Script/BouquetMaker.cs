using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

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
	private Transform hand = null;

	/// <summary>
	/// 花束パーツプレハブ
	/// TODO: 咲いてる用とパーツ用にマテリアル&プレハブを別々に用意 後者には葉っぱなしマテリアルを使用する
	/// </summary>
	[SerializeField]
	BouquetPart bouquetPartPrefab;

	/// <summary>
	/// 花束ベース
	/// </summary>
	Transform bouquetBase = null;

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
	/// 現在掴んでいるパーツ
	/// </summary>
	private BouquetPart heldPart;

	/// <summary>
	/// 掴み候補パーツ
	/// </summary>
	private BouquetPart heldTargetPart;

	/// <summary>
	/// 完成時のエフェクト
	/// </summary>
	public GameObject effect;

	/// <summary>
	/// 花を掴める距離メートル
	/// </summary>
	public float pickRange = 1f;

	/// <summary>
	/// 花を追加できる距離メートル
	/// </summary>
	public float attachRange = 1f;

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
		if( hand == null )
		{
			return;
		}

		// 摑み候補
		BouquetPart tempHoldTarget = null;

		if (BouquetPart.bList == null) 
		{
			BouquetPart.bList = new List<BouquetPart> ();
		}

		// 摑み対象を判別
		for( int i=0 ; i < BouquetPart.bList.Count ; i++ )
		{
			var b = BouquetPart.bList[i];

			// 摑み範囲外だったらスキップ
			float distance = Mathf.Abs( ( b.transform.position - hand.position ).magnitude );
			if( distance > pickRange )
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
			heldTargetPart  = tempHoldTarget;

			// 摑み候補を光らせる
			heldTargetPart.Sparcle(true);
		}
		else
		{
			heldTargetPart = null;
		}
	}

	/// <summary>
	/// 範囲内の花を掴む
	/// 花束ベースの手で行われた際は花束を完成
	/// </summary>
	public void TryHold( Transform holdHand )
	{
		if (currentTriggerState.Equals (TriggerState.Pressed))
		{
			return;
		}

		if( hand == null || bouquetBase == null)
		{
			Debug.LogError( "手、花束ベース、または両方が未定義" );
			return;
		}

		if( holdHand.Equals( bouquetBase ) )
		{
			// 花束完成
			CompleteBouquet();
			return;
		}
		
		currentTriggerState = TriggerState.Pressed;

		if( heldTargetPart == null )
		{
			Debug.Log("摑み候補なし");
			return;
		}

		// 掴んでいるものがある
		if( heldPart != null )
		{
			Debug.Log("既に掴んでいるものがある");
			return;	
		}

		// 右手の子要素にセット
		heldTargetPart.transform.SetParent( hand );
		heldPart = heldTargetPart;
	}

	/// <summary>
	/// 掴んでいるものを離す
	/// </summary>
	public void Release( Transform holdHand )
	{
		currentTriggerState = TriggerState.Released;

		if( hand == null || bouquetBase == null )
		{
			Debug.LogError( "手、花束ベース、または両方が未定義" );
			return;
		}

		if( !holdHand.Equals( hand ) )
		{
			Debug.Log( "掴む手ではないのでなにもしない" );
			return;
		}

		if( heldPart == null )
		{
			Debug.Log("掴んでいるものなし");
			return;	
		}

		// 花束ベースとの距離が近ければ花束ベースの子要素に加える
		float distance = Mathf.Abs( ( heldPart.transform.position - bouquetBase.position ).magnitude );
		if( distance <= attachRange )
		{
			heldPart.Attatch( bouquetBase );
			heldPart = null;
			// se
			bouquetBase.GetComponent<AudioSource>().PlayOneShot( bouquetAttach_se );
		}
		else
		{
			heldPart.Release();
			heldPart = null;
		}
	}

	[SerializeField]
	private AudioClip bouquetStart_se;

	/// <summary>
	/// 種類ごとに並ぶ花の数上限
	/// </summary>
	public int maxLinedFlowerNum = 10;

	private float[] posxList = new float[6]{0.5f, 1f, 0.5f, -0.5f, -1f, -0.5f}; 

	private float[] poszList = new float[6]{1f, 0, -1f, -1f, 0, 1f};

	/// <summary>
	/// 所持している花束パーツを並べる
	/// TODO: 花束の紙にClothを使ってみる
	/// TODO: 拾った時の振動をチェック
	/// TODO: 生えているときは葉っぱありテクスチャ、花束パーツになったら葉っぱなしテクスチャ
	/// TODO: 花のスケール感にもっとリアリティを
	/// </summary>
	public void CreateBouquetParts( Transform bouquetBase, Transform hand )
	{
		// 花束ベースを登録
		this.bouquetBase = bouquetBase;

		// 掴む手を登録
		this.hand = hand;

		Debug.Log("花束関連オブジェクト生成");

		// 所持している花をグリッド状に並べる

		// グリッドの中心点
		var centerPos = hand.transform.position;
		var unit = 0.25f;

		for( int i=0 ; i < SceneManager.instance.fList.Count ; i++ )
		{			
			var posY = centerPos.y;	
//			var posY = 0;	

			var flower = SceneManager.instance.fList[i];
			for( int j = 0 ; j < flower.count ; j++ )
			{
				if( j >= maxLinedFlowerNum )
				{
					Debug.Log( maxLinedFlowerNum.ToString() + "輪以上の花は並べない" );
					break;
				}
					
				var posX = centerPos.x + posxList[i] + ( posxList[j % posxList.Length] * unit);
				var posZ = centerPos.z + poszList[i] + ( poszList[j % poszList.Length] * unit); 
//				var posX = posxList[i] + ( posxList[j % posxList.Length] * unit);
//				var posZ = poszList[i] + ( poszList[j % poszList.Length] * unit); 

				var b = Instantiate( bouquetPartPrefab ).GetComponent<BouquetPart>();
				b.transform.position = centerPos;
				var type = flower.flowerType;

				b.Create( type, new Vector3( posX, posY, posZ ), hand );
//				b.Create( type, hand.TransformPoint( new Vector3( posX, posY, posZ ) ), hand );

			}
		}

		// SE
		this.bouquetBase.GetComponent<AudioSource>().PlayOneShot(bouquetPartsGone_se);

		// 左手に花束の紙部分を持たせる
		if( bouquetPackage == null)
		{
			bouquetPackage = Instantiate(bouquetPackagePrefab) as GameObject;
            bouquetPackage.transform.SetParent( this.bouquetBase, false );
		}
	}

	[SerializeField]
	private AudioClip bouquetPartsGone_se;

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

		// SE
		bouquetBase.GetComponent<AudioSource>().PlayOneShot(bouquetPartsGone_se);

		bouquetBase = null;
		hand = null;
	}

	[SerializeField]
	private SpriteRenderer whiteScreen;

	/// <summary>
	/// 花束完成
	/// </summary>
	public void CompleteBouquet()
	{
		Debug.Log("花束完成");

		// エフェクト
		GameObject g = GameObject.Instantiate(effect);
		g.transform.SetParent( bouquetBase );
		g.transform.localPosition = Vector3.zero;
		g.transform.localRotation = Quaternion.identity;

		// se 
		bouquetBase.GetComponent<AudioSource>().PlayOneShot(bouquetComp_se);
		var now = System.DateTime.Now.ToString ();
		now = now. Replace ("/", "").Replace (" ", "").Replace (":", "");

		Application.CaptureScreenshot ( "JoyfulowerScreenShot" + now + ".png" );

		StartCoroutine( Flash() );
	}


	Tween tween;
	/// <summary>
	/// カメラのフラッシュ
	/// </summary>
	private IEnumerator Flash()
	{
		tween = whiteScreen.DOColor(Color.white, 0.25f).OnComplete( () => tween = null );
		while( tween != null ) yield return null;
		tween = whiteScreen.DOColor(Color.clear, 0.75f).OnComplete( () => tween = null );
		while( tween != null ) yield return null;
	}
}