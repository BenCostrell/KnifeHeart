using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class DialogueDataManager {

	private Dictionary<List<Ability.Type>,List<Dialogue>> callDialogueDict;
    private Dictionary<List<Ability.Type>, List<Dialogue>> responseDialogueDict;
    private Dictionary<string[], string[]> rpsDialogueDict;

	private class ListComparer<T> : IEqualityComparer<List<T>>{
		public bool Equals(List<T> x, List<T> y){
			return x.SequenceEqual (y);
		}

		public int GetHashCode(List<T> obj){
			int hashcode = 0;
			foreach (T t in obj) {
				hashcode ^= t.GetHashCode ();
			}
			return hashcode;
		}
	}

    public class StringArrayComparer : IEqualityComparer<string[]>
    {
        public bool Equals(string[] x, string[] y)
        {
            if (x.Length != y.Length)
            {
                return false;
            }
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(string[] obj)
        {
            int hashcode = 0;
            foreach (string t in obj)
            {
                hashcode ^= t.GetHashCode();
            }
            return hashcode;
        }
    }

    public void ParseRpsDialogueFile(TextAsset dialogueFile)
    {
        rpsDialogueDict = new Dictionary<string[], string[]>(new StringArrayComparer());
        string fileFullString = dialogueFile.text;
        string[] fileLines;
        string[] lineEntries;
        string fileLine;
        string[] lineSeparator = new string[] { "\r\n", "\r", "\n" };
        char[] entrySeparator = new char[] { '\t' };
        fileLines = fileFullString.Split(lineSeparator, System.StringSplitOptions.None);
        for (int i = 1; i < fileLines.Length; i++)
        {
            fileLine = fileLines[i];
            lineEntries = fileLine.Split(entrySeparator);
            string[] key = new string[3] { lineEntries[0].ToUpper().Trim(), lineEntries[1].ToUpper().Trim(), lineEntries[2].ToUpper().Trim() };
            string[] entry = new string[3] { lineEntries[3], lineEntries[4], lineEntries[5] };
            rpsDialogueDict.Add(key, entry);
        }
    }

    public void ParseDialogueFile(TextAsset dialogueFile){
		callDialogueDict = new Dictionary<List<Ability.Type>, List<Dialogue>> (new ListComparer<Ability.Type>());
        responseDialogueDict = new Dictionary<List<Ability.Type>, List<Dialogue>>(new ListComparer<Ability.Type>());
        string fileFullString = dialogueFile.text;
		string[] fileLines;
		string[] lineEntries;
		string fileLine;
		string[] lineSeparator = new string[] { "\r\n", "\r", "\n" };
		char[] entrySeparator = new char[] { '\t' };
		fileLines = fileFullString.Split (lineSeparator, System.StringSplitOptions.None);
		for (int i = 6; i < fileLines.Length; i++) {
			fileLine = fileLines [i];
			lineEntries = fileLine.Split (entrySeparator);
			List<Ability.Type> abilityList = new List<Ability.Type> ();
			abilityList.Add(GetAbilityTypeFromString(lineEntries[1]));
			if (lineEntries [2] != "") {
				abilityList.Add (GetAbilityTypeFromString (lineEntries [2]));
				if (lineEntries [3] != "") {
					abilityList.Add (GetAbilityTypeFromString (lineEntries [3]));
				}
			}
			string callBlurbText = lineEntries [4];
            string callMainText = lineEntries[5];
            string responseBlurbText = lineEntries[6];
            string responseMainText = lineEntries[7];
			Dialogue callDialogue = new Dialogue (callMainText, callBlurbText, abilityList [abilityList.Count - 1]);
            Dialogue responseDialogue = new Dialogue(responseMainText, responseBlurbText, abilityList[abilityList.Count - 1]);
            AddDialogueEntry(abilityList, callDialogue, callDialogueDict);
            AddDialogueEntry(abilityList, responseDialogue, responseDialogueDict);
		}
	}

    public string ParseTextForLineBreaks(string inputText, Text textComponent)
    {
        string parsedText = "";
        Font font = textComponent.font;
        int size = textComponent.fontSize;
        font.RequestCharactersInTexture(" ", size, FontStyle.Normal);
        string[] words = inputText.Split(new char[] { " "[0] });
        float lineWidth = textComponent.gameObject.GetComponent<RectTransform>().rect.width;
        float spaceLeft = lineWidth;
        float wordWidth = 0;
        float spaceWidth = StringWidth(" ", textComponent);
        for (int i = 0; i < words.Length; i++)
        {
            wordWidth = StringWidth(words[i], textComponent);
            if (wordWidth > lineWidth)
            {
                parsedText += words[i] + " ";
                spaceLeft = Mathf.Abs(wordWidth % lineWidth - spaceLeft);
            }
            else if (wordWidth > spaceLeft)
            {
                parsedText += "\n" + words[i] + " ";
                spaceLeft = lineWidth - (wordWidth + spaceWidth);
            }
            else
            {
                parsedText += words[i] + " ";
                spaceLeft -= (wordWidth + spaceWidth);
            }
        }

        return parsedText;
    }

    float StringWidth(string str, Text textComponent)
    {
        float width = 0;

        CharacterInfo charInfo;
        Font font = textComponent.font;
        int size = textComponent.fontSize;
        font.RequestCharactersInTexture(str, size, FontStyle.Normal);

        for (int i = 0; i < str.Length; i++)
        {
            font.GetCharacterInfo(str[i], out charInfo, size, FontStyle.Normal);
            width += charInfo.advance;
        }

        return width;
    }


	void AddDialogueEntry(List<Ability.Type> abilityList, Dialogue newDialogue, Dictionary<List<Ability.Type>, List<Dialogue>> dict){
        if (dict.ContainsKey (abilityList)) {
			dict [abilityList].Add (newDialogue);
		} else {
			List<Dialogue> dialogueList = new List<Dialogue> ();
			dialogueList.Add (newDialogue);
			dict.Add (abilityList, dialogueList);
		}
	}

	Ability.Type GetAbilityTypeFromString(string abilityString){
		string str = abilityString.ToUpper ().Trim ();
		switch (str) {
		case "FIREBALL":
			return Ability.Type.Fireball;
		case "SHIELD":
			return Ability.Type.Shield;
		case "SING":
			return Ability.Type.Sing;
		case "LUNGE":
			return Ability.Type.Lunge;
		case "WALLOP":
			return Ability.Type.Wallop;
		case "PULL":
			return Ability.Type.Pull;
        case "BLINK":
            return Ability.Type.Blink;
		default:
			return Ability.Type.None;
		}
	}

    public string[] GetRpsDialogue(int scenarioNum, VisualNovelScene.RpsOption winningChoice, VisualNovelScene.RpsOption losingChoice)
    {
        string[] key = new string[3] { scenarioNum.ToString(),
            RpsOptionToString(winningChoice),
            RpsOptionToString(losingChoice) };
        return rpsDialogueDict[key];
    }

    string RpsOptionToString(VisualNovelScene.RpsOption rpsOption)
    {
        string rpsString = "";
        switch (rpsOption)
        {
            case VisualNovelScene.RpsOption.Rock:
                rpsString = "BE AGGRESSIVE";
                break;
            case VisualNovelScene.RpsOption.Paper:
                rpsString = "BE NICE";
                break;
            case VisualNovelScene.RpsOption.Scissors:
                rpsString = "BE PASSIVE AGGRESSIVE";
                break;
            case VisualNovelScene.RpsOption.None:
                break;
            default:
                break;
        }

        return rpsString;
    }

	public Dialogue GetDialogue (List<Ability.Type> abilityList, bool call){
        Dictionary<List<Ability.Type>, List<Dialogue>> dict;
        if (call)
        {
            dict = callDialogueDict;
        }
        else
        {
            dict = responseDialogueDict;
        }
        if (dict.ContainsKey (abilityList)) {
			List<Dialogue> possibleDialogueOptions = dict [abilityList];
			int index = Random.Range (0, possibleDialogueOptions.Count);
			return possibleDialogueOptions[index];
		} else {
			Debug.Log ("can't find: ");
			for (int i = 0; i < abilityList.Count; i++) {
				Debug.Log (abilityList [i]);
			}
			return null;
		}
	}

	public Dialogue GetDialogue(Ability.Type stage1Ability, bool call){
		List<Ability.Type> abilityList = new List<Ability.Type> ();
		abilityList.Add (stage1Ability);
		return GetDialogue (abilityList, call);
	}

	public Dialogue GetDialogue(Ability.Type stage1Ability, Ability.Type stage2Ability, bool call){
		List<Ability.Type> abilityList = new List<Ability.Type> ();
		abilityList.Add (stage1Ability);
		abilityList.Add (stage2Ability);
		return GetDialogue (abilityList, call);
	}

	public Dialogue GetDialogue(Ability.Type stage1Ability, Ability.Type stage2Ability, Ability.Type stage3Ability, bool call){
		List<Ability.Type> abilityList = new List<Ability.Type> ();
		abilityList.Add (stage1Ability);
		abilityList.Add (stage2Ability);
		abilityList.Add (stage3Ability);
		return GetDialogue (abilityList, call);
	}
}
