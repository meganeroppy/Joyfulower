using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModeIconDisplay : MonoBehaviour
{
	/// <summary>
	/// アイコンイメージ
	/// </summary>
	[SerializeField]
	private SpriteRenderer iconImage;

	/// <summary>
	/// 表示するスプライト
	/// </summary>
	[SerializeField]
	private List<Sprite> iconSprites;

	private int curSpriteIdx = 0;

	private void Update()
	{
		// 現在のモードによってアイコンを切り替え
		var curModeIdx = (int)SceneManager.instance.sceneType;

		if( curSpriteIdx != curModeIdx )
		{
			Debug.Log( "表示するアイコンを切り替え" );

			curSpriteIdx = curModeIdx;

			iconImage.sprite = iconSprites[ curSpriteIdx ];
		}
	}
}