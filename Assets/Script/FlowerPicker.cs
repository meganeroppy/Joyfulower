using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 花を摘む
/// </summary>
public class FlowerPicker : MonoBehaviour {

	/// <summary>
	/// 摘み可能範囲(m)
	/// </summary>
	public float pickRange = 5f;

	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.P) )
		{
			// 花を摘む
			TryPick(transform.position);
		}
	}

	/// <summary>
	/// 範囲内の花を摘む
	/// </summary>
	public bool TryPick( Vector3 originPos )
	{
		// 基準座標から最寄りの花を取得
		FlowerBase fb = FlowerBase.GetNearestFlower( originPos, pickRange );
		if( fb != null )
		{
			return fb.Pick();
		}

		return false;
	}
}
