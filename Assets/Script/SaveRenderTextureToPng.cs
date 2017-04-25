using UnityEngine;
using System.Collections;
using System.IO;
 
public class SaveRenderTextureToPng : MonoBehaviour {
	 
	    public RenderTexture RenderTextureRef;
	[SerializeField]	 
	private Camera myCamera;

	    // Use this for initialization
	    void Start () {
	}

	// Update is called once per frame
	void Update () {
		//        savePng();

		if (Input.GetKeyDown (KeyCode.Q)) {
			StartCoroutine (SavePhoto ());
		}
	}
	 
	    void savePng()
	    {
		 
		        Texture2D tex = new Texture2D(RenderTextureRef.width, RenderTextureRef.height, TextureFormat.RGB24, false);
		        RenderTexture.active = RenderTextureRef;
		        tex.ReadPixels(new Rect(0, 0, RenderTextureRef.width, RenderTextureRef.height), 0, 0);
		        tex.Apply();
		 
		        // Encode texture into PNG
		        byte[] bytes = tex.EncodeToPNG();
		        Object.Destroy(tex);
		 
		        //Write to a file in the project folder
		        File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
		 
		    }
	 


	public IEnumerator SavePhoto()
	{
		Debug.Log ("pngで保存");

		myCamera.targetTexture = RenderTextureRef;

		yield return null;

		savePng ();
	}
	 
}