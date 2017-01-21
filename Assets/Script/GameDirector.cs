using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameDirector : MonoBehaviour 
{
	public static GameDirector instance;

    // (1)何を(2)どれだけ持っているか、を把握するためのList

    //ItemPanel・ImgPrefabの設定用
    public GameObject ItemPrefab;
    public GameObject ItemPanel;

    //UIの描画
	public void SetUI()
    {
        //一回、itempanelの子要素を全削除
        //※全削除→再描画方式が最善かは要検討。順番やエフェクト等考えるとなんだかんだ一番効率的な気はしている。
        for (int j = ItemPanel.transform.childCount - 1; j >= 0; j--)
        {
            GameObject.DestroyImmediate(ItemPanel.transform.GetChild(j).gameObject);
        }

		var flowerList = SceneManager.instance.fList;

        //リストにしたがってアイテムを順に描画。
		for (int i = flowerList.Count - 1; i > -1 ; i--)
        {
            GameObject item = Instantiate(ItemPrefab) as GameObject;
			var fItem = flowerList[i];
			item.name = fItem.name;
            item.transform.SetParent(ItemPanel.transform, false);
            
            Image img = item.GetComponent<Image>();
			//img.sprite = Resources.Load<Sprite>(fItem.name);
			var sprite = Resources.Load<Sprite>(fItem.name);
			if( sprite == null )
			{
				// 該当スプライトがなかったらとりあえずねこ
				sprite = Resources.Load<Sprite>("cat");
			}
			img.sprite = sprite;

            Text txt = item.transform.FindChild("Text").gameObject.GetComponent<Text>();
			txt.text = "" + fItem.count;

            //アニメーションを追加
            Animator animator = item.GetComponent<Animator>();
			animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(fItem.name);

        }
    }

    // Use this for initialization
    void Start () 
	{
		instance = this;

        // Listに初期値を設定
		/*
		var item = new SceneManager.FlowerItem();
		item.name = "tama";
		item.count = 2;
		SceneManager.instance.fList.Add( item );

		item = new SceneManager.FlowerItem();
		item.name = "yama";
		item.count = 3;
		SceneManager.instance.fList.Add( item );

		item = new SceneManager.FlowerItem();
		item.name = "blcok";
		item.count = 1;
		SceneManager.instance.fList.Add( item );
		*/
        //UIを描画
        SetUI();
    }
}
