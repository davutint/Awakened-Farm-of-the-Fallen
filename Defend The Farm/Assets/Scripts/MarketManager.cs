using System;
using System.Collections;
using System.Collections.Generic;
using cowsins;
using CrazyGames;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MarketManager : MonoBehaviour
{
	public static MarketManager instance;
	public static event Action BuyWeapon;
	[SerializeField]private TextMeshProUGUI coinText;
	[SerializeField]public GameObject[] WeaponsToSpawn;
	private int coins;
	private void Awake()
	{
		instance=this;
		
		
	}
	private void Start()
	{
		coins=GetCoinData();
		coinText.text=coins.ToString();
		
	}
	
	public  int GetCoinData()
	{
		return PlayerPrefs.GetInt("Coin");
	}
	
	
	
	public void RevolverBuy(int amount)//Silahı satın aldığında
	{
		if(coins>=amount&&PlayerPrefs.GetInt(WeaponsToSpawn[0].GetComponent<WeaponPickeable>().weapon._name)!=1)
		{
			
			coins-=amount;
			SetWeaponSaleData(0);
			BuyWeapon?.Invoke();
			coinText.text = CoinManager.Instance.coins.ToString();
			CoinSave(coins);
		
		}
		else
		{
			Debug.Log("PARAN YETERSİZ VEYA ZATEN ALDIN");
		}
		
	}
	
	public void MP5Buy(int amount)//Silahı satın aldığında
	{
		if(coins>=amount&&PlayerPrefs.GetInt(WeaponsToSpawn[1].GetComponent<WeaponPickeable>().weapon._name)!=1)
		{
			
			coins-=amount;
			SetWeaponSaleData(1);
			BuyWeapon?.Invoke();
			coinText.text = coins.ToString();
			CoinSave(coins);
		}
		else
		{
			Debug.Log("PARAN YETERSİZ VEYA ZATEN ALDIN");
		}
	}
	public void BurstRifleBuy(int amount)//Silahı satın aldığında
	{
		if(coins>=amount&&PlayerPrefs.GetInt(WeaponsToSpawn[2].GetComponent<WeaponPickeable>().weapon._name)!=1)
		{
			
			coins-=amount;
			SetWeaponSaleData(2);
			BuyWeapon?.Invoke();
			coinText.text = coins.ToString();
			CoinSave(coins);
		}
		else
		{
			Debug.Log("PARAN YETERSİZ VEYA ZATEN ALDIN");
		}
		
	}
	/*public void RifleBuy(int amount)//Silahı satın aldığında
	{
		if(coins>=amount&&PlayerPrefs.GetInt(WeaponsToSpawn[3].GetComponent<WeaponPickeable>().weapon._name)!=1)
		{
			
			coins-=amount;
			SetWeaponSaleData(3);
			BuyWeapon?.Invoke();
			coinText.text = coins.ToString();
			CoinSave(coins);
		}
		else
		{
			Debug.Log("PARAN YETERSİZ VEYA ZATEN ALDIN");
		}
		
	}*/
	public void ShotgunBuy(int amount)//Silahı satın aldığında
	{
		if(coins>=amount&&PlayerPrefs.GetInt(WeaponsToSpawn[3].GetComponent<WeaponPickeable>().weapon._name)!=1)
		{
			
			coins-=amount;
			SetWeaponSaleData(3);
			BuyWeapon?.Invoke();
			coinText.text = coins.ToString();
			CoinSave(coins);
		}
		else
		{
			Debug.Log("PARAN YETERSİZ VEYA ZATEN ALDIN");
		}
		
	}
	
	private void SetWeaponSaleData(int index)
	{
		PlayerPrefs.SetInt(WeaponsToSpawn[index].GetComponent<WeaponPickeable>().weapon._name,1);
		Debug.Log(WeaponsToSpawn[index].GetComponent<WeaponPickeable>().weapon._name+ "Satın alındı olarak kaydedildi");
		
	}

	
	private void GetWeaponSaleData()
	{
		for (int i = 0; i < WeaponsToSpawn.Length; i++)
		{
			if(PlayerPrefs.GetInt(WeaponsToSpawn[i].GetComponent<WeaponPickeable>().weapon._name)==1)
			{
				WeaponsToSpawn[i].SetActive(true);
				Debug.Log("aktif edilen silah; "+WeaponsToSpawn[i].GetComponent<WeaponPickeable>().weapon.name);
				
			}
		}
		
	}
	private void CoinSave(int value)
	{
		PlayerPrefs.SetInt("Coin",value);
		Debug.Log("Coin Kaydedildi");
		//CrazySDK.User.SyncUnityGameData();

	}
	public bool CheckIfEnoughCoinsAndPurchase(int amount)
	{
		if (coins >= amount) // Check if enough coins are available
		{
			RemoveCoins(amount); // Deduct coins from the total
			return true; // Purchase successful
		}
		return false; // Not enough coins for purchase
	}
	public void RemoveCoins(int amount)
	{
		coins -= Mathf.Abs(amount); // Subtract positive amount of coins

		if (coins <= 0) // Ensure coins don't go negative
		{
			coins = 0; // Set coins to zero if they go below
		}
		coinText.text=coins.ToString();
		CoinSave(amount);
	}
	public void WatchAdForM4()//Reklam butonuna ata
	{
		CrazySDK.Ad.RequestAd(CrazyAdType.Rewarded, () => { print("Ad started"); }, (error) =>
		{
			
			print("Ad error, not respawning: " + error);
		}, () =>
		{
			print("Ad Finished! ");
		
			SetWeaponSaleData(4);
			BuyWeapon?.Invoke();
			 
		});
	}
	
	 
}
