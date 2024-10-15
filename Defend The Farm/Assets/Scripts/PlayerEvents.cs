using System;
using System.Collections;
using System.Collections.Generic;
using cowsins;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
   public static Action PlayerDead;
   [SerializeField]AudioSource[] clothRipped;
	
	float _distance;		

	public void SetDistance(float distance)
	{
		_distance=distance;
		
		
	}
	public void PlayerDamage(int damage)
	{
		if (_distance<=3)
		{
			PlayerStats playerStats=GetComponent<PlayerStats>();
		
			playerStats.Damage(damage,false);
		
			Debug.Log("HASAR ALDIN");
		}
	}
	public void WereWolfDamage(float distance)
	{
		if (distance<=5)
		{
			int rnd=UnityEngine.Random.Range(0,2);
			clothRipped[rnd].Play();
			
			PlayerStats playerStats=GetComponent<PlayerStats>();
		
			playerStats.Damage(20,false);
		
			Debug.Log("WereWolf Tarafından HASAR ALDIN");
		}
	}
	
	
}
