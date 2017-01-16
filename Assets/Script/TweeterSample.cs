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

	public void Tweet()
	{
		FlowerBase fb = FlowerBase.GetNearestFlower(transform.position);
		if( fb != null )
		{
			int key = Random.Range (0, (int)FlowerBase.FlowerType.Count);
			fb.AddEnergy((FlowerBase.FlowerType)key);
		}
	}
}
