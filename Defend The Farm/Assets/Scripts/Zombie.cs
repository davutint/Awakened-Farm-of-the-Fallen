using UnityEngine;
using RenownedGames.AITree;
using UnityEngine.AI;
using cowsins;
using System;
using DamageNumbersPro;
using System.Collections;


public class Zombie : MonoBehaviour,IEnemies
{
	BehaviourRunner behaviourRunner;
	NavMeshAgent navMeshAgent;
	public bool isFall;
	public static event Action OnZombieKilled;

	[SerializeField]private AudioSource[] runAudios;
	[SerializeField]private AudioSource[] breahingAudios;

	public bool alive { get ; set ; }

	public DamageNumber damageNumber;
	public DamageNumber noDamage;
	public DamageNumber dismemberment;
	Collider[]colliders;
	int runRnd;
	int brRnd;
	[SerializeField]private int Damage;
	private float speed;
	private void Awake()
	{
		behaviourRunner=GetComponent<BehaviourRunner>();
		navMeshAgent=GetComponent<NavMeshAgent>();
		behaviourRunner=GetComponent<BehaviourRunner>();
		SetAlive(true);
		
		
	}
	private void Start()
	{
		speed=UnityEngine.Random.Range(2,3f);
		navMeshAgent.speed=speed;
		Blackboard blackboard = behaviourRunner.GetBlackboard();
		if (blackboard.TryFindKey("damage", out IntKey damagekey))
		{
			damagekey.SetValue(Damage);
			
		}
		ChoseBreathingAndPlay();
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
	public void SetDead()
	{
		SetAlive(false);
		SpawnManager.earnedCoin+=10;
		OnZombieKilled?.Invoke();
		Debug.Log("ÖLÜMÜ AYARLADIK");
		
		CoinManager.Instance.AddCoins(10);
		ColliderDisable();
		PlayerPrefs.SetInt("Coin",CoinManager.Instance.coins);
		StopAllSound();
		StartCoroutine(Wait());
	}
	
	IEnumerator Wait()
	{
		yield return new WaitForSeconds(3f);
		behaviourRunner.enabled=false;
		navMeshAgent.enabled=false;
		
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
			StopAllSound();
		}
	}
	
	private void StopAllSound()
	{
		breahingAudios[brRnd].loop=false;
		breahingAudios[brRnd].Stop();
		runAudios[runRnd].loop=false;
		runAudios[runRnd].Stop();
	}
	private void ChoseRunVoiceAndPlay()//Run Sound Chose
	{
		breahingAudios[brRnd].loop=false;
		breahingAudios[brRnd].Stop();
		runAudios[runRnd].pitch=UnityEngine.Random.Range(.7f,1);
		runAudios[runRnd].loop=true;
		runAudios[runRnd].Play();
	
	}
	private void ChoseBreathingAndPlay()//Run Sound Chose
	{
		runAudios[runRnd].loop=false;
		runAudios[runRnd].Stop();
		breahingAudios[brRnd].pitch=UnityEngine.Random.Range(.7f,1);
		breahingAudios[brRnd].loop=true;
		breahingAudios[brRnd].Play();
	
	}
	
	
	IEnumerator WaitALittle()
	{
		
		float rnd=UnityEngine.Random.Range(1f,10);
		yield return new WaitForSeconds(rnd);
		Blackboard blackboard = behaviourRunner.GetBlackboard();
		if (blackboard.TryFindKey("Player", out TransformKey playertransform))
		{
			GameObject gameObject=GameObject.FindGameObjectWithTag("Player");
			playertransform.SetValue(gameObject.transform);
		}
		
	}

	
	public void SetFall()
	{
		Blackboard blackboard = behaviourRunner.GetBlackboard();

		if (blackboard.TryFindKey("fall", out BoolKey fallkey))
		{
			fallkey.SetValue(true);
			Debug.Log("bir kere düştü");
			isFall=true;
			
		}
	}
	public void Dismemberment(Vector3 pos)
	{
		dismemberment.Spawn(pos+Vector3.right);
	}

	public void DamageSpawn(Vector3 pos, int damage)
	{
		damageNumber.Spawn(pos+Vector3.up,damage);
	}

	public void NoDamageSpawn(Vector3 pos)
	{
		noDamage.Spawn(pos+Vector3.up);
	}
	private void ColliderDisable()
	{
		colliders=GetComponentsInChildren<Collider>();
		foreach (Collider collider in colliders)
		{
			collider.enabled=false;
		}
	}
}
