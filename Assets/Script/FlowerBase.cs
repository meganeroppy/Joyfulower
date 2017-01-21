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

	public bool full
	{
		get{
			return energyList.Count >= energyToBloom;
		}
	}

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

	/// <summary>
	/// 花の寿命
	/// </summary>
	public float lifeTime = 60f;
	private float timer = 0;

	public static List<FlowerBase> fList;

	/// <summary>
	/// 実行中のTween
	/// </summary>
	Tween tween = null;

	void Start()
	{
		if( fList == null )
		{
			fList = new List<FlowerBase>();
		}
		fList.Add(this);

		energyList = new List<FlowerEnergy>();

		currentFlowerType = FlowerType.None;
	}

	void Update()
	{
		UpdateLifeTimer();
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
		if( timer <= 0 )
		{
			Die();
		}
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
		// すでに咲く条件を満たしていたらなにもしない
		if( energyList.Count >= energyToBloom )
		{
 			yield break;
		}

		energyList.Add(energy);

		Debug.Log( "[ " + fList.IndexOf(this).ToString() + " ]"  + "番目の花ポイントに [ " + energy.ToString() + " ] を加算 現在 ( " + energyList.Count.ToString() + " / " + energyToBloom.ToString() + " )"  );

		// ゲージ割合更新
		var rate = (float)energyList.Count / energyToBloom;

//		// もとより少ないときは一旦0
//		if( rate < gauge.fillAmount )
//		{
//			gauge.fillAmount = 0;
//		}

		while(tween != null) yield return null;

		// ゲージ増加演出
		tween = DOTween.To( () => gauge.fillAmount, v => gauge.fillAmount = v, rate, 0.5f).OnComplete( () => { tween = null;} );
		while(tween != null) yield return null;

		if( energyList.Count >= energyToBloom )
		{
			// 咲くのに必要なエナジーがたまったら花生成演出
			StartCoroutine( Bloom( GetMostContainedType() ) );
		}
	}

	/// <summary>
	/// 最も多く含有しているエナジータイプを調べる
	/// </summary>
	/// <returns>The ingredient.</returns>
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
	/// 花をセット
	/// </summary>
	/// <param name="type">Type.</param>
	private IEnumerator Bloom(FlowerType type)
	{		
		if( model != null )
		{
			Debug.Log("生成済み");
			yield break;
		}

		// 花モデルをセット
		int idx = (int)type;
		if(idx >= (int)FlowerType.Count){
			Debug.LogError("花タイプが無効");
			yield break;
		}

		if( tween != null )
		{
			yield break;
		}

		// ゲージアニメーション
		var canvas = gauge.transform.parent;
		tween = canvas.DOLocalMoveY( 0, 0.75f ).SetEase(Ease.InBack).OnComplete( () => tween = null );		
		while( tween != null ) yield return null;

		// 非表示
		gauge.enabled = false;

		if( model != null )
		{
			yield break;
		}

		model = GameObject.Instantiate<GameObject>(modelPrefab[idx]);
		model.transform.SetParent(modelBase);
		model.transform.localPosition = Vector3.zero;
		model.transform.localScale = Vector3.one;

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

		timer = lifeTime;

		energyList.Clear();

		// 現在の花タイプをセット
		currentFlowerType = type;
	}

	/// <summary>
	/// 花をセット 種類はランダム
	/// デバッグ用
	/// </summary>
	public void Bloom()
	{
		FlowerType seed = (FlowerType)Random.Range(0, (int)FlowerType.Count);
		StartCoroutine( Bloom(seed) );
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

		currentFlowerType = FlowerType.None;

		// ゲージを元に戻す
		var canvas = gauge.transform.parent;
		canvas.transform.localPosition = Vector3.up * 8;
		gauge.enabled = true;
		gauge.fillAmount = 0;
	}

	/// <summary>
	/// 花を摘む
	/// </summary>
	public void Pick()
	{
		if( model != null )
		{
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
