using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlowerBaseGroup : MonoBehaviour {

	[SerializeField]
	private List<FlowerBase> child;

	// Use this for initialization
	void Start () {
		child.ForEach( f => 
			{
				FlowerBase.FlowerType seed = (FlowerBase.FlowerType)Random.Range(0, (int)FlowerBase.FlowerType.Count);
				f.SetFlower(seed);
			} );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
