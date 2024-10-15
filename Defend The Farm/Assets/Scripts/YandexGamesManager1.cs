using System;
using UnityEngine;
using YG;

public class YandexGamesManager1 : MonoBehaviour
{
	
	private void OnEnable() => YandexGame.RewardVideoEvent += Rewarded;
	// Unsubscribe from the ad opening event in OnDisable
	private void OnDisable() => YandexGame.RewardVideoEvent -= Rewarded;
	void Rewarded(int id)//butana ver
	{
		//id'ye göre ödülü ver
		if (id==1)
		{
			//silah verme fonk
		}
		if(id==2)
		{
			//
		}

	}
}
