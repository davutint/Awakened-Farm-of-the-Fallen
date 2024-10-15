using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cowsins;
using TMPro;
using System;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CrazyGames;


public class SpawnManager : MonoBehaviour
{
	public IEnemies[] enemies;
	
	public TextMeshProUGUI killedZombieCountText;  

	public TextMeshProUGUI zombieCountText;  
	public TextMeshProUGUI earnedCoinText;  
	private int killedZombieCount=0;
	private int zombieCount;
	public static Action OnGameOver;
	
	public static int earnedCoin=0;
	[SerializeField]private int testint;
	[SerializeField]private Button doubleCoinButton;
	
	private void OnEnable()
	{
		Zombie.OnZombieKilled+=CheckEnemiesAlive;
		Zombie.OnZombieKilled+=KilledZombieCounter;
		WereWolf.OnWereWolfKilled+=CheckEnemiesAlive;
		WereWolf.OnWereWolfKilled+=KilledZombieCounter;
		BossZombies.OnBossZombieKilled+=CheckEnemiesAlive;
		BossZombies.OnBossZombieKilled+=KilledZombieCounter;
	}
	private void OnDisable()
	{
		WereWolf.OnWereWolfKilled-=CheckEnemiesAlive;
		WereWolf.OnWereWolfKilled-=KilledZombieCounter;
		Zombie.OnZombieKilled-=CheckEnemiesAlive;
		Zombie.OnZombieKilled-=KilledZombieCounter;
		BossZombies.OnBossZombieKilled-=CheckEnemiesAlive;
		BossZombies.OnBossZombieKilled-=KilledZombieCounter;
	}
	private void Start()
	{
		
		killedZombieCountText.text=killedZombieCount.ToString(); 
		FindEnemies();
		
		earnedCoinText.text="Coin Earned: "+ earnedCoin.ToString();
		doubleCoinButton.interactable=true;
		doubleCoinButton.gameObject.SetActive( true);
		testint=earnedCoin;
	}
	
	
	private void FindEnemies()
	{
	  	Zombie[]zombies = FindObjectsByType<Zombie>(FindObjectsSortMode.None);
		WereWolf[] wereWolves= FindObjectsByType<WereWolf>(FindObjectsSortMode.None);
		BossZombies[]bossZombies=FindObjectsByType<BossZombies>(FindObjectsSortMode.None);
		List<IEnemies> enemiesList = new List<IEnemies>();

		// Zombies'i listeye ekle
		foreach (var zombie in zombies)
		{
			enemiesList.Add(zombie);
		}

		// WereWolves'u listeye ekle
		foreach (var wereWolf in wereWolves)
		{
			enemiesList.Add(wereWolf);
		}

		// BossZombies'i listeye ekle
		foreach (var bossZombie in bossZombies)
		{
			enemiesList.Add(bossZombie);
		}

		// Listeyi array'e dönüştür
		enemies = enemiesList.ToArray();
		zombieCount=enemies.Length;
		zombieCountText.text=zombieCount.ToString();
		
	}
	private void KilledZombieCounter()
	{
		killedZombieCount++;
		//başlangıç parası ile oyun sonu parasını karşılaştır
		
		killedZombieCountText.text=killedZombieCount.ToString(); 
		zombieCount--;
		zombieCountText.text=zombieCount.ToString();
		earnedCoinText.text="Coin Earned: "+earnedCoin.ToString();
		testint=earnedCoin;
	}
	public void DoubleIt()
	{
		DoubleCoinWithAnimation();
	}
	public void DoubleCoinWithAnimation()
	{
		doubleCoinButton.interactable=false;
		int targetValue = earnedCoin * 2;
		// DOTween ile değerler arasında yumuşak geçiş yap
		DOTween.To(() => earnedCoin, x => 
		{
			earnedCoin = x;
			earnedCoinText.text ="Coin Earned: "+ earnedCoin.ToString();
		}, targetValue, 1f) // 1 saniyelik animasyon süresi
		.SetEase(Ease.OutQuad);
		 // Yavaş başlayıp hızlanan bir animasyon kullan
		CoinManager.Instance.AddCoins(targetValue);
		PlayerPrefs.SetInt("Coin",CoinManager.Instance.coins);
		CrazySDK.User.SyncUnityGameData();

		doubleCoinButton.gameObject.SetActive(false);
		testint=earnedCoin;
	}

	

	void CheckEnemiesAlive()
	{
		foreach (IEnemies enemies in enemies)
		{
			if (enemies.alive)
			{
				// Eğer bir zombi bile hayattaysa fonksiyondan çık
				return;
			}
		}
			
		

		// Eğer bu noktaya gelindiyse tüm zombiler ölmüş demektir
	
		earnedCoinText.text="Coin Earned: "+earnedCoin.ToString();
		testint=earnedCoin;
		OnGameOver?.Invoke();
		Successfull();
		Debug.Log("TÜM ZOMBİLER ÖLDÜRÜLDÜ");
		
	}
	public void RestartScene()
	{
		CrazySDK.Game.GameplayStart();

		Scene currentScene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(currentScene.name);
	}
	
	public void Successfull()
	{
		string scenename=SceneManager.GetActiveScene().name;
		PlayerPrefs.SetInt(scenename,1);
	}

   
}
