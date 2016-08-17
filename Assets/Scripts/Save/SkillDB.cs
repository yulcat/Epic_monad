using System;
using System.Collections.Generic;
using UnityEngine;

namespace Save
{
public class SkillDB
{
	private static Dictionary<string, List<string>> level1Skills = null;

	private static Dictionary<string, List<string>> GetLevel1Skills()
	{
		if (level1Skills != null)
		{
			return level1Skills;
		}

		level1Skills = new Dictionary<string, List<string>>();

		List<SkillInfo> allSkills = Parser.GetParsedSkillInfo();

		foreach (SkillInfo skill in allSkills)
		{
			if (skill.requireLevel == 1)
			{
				AddToDictionary(level1Skills, skill.owner, skill.skill.GetName());
			}
		}

		return level1Skills;
	}

	private static void AddToDictionary(Dictionary<string, List<string>> dic, string key, string elem)
	{
		if (dic.ContainsKey(key))
		{
			dic[key].Add(elem);
		}
		else
		{
			dic[key] = new List<string> { elem };
		}
	}

	public static List<string> GetLearnedSkillNames(string unitName)
	{
		List<SkillSaveData> skillSaveDatas = SaveDataCenter.GetSaveData().skills;

		if (skillSaveDatas.Count == 0)
		{
			List<string> level1Skills = GetLevel1Skills()[unitName];
			foreach (string level1Skill in level1Skills)
			{
				skillSaveDatas.Add(new SkillSaveData(level1Skill, 1));
			}
			SaveDataCenter.Save();
		}

		List<string> skillNames = new List<string>();
		List<SkillInfo> allUnitSkills = Parser.GetSkillInfoByUnit(unitName);
		foreach (SkillSaveData skillSaveData in skillSaveDatas)
		{
			if (Parser.GetSkillInfoByName(skillSaveData.skillName).owner == unitName)
			{
				skillNames.Add(skillSaveData.skillName);
			}
		}

		return skillNames;
	}

	public static bool IsLearned(string unitName, string skillName)
	{
		List<string> learnedSkillNames = GetLearnedSkillNames(unitName);
		return learnedSkillNames.Contains(skillName);
	}

	public static int GetEnhanceLevel(string character, string skillName)
	{
		List<SkillSaveData> skillSaveDatas = SaveDataCenter.GetSaveData().skills;

		foreach (SkillSaveData skillSaveData in skillSaveDatas)
		{
			if (skillSaveData.skillName == skillName)
			{
				return skillSaveData.level;
			}
		}

		return 0;
	}

	public static void Learn(string character, List<string> newSkillNames)
	{
		foreach (string skillName in newSkillNames)
		{
			Learn(character, skillName);
		}
	}

	public static void Learn(string character, string newSkillName)
	{
		List<string> skillNames = GetLearnedSkillNames(character);

		foreach (string skillName in skillNames)
		{
			if (skillName == newSkillName)
			{
				Debug.LogWarning("Already learend skill " + newSkillName + " of " + character);
				return;
			}
		}

		List<SkillSaveData> skillSaveDatas = SaveDataCenter.GetSaveData().skills;
		skillSaveDatas.Add(new SkillSaveData(newSkillName, 1));
		SaveDataCenter.Save();
	}

	public static void Enhance(string character, string skillName)
	{
		List<SkillSaveData> skillSaveDatas = SaveDataCenter.GetSaveData().skills;

		foreach (SkillSaveData skillSaveData in skillSaveDatas)
		{
			if (skillSaveData.skillName == skillName)
			{
				skillSaveData.level += 1;
				SaveDataCenter.Save();
				return;
			}
		}

		Debug.LogError("Cannot enhance not learned skill.");
	}
}
}
