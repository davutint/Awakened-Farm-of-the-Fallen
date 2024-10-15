using System.Collections;
using CrazyGames;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
	public Button[] levelButtons;
	[SerializeField]private int[]levelints;
	private void Awake()
	{
		levelints=new int[levelButtons.Length];
		
	}
	void Start()
	{
		  
	  if (CrazySDK.IsAvailable)
	  {
			CrazySDK.Init(() =>
			{
				 CrazySDK.User.GetUser((user => { Debug.Log(("Got user" + user)); }));
					CrazySDK.Ad.HasAdblock((adblockPresent) => { Debug.Log( "Has adblock: " + adblockPresent); });
			});
		}
		
		  LevelUnlocker();
	}
	
	
	public void LevelUnlocker()
	{
		levelints[0]=PlayerPrefs.GetInt("Farm",0);
		levelints[1]=PlayerPrefs.GetInt("Forest",0);
		levelints[2]=PlayerPrefs.GetInt("Military",0);
		
	
		for (int i = 0; i < levelButtons.Length; i++)
		{
			if (levelints[i]==1)
			{
				levelButtons[i].gameObject.GetComponent<Animator>().enabled=true;
				levelButtons[i].gameObject.transform.Find("Content/Background").GetComponent<Animator>().enabled=true;;
				levelButtons[i].interactable=true;
			}
		}
	}
	
	public void OpenLevel(string scenename)
	{
		CrazySDK.Game.GameplayStart();

		SceneManager.LoadScene(sceneName:scenename);
	}
		
}
