﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Parser : MonoBehaviour {

	public static List<UnitInfo> GetParsedUnitInfo()
	{
        List<UnitInfo> unitInfoList = new List<UnitInfo>();
		
		TextAsset csvFile = Resources.Load("Data/testStageUnitData") as TextAsset;
		string csvText = csvFile.text;
		string[] unparsedUnitInfoStrings = csvText.Split('\n');
		
		for (int i = 1; i < unparsedUnitInfoStrings.Length; i++)
		{
			UnitInfo unitInfo = new UnitInfo(unparsedUnitInfoStrings[i]);
			unitInfoList.Add(unitInfo);
		}
        
        return unitInfoList;
	}
    
    public static List<SkillInfo> GetParsedSkillInfo()
	{
        List<SkillInfo> skillInfoList = new List<SkillInfo>();
		
		TextAsset csvFile = Resources.Load("Data/testSkillData") as TextAsset;
		string csvText = csvFile.text;
		string[] unparsedSkillInfoStrings = csvText.Split('\n');
		
		for (int i = 1; i < unparsedSkillInfoStrings.Length; i++)
		{
			SkillInfo skillInfo = new SkillInfo(unparsedSkillInfoStrings[i]);
			skillInfoList.Add(skillInfo);
		}
        
        return skillInfoList;
	}
    
    public static List<TileInfo> GetParsedTileInfo()
	{
        List<TileInfo> tileInfoList = new List<TileInfo>();
		
		TextAsset csvFile = Resources.Load("Data/testMapData") as TextAsset;
		string csvText = csvFile.text;
		string[] unparsedTileInfoStrings = csvText.Split('\n');
		
		for (int reverseY = unparsedTileInfoStrings.Length -1; reverseY >= 0 ; reverseY--)
		{
            string[] parsedTileInfoStrings = unparsedTileInfoStrings[reverseY].Split(',');
			for (int x = 0; x < parsedTileInfoStrings.Length; x++)
            {
                Vector2 tilePosition = new Vector2(x, unparsedTileInfoStrings.Length - reverseY);
                TileInfo tileInfo = new TileInfo(tilePosition, parsedTileInfoStrings[x]);
    			tileInfoList.Add(tileInfo);
            }
        }
        
        return tileInfoList;
	}
}
