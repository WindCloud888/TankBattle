using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour {
	public AudioClip hitAudio;
	// Use this for initialization

		private void PlayAudio()
	{
		AudioSource.PlayClipAtPoint(hitAudio, transform.position);
	}

	private void Die()
	{
		Destroy(gameObject);
	}
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
