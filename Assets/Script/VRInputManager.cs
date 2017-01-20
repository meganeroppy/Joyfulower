using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Viveのコントローラからの入力を検出
/// </summary>
public class VRInputManager : MonoBehaviour {

	/// <summary>
	/// 0=Left 1=Right
	/// </summary>
	public enum HandType{
		Left,
		Right,
	};

	public SteamVR_TrackedObject rightHand;
	public SteamVR_TrackedObject leftHand;

	private List<SteamVR_Controller.Device> hands;

	private TweeterSample tweet;
	private FlowerPicker picker;
	private BouquetMaker bouquetMaker;

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
#if UNITY_EDITOR
		if( !UnityEditor.PlayerSettings.virtualRealitySupported )
		{
			Debug.Log("virtualRealitySupportedが無効 キーボード操作モードに移行します");
			controllerNonVR.SetActive(true);
			cameraRig.SetActive(false);
			yield break;
		}
#endif

		int totalWaitSec = 0;
		int maxWaitCount = 15;
		while( checkControllerVilidation && !leftHand.gameObject.activeInHierarchy || !rightHand.gameObject.activeInHierarchy )
		{
			Debug.Log("Viveコントローラへの接続が確認ができるまで待機中 ["  +  totalWaitSec.ToString() + " / " + maxWaitCount.ToString() + " ]");
			yield return new WaitForSeconds(1);

			if( ++totalWaitSec > maxWaitCount )
			{
				Debug.Log(maxWaitCount.ToString() + "秒待ってもViveコントローラへの接続が確認できなかったので、キーボード捜査モードに移行します。");
				controllerNonVR.SetActive(true);
				cameraRig.SetActive(false);
				yield break;
			}
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
		bouquetMaker = GetComponent<BouquetMaker>();

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
				if ( hand.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) )
				{
					OnReleaseTrigger((HandType)i);
				}
					
				if( hand.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
				{
					OnPressTouchPad((HandType)i);
				}

				if( hand.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
				{
					OnPressGrip((HandType)i);
				}		
			}	
		}
	}

	/// <summary>
	/// コントローラのトリガーが引かれた
	/// </summary>
	/// <param name="type">右手または左手</param>
	/// <param name="half">半押しか？</param>
	private void OnPressTrigger( HandType handType, bool half )
	{
		if( half ){
			Debug.Log( ((HandType)handType).ToString() + "のトリガーがちょっと引かれた" );
		}else{
			Debug.Log( ((HandType)handType).ToString() + "のトリガーががっつり引かれた" );
		}

		// 花束作成シーンなら摑み 歩きなら摘み
		if( SceneManager.instance.sceneType.Equals( SceneManager.SceneType.Bouquet ) )
		{
			if( handType == HandType.Right )
			{
				bouquetMaker.TryHold();
			}
			else
			{
				bouquetMaker.CompleteBouquet();
			}

		}
		else
		{
			var idx = (int)handType;

			if( hands.Count >= idx )
			{
				// TODO "pos"でワールド座標が取れているかチェックすること
				picker.TryPick(hands[ idx ].transform.pos);
			}
		}
	}

	/// <summary>
	/// コントローラのトリガーが引かれた状態から解除された
	/// </summary>
	/// <param name="type">右手または左手</param>
	/// <param name="half">半押しか？</param>
	private void OnReleaseTrigger( HandType type )
	{
		Debug.Log( ((HandType)type).ToString() + "のトリガーが解除された" );

		// 左手ならなにもしない
		if( type.Equals(HandType.Left) )
		{
			return;
		}

		// 花束作成シーンなら花束パーツ解放
		if( SceneManager.instance.sceneType.Equals( SceneManager.SceneType.Bouquet ))
		{
			bouquetMaker.Release();
		}
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

	/// <summary>
	/// コントローラのグリップが押された
	/// </summary>
	/// <param name="type">Type.</param>
	private void OnPressGrip( HandType type )
	{
		Debug.Log( ((HandType)type).ToString() + "のグリップが押された" );

		// 通常シーンと花束作成シーンを切り替え
		SceneManager sm = SceneManager.instance;
		sm.sceneType = sm.sceneType.Equals( SceneManager.SceneType.Walk ) ? SceneManager.SceneType.Bouquet : SceneManager.SceneType.Walk;

		if( sm.sceneType.Equals( SceneManager.SceneType.Bouquet ) )
		{
			// 花束作成シーンに切り替え時は花束パーツ生成
			bouquetMaker.CreateBouquetParts();
		}
		else
		{
			// 歩きシーンに切り替え時は花束作成関連オブジェクト破棄
			bouquetMaker.RemoveBouquetParts();
		}
	}
}
