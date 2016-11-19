using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// API のパラメータ群
/// </summary>
namespace APIParameter 
{
	public class ResponseParameter{
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
}
