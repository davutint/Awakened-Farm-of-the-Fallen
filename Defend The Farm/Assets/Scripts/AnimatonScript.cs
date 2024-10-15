using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatonScript : MonoBehaviour
{
	[SerializeField]private GameObject leftHand;
	[SerializeField]private GameObject magPos;
	private Transform startPos;
	private void Start()
	{
		
		FindMag();
		
		
	}
	
	public void FindMag()
	{
		// Parent objeye eriş
		Transform parentTransform = transform.parent;

		magPos=parentTransform.Find("SM_Wep_Rifle_Assault_Mag_01").gameObject;
		startPos.position=magPos.transform.position;
		
	}
	
	public void MagPosSet()
	{
		
		magPos.transform.position=leftHand.transform.position;
	}
	
	public void SetMagStartPos()
	{
		magPos.transform.position=startPos.position;
	}
}
