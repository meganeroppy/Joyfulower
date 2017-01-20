using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameDirector : MonoBehaviour {

	public static GameDirector instance;

    // (1)何を(2)どれだけ持っているか、を把握するためのList
    //private List<string> name_List = new List<string>(); //持っている種類リスト
    //private List<int> num_List = new List<int>(); //持っている数リスト

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

        //リストにしたがってアイテムを順に描画。
		var flowerList = SceneManager.instance.fList;

		for (int i = flowerList.Count - 1; i > -1 ; i--)
        {
			var fItem = flowerList[i];
            GameObject item = Instantiate(ItemPrefab) as GameObject;
			item.name = fItem.name;
            item.transform.SetParent(ItemPanel.transform, false);
            
			// 画像
            Image img = item.GetComponent<Image>();
			var sprite = Resources.Load<Sprite>(fItem.name);
			if( sprite == null )
			{
				// 該当スプライトがなかったらとりあえずねこ
				sprite = Resources.Load<Sprite>("cat");
			}
			img.sprite = sprite;
			// 所持数
            Text txt = item.transform.FindChild("Text").gameObject.GetComponent<Text>();
			txt.text = "" + fItem.count;

            //アニメーションを追加
            Animator animator = item.GetComponent<Animator>();
			if( animator == null )
			{
				Debug.LogWarning( item.name + "にAnimatorがセットされてない" );
				return;
			}
			animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(fItem.name);
        }
    }
	/*
    //持ち物の加算
    public void CountObj(string flowername)
    {

        int flag = name_List.IndexOf(flowername);
 
        //既に持ち物にある場合は個数を1追加
        if(flag >= 0)
        {
            num_List[flag]++;
        }
        //ない場合は、新たなnameとして追加して、個数は1に。
        else
        {
            name_List.Add(flowername);
            num_List.Add(1);
        }

        //UIを描画
        SetUI();
    }
	*/


    // Use this for initialization
    void Start () {

        // Listに初期値を設定
//       name_List.Add("maru");
 //       num_List.Add(2);
  //      name_List.Add("sankaku");
   //     num_List.Add(3);
    //    name_List.Add("shikaku");
     //   num_List.Add(1);

        //UIを描画
        SetUI();

		instance = this;
    }




	// Update is called once per frame
	void Update () {
	
	}
}
