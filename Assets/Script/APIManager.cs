using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// API のパラメータ群
/// </summary>
namespace api 
{
	/// <summary>
	/// 定数定義
	/// </summary>
	public static class Const
	{
		/// <summary>
		/// リトライ間隔
		/// </summary>
		public const int  RETRY_INTERVAL = 3;

		/// <summary>
		/// 最大リトライ回数
		/// </summary>
		public const int  RETRY_NUM = 3;
	}

	/// <summary>
	/// APIとの通信を行う
	/// </summary>
	public class APIManager : MonoBehaviour 
	{
		public static APIManager instance;

		void Awake(){
			instance = this;
		}

		/// <summary>
		/// 呼び出し
		/// </summary>
		private IEnumerator Call<ResType>(string urlStr, WWWForm form=null, System.Action<ResType> callback=null){
			Debug.Log("通信開始 : " + urlStr);

			WWW www = null;
			int retryCnt = Const.RETRY_NUM; //  リトライする回数
			float retryInterval = Const.RETRY_INTERVAL; // 一回リトライするまでの秒数

			bool succeed = false;
			for(int i=0 ; i < retryCnt ; i++){

				www = null;
				if(form != null){
					www = new WWW(urlStr, form);
				}else{
					www = new WWW(urlStr);
				}

				Debug.Log("待機中 " + (i+1).ToString() + "回目 " + urlStr);
				yield return www;

				if(!string.IsNullOrEmpty( www.error )){
					Debug.LogWarning("通信エラー\n" + retryInterval.ToString() + "秒後にリトライします : " + urlStr);
					yield return new WaitForSeconds(retryInterval);
				}else{
					succeed = true;
					break;
				}
			}

			if(!succeed){
				Debug.LogWarning("リトライを" + retryCnt.ToString() + "回行いましたが、正しい値を得られませんでした。");
			}

			string str = succeed ? "成功" : "失敗";
			Debug.Log(str + " : " + urlStr);

			yield return www;

			// TODO; 値を取り出す 
			var res = www;

			if( res != null )
			{
				// 適切な値を返す
				callback( default( ResType ) );
			}
		}


		public IEnumerator GetTweetInfo( System.Action< GetTweetInfoResponseParameter > callback )
		{
			var request = new GetTweetInfoRequestParameter();

			yield return StartCoroutine( Call<GetTweetInfoResponseParameter>( request.url, null, res =>
				{
					callback( res );
				}) );
		}
	}		

	/// <summary>
	/// GPS座標データ
	/// </summary>
	public struct GPS
	{
		/// <summary>
		/// 経度
		/// </summary>
		public float longitude;

		/// <summary>
		/// 緯度
		/// </summary>
		public float latitude;

		/// <summary>
		/// 高度
		/// </summary>
		public float altitude;
	}

	/// <summary>
	/// ツイート情報取得リクエストパラメータ
	/// </summary>
	public class GetTweetInfoResponseParameter
	{
		/// <summary>
		/// つぶやき情報
		/// </summary>
		public struct TweetInfo
		{
			/// <summary>
			/// 位置情報
			/// </summary>
			public GPS gps;

			/// <summary>
			/// 幸福指数
			/// </summary>
			public int happiness;

			/// <summary>
			/// つぶやかれた時間
			/// </summary>
			public DateTime time;
		}

		/// <summary>
		/// ブルームポイント情報
		/// </summary>
		public struct BloomPointInfo
		{
			/// <summary>
			/// 位置情報
			/// </summary>
			public GPS gps;

			/// <summary>
			/// 幸福指数
			/// </summary>
			public int happiness;
		}
			
		/// <summary>
		/// つぶやきリスト
		/// </summary>
		public List<TweetInfo> tweetInfoList;

		/// <summary>
		/// ブルームポイントリスト
		/// </summary>
		public List<BloomPointInfo> bloomPointInfoList;
	}

	/// <summary>
	/// ツイート情報取得レスポンスパラメータ
	/// </summary>
	public  class GetTweetInfoRequestParameter
	{
		/// <summary>
		/// 自分の座標
		/// </summary>
		//public GPS my_position;

		/// <summary>
		/// 取得する距離（メートル）
		/// </summary>
		//public float range;

		/// <summary>
		/// URL
		/// </summary>
		public string url = "http://www.triaws.com/~joyfulower/keiji/prot/API001.php";
	}
}
