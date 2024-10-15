using System;
using System.Collections;
using System.Collections.Generic;
using cowsins;
using DamageNumbersPro;
using RenownedGames.AITree;
using UnityEngine;
using UnityEngine.AI;

public class BossZombies : MonoBehaviour,IDamageable,IEnemies
{
	BehaviourRunner behaviourRunner;
	
	public bool isFall;
	public static event Action OnBossZombieKilled;

	private AudioSource audioSource;
	[SerializeField]private int _health=500;
	public DamageNumber damageNumber;
	public DamageNumber noDamageNumber;
	[SerializeField]private int _Damage;
	public bool alive { get; set ; }
	Collider[]colliders;
	private void Awake()
	{
		behaviourRunner=GetComponent<BehaviourRunner>();
		behaviourRunner=GetComponent<BehaviourRunner>();
		SetAlive(true);
		audioSource=GetComponent<AudioSource>();
		
	}
	private void Start()
	{
		Blackboard blackboard = behaviourRunner.GetBlackboard();
		if (blackboard.TryFindKey("damage", out IntKey damagekey))
		{
			damagekey.SetValue(_Damage);
			
		}
		StartCoroutine(WaitALittle());
	}
	
	
	private void SetAlive(bool value)
	{
		Blackboard blackboard = behaviourRunner.GetBlackboard();

		if (blackboard.TryFindKey("alive", out BoolKey alivekey))
		{
			alivekey.SetValue(value);
			alive=value;
		}
		
	}
	IEnumerator WaitALittle()
	{
		
		float rnd=UnityEngine.Random.Range(5f,10);
		yield return new WaitForSeconds(rnd);
		Blackboard blackboard = behaviourRunner.GetBlackboard();
		if (blackboard.TryFindKey("Player", out TransformKey playertransform))
		{
			GameObject gameObject=GameObject.FindGameObjectWithTag("Player");
			playertransform.SetValue(gameObject.transform);
			
		}
		
	}
	private void OnEnable()
	{
		PlayerEvents.PlayerDead+=PlayerDead;
	}
	private void OnDisable()
	{
		PlayerEvents.PlayerDead-=PlayerDead;
	}
	private void PlayerDead()
	{
		Blackboard blackboard = behaviourRunner.GetBlackboard();
		if (blackboard.TryFindKey("Player", out TransformKey playertransform))
		{
			
			playertransform.SetValue(null);
			audioSource.enabled=false;
		}
	}
	public void ChoseRunVoiceAndPlay()
	{
		float rnd=UnityEngine.Random.Range(.7f,1);
		audioSource.pitch=rnd;
		audioSource.loop=true;
		audioSource.Play();
	}
	public void SetDead()
	{
		SetAlive(false);
		
		Debug.Log("ÖLÜMÜ AYARLADIK");
		CoinManager.Instance.AddCoins(100);
		SpawnManager.earnedCoin+=100;
		PlayerPrefs.SetInt("Coin",CoinManager.Instance.coins);
		GetComponentInChildren<Collider>().enabled=false;
		audioSource.enabled=false;
		StartCoroutine(Wait());
		OnBossZombieKilled?.Invoke();
		
	}
	private void ColliderDisable()
	{
		colliders=GetComponentsInChildren<Collider>();
		foreach (Collider collider in colliders)
		{
			collider.enabled=false;
		}
	}
	IEnumerator Wait()
	{
		yield return new WaitForSeconds(3f);
		behaviourRunner.enabled=false;
		GetComponent<NavMeshAgent>().enabled=false;
		
	}
	public void Damage(float damage, bool isHeadshot)
	{
	   _health-=(int)damage;
		if (_health<=0)
		{
			//dead
			SetDead();
		}
	}

	public void DamageSpawn(Vector3 pos, int damage)
	{
		damageNumber.Spawn(pos+Vector3.up,damage);
	}

	public void NoDamageSpawn(Vector3 pos)
	{
		noDamageNumber.Spawn(pos+Vector3.right);
	}
}
