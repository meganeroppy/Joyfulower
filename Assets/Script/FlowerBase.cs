using UnityEngine;
using System.Collections;

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
	public GameObject[] model;

	/// <summary>
	/// パーティクル
	/// </summary>
	public ParticleSystem particle;

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
	/// 花をセット
	/// </summary>
	/// <param name="type">Type.</param>
	public void SetFlower(FlowerType type)
	{		
		// 花モデルをセット
		int idx = (int)type;
		if(idx >= (int)FlowerType.Count){
			Debug.LogError("花タイプが無効");
			return;
		}
		GameObject f = GameObject.Instantiate<GameObject>(model[idx]);
		f.transform.SetParent(modelBase);
		f.transform.localPosition = Vector3.zero;
		f.transform.localScale = Vector3.one;

		// パーティクルをセット
		ParticleSystem s = GameObject.Instantiate<ParticleSystem>(particle);
		s.transform.SetParent( particleBase );
		s.transform.localPosition = Vector3.forward * posY;
		s.transform.localRotation = Quaternion.identity;
	}

}
