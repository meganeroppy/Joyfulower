using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
/// <summary>
/// 花が咲くポイント
/// </summary>
public class FlowerBase : MonoBehaviour {

	/// <summary>
	/// 花のタイプ
	/// </summary>
	public enum FlowerType
	{
		Yellow,
		Red,
		Pink,
		Purple,
		Rose,
		Sunflower,
		Count,
		None,
	}

	/// <summary>
	/// 現在の花タイプ
	/// </summary>
	private FlowerType currentFlowerType;

	/// <summary>
	/// 花エナジー
	/// </summary>
	public class FlowerEnergy{
		public FlowerType type;
		public FlowerEnergy(FlowerType type)
		{
			this.type = type;
		}
	}

	/// <summary>
	/// 所持エナジー
	/// タイプ別に保持する
	/// 必要エナジーに達した時、含有栗の最も多いタイプの花がさく
	/// </summary>
	private List<FlowerEnergy> energyList;

	/// <summary>
	/// 開花に必要なエナジー
	/// </summary>
	private const int energyToBloom = 6;

	/// <summary>
	/// 花のモデルリスト
	/// </summary>
	public GameObject[] modelPrefab;
	private GameObject model;
	/// <summary>
	/// パーティクル
	/// </summary>
	public ParticleSystem particlePrefab;
	private ParticleSystem particle;

	/// <summary>
	/// パーティクルベース
	/// </summary>
	public Transform particleBase;

	/// <summary>
	/// モデルベース
	/// </summary>
	public Transform modelBase;

	/// <summary>
	/// 花のモデル生成ローカル高度
	/// </summary>
	private const float posY = 1f;

	/// <summary>
	/// 生成時のエフェクト
	/// </summary>
	public GameObject effect;

	/// <summary>
	/// エネルギーゲージ
	/// </summary>
	[SerializeField]
	private Image gauge;

	[SerializeField]
	private List<Sprite> gaugeSprite;

	/// <summary>
	/// 花の寿命
	/// </summary>
	public float lifeTime = 60f;
	private float timer = 0;

	/// <summary>
	/// 花が咲いているか？
	/// </summary>
	bool blooming = false;

	/// <summary>
	/// 演出の最中か？
	/// </summary>
	bool isExpGoing = false;

	public static List<FlowerBase> fList;

	/// <summary>
	/// ゲージ増加演出待ち数
	/// </summary>
	private int expWaitCount = 0;

	private AudioSource myAudio;

	[SerializeField]
	private AudioClip bloom_se;

	[SerializeField]
	private AudioClip wether_se;

	[SerializeField]
	private AudioClip picked_se;

	[SerializeField]
	private AudioClip gainGauge_se;

	/// <summary>
	/// 実行中のTween
	/// </summary>
	Tween tween = null;

	void Awake()
	{
		myAudio = GetComponent<AudioSource>();

		if( fList == null )
		{
			fList = new List<FlowerBase>();
		}
		fList.Add(this);

		energyList = new List<FlowerEnergy>();

		currentFlowerType = FlowerType.None;

		// 最初はゲージが空
		gauge.fillAmount = 0;
	}

	void Update()
	{
		UpdateLifeTimer();

		if (expWaitCount >= 1 && !isExpGoing) {
			StartCoroutine ( ShowGainExp() );
		}
	}

	/// <summary>
	/// タイマー更新
	/// </summary>
	void UpdateLifeTimer()
	{
		if( timer <= 0 )
		{
			return;
		}

		timer -= Time.deltaTime;
		if( timer <= 0 && model != null)
		{
			StartCoroutine( Wither() );
		}
	}

	/// <summary>
	/// 花が枯れる演出
	/// </summary>
	IEnumerator Wither()
	{
		if( tween != null )
		{
			yield break;
		}

		// 徐々に小さくなる
		tween = model.transform.DOScale(0, 0.75f).OnComplete( () => tween = null );
		while( tween != null ) yield return null;

		// SE
		myAudio.PlayOneShot(wether_se);

		// 破棄
		Die();
	}

