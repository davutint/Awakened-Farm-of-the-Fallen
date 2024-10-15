using UnityEngine;
using UnityEngine.UI;
using TMPro;
using cowsins;
using UnityEngine.Rendering;

namespace Michsky.UI.Shift
{
	public class ChapterButton : MonoBehaviour
	{
		[Header("Resources")]
		public Sprite backgroundImage;
		public string buttonTitle = "My Title";
		[TextArea] public string buttonDescription = "My Description";

		[Header("Settings")]
		public bool useCustomResources = false;

		[Header("Status")]
		public bool enableStatus;
		public StatusItem statusItem;

		Image backgroundImageObj;
		TextMeshProUGUI titleObj;
		TextMeshProUGUI descriptionObj;
		Transform statusNone;
		Transform statusLocked;
		Transform statusCompleted;
		
		[SerializeField]private int index;
		
		
		public enum StatusItem
		{
			None,
			Locked,
			Completed
		}
		private void OnEnable()
		{
			MarketManager.BuyWeapon+=GetWeaponSaleData;
		}
		private void OnDisable()
		{
			MarketManager.BuyWeapon-=GetWeaponSaleData;
		}
		void Start()
		{
			
			
			if (useCustomResources == false)
			{
				backgroundImageObj = gameObject.transform.Find("Content/Background").GetComponent<Image>();
				titleObj = gameObject.transform.Find("Content/Texts/Title").GetComponent<TextMeshProUGUI>();
				descriptionObj = gameObject.transform.Find("Content/Texts/Description").GetComponent<TextMeshProUGUI>();

				backgroundImageObj.sprite = backgroundImage;
				titleObj.text = buttonTitle;
				descriptionObj.text = buttonDescription;
			}
			if (enableStatus == true)
			{
				statusNone = gameObject.transform.Find("Content/Texts/Status/None").GetComponent<Transform>();
				statusLocked = gameObject.transform.Find("Content/Texts/Status/Locked").GetComponent<Transform>();
				statusCompleted = gameObject.transform.Find("Content/Texts/Status/Completed").GetComponent<Transform>();

				/*if (statusItem == StatusItem.None)
				{
					statusNone.gameObject.SetActive(true);
					statusLocked.gameObject.SetActive(false);
					statusCompleted.gameObject.SetActive(false);
				}

				else if (statusItem == StatusItem.Locked)
				{
					statusNone.gameObject.SetActive(false);
					statusLocked.gameObject.SetActive(true);
					statusCompleted.gameObject.SetActive(false);
				}

				else if (statusItem == StatusItem.Completed)
				{
					statusNone.gameObject.SetActive(false);
					statusLocked.gameObject.SetActive(false);
					statusCompleted.gameObject.SetActive(true);
					GetComponent<Button>().interactable=false;
				}*/
			}
			GetWeaponSaleData();
			
			
		}
		
	
		private void StatusCheck()
		{
			
			if (statusItem == StatusItem.None)
			{
				statusNone.gameObject.SetActive(true);
				statusLocked.gameObject.SetActive(false);
				statusCompleted.gameObject.SetActive(false);
			}

			else if (statusItem == StatusItem.Locked)
			{
				statusNone.gameObject.SetActive(false);
				statusLocked.gameObject.SetActive(true);
				statusCompleted.gameObject.SetActive(false);
			}

			else if (statusItem == StatusItem.Completed)
			{
				statusNone.gameObject.SetActive(false);
				statusLocked.gameObject.SetActive(false);
				statusCompleted.gameObject.SetActive(true);
				GetComponent<Button>().interactable=false;
			}
			
		}
	
	
		public void GetWeaponSaleData()
		{
			for (int i = 0; i < MarketManager.instance.WeaponsToSpawn.Length; i++)
			{
				if(PlayerPrefs.GetInt(MarketManager.instance.WeaponsToSpawn[index].GetComponent<WeaponPickeable>().weapon._name)==1)
				{
					
					statusItem=StatusItem.Completed;
					StatusCheck();
				}
				else
				{
					statusItem=StatusItem.Locked;
					StatusCheck();
				}
			}
		
		}
	}
	
	
}
