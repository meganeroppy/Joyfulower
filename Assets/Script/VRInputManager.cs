using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Viveのコントローラからの入力を検出
/// </summary>
public class VRInputManager : MonoBehaviour {

	enum HandType{
		Left,
		Right,
	};

	public SteamVR_TrackedObject rightHand;
	public SteamVR_TrackedObject leftHand;

	private List<SteamVR_Controller.Device> hands;

	private TweeterSample tweet;
	private FlowerPicker picker;

	/// <summary>
	/// キーボード操作用コントローラ
	/// </summary>
	public GameObject controllerNonVR;

	/// <summary>
	/// カメラリグ キーボード操作時は無効になる
	/// </summary>
	public GameObject cameraRig;

	public bool checkControllerVilidation = true;

	private bool initialized = false;

	void Start()
	{
		StartCoroutine ( StartLoad() );	
	}

	IEnumerator StartLoad()
	{
		if( !UnityEditor.PlayerSettings.virtualRealitySupported )
		{
			Debug.Log("virtualRealitySupportedが無効 キーボード操作モードに移行します");
			controllerNonVR.SetActive(true);
			cameraRig.SetActive(false);
			yield break;
		}
		while( checkControllerVilidation && !leftHand.gameObject.activeInHierarchy || !rightHand.gameObject.activeInHierarchy )
		{
			Debug.Log("Viveコントローラへの接続が確認ができるまで待機中");
			yield return null;
		}

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

		initialized = true;
		Debug.Log("Viveコントローラの初期化が完了");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!initialized) 
		{
			return;	
		}

		for( int i=0 ; i<hands.Count ; i++ )
		{
			var hand = hands[i];
			if( hand != null )
			{			
				if (hand.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) || Input.GetKeyDown(KeyCode.A)) {
					OnPressTrigger((HandType)i,  false);
				}
				if (hand.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
					OnPressTrigger((HandType)i, true);
				}

				if( hand.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
				{
					OnPressTouchPad((HandType)i);
				}
			}	
		}
	}

	/// <summary>
	/// コントローラのトリガーが引かれた
	/// </summary>
	/// <param name="type">右手または左手</param>
	/// <param name="half">半押しか？</param>
	private void OnPressTrigger( HandType type, bool half )
	{
		if( half ){
			Debug.Log( ((HandType)type).ToString() + "のトリガーがちょっと引かれた" );
		}else{
			Debug.Log( ((HandType)type).ToString() + "のトリガーががっつり引かれた" );
		}
		picker.TryPick();
	}

	/// <summary>
	/// コントローラのタッチパッドが押された
	/// </summary>
	/// <param name="type">Type.</param>
	private void OnPressTouchPad( HandType type )
	{
		Debug.Log( ((HandType)type).ToString() + "のタッチパッドが押された" );
		tweet.Tweet();
	}
}
