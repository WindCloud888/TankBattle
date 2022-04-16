﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour {
	public float moveSpeed = 10;

	public bool isPlayerBullet;
	public bool isSuperBullet;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(transform.up*moveSpeed*Time.deltaTime,Space.World);     //
	}
	
	private void OnTriggerEnter2D(Collider2D collision)
	{
		switch (collision.tag)
		{
			case "Tank":
				if (!isPlayerBullet)
				{
					collision.SendMessage("Die");
					Destroy(gameObject);
				}
				
				break;
			case "Enemy":
				if (isPlayerBullet)
				{
					collision.SendMessage("Die");
					Destroy(gameObject);
				}
				break;
			//case "GoldEnemy":
			//	if (isPlayerBullet)
			//	{
			//		collision.SendMessage("Die");
			//		Destroy(gameObject);
			//	}
			//	break;
			//case "RedEnemy":
			//	if (isPlayerBullet)
			//	{
			//		collision.SendMessage("Die");
			//		Destroy(gameObject);
			//	}
			//	break;
			//case "GreenEnemy":
			//	if (isPlayerBullet)
			//	{
			//		collision.SendMessage("Die");
			//		Destroy(gameObject);
			//	}
			//	break;
			//case "WhiteEnemy":
			//	if (isPlayerBullet)
			//	{
			//		collision.SendMessage("Die");
			//		Destroy(gameObject);
			//	}
			//	break;
			//case "AtkEnemy":
			//	if (isPlayerBullet)
			//	{
			//		collision.SendMessage("Die");
			//		Destroy(gameObject);
			//	}
			//	break;
			//case "AwardEnemy":
			//	if (isPlayerBullet)
			//	{
			//		collision.SendMessage("Die");
			//		Destroy(gameObject);
			//	}
			//	break;
			case "Heart":
				collision.SendMessage("Die");
				Destroy(gameObject);
				break;
			case "Wall":
				Destroy(collision.gameObject);
				Destroy(gameObject);
				break;
			case "Barrier":
				if (isPlayerBullet)
				{
					collision.SendMessage("PlayAudio");
				}
				if (isSuperBullet)
				{
					collision.SendMessage("PlayAudio");
					collision.SendMessage("Die");

				}
				Destroy(gameObject);
				break;
			case "AirBarrier":
				Destroy(gameObject);
				break;
			default:
				break;
		}
	}
}
