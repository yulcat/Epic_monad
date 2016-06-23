﻿using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill {

	// base info.
	string name;
	int requireAP;
	int cooldown;
	
	// damage factors in datatype Dictionary
	Dictionary<string, float> powerFactor = new Dictionary<string, float>();
	
	// reach & range
	// 지정/범위/경로. 
	SkillType skillType;
	// 1차범위.
	RangeForm firstRangeForm;
	int firstMinReach;
	int firstMaxReach;
	int firstWidth;
	// 2차범위.   ** 범위형의 경우 반드시 1차범위 = 2차범위! **
	RangeForm secondRangeForm;
	int secondMinReach;
	int secondMaxReach;
	int secondWidth;
	
	SkillApplyType skillApplyType; // 데미지인지 힐인지 아니면 상태이상만 주는지
	
	// 이펙트 관련 정보
	string effectName;
	EffectVisualType effectVisualType;
	EffectMoveType effectMoveType;
	
	public Skill(string name, int requireAP, int cooldown, 
				 Dictionary<string, float> powerFactor,
				 SkillType skillType,
				 RangeForm firstRangeForm, int firstMinReach, int firstMaxReach, int firstWidth,
				 RangeForm secondRangeForm, int secondMinReach, int secondMaxReach, int secondWidth,
				 SkillApplyType skillApplyType,
				 string effectName, EffectVisualType effectVisualType, EffectMoveType effectMoveType)
	{
		this.name = name;
		this.requireAP = requireAP;
		this.cooldown = cooldown;
		this.powerFactor = powerFactor;
		this.skillType = skillType;
		this.firstRangeForm = firstRangeForm;
		this.firstMinReach = firstMinReach;
		this.firstMaxReach = firstMaxReach;
		this.firstWidth = firstWidth;
		this.secondRangeForm = secondRangeForm;
		this.secondMinReach = secondMinReach;
		this.secondMaxReach = secondMaxReach;
		this.secondWidth = secondWidth;
		this.skillApplyType = skillApplyType;
		this.effectName = effectName;
		this.effectVisualType = effectVisualType;
		this.effectMoveType = effectMoveType;
	}  
	
	public string GetName() {return name;}
	public int GetRequireAP() {return requireAP;}
	public int GetCooldown() {return cooldown;}	
	public float GetPowerFactor() {return powerFactor["basePower"];} // 계수의 종류를 입력받아 반환하는 형태로 보완할 예정
	public SkillType GetSkillType() {return skillType;}
	public RangeForm GetFirstRangeForm() {return firstRangeForm;}
	public int GetFirstMinReach() {return firstMinReach;}
	public int GetFirstMaxReach() {return firstMaxReach;}
	public int GetFirstWidth() {return firstWidth;}
	public RangeForm GetSecondRangeForm() {return secondRangeForm;}
	public int GetSecondMinReach() {return secondMinReach;}
	public int GetSecondMaxReach() {return secondMaxReach;}
	public int GetSecondWidth() {return secondWidth;}
	public SkillApplyType GetSkillApplyType() {return skillApplyType;}
	public string GetEffectName() {return effectName;}
	public EffectVisualType GetEffectVisualType() {return effectVisualType;}
	public EffectMoveType GetEffectMoveType() {return effectMoveType;}
}
