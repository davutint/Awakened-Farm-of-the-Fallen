using cowsins;
using CrazyGames;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[SerializeField]private GameObject[] WeaponsToSpawn;
	SpawnManager spawnManager;
	private void Start()
	{
		spawnManager=GetComponent<SpawnManager>();
		GetWeaponSaleData();
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
	
	public void DoubleIt()//butona bağla reklam ile ikiye katla
	{
		spawnManager.DoubleIt();
	}
	public void MainMenu()
	{
		CrazySDK.Game.GameplayStop();

		SceneManager.LoadScene(0);
	}
	
	public void WatchAd()//Reklam butonuna ata
	{
		CrazySDK.Ad.RequestAd(CrazyAdType.Rewarded, () => { print("Ad started"); }, (error) =>
		{
			
			print("Ad error, not respawning: " + error);
		}, () =>
		{
			print("Ad Finished! ");
			DoubleIt();
			 
		});
	}
	
}
