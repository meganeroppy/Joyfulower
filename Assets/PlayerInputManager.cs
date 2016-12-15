using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// プレイヤーの入力を検出
/// </summary>
public class PlayerInputManager : MonoBehaviour {

	enum Hand{
		Left,
		Right,
	};

	public SteamVR_TrackedObject rightHand;
	public SteamVR_TrackedObject leftHand;

	private List<SteamVR_Controller.Device> hands;

	private TweeterSample tweet;
	private FlowerPicker picker;

	void Start()
	{
		hands = new List<SteamVR_Controller.Device>();
			
		var hand = SteamVR_Controller.Input((int) leftHand.index);
		if( hand != null )
		{
			hands.Add(hand);
		}

		hand = SteamVR_Controller.Input((int) rightHand.index);
		if( hand != null )
		{
			hands.Add(hand);
		}

		tweet = GetComponent<TweeterSample>();
		picker = GetComponent<FlowerPicker>();
	}
	
	// Update is called once per frame
	void Update () {
		for( int i=0 ; i<hands.Count ; i++ )
		{
			var hand = hands[i];
			if( hand != null )
			{			
				if (hand.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger)) {
					Debug.Log( ((Hand)i).ToString() + "のトリガーをちょい引き" );
				}
				if (hand.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
					Debug.Log( ((Hand)i).ToString() + "のトリガーをがっつり引き" );
				}
			}	
		}
	}
}
