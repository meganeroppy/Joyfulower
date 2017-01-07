using UnityEngine;
using System.Collections;

// クリックしたアイテムを数えて削除

public class ObjController : MonoBehaviour {

    public GameObject Obj;

    public GameObject getClickObject()
    {
        GameObject result = null;
        // 左クリックされた場所のオブジェクトを取得
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                result = hit.collider.gameObject;
            }
        }
        return result;
    }


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GameObject obj = getClickObject();
        if (obj == Obj)
        {
            // 以下オブジェクトがクリックされた時の処理
            Destroy(Obj);

//            GameObject director = GameObject.Find("GameDirector");
//            director.GetComponent<GameDirector>().CountObj(Obj.name);
			GameDirector.instance.CountObj(obj.name);

        }
    }
}
