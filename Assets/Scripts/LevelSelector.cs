using UnityEngine;
using System.Collections;
using System.IO;

public class LevelSelector : MonoBehaviour {
	
	public GameObject mSelectionPrefab;
	private TextAsset mLevelFile;
	public static int mLevelSelected = 15;
	
	private const string LEVEL = "level:";	
	private const string LEVEL_FILE = "Levels/levels";
	
	public const int PLAYED = 0;
	public const int PASSED = 1;
	public const float CHILD_HEIGHT = 80f;
	
	private bool mbLevelSelected;
	
	void Start () 
	{
		StringReader reader;
		GameObject selection;
		string line;
		//Using PlayerPrefs to save level completion data/last level completed data.	
		mLevelFile = Resources.Load (LEVEL_FILE) as TextAsset;
		reader = new StringReader (mLevelFile.text);
		do
		{
			line = reader.ReadLine ();
			if (line != null && line.StartsWith (LEVEL))
			{
				selection = (GameObject) Instantiate (mSelectionPrefab);
				LevelSelection select = selection.GetComponent <LevelSelection> ();
				if (PlayerPrefs.HasKey (line))
				{
					int levelInfo = PlayerPrefs.GetInt (line);
					if (levelInfo == PLAYED)
					{
						select.SetInfo (this, line, true, false);
					}
					else if (levelInfo == PASSED)
					{
						select.SetInfo (this, line, true, true);
					}
				}
				else
				{
					select.SetInfo (this, line, false, false);
				}
				RectTransform selectTransform = selection.GetComponent <RectTransform> ();
				selectTransform.SetParent (transform, false);
			}
			
		} while (line != null);
		
		mbLevelSelected = false;
		
		float height = transform.childCount * CHILD_HEIGHT;
		RectTransform tr = GetComponent <RectTransform> ();
		tr.sizeDelta = new Vector2 (0, height);
		tr.anchoredPosition  = new Vector2 (0, -height / 2);
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
	
	public void SetSelectedLevel (LevelSelection levelSelected, int level)
	{
		foreach (LevelSelection selection in GetComponentsInChildren <LevelSelection> ())
		{
			if (mLevelSelected.Equals (int.Parse (selection.mLevel.text.Substring (LEVEL.Length))))
			{
				selection.DeselectLevel ();
			}
		}
		mLevelSelected = level;
		mbLevelSelected = true;
	}
	
	public void StartLevel ()
	{
		if (mbLevelSelected)
		{
			Application.LoadLevel ("base");
		}
	}
	
	public void BackToStart ()
	{
		Application.LoadLevel ("menu");
	}
}
