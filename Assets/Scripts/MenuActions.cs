using UnityEngine;
using System.Collections;

public class MenuActions : MonoBehaviour {

	private RingController.Ring.Direction direction;
	
	private int NUM_LEVELS = 13;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{

	}
	
	public void LevelSelect ()
	{
		Application.LoadLevel ("levelselect");
	}
	
	public void StartGame ()
	{
		int level = 1;
		bool bContinue = true;
		
		while (bContinue)
		{
			if (!PlayerPrefs.HasKey ("level:" + level.ToString ()))
			{
				bContinue = false;
			}
		}
		
		if (level > NUM_LEVELS)
		{
			level = 1;
		}
		
		LevelSelector.mLevelSelected = level;
		
		Application.LoadLevel ("base");
	}
	
}
