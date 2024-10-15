using System;
using System.Collections;
using cowsins;
using DamageNumbersPro;
using RenownedGames.AITree;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class WereWolf : MonoBehaviour,IDamageable,IEnemies
{
  	BehaviourRunner behaviourRunner;
	NavMeshAgent navMeshAgent;
	public bool isFall;
	public static event Action OnWereWolfKilled;

	[SerializeField]private AudioSource[] audioSource;
	[SerializeField]private AudioSource attackVoice;
	[SerializeField]private AudioSource runVoice;
	private int _health=100;

	public bool alive { get; set; }
	[SerializeField]int rnd;
	public DamageNumber damageNumber;
	public DamageNumber noDamage;
	Collider[]colliders;
	private void Awake()
	{
		behaviourRunner=GetComponent<BehaviourRunner>();
		navMeshAgent=GetComponent<NavMeshAgent>();
		behaviourRunner=GetComponent<BehaviourRunner>();
		SetAlive(true);
		rnd=UnityEngine.Random.Range(0,audioSource.Length);
		
	}
	private void Start()
	{
		
		BreahtVoice();
		StartCoroutine(WaitALittle());
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
			StopAllVoices();
		}
	}
	
	IEnumerator WaitALittle()
	{
		
		float rnd=UnityEngine.Random.Range(.5f,10);
		yield return new WaitForSeconds(rnd);
		Blackboard blackboard = behaviourRunner.GetBlackboard();
		if (blackboard.TryFindKey("Player", out TransformKey playertransform))
		{
			GameObject gameObject=GameObject.FindGameObjectWithTag("Player");
			playertransform.SetValue(gameObject.transform);
			RunVoice();
		}
		
	}
	private void RunVoice()//nefen almayı durdur,koşmayı çal
	{
		audioSource[rnd].loop=false;
		audioSource[rnd].Stop();
		runVoice.pitch=UnityEngine.Random.Range(.7f,1);
		runVoice.loop=true;
		runVoice.Play();
	
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
	
	public void SetDead()
	{
		ColliderDisable();
		SetAlive(false);
		Debug.Log("ÖLÜMÜ AYARLADIK");
		SpawnManager.earnedCoin+=25;
		CoinManager.Instance.AddCoins(25);
		PlayerPrefs.SetInt("Coin",CoinManager.Instance.coins);
		StopAllVoices();
		OnWereWolfKilled?.Invoke();
	}
	private void ColliderDisable()
	{
		colliders=GetComponentsInChildren<Collider>();
		foreach (Collider collider in colliders)
		{
			collider.enabled=false;
		}
	}
	
	private void BreahtVoice()
	{
		runVoice.loop=false;
		runVoice.Stop();
		rnd=UnityEngine.Random.Range(0,audioSource.Length);
		audioSource[rnd].pitch=UnityEngine.Random.Range(.7f,1);
		audioSource[rnd].loop=true;
		audioSource[rnd].Play();
		
		
	}
	
	public void NormalVoicePlay()
	{
		runVoice.loop=true;
		runVoice.Play();
	}
	
	public void AttackVoicePlay()
	{
		audioSource[rnd].loop=false;
		audioSource[rnd].Stop();
		runVoice.loop=false;
		runVoice.Stop();
		attackVoice.Play();
	}
	
	private void StopAllVoices()
	{
		audioSource[rnd].loop=false;
		audioSource[rnd].Stop();
		runVoice.loop=false;
		runVoice.Stop();
		attackVoice.Stop();
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
		Debug.Log("Damage Spawn Edildi");
	}

	public void NoDamageSpawn(Vector3 pos)
	{
		noDamage.Spawn(pos+Vector3.up);
		Debug.Log("NoDamage Spawn Edildi");
	}
}