	/// <summary>
	/// 最寄りのブルームポイントを返す
	/// </summary>
	/// <param name="myPos">自分の位置</param>
	/// <param name="pickRange">自分の位置基準の検索範囲 指定しなければ範囲無限</param>
	public static FlowerBase GetNearestFlower( Vector3 myPos, float range=float.MaxValue)
	{
		List<FlowerBase> inRange = new List<FlowerBase>();

		foreach( FlowerBase fb in FlowerBase.fList )
		{
			float distance = Mathf.Abs( (fb.transform.position - myPos).magnitude );
			if( distance <= range )
			{
				inRange.Add( fb );
			}
		}

		if( inRange.Count == 0 )
		{
			Debug.Log("範囲内に花なし");
			return null;
		}

		int nearestIdx = 0;
		float num = float.MaxValue;

		for( int i = 0 ; i < inRange.Count ; i++ )
		{
			FlowerBase fb = inRange[i];
			float distance = Mathf.Abs( (fb.transform.position - myPos).magnitude );
			if( distance < num )
			{
				num = distance;
				nearestIdx = i;
			}
		}

		return inRange[ nearestIdx ];
	}

	/// <summary>
	/// エナジーを付与
	/// </summary>
	public IEnumerator AddEnergy( FlowerEnergy energy )
	{ 
		// 咲いていたらなにもしない
		if( energyList.Count >= energyToBloom )
		{
			Debug.Log ( gameObject.name + "はエナジーが満タン");
 			yield break;
		}

		// エナジー追加
		energyList.Add(energy);

		// 現在の花タイプをセット
		currentFlowerType = GetMostContainedType();

		// 増加演出順番待ち数を増加
		expWaitCount++;

		Debug.Log( "[ " + fList.IndexOf(this).ToString() + " ]"  + "番目の花ポイントに [ " + energy.type.ToString() + " ] を加算 現在 ( " + energyList.Count.ToString() + " / " + energyToBloom.ToString() + " )"  );
	}

	/// <summary>
	/// ゲージ増加演出
	/// </summary>
	/// <returns>The gain exp.</returns>
	IEnumerator ShowGainExp()
	{
		isExpGoing = true;

		while (expWaitCount > 0) 
		{
			expWaitCount--;

			// 増加後のゲージ割合更新
			var rate = (float)(energyList.Count-expWaitCount) / energyToBloom;
			// 演出の最中だったら待つ
			while(tween != null) yield return null;
			// se 
			myAudio.PlayOneShot( gainGauge_se );
			// ゲージ増加演出
			tween = DOTween.To( () => gauge.fillAmount, v => gauge.fillAmount = v, rate, 0.5f).OnComplete( () =>
				{
					tween = null;
				} );
			// 演出の最中だったら待つ
			while(tween != null) yield return null;

			// 咲きフラグ
			if( rate >= 1 )
			{
				yield return StartCoroutine( Bloom() );
			}
		}

		isExpGoing = false;
	}

	/// <summary>
	/// 最も多く含有しているエナジータイプを調べる
	/// </summary>
	private FlowerType GetMostContainedType()
	{
		var counter = new List<int>();

		for( int i=0 ; i < (int)FlowerType.Count ; i++)
		{
			counter.Add(0);
		}

		// リストに分けて保存
		for( int i=0 ; i< energyList.Count ; i++){
			var idx = energyList[i].type;
			if( (int)idx >= counter.Count )
			{
				Debug.LogWarning("花タイプが不正 指定タイプ:" + idx.ToString() );
				continue;
			}
			counter[(int)idx]++;
		}
				
		int largestIdx = 0;

		for( int i=0 ; i< counter.Count ; i++)
		{
			if( counter[i] > counter[ largestIdx ] )
			{
				largestIdx = i;
			}
		}

		// 同着インデックスリスト
		var sameAmounctIdx = new List<int>();
		for( int i=0 ; i< counter.Count ; i++)
		{
			if( counter[i] == counter[ largestIdx ] )
			{
				sameAmounctIdx.Add(i);
			}
		}

		// 同着がいたらランダム
		if( sameAmounctIdx.Count > 0 )
		{
			sameAmounctIdx.Add( largestIdx );
			int idx = Random.Range(0, sameAmounctIdx.Count);

			return (FlowerType)sameAmounctIdx[ idx ]; 
		}
		else
		{
			return (FlowerType)largestIdx;
		}	
	}


