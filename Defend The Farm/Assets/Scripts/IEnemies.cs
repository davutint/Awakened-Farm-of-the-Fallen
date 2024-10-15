using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  interface IEnemies
{
	 bool alive { get; set; }
	public void DamageSpawn(Vector3 pos,int damage);

	public void NoDamageSpawn(Vector3 pos);
	

}
