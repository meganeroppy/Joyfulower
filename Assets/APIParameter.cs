using UnityEngine;
using System.Collections;

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
		/// 座標
		/// </summary>
		public GPS gps;

		/// <summary>
		/// 幸福度
		/// </summary>
		public int happiness;

	}
}