	/// <summary>
	/// 花生成演出
	/// </summary>
	private IEnumerator Bloom()
	{		
		// 花モデルを生成
		model = GameObject.Instantiate<GameObject>(modelPrefab[(int)currentFlowerType]);
		model.transform.SetParent(modelBase);
		model.transform.localPosition = Vector3.zero;
		model.transform.localRotation = Quaternion.identity;
		model.transform.localScale = Vector3.zero;

		// 名前セット
		model.name = "Flower_0" + ((int)currentFlowerType+1).ToString();

		// ゲージスプライト差し替え
		gauge.sprite = gaugeSprite[1];

		// ゲージアニメーション
		tween = gauge.transform.DOScale(Vector3.one * 0.35f, 0.25f).OnComplete( () => tween = null );
		while( tween != null ) yield return null;
		var canvas = gauge.transform.parent;
		tween = canvas.DOLocalMoveY( 0, 0.75f ).SetEase(Ease.InBack).SetDelay(0.25f).OnComplete( () => tween = null );		
		while( tween != null ) yield return null;

		// ゲージ非表示
		gauge.enabled = false;
			
		// 花が生成されるアニメ
		model.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

		// パーティクルをセット
		particle = GameObject.Instantiate<ParticleSystem>(particlePrefab);
		particle.transform.SetParent( particleBase );
		particle.transform.localPosition = Vector3.forward * posY;
		particle.transform.localRotation = Quaternion.identity;

		// 生成エフェクト
		GameObject g = GameObject.Instantiate(effect);
		g.transform.SetParent( particleBase );
		g.transform.localPosition = Vector3.forward * posY;
		g.transform.localRotation = Quaternion.identity;

		// SE
		myAudio.PlayOneShot(bloom_se);

		timer = lifeTime;

		blooming = true;
	}
		
	/// <summary>
	/// 花のモデルとパーティクルを削除
	/// </summary>
	private void Die()
	{
		if( model != null )
		{
			Destroy( model );
		}

		if( particle != null )
		{
			Destroy( particle.gameObject );
		}

		// エナジーリスト初期化
		energyList.Clear();

		blooming = false;

		currentFlowerType = FlowerType.None;

		// ゲージを元に戻す
		var canvas = gauge.transform.parent;
		canvas.transform.localPosition = Vector3.up * 8;
		gauge.enabled = true;
		gauge.fillAmount = 0;
		gauge.transform.localScale = Vector3.one;
		gauge.sprite = gaugeSprite[0];
	}

	/// <summary>
	/// 花を摘む
	/// </summary>
	public void Pick()
	{
		if( blooming )
		{
			if (currentFlowerType.Equals (FlowerType.None)) {
				return;
			}

			// UIに反映
//			if (GameDirector.instance != null) {
//				GameDirector.instance.CountObj (model.gameObject.name);
//			}

			// 花アイテム情報をリストに追加
			// 同じ花タイプのアイテムがすでにリストに入っているかチェック
			var fItem = SceneManager.instance.fList.Find( f => f.flowerType.Equals( currentFlowerType ) );

			if( fItem != null )
			{
				
				// 入っていたらカウントを加算
				fItem.count++;
				Debug.Log( fItem.flowerType + "の数を更新[ " + fItem.count.ToString() + " ]"); 
			}
			else
			{
				// 入っていなかったら新しく追加する
				fItem = new SceneManager.FlowerItem();
				fItem.flowerType = currentFlowerType;
				fItem.name = model.name;
				fItem.count = 1;
				SceneManager.instance.fList.Add( fItem );
				Debug.Log( fItem.flowerType + "を新しく追加");
			}

			// UI更新
			GameDirector.instance.SetUI();

			//se 
			myAudio.PlayOneShot(picked_se);

			// モデルを削除
			Die();

			// エフェクト
			GameObject g = GameObject.Instantiate(effect);
			g.transform.SetParent( particleBase );
			g.transform.localPosition = Vector3.forward * posY;
			g.transform.localRotation = Quaternion.identity;
		}
	}
}
