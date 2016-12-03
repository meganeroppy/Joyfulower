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

	void Tweet()
	{
		FlowerBase fb = FlowerBase.GetNearestFloer(transform.position);
		if( fb != null )
		{
			fb.Bloom();
		}
	}
}
