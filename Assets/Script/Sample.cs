using UnityEngine;

public sealed class Sample : MonoBehaviour
{
	void Start()
	{
		GameObject obj1 = new GameObject("Camera(Left)");
		Camera camera1 = obj1.AddComponent<Camera>();
		camera1.rect = new Rect(0, 0, 0.5F, 1);
		camera1.backgroundColor = Color.blue;

		GameObject obj2 = new GameObject("Camera(Right)");
		Camera camera2 = obj2.AddComponent<Camera>();
		camera2.rect = new Rect(0.5F, 0, 0.5F, 1);
		camera2.backgroundColor = Color.red;
	}
}