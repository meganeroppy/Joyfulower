using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Viveのコントローラからの入力を検出
/// </summary>
public class VRInputManager : MonoBehaviour 
{
	/// <summary>
	/// 0=Left 1=Right
	/// </summary>
	public enum HandType{
		Left,
		Right,
	};

	/// <summary>
	/// オブジェクトとしての手
	/// </summary>
	public List<SteamVR_TrackedObject> handObjects;

	/// <summary>
	/// 入力装置としての手
	/// </summary>
	private List<SteamVR_Controller.Device> handDevices;

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

	[SerializeField]
	GameObject flower_bg;

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
		while( checkControllerVilidation && !handObjects[(int)HandType.Left].gameObject.activeInHierarchy || !handObjects[(int)HandType.Right].gameObject.activeInHierarchy )
		{
			Debug.Log("Viveコントローラへの接続が確認ができるまで待機中 ["  +  totalWaitSec.ToString() + " / " + maxWaitCount.ToString() + " ] スペースキーでスキップ");
			yield return new WaitForSeconds(1);

			if( ++totalWaitSec > maxWaitCount || Input.GetKey(KeyCode.Space))
			{
				Debug.Log(maxWaitCount.ToString() + "秒待ってもViveコントローラへの接続が確認できなかったので、キーボード捜査モードに移行します。");
				controllerNonVR.SetActive(true);
				cameraRig.SetActive(false);
				yield break;
			}
		}

		handDevices = new List<SteamVR_Controller.Device>();

		var hand = SteamVR_Controller.Input((int) handObjects[(int)HandType.Left].index);
		if( hand != null )
		{
			handDevices.Add(hand);
		}

		hand = SteamVR_Controller.Input((int) handObjects[(int)HandType.Right].index);
		if( hand != null )
		{
			handDevices.Add(hand);
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

		var gripCnt = 0;
		var padCnt = 0;

		for( int i=0 ; i<handDevices.Count ; i++ )
		{
			var hand = handDevices[i];
			if( hand != null )
			{			
				if (hand.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger) || Input.GetKeyDown(KeyCode.A)) {
					OnPressTrigger( (HandType)i, true);
				}
				if (hand.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
					OnPressTrigger( (HandType)i, false);
				}
				if ( hand.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) )
				{
					OnReleaseTrigger((HandType)i);
				}
					
				if( hand.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
				{
					OnPressTouchPad( (HandType)i);
					padCnt++;
				}

				if( hand.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
				{
					OnPressGrip((HandType)i);
					gripCnt++;
				}

			}
		}

		if ( (padCnt >= 2 && gripCnt >= 2) || Input.GetKeyDown (KeyCode.D) ) {
			Debug.Log (" DEBUG COMMAND ");
			flower_bg.SetActive (!flower_bg.activeInHierarchy);
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
			Debug.Log( ((HandType)handType).ToString() + "のトリガーがちょっと引かれた ハーフ入力は無視" );
			return;
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

			if( handObjects.Count >= idx )
			{
				// TODO "pos"でワールド座標が取れているかチェックすること
				picker.TryPick(handObjects[ idx ].transform.position);
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
		var handObj = handObjects [(int)type];
		tweet.Tweet(handObj);
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
