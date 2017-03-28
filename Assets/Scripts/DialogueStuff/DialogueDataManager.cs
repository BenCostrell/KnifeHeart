using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DialogueDataManager {

	private Dictionary<List<Ability.Type>,List<Dialogue>> dialogueDict;
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
		dialogueDict = new Dictionary<List<Ability.Type>, List<Dialogue>> (new ListComparer<Ability.Type>());
		string fileFullString = dialogueFile.text;
		string[] fileLines;
		string[] lineEntries;
		string fileLine;
		string[] lineSeparator = new string[] { "\r\n", "\r", "\n" };
		char[] entrySeparator = new char[] { '\t' };
		fileLines = fileFullString.Split (lineSeparator, System.StringSplitOptions.None);
		for (int i = 1; i < fileLines.Length; i++) {
			fileLine = fileLines [i];
			lineEntries = fileLine.Split (entrySeparator);
			List<Ability.Type> abilityList = new List<Ability.Type> ();
			abilityList.Add(GetAbilityTypeFromString(lineEntries[0]));
			if (lineEntries [1] != "") {
				abilityList.Add (GetAbilityTypeFromString (lineEntries [1]));
				if (lineEntries [2] != "") {
					abilityList.Add (GetAbilityTypeFromString (lineEntries [2]));
				}
			}
			string blurbText = lineEntries [3];
			string mainText = lineEntries [4];
			Dialogue dialogue = new Dialogue (mainText, blurbText, abilityList [abilityList.Count - 1]);
			AddDialogueEntry (abilityList, dialogue);
		}
	}

	void AddDialogueEntry(List<Ability.Type> abilityList, Dialogue newDialogue){
		if (dialogueDict.ContainsKey (abilityList)) {
			dialogueDict [abilityList].Add (newDialogue);
		} else {
			List<Dialogue> dialogueList = new List<Dialogue> ();
			dialogueList.Add (newDialogue);
			dialogueDict.Add (abilityList, dialogueList);
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
		default:
			return Ability.Type.None;
		}
	}

    public string[] GetRpsDialogue(int scenarioNum, string winningChoice, string losingChoice)
    {
        string[] key = new string[3] { scenarioNum.ToString(), winningChoice, losingChoice };
        return rpsDialogueDict[key];
    }

	public Dialogue GetDialogue (List<Ability.Type> abilityList){
		if (dialogueDict.ContainsKey (abilityList)) {
			List<Dialogue> possibleDialogueOptions = dialogueDict [abilityList];
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

	public Dialogue GetDialogue(Ability.Type stage1Ability){
		List<Ability.Type> abilityList = new List<Ability.Type> ();
		abilityList.Add (stage1Ability);
		return GetDialogue (abilityList);
	}

	public Dialogue GetDialogue(Ability.Type stage1Ability, Ability.Type stage2Ability){
		List<Ability.Type> abilityList = new List<Ability.Type> ();
		abilityList.Add (stage1Ability);
		abilityList.Add (stage2Ability);
		return GetDialogue (abilityList);
	}

	public Dialogue GetDialogue(Ability.Type stage1Ability, Ability.Type stage2Ability, Ability.Type stage3Ability){
		List<Ability.Type> abilityList = new List<Ability.Type> ();
		abilityList.Add (stage1Ability);
		abilityList.Add (stage2Ability);
		abilityList.Add (stage3Ability);
		return GetDialogue (abilityList);
	}
}
