using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 花が咲くポイント
/// </summary>
public class FlowerBase : MonoBehaviour {

	/// <summary>
	/// 花のタイプ
	/// </summary>
	public enum FlowerType
	{
		Red,
		White,
		Purple,
		Blue,
		Count,
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
	/// 花の寿命
	/// </summary>
	public float lifeTime = 15f;
	private float timer = 0;

	public static List<FlowerBase> fList;

	void Start()
	{
		if( fList == null )
		{
			fList = new List<FlowerBase>();
		}
		fList.Add(this);
	}

	public static FlowerBase GetNearestFloer( Vector3 myPos, float pickRange=float.MaxValue)
	{
		List<FlowerBase> inRange = new List<FlowerBase>();

		foreach( FlowerBase fb in FlowerBase.fList )
		{
			float distance = Mathf.Abs( (fb.transform.position - myPos).magnitude );
			if( distance <= pickRange )
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
	/// 花をセット
	/// </summary>
	/// <param name="type">Type.</param>
	public void Bloom(FlowerType type)
	{		
		if( model != null )
		{
			Debug.Log("生成済み");
			return;
		}

		// 花モデルをセット
		int idx = (int)type;
		if(idx >= (int)FlowerType.Count){
			Debug.LogError("花タイプが無効");
			return;
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

		StartCoroutine( WaitAndDie() );
	}

	/// <summary>
	/// 花をセット 種類はランダム
	/// </summary>
	public void Bloom()
	{
		FlowerType seed = (FlowerType)Random.Range(0, (int)FlowerType.Count);
		Bloom(seed);
	}
	/// <summary>
	/// 一定時間後に花を削除
	/// </summary>
	/// <returns>The and die.</returns>
	IEnumerator WaitAndDie()
	{
		while(true)
		{
			timer++;
			if( model == null )
			{
				yield break;
			}
			else if( timer >= lifeTime )
			{
				break;
			}
			yield return new WaitForSeconds(1);
		}

		Destroy( model );
		Destroy( particle.gameObject );

		timer = 0;
	}

	public void Pick()
	{
		Destroy( model );
		Destroy( particle.gameObject );

		// エフェクト
		// 生成エフェクト
		GameObject g = GameObject.Instantiate(effect);
		g.transform.SetParent( particleBase );
		g.transform.localPosition = Vector3.forward * posY;
		g.transform.localRotation = Quaternion.identity;
	}

}
