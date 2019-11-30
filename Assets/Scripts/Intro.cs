using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
	// Start is called before the first frame update
	public Text text1;
	public Text text2;
	public Text text3;
	public Button Next;
	private float tinyDelay = 0.5f;
	float curTime;
	public Text buttonText;
	Text[] fields;


    string[] para1 = { "The year is 2112.", "Our story follows one Dr.Rex `Baldy` Garibaldi,", "a renowned theoretical physicist specializing in multi-dimensional travel." };
	string[] para2 = { "A man with much inside his head, and sadly, less on top with each passing year.", "On an excursion to the remote and savage wastes of Dimension YYZ,the good doctor has a scuffle", "with thewarlike gelatine-people, the Morpaglorps, and while he escapes with his life, his beloved hairpiece does not make the journey with him." };
	string[] para3 = { "Torn between risking life and limb on a vain and dangerous journey into the interdimensional wilds", "facing the ridicule of his colleagues at the next physics department social hour", "Dr.Garibaldi sets out onan intrepid dimension-leaping expedition to restore his 'do to its former glory..." };
	//string[] para1 = { "This is baldy! An experimental physicist", "Recently baldy has been working on some new interdimensional technology", "But one of his experiments went horribly wrong..." };
	//string[] para2 = { "Baldy got caught in a dimension warp!", "He managed to escape alive, but most of his hair was lost in another world", "many of his belongings were lost too" };
	//string[] para3 = { "Help baldy on his quest to regrow his hair!", "Shift between worlds to solve puzzles and find the hair syrum", "Baldy is counting on you." };
    string[][] tuples;
	private float wordGap = .5f;
	private float alphaTime = 2f;
	private float fastAlph = 0.5f;
	private float endOfSection = 0.5f;

	int para = 0;
	int section = 0;

	void Start()
	{
		curTime = Time.time;
		tuples = new string[][] { para1, para2, para3};
		fields = new Text[] { text1, text2, text3 };
		setText(tuples[0]);
		setAlpha(0);
		text1.CrossFadeAlpha(1, alphaTime, true);
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void setText(string[] tuple)
	{
		text1.text = tuple[0];
		text2.text = tuple[1];
		text3.text = tuple[2];
	}

	public void setAlpha(float alpha, float time = 0)
	{
		text1.CrossFadeAlpha(alpha, time, true);
		text2.CrossFadeAlpha(alpha, time, true);
		text3.CrossFadeAlpha(alpha, time, true);
	}

	public void LoadNext()
	{
		if (Time.time - curTime > tinyDelay)
		{
			if (para == 2 && section == 1)
			{
				buttonText.text = "Play";
			}
			if (para == 2 && section == 2)
			{
				text1.CrossFadeAlpha(0, 0,true);
				text3.CrossFadeAlpha(0, 0,true);
				text2.text = "Loading";
				SceneLoader.LoadFirstLevel();
			}
			else
			{
				if (section == 2)
				{
					StartCoroutine(nextParagraph());
				}
				else
				{
					section += 1;
					fields[section].CrossFadeAlpha(1, alphaTime, true);
				}
			}
			curTime = Time.time;
		}
	}

	IEnumerator nextParagraph()
	{
		setAlpha(0, fastAlph);
		yield return new WaitForSeconds(fastAlph);
		section = 0;
		para += 1;
		setText(tuples[para]);
		fields[section].CrossFadeAlpha(1, alphaTime, true);
		
	}
}
