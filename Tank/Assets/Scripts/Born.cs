using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Born : MonoBehaviour
{
	public GameObject playerPrefab;

	public GameObject[] enemyPrefabList;

	public bool creatPlayer;
	// Use this for initialization
	void Start()
	{
		Invoke("BornTank", 1f);     //延时调用方法
		Destroy(gameObject, 1f);
	}

	// Update is called once per frame
	void Update()
	{

	}

	private void BornTank()
	{
		if (creatPlayer)
		{
			Instantiate(playerPrefab, transform.position, Quaternion.identity);
		}
		else
		{
			int num = Random.Range(0, 10);
			GameObject go=Instantiate(enemyPrefabList[num], transform.position, Quaternion.identity);
			MapCreation.Instance.enemyList.Add(go);
		}
	}
}