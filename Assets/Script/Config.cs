using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;      //!< デプロイ時にEditorスクリプトが入るとエラーになるので UNITY_EDITOR で括ってね！
#endif

/**
 * 設定項目
 */
public class Config : MonoBehaviour
{

	/// <summary>
	/// 更新頻度
	/// </summary>
	[SerializeField]
	public static int	update_interval;

	/// <summary>
	/// 花が枯れるまでの時間
	/// </summary>
	[SerializeField]
	public static int	flower_lifetime;

	/* ---- ここから拡張コード ---- */
	#if UNITY_EDITOR
	/**
     * Inspector拡張クラス
     */
	[CustomEditor(typeof(Config))]               //!< 拡張するときのお決まりとして書いてね
	public class CharacterEditor : Editor           //!< Editorを継承するよ！
	{
		public override void OnInspectorGUI()
		{
			/* -- カスタム表示 -- */
			Config.update_interval      = EditorGUILayout.IntField( "更新頻度", Config.update_interval );

			// -- 速度 --
			Config.flower_lifetime     = EditorGUILayout.IntField( "花が枯れるまでの時間", Config.flower_lifetime );
		}
	}
	#endif
}