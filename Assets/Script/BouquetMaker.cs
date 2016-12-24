using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 花束作成クラス
/// </summary>
public class BouquetMaker : MonoBehaviour {

	/// <summary>
	/// 花束ベース
	/// </summary>
	Transform bouquetbase;

	/// <summary>
	/// 花パーツ
	/// </summary>
	List<GameObject> parts;

	/// <summary>
	/// 現在掴んでいる花パーツ
	/// </summary>
	GameObject heldPart;

	/// <summary>
	/// 花を追加できる距離メートル
	/// </summary>
	public float validRange = 1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
