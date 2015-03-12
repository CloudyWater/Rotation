using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour {

	public Text mLevel;
	public Text mLevelPassed;
	public LevelSelector mSelector;
	public bool mbLevelPlayed;
	
	private const string YES = "Yes";
	private const string NO = "No";
	private const string LEVEL = "level:";
	
	private Color mSelected;
	private Color mNormal;
	private Color mDisabled;
	
	// Use this for initialization
	void Start () 
	{
		mNormal = new Color (.101f, 1f, 0f);
		mSelected = new Color (0f,0f,1f);
	}
	
	// Update is called once per frame
	void Update () 
	{	

	}
	
	public void SetInfo (LevelSelector selector, string Level, bool bPlayed, bool bPassed)
	{
		mDisabled = new Color (.5f,.5f,.5f, 1f);
		mLevel.text = Level;
		mSelector = selector;
		if (bPassed)
		{
			mLevelPassed.text = YES;
		}
		else
		{
			mLevelPassed.text = NO;
		}
		mbLevelPlayed = bPlayed;
		
		if (!bPlayed)
		{
			Image image = GetComponent <Image> ();
			image.color = mDisabled;
			foreach (Text text in GetComponentsInChildren <Text> ())
			{
				text.color = mDisabled;
			}
		}
	}
	
	public void SelectLevel ()
	{
		if (mbLevelPlayed)
		{
			Image image = GetComponent <Image> ();
			int levelNum = int.Parse (mLevel.text.Substring (LEVEL.Length));
			image.color = mSelected;
			mSelector.SetSelectedLevel (this, levelNum);
		}
	}
	
	public void DeselectLevel ()
	{
		Image image = GetComponent <Image> ();
		image.color = mNormal;
	}
}
