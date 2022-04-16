using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

	//属性值
	public float moveSpeed = 3;
	private Vector3 bullectAulers;
	private float v = -1;
	private float h;
	public float attackSpeed = 3;
	public int colorTanksIndex;
	public bool isAward;
	public bool isStop = false;
	public float stopTime = 20.0f;            //敌人冻结的时间
	private bool isDefended = true;
	//计时器
	private float timeVal;
	private float timeValChangeDirection = 0;
	public float defendTimeVal = 0;

	//引用
	private SpriteRenderer sr;
	public Sprite[] tankSprite;   //上 右 下 左
	public GameObject bulletPrefab;
	public GameObject explosionPrefab;
	public GameObject defendEffectPrefab;
	public GameObject newTankPrefab;
	public GameObject[] awardPrefabs;
	public GameObject createGameObject;
	public GameObject superBulletPrefab;
	//private GameObject player;


	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
	}
	// Use this for initialization
	void Start()
	{
		//player = GameObject.FindGameObjectWithTag("Tank");
	}

	// Update is called once per frame
	void Update()//每一帧时间并不匀称，依赖于cpu性能
	{
		////如果玩家死亡，则一直寻找新的玩家实例，直到找到为止
  //      if (!player)
  //      {
		//	player = GameObject.FindGameObjectWithTag("Tank");
		//}
		if (isStop)
		{
			//Debug.Log(333);
			return;
		}
		//是否处于无敌状态
		if (isDefended)
		{
			defendEffectPrefab.SetActive(true);
			defendTimeVal -= Time.deltaTime;
			//print(defendTimeVal);
			if (defendTimeVal <= 0)
			{
				isDefended = false;
				defendEffectPrefab.SetActive(false);

			}
		}
		//攻击的时间间隔
		if (timeVal >= attackSpeed)
		{
			Attack();
			timeVal = 0;
		}
		else
		{
			timeVal += Time.deltaTime;
		}
	}

	private void FixedUpdate()            //每一帧所用的时间是匀称的，可以解决因力不均衡导致的抖动问题
	{
		if (isStop)
		{
			//Debug.Log(333);
			return;
		}
		Move();

	}

	//坦克攻击方法
	private void Attack()
	{

		//子弹产生的角度：当前子弹的角度+子弹应该旋转的角度
		Instantiate(bulletPrefab, transform.position, Quaternion.Euler(transform.eulerAngles + bullectAulers));
		timeVal = 0;

	}
	//坦克移动方法
	private void Move()
	{
		if (timeValChangeDirection >= 4)
		{
			int num = Random.Range(0, 8);
			if (num > 5)
			{
				v = -1;
				h = 0;
			}
			else if (num == 0)
			{
				v = 1;
				h = 0;
			}
			else if (num > 0 && num <= 2)
			{
				h = -1;
				v = 0;
			}
			else if (num > 2 && num <= 4)
			{
				h = 1;
				v = 0;
			}
			timeValChangeDirection = 0;
		}
		else
		{
			timeValChangeDirection += Time.fixedDeltaTime;
		}

		transform.Translate(Vector3.up * v * moveSpeed * Time.fixedDeltaTime, Space.World);
		if (v < 0)
		{
			sr.sprite = tankSprite[2];
			bullectAulers = new Vector3(0, 0, 180);
		}
		else if (v > 0)
		{
			sr.sprite = tankSprite[0];
			bullectAulers = new Vector3(0, 0, 0);
		}

		if (v != 0)
		{
			return;
		}

		//print(h);
		transform.Translate(Vector3.right * h * moveSpeed * Time.fixedDeltaTime, Space.World);
		if (h < 0)
		{
			sr.sprite = tankSprite[1];
			bullectAulers = new Vector3(0, 0, 90);
		}
		else if (h > 0)
		{
			sr.sprite = tankSprite[3];
			bullectAulers = new Vector3(0, 0, -90);
		}

	}
	//坦克死亡方法
	private void Die()
	{
		PlayerManger.Instance.playerScore++;
		//print(isDefended);
		if (isDefended)
		{
			return;
		}

		//产生爆炸特效或变色
		if (colorTanksIndex > 0)
		{
			GameObject go = Instantiate(newTankPrefab, transform.position, transform.rotation);
			if (isAward)
			{
				go.GetComponent<Enemy>().isAward = isAward;
			}
			//死亡
			MapCreation.Instance.enemyList.Add(go);
			//MapCreation.Instance.enemyList.Remove(gameObject);
			Destroy(gameObject);
		}
		else
		{
			Instantiate(explosionPrefab, transform.position, transform.rotation);
			Vector3 position = gameObject.transform.position;
			Destroy(gameObject);
			if (isAward)
			{
				int num = Random.Range(0, awardPrefabs.Length);
				Instantiate(awardPrefabs[num], position, Quaternion.identity);
			}
		}

	}

	//使敌方坦克都暂停的方法

	public void StopTheEnemy()
	{
		//Debug.Log(456);
		StartCoroutine(StopEnemy());
	}
	IEnumerator StopEnemy()
	{
		//Debug.Log(111);
		isStop = true;
		yield return new WaitForSeconds(stopTime);
		isStop = false;
		//Debug.Log(222);
	}

	public void IncreaseEnemy()
	{
		Instantiate(createGameObject, new Vector3(0, 8, 0), Quaternion.identity);
		Instantiate(createGameObject, new Vector3(10, 8, 0), Quaternion.identity);
		Instantiate(createGameObject, new Vector3(-10, 8, 0), Quaternion.identity);
	}

	//public void MakePlayerStop()
	//{
	//	if(player)
	//	player.SendMessage("StopThePlayer");
	//}
	private void OnCollisionEnter2D(Collision2D collision)
	{
		switch (collision.gameObject.tag)
		{
			case "Enemy":
				timeValChangeDirection = 4;
				break;


		}
	}
	//敌方坦克升级
	private void StarUp()
    {
		moveSpeed = 10;
		attackSpeed = 0.05f;
		bulletPrefab = superBulletPrefab;
    }
	//敌人得到护盾
	private void GetDefend()
	{
		isDefended = true;
		defendTimeVal = 6;

	}
	////得到玩家的新实例
	//public void GetNewPlayerPrefabs()
 //   {
	//	player = GameObject.FindGameObjectWithTag("Tank");
	//}
	private void OnTriggerEnter2D(Collider2D collider)
	{
		switch (collider.gameObject.tag)
		{
			case "Increase":
				IncreaseEnemy();
				Destroy(collider.gameObject);
				isAward = true;
				break;
			case "Stop":
				PlayerManger.Instance.MakePlayerStop();
				Destroy(collider.gameObject);
				isAward = true;
				break;
			case "Engineer":
				MapCreation.Instance.EnemyEngineer();
				Destroy(collider.gameObject);
				isAward = true;
				break;
			case "Boom":
				//if(player)
				//	player.SendMessage("Boom");
				PlayerManger.Instance.MakePlayerBoom();
				PlayerManger.Instance.DecreaseLife();
				Destroy(collider.gameObject);
				isAward = true;
				break;
			case "Star":
				StarUp();
				Destroy(collider.gameObject);
				isAward = true;
				break;
			case "Defend":
				GetDefend();
				Destroy(collider.gameObject);
				isAward = true;
				break;

		}
	}
}
