using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManger : MonoBehaviour {
	//属性值
	public int lifeValue = 5;
	public int playerScore = 0;
	public bool isDead;
	public bool isDefeat;
	private bool haveTank;
	//引用
	public GameObject born;
	public Text playerScoreText;
	public Text playerLifeValueText;
	public GameObject isDefeatUI;
	public GameObject playerEnemy;
	private GameObject player;

	//单例
	private static PlayerManger instance;

	public static PlayerManger Instance
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
	}
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Tank");

	}

	// Update is called once per frame
	void Update () {
		if (isDefeat){
			isDefeatUI.SetActive(true);
			Invoke("ReturnToTheMainMenu", 3);
			return;
		}
		if (isDead)
		{
			Recover();
		}
		//如果玩家死亡，则一直寻找新的玩家实例，直到找到为止
		if (!player)
		{
			player = GameObject.FindGameObjectWithTag("Tank");
		}
		playerScoreText.text = playerScore.ToString();
		playerLifeValueText.text = lifeValue.ToString();
		//设置haveTank这个bool值是为了防止分数在几十的时候不停的更新敌方坦克
		if (!haveTank)
		{
			if (playerScore % 10 == 0 && playerScore != 0)
			{
				Instantiate(playerEnemy, new Vector3(0, 8, 0), Quaternion.identity);
				haveTank =true;
			}
		}
		if (haveTank)
		{
			if (playerScore % 10 != 0)
			{
				haveTank = false;
			}
		}
	}
	//复活
	  private void Recover()
	{
		if (lifeValue <= 0)
		{
			isDefeat = true;
		}

		else
		{
			lifeValue--;
			GameObject go = Instantiate(born, new Vector3(-2, -8, 0), Quaternion.identity);
			go.GetComponent<Born>().creatPlayer = true;
			isDead = false;
		}
	}
	public void IncreaseLife()
	{
		lifeValue += Random.Range(1, 4);
	}

	public void DecreaseLife()
    {
		lifeValue -= Random.Range(2, 4);
    }
	private void ReturnToTheMainMenu()
	{
		SceneManager.LoadScene(0);
	}
	//使玩家暂停
	public void MakePlayerStop()
	{
		if (player)
			player.SendMessage("StopThePlayer");
	}

	//使玩家爆炸（敌人吃了爆炸道具后）
	public void MakePlayerBoom()
    {
		if (player)
			player.SendMessage("Boom");
    }
}
