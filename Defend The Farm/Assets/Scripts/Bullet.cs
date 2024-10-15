using System.Collections;
using System.Collections.Generic;
using DestroyIt;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
	
		
	private Rigidbody rb;
	private BoxCollider cd;
	private MeshRenderer meshRenderer;
	
	private Vector3 startPosition;
	private bool bulletDisable;
	
	private void Awake()
	{
		cd=GetComponent<BoxCollider>();
		rb=GetComponent<Rigidbody>();
		meshRenderer=GetComponent<MeshRenderer>();
	}
	private void Update()
	{
		DisableBullet();
	
		
	}
	private void DisableBullet()
	{
		if (Vector3.Distance(startPosition,transform.position)>100&&!bulletDisable)
		{
			cd.enabled=false;
			meshRenderer.enabled=false;
			bulletDisable=true;
			
			Destroy(gameObject);
		}
	}

	
	 async void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag=="Enemy")
		{
			ProcessBulletHit(other,Vector3.forward);
		}
		
		/*if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
		{
				// Kesim işlemi için dilimleme düzlemi oluşturma
			Vector3 hitPoint = other.contacts[0].point;
			Vector3 hitNormal = other.contacts[0].normal;

			// Bu örnekte, merminin hareket yönünü kullanarak kesim düzlemi oluşturuyoruz.
			Vector3 sliceDirection = rb.velocity.normalized;
			Vector3 combinedDirection = (sliceDirection + hitNormal).normalized;
			Plane slicePlane = new Plane(combinedDirection, hitPoint);
			//Plane slicePlane = new Plane(sliceDirection, hitPoint+Vector3.right);
		
			// Mermi çarpışma noktasını kesim işlemi olarak kullanmak için, slicer arayüzünü alıyoruz.
			var slicer = other.collider.GetComponentInParent<BzSliceableBase>();
			
			Debug.Log(slicer);
			if (slicer != null)
			{
				Debug.Log("kesilmeye girdi");
				await slicer.SliceAsync(slicePlane,null);
				
				Debug.Log("KESİLDİ");
				// Kesilen parçaya kuvvet uygulama
				
			
			}
			
			
		}*/
	}
	 private static void ProcessBulletHit(Collision hitInfo, Vector3 bulletDirection)
		{
			HitEffects hitEffects = hitInfo.gameObject.GetComponentInParent<HitEffects>();
			if (hitEffects != null && hitEffects.effects.Count > 0)
				hitEffects.PlayEffect(HitBy.Bullet, hitInfo.contacts[0].point, hitInfo.contacts[0].normal);

			// Apply damage if object hit was Destructible
			// Only do this for the first active and enabled Destructible script found in parent objects
			// Special Note: Destructible scripts are turned off on terrain trees by default (to save resources), so we will make an exception for them and process the hit anyway
			Destructible[] destObjs = hitInfo.gameObject.GetComponentsInParent<Destructible>(false);
			foreach (Destructible destObj in destObjs)
			{
				if (!destObj.isActiveAndEnabled && !destObj.isTerrainTree) continue;
				ImpactDamage bulletDamage = new ImpactDamage { DamageAmount = 50, AdditionalForce = 25, AdditionalForcePosition = hitInfo.contacts[0].point, AdditionalForceRadius = .5f };
				destObj.ApplyDamage(bulletDamage);
				break;
			}

		   

			// Apply impact force to rigidbody hit
		   

			// Check for Chip-Away Debris
		
		}
}

		

