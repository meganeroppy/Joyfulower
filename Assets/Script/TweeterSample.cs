using UnityEngine;
using System.Collections;

public class TweeterSample : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.T) )
		{
			// 花を摘む
			Tweet();
		}
	}

	public void Tweet(SteamVR_TrackedObject hand=null)
	{
		if (hand != null) {
			Debug.Log ( "自分の座標は" + transform.position.ToString() + "自分のローカル座標は" + transform.localPosition.ToString() + "  handの座標は" + hand.transform.position.ToString());
		}

		var selfPos = hand != null ? hand.transform.position : transform.position;
		FlowerBase fb = FlowerBase.GetNearestFlower(selfPos);
		if( fb != null )
		{
			var key = Random.Range (0, (int)FlowerBase.FlowerType.Count);
			var energy = new FlowerBase.FlowerEnergy((FlowerBase.FlowerType)key);
			StartCoroutine( fb.AddEnergy(energy) );
		}
	}
}
