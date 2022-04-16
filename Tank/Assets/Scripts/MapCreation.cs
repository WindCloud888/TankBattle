//using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapCreation : MonoBehaviour {
	public GameObject[] item;    //用来装饰初始化地图所需物体的数组。
								 //0.老家    1.墙    2.障碍    3.出生效果   4.河流  5.草  6.空气墙  
	public GameObject[] heartWall;  //存放基地围墙物体
	public GameObject explosionPrefab;
	public List<GameObject> enemyList = new List<GameObject>();//用来存放所有敌方坦克
	private List<Vector3> itemPositionList = new List<Vector3>();  //已经有东西的位置列表

	public float changeTime = 10f;  //变成铁墙的时间
	public float destroyTime = 5f;  //基地墙消失的时间
	private static MapCreation instance;

	public static MapCreation Instance
	{
		get
		{
			return instance;
		}

		set
		{
			instance = value;
		}
	}
	private void Awake()
	{
		Instance = this;
		heartWall = new GameObject[5];
		InitMap();
		
	}
	private void InitMap()
	{
		//实例化老家
		CreateItem(item[0], new Vector3(0, -8, 0), Quaternion.identity);
		//用墙把老家围起来
		heartWall[0]=CreateItem(item[1], new Vector3(-1, -8, 0), Quaternion.identity);
		heartWall[1]=CreateItem(item[1], new Vector3(1, -8, 0), Quaternion.identity);
		heartWall[2]=CreateItem(item[2], new Vector3(0, -7, 0), Quaternion.identity);
		heartWall[3]=CreateItem(item[1], new Vector3(-1, -7, 0), Quaternion.identity);
		heartWall[4]=CreateItem(item[1], new Vector3(1, -7, 0), Quaternion.identity);
		// Quaternion.identity指无旋转
		//实例化外围墙
		for (int i = -11; i < 12; i++)
		{
			CreateItem(item[6], new Vector3(i, 9, 0), Quaternion.identity);
		}
		for (int i = -11; i < 12; i++)
		{
			CreateItem(item[6], new Vector3(i, -9, 0), Quaternion.identity);
		}
		for (int j = -8; j < 9; j++)
		{
			CreateItem(item[6], new Vector3(-11, j, 0), Quaternion.identity);
		}
		for (int j = -8; j < 9; j++)
		{
			CreateItem(item[6], new Vector3(11, j, 0), Quaternion.identity);
		}
		//初始化玩家
		GameObject go = Instantiate(item[3], new Vector3(-2, -8, 0), Quaternion.identity);
		go.GetComponent<Born>().creatPlayer = true;

		//产生敌人
		CreateItem(item[3], new Vector3(-10, 8, 0), Quaternion.identity);
		CreateItem(item[3], new Vector3(-5, 8, 0), Quaternion.identity);
		CreateItem(item[3], new Vector3(5, 8, 0), Quaternion.identity);
		CreateItem(item[3], new Vector3(10, 8, 0), Quaternion.identity);


		InvokeRepeating("CreateEnemy", 4, 5);


		//实例化地图
		for (int i = 0; i < 65; i++)
		{
			CreateItem(item[1], CreateRandomPosition(), Quaternion.identity);
		}
		for (int i = 0; i < 40; i++)
		{
			CreateItem(item[2], CreateRandomPosition(), Quaternion.identity);
		}
		for (int i = 0; i < 10; i++)
		{
			CreateItem(item[4], CreateRandomPosition(), Quaternion.identity);
		}
		for (int i = 0; i < 20; i++)
		{
			CreateItem(item[5], CreateRandomPosition(), Quaternion.identity);
		}
	}
	private GameObject CreateItem(GameObject createGameObject,Vector3 createPosition,Quaternion createRotation)
	{
		GameObject itemGo = Instantiate(createGameObject, createPosition, createRotation);
		itemGo.transform.SetParent(gameObject.transform);
		itemPositionList.Add(createPosition);
		return itemGo;
	}

	//产生随机位置的方法
	private Vector3 CreateRandomPosition()
	{
		//不生成x=-10,10的两列,y=-8,8两行的位置
		while (true)
		{
			Vector3 createPosition = new Vector3(Random.Range(-9, 10), Random.Range(-7, 8),0);
			if(!HasThePosition(createPosition))
				return createPosition;
		}

	}
	
	//用来判断位置列表中是否有这个位置
	private bool HasThePosition(Vector3 createPos)
	{
		for (int i = 0; i < itemPositionList.Count; i++)
		{
			if (createPos == itemPositionList[i])
			{
				return true;
			}
		}
		return false;
	}
	
	//产生敌人的方法
	public void CreateEnemy()
	{
		int num = Random.Range(0, 4);
		Vector3 EnemyPos = new Vector3();
		if (num == 0)
		{
			EnemyPos = new Vector3(-10, 8, 0);
		}
		else if (num == 1)
		{
			EnemyPos = new Vector3(5, 8, 0);
		}
		else if (num == 2) 
		{ 
			EnemyPos = new Vector3(10, 8, 0);
		}
		else 
		{
			EnemyPos = new Vector3(-5, 8, 0);
        }
		CreateItem(item[3], EnemyPos, Quaternion.identity);
	}

	//使敌人都暂停的方法
	public void MakeEnemysStop()
    {
		for(int i = 0; i < enemyList.Count; i++)
        {
			//Debug.Log(123);
			if(enemyList[i])
			enemyList[i].SendMessage("StopTheEnemy");
        }
    } 

	//使所有基地墙变一种物体的方法，为降低难度基地正上方的墙保持为铁墙
	public void MakeWallChange(GameObject go)
    {
		DestroyHeartWall();
		heartWall[0] = CreateItem(go, new Vector3(-1, -8, 0), Quaternion.identity);
		heartWall[1] = CreateItem(go, new Vector3(1, -8, 0), Quaternion.identity);
		heartWall[2] = CreateItem(item[2], new Vector3(0, -7, 0), Quaternion.identity);
		heartWall[3] = CreateItem(go, new Vector3(-1, -7, 0), Quaternion.identity);
		heartWall[4] = CreateItem(go, new Vector3(1, -7, 0), Quaternion.identity);
	}

	//摧毁所有基地墙

	public void DestroyHeartWall()
    {
		for (int i = 0; i < 5; i++)
		{
			if (heartWall[i])
				Destroy(heartWall[i]);
		}
	}

	//玩家吃了工程奖励后的奖励
	public void PlayerEngineer()
    {
		StartCoroutine(EngineerTheWall());
    }

	//玩家吃到工程奖励后，将基地墙变成钢铁墙一段时间后恢复为土墙
	IEnumerator EngineerTheWall()
    {
		MakeWallChange(item[2]);
		yield return new WaitForSeconds(changeTime);
		MakeWallChange(item[1]);
	}


	public void EnemyEngineer()
    {
		StartCoroutine(EnemyDestroyTheWall());
	}
	//摧毁我方所有基地墙一段时间
	IEnumerator EnemyDestroyTheWall()
    {
		DestroyHeartWall();
		yield return new WaitForSeconds(destroyTime);
		MakeWallChange(item[1]);
    }

	//摧毁所有敌人，捎带着把存放敌人的那个泛型清空一下
	public void BoomAllEnmey()
    {
		for (int i = 0; i < enemyList.Count; i++)
		{
			if (enemyList[i])
			{
				Instantiate(explosionPrefab, enemyList[i].transform.position, enemyList[i].transform.rotation);
				Destroy(enemyList[i]);
				
			}
		}
		enemyList.Clear();
	}

	////告诉所有敌人玩家死亡信息以便于他们更新玩家实例
	//public void TellEnmeyPlayerDie()
 //   {
	//	for(int i = 0; i < enemyList.Count; i++)
 //       {
 //           if (enemyList[i])
 //           {
	//			enemyList[i].SendMessage("GetNewPlayerPrefabs");
 //           }
 //       }
 //   }

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

