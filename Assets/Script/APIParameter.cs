using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// API のパラメータ群
/// </summary>
namespace APIParameter 
{
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
	/// リクエストパラメータ
	/// </summary>
	public class ResponseParameter
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
	/// Request parameter.
	/// </summary>
	public  class RequestParameter
	{
		/// <summary>
		/// 自分の座標
		/// </summary>
		public GPS my_position;

		/// <summary>
		/// 取得する距離（メートル）
		/// </summary>
		public float range;

		/// <summary>
		/// URL
		/// </summary>
		public string url = "http://www.triaws.com/~joyfulower/keiji/prot/API001.php";
	}
}
