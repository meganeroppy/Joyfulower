using UnityEngine;
using System.Collections;

/// <summary>
/// Duration時間が経過すると削除
/// </summary>
public class OneShotEffect : MonoBehaviour {

	public ParticleSystem ps;

	// Use this for initialization
	void Start () {
		StartCoroutine( DeleteWhenParticleFinish() );	
	}

	IEnumerator DeleteWhenParticleFinish()
	{
		float timer = 0;
		while(true)
		{
			timer += Time.deltaTime;
			if(timer >= ps.duration)
			{
				Destroy(gameObject);
				yield break;
			}
			else{
				yield return null;
			}
		}
	}
}

