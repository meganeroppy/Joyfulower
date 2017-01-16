using UnityEngine;
using System.Collections;

class RotateCamera : MonoBehaviour
{
	[SerializeField]
	Camera rotCamera;

	void Update () 
	{
		transform.rotation = rotCamera.transform.rotation;
	}

	void Disable() 
	{
		this.gameObject.SetActive(false);
	}
}