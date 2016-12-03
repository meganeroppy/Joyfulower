using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlowerPicker : MonoBehaviour {

	public float pickRange = 2f;

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
	void TryPick()
	{
		List<FlowerBase> inRange = new List<FlowerBase>();

		foreach( FlowerBase fb in FlowerBase.fList )
		{
			float distance = Mathf.Abs( (fb.transform.position - this.transform.position).magnitude );
			if( distance <= pickRange )
			{
				inRange.Add( fb );
			}

		}

		if( inRange.Count == 0 )
		{
			Debug.Log("範囲内に花なし");
			return;
		}

		int nearestIdx = 0;
		float num = float.MaxValue;

		for( int i = 0 ; i < inRange.Count ; i++ )
		{
			FlowerBase fb = inRange[i];
			float distance = Mathf.Abs( (fb.transform.position - this.transform.position).magnitude );
			if( distance < num )
			{
				num = distance;
				nearestIdx = i;
			}
		}

		inRange[ nearestIdx ].Pick();

	}
}
