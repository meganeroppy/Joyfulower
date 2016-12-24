using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 花を摘む
/// </summary>
public class FlowerPicker : MonoBehaviour {

	public float pickRange = 5f;

	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.P) )
		{
			// 花を摘む
			TryPick();
		}
	}

	/// <summary>
	/// 範囲内の花を摘む
	/// </summary>
	public void TryPick()
	{
		FlowerBase fb = FlowerBase.GetNearestFloer( this.transform.position, pickRange );
		if( fb != null )
		{
			fb.Pick();
		}
	}
}
