using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
	//属性值
	public float moveSpeed = 3;
	private Vector3 bullectAulers;
	private float timeVal;
	public float defendTimeVal = 3;
	private bool isDefended = true;
	public float AttackSpeed = 3;
	public bool isStop=false;
	public float stopTime = 15f;
	public int starLevel = 0;
	//引用
	private SpriteRenderer sr;
	public Sprite[] tankSprite;   //上 右 下 左
	public GameObject bulletPrefab;
	public GameObject playerSuperPrefab;
	public GameObject explosionPrefab;
	public GameObject defendEffectPrefab;
	public AudioSource moveAudio;
	public AudioClip[] tankAudio;
	public GameObject[] awardPrefabs;
	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
	}
	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()//每一帧时间并不匀称，依赖于cpu性能
	{
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

	}

	private void FixedUpdate()            //每一帧所用的时间是匀称的，可以解决因力不均衡导致的抖动问题
	{
		if (PlayerManger.Instance.isDefeat)
		{
			return;
		}
        if (isStop)
        {
			return;
        }
		Move();
		//攻击的CD
		if (timeVal > AttackSpeed)
		{
			Attack();
		}
		else
		{
			timeVal += Time.fixedDeltaTime;
		}


	}

	//坦克攻击方法
	private void Attack()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{    //子弹产生的角度：当前子弹的角度+子弹应该旋转的角度
			Instantiate(bulletPrefab, transform.position, Quaternion.Euler(transform.eulerAngles + bullectAulers));
			timeVal = 0;
		}
	}
	//坦克移动方法
	private void Move()
	{
		float v = Input.GetAxisRaw("Vertical");          //按下向上方向键，返回值为1.0,按下向下方向键，返回值为0，继续按下返回值为 - 1.0

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

		float h = Input.GetAxisRaw("Horizontal");            //按左返回值为-1，按右返回值为1   
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
		if (Mathf.Abs(v) > 0.05f || Mathf.Abs(h) > 0.05f)
		{
			moveAudio.clip = tankAudio[1];
			if (!moveAudio.isPlaying)
			{
				moveAudio.Play();
			}
		}
		else
		{
			moveAudio.clip = tankAudio[0];
			if (!moveAudio.isPlaying)
			{
				moveAudio.Play();
			}
		}
	}
	//坦克死亡方法
	private void Die()
	{
		//print(isDefended);
		if (isDefended)
		{
			return;
		}
		//
		PlayerManger.Instance.isDead = true;
		//产生爆炸特效
		Instantiate(explosionPrefab, transform.position, transform.rotation);
		//产生战利品
		int num = Random.Range(0, awardPrefabs.Length);
		Instantiate(awardPrefabs[num], transform.position, transform.rotation);
		//死亡
		Destroy(gameObject);
	}
	//坦克强行死亡方法（即便有护盾）
	public void Boom()
    {
		PlayerManger.Instance.isDead = true;
		//产生爆炸特效
		Instantiate(explosionPrefab, transform.position, transform.rotation);
		//产生战利品
		int num = Random.Range(0, awardPrefabs.Length);
		Instantiate(awardPrefabs[num], transform.position, transform.rotation);
		//死亡
		Destroy(gameObject);
	}
	public void StopThePlayer()
	{
		StartCoroutine(StopPlayer());
	}
	IEnumerator StopPlayer()
	{
		isStop = true;
		yield return new WaitForSeconds(stopTime);
		isStop = false;
	}

    //使坦克火力变强速度变快
    private void StarUp()
    {
        if (moveSpeed <= 10)
            moveSpeed+=0.5f;

		//print(123);
		starLevel++;
		if (starLevel >= 6)
		{
			AttackSpeed = 0.05f;
			bulletPrefab = playerSuperPrefab;
			
		}
        else
        {
			AttackSpeed -= 0.05f;
		}

    }

	private void GetDefend()
    {
		isDefended = true;
		defendTimeVal = 3;

	}

	//告诉敌方坦克已死亡以便让他们获取新的玩家坦克实例
	//private void TellEnmeysDie()
 //   {
	//	//调用MapCreation中的方法
	//	MapCreation.Instance.TellEnmeyPlayerDie();
 //   }
    private void OnTriggerEnter2D(Collider2D collider)
	{
		switch(collider.gameObject.tag) 
		{
			case "Increase":
				//Debug.Log(3);
				PlayerManger.Instance.IncreaseLife();
				Destroy(collider.gameObject);
				break;
			case "Stop":
				MapCreation.Instance.MakeEnemysStop();
				Destroy(collider.gameObject);
				break;
			case "Engineer":
				MapCreation.Instance.PlayerEngineer();
				Destroy(collider.gameObject);
				break;
			case "Boom":
				MapCreation.Instance.BoomAllEnmey();
				Destroy(collider.gameObject);
				break;
			case "Star":
				StarUp();
				print(456);
				Destroy(collider.gameObject);
				break;
			case "Defend":
				GetDefend();
				Destroy(collider.gameObject);
				break;
		}
	}
}


	



