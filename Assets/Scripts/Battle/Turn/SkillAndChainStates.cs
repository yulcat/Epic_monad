﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Enums;

namespace Battle.Turn
{
	public class SkillAndChainStates
	{
		public static IEnumerator SelectSkillState(BattleData battleData)
		{
			while (battleData.currentState == CurrentState.SelectSkill)
			{
				battleData.uiManager.UpdateSkillInfo(battleData.selectedUnitObject);
				battleData.uiManager.CheckUsableSkill(battleData.selectedUnitObject);

				battleData.rightClicked = false;
				battleData.cancelClicked = false;

				battleData.isWaitingUserInput = true;
				battleData.indexOfSeletedSkillByUser = 0;
				while (battleData.indexOfSeletedSkillByUser == 0)
				{
					if (battleData.rightClicked || battleData.cancelClicked)
					{
						battleData.rightClicked = false;
						battleData.cancelClicked = false;

						battleData.uiManager.DisableSkillUI();
						battleData.currentState = CurrentState.FocusToUnit;
						battleData.isWaitingUserInput = false;
						yield break;
					}
					yield return null;
				}
				battleData.isWaitingUserInput = false;

				battleData.uiManager.DisableSkillUI();

				BattleManager battleManager = battleData.battleManager;
				Skill selectedSkill = battleData.SelectedSkill;
				SkillType skillTypeOfSelectedSkill = selectedSkill.GetSkillType();
				if (skillTypeOfSelectedSkill == SkillType.Area)
				{
					battleData.currentState = CurrentState.SelectSkillApplyDirection;
					yield return battleManager.StartCoroutine(SelectSkillApplyDirection(battleData, battleData.selectedUnitObject.GetComponent<Unit>().GetDirection()));
				}
				else
				{
					battleData.currentState = CurrentState.SelectSkillApplyPoint;
					yield return battleManager.StartCoroutine(SelectSkillApplyPoint(battleData, battleData.selectedUnitObject.GetComponent<Unit>().GetDirection()));
				}
			}
		}

		private static IEnumerator SelectSkillApplyDirection(BattleData battleData, Direction originalDirection)
		{
			Direction beforeDirection = originalDirection;
			List<GameObject> selectedTiles = new List<GameObject>();
			Unit selectedUnit = battleData.selectedUnitObject.GetComponent<Unit>();
			Skill selectedSkill = battleData.SelectedSkill;

			battleData.rightClicked = false;
			battleData.isWaitingUserInput = true;
			battleData.isSelectedTileByUser = false;

			if (battleData.currentState == CurrentState.SelectSkill)
			{
				battleData.uiManager.DisableCancelButtonUI();
				yield break;
			}

			if (battleData.currentState == CurrentState.SelectSkillApplyDirection)
			{
				selectedTiles = battleData.tileManager.GetTilesInRange(selectedSkill.GetSecondRangeForm(),
															selectedUnit.GetPosition(),
															selectedSkill.GetSecondMinReach(),
															selectedSkill.GetSecondMaxReach(),
															selectedUnit.GetDirection(),
															false);

				battleData.tileManager.ChangeTilesToSeletedColor(selectedTiles, TileColor.Red);
			}

			while (battleData.currentState == CurrentState.SelectSkillApplyDirection)
			{
				Direction newDirection = Utility.GetMouseDirectionByUnit(battleData.selectedUnitObject);
				// Debug.LogWarning(newDirection);
				if (beforeDirection != newDirection)
				{
					battleData.tileManager.ChangeTilesFromSeletedColorToDefaultColor(selectedTiles);

					beforeDirection = newDirection;
					selectedUnit.SetDirection(newDirection);
					selectedTiles = battleData.tileManager.GetTilesInRange(selectedSkill.GetSecondRangeForm(),
																selectedUnit.GetPosition(),
																selectedSkill.GetSecondMinReach(),
																selectedSkill.GetSecondMaxReach(),
																selectedUnit.GetDirection(),
																false);

					battleData.tileManager.ChangeTilesToSeletedColor(selectedTiles, TileColor.Red);
				}

				if (battleData.rightClicked || battleData.cancelClicked)
				{
					battleData.rightClicked = false;
					battleData.cancelClicked = false;
					battleData.uiManager.DisableCancelButtonUI();

					selectedUnit.SetDirection(originalDirection);
					battleData.tileManager.ChangeTilesFromSeletedColorToDefaultColor(selectedTiles);
					battleData.currentState = CurrentState.SelectSkill;
					yield break;
				}

				if (battleData.isSelectedTileByUser)
				{
					battleData.isWaitingUserInput = false;
					battleData.uiManager.DisableCancelButtonUI();
					battleData.tileManager.ChangeTilesFromSeletedColorToDefaultColor(selectedTiles);

					BattleManager battleManager = battleData.battleManager;
					battleData.currentState = CurrentState.CheckApplyOrChain;
					yield return battleManager.StartCoroutine(CheckApplyOrChain(battleData, selectedUnit.GetPosition(), originalDirection));
				}
				yield return null;
			}
			yield return null;
		}

		private static IEnumerator SelectSkillApplyPoint(BattleData battleData, Direction originalDirection)
		{
			Direction beforeDirection = originalDirection;
			Unit selectedUnit = battleData.selectedUnitObject.GetComponent<Unit>();

			if (battleData.currentState == CurrentState.SelectSkill)
			{
				battleData.uiManager.DisableCancelButtonUI();
				yield break;
			}

			while (battleData.currentState == CurrentState.SelectSkillApplyPoint)
			{
				Vector2 selectedUnitPos = battleData.selectedUnitObject.GetComponent<Unit>().GetPosition();

				List<GameObject> activeRange = new List<GameObject>();
				Skill selectedSkill = battleData.SelectedSkill;
				activeRange = battleData.tileManager.GetTilesInRange(selectedSkill.GetFirstRangeForm(),
														selectedUnitPos,
														selectedSkill.GetFirstMinReach(),
														selectedSkill.GetFirstMaxReach(),
														battleData.selectedUnitObject.GetComponent<Unit>().GetDirection(),
														selectedSkill.GetIncludeMyself());
				battleData.tileManager.ChangeTilesToSeletedColor(activeRange, TileColor.Red);

				battleData.rightClicked = false;
				battleData.cancelClicked = false;
				battleData.uiManager.EnableCancelButtonUI();

				battleData.isWaitingUserInput = true;
				battleData.isSelectedTileByUser = false;
				while (!battleData.isSelectedTileByUser)
				{
					Direction newDirection = Utility.GetMouseDirectionByUnit(battleData.selectedUnitObject);
					if (beforeDirection != newDirection)
					{
						beforeDirection = newDirection;
						selectedUnit.SetDirection(newDirection);
					}

					if (battleData.rightClicked || battleData.cancelClicked)
					{
						battleData.rightClicked = false;
						battleData.cancelClicked = false;
						battleData.uiManager.DisableCancelButtonUI();

						battleData.tileManager.ChangeTilesFromSeletedColorToDefaultColor(activeRange);
						battleData.currentState = CurrentState.SelectSkill;
						battleData.isWaitingUserInput = false;
						yield break;
					}
					yield return null;
				}
				battleData.isSelectedTileByUser = false;
				battleData.isWaitingUserInput = false;
				battleData.uiManager.DisableCancelButtonUI();

				// 타겟팅 스킬을 타겟이 없는 장소에 지정했을 경우 적용되지 않도록 예외처리 필요 - 대부분의 스킬은 논타겟팅. 추후 보강.

				battleData.tileManager.ChangeTilesFromSeletedColorToDefaultColor(activeRange);
				battleData.uiManager.DisableSkillUI();

				BattleManager battleManager = battleData.battleManager;
				battleData.currentState = CurrentState.CheckApplyOrChain;
				yield return battleManager.StartCoroutine(CheckApplyOrChain(battleData, battleData.selectedTilePosition, originalDirection));
			}
		}

		private static IEnumerator CheckApplyOrChain(BattleData battleData, Vector2 selectedTilePosition, Direction originalDirection)
		{
			while (battleData.currentState == CurrentState.CheckApplyOrChain)
			{
				GameObject selectedTile = battleData.tileManager.GetTile(selectedTilePosition);
				Camera.main.transform.position = new Vector3(selectedTile.transform.position.x, selectedTile.transform.position.y, -10);

				Skill selectedSkill = battleData.SelectedSkill;
				List<GameObject> selectedTiles = battleData.tileManager.GetTilesInRange(selectedSkill.GetSecondRangeForm(),
																			selectedTilePosition,
																			selectedSkill.GetSecondMinReach(),
																			selectedSkill.GetSecondMaxReach(),
																			battleData.selectedUnitObject.GetComponent<Unit>().GetDirection(),
																			true);
				if ((selectedSkill.GetSkillType() == SkillType.Area) && (!selectedSkill.GetIncludeMyself()))
					selectedTiles.Remove(battleData.tileManager.GetTile(selectedTilePosition));
				battleData.tileManager.ChangeTilesToSeletedColor(selectedTiles, TileColor.Red);

				CheckChainPossible(battleData);
				battleData.uiManager.SetSkillCheckAP(battleData.selectedUnitObject, selectedSkill);

				battleData.rightClicked = false;
				battleData.cancelClicked = false;

				battleData.skillApplyCommand = SkillApplyCommand.Waiting;
				while (battleData.skillApplyCommand == SkillApplyCommand.Waiting)
				{
					if (battleData.rightClicked || battleData.cancelClicked)
					{
						battleData.rightClicked = false;
						battleData.cancelClicked = false;

						Camera.main.transform.position = new Vector3(battleData.selectedUnitObject.transform.position.x, battleData.selectedUnitObject.transform.position.y, -10);
						battleData.uiManager.DisableSkillCheckUI();
						battleData.tileManager.ChangeTilesFromSeletedColorToDefaultColor(selectedTiles);
						battleData.selectedUnitObject.GetComponent<Unit>().SetDirection(originalDirection);
						if (selectedSkill.GetSkillType() == SkillType.Area)
							battleData.currentState = CurrentState.SelectSkill;
						else
							battleData.currentState = CurrentState.SelectSkillApplyPoint;
						yield break;
					}
					yield return null;
				}

				battleData.tileManager.ChangeTilesFromSeletedColorToDefaultColor(selectedTiles);
				BattleManager battleManager = battleData.battleManager;
				if (battleData.skillApplyCommand == SkillApplyCommand.Apply)
				{
					battleData.skillApplyCommand = SkillApplyCommand.Waiting;
					// 체인이 가능한 스킬일 경우. 체인 발동.
					if (selectedSkill.GetSkillApplyType() == SkillApplyType.Damage)
					{
						// 자기 자신을 체인 리스트에 추가.
						ChainList.AddChains(battleData.selectedUnitObject, selectedTiles, battleData.indexOfSeletedSkillByUser);
						// 체인 체크, 순서대로 공격.
						List<ChainInfo> allVaildChainInfo = ChainList.GetAllChainInfoToTargetArea(battleData.selectedUnitObject, selectedTiles);
						int chainCombo = allVaildChainInfo.Count;
						battleData.currentState = CurrentState.ApplySkill;

						foreach (var chainInfo in allVaildChainInfo)
						{
							GameObject focusedTile = chainInfo.GetTargetArea()[0];
							Camera.main.transform.position = new Vector3(focusedTile.transform.position.x, focusedTile.transform.position.y, -10);
							yield return battleManager.StartCoroutine(ApplySkill(battleData, chainInfo, chainCombo));
						}

						Camera.main.transform.position = new Vector3(battleData.selectedUnitObject.transform.position.x, battleData.selectedUnitObject.transform.position.y, -10);
						battleData.currentState = CurrentState.FocusToUnit;
						// yield return battleManager.StartCoroutine(BattleManager.FocusToUnit(battleData));
					}
					// 체인이 불가능한 스킬일 경우, 그냥 발동.
					else
					{
						battleData.currentState = CurrentState.ApplySkill;
						yield return battleManager.StartCoroutine(ApplySkill(battleData, selectedTiles));
					}
				}
				else if (battleData.skillApplyCommand == SkillApplyCommand.Chain)
				{
					battleData.skillApplyCommand = SkillApplyCommand.Waiting;
					battleData.currentState = CurrentState.ChainAndStandby;
					yield return battleManager.StartCoroutine(ChainAndStandby(battleData, selectedTiles));
				}
				else
					yield return null;
			}
			yield return null;
		}

		private static void CheckChainPossible(BattleData battleData)
		{
			bool isPossible = false;

			// ap 조건으로 체크.
			int requireAP = battleData.SelectedSkill.GetRequireAP();
			int remainAPAfterChain = battleData.selectedUnitObject.GetComponent<Unit>().GetCurrentActivityPoint() - requireAP;

			foreach (var unit in battleData.unitManager.GetAllUnits())
			{
				if ((unit != battleData.selectedUnitObject) &&
				(unit.GetComponent<Unit>().GetCurrentActivityPoint() > remainAPAfterChain))
				{
					isPossible = true;
				}
			}

			// 스킬 타입으로 체크. 공격스킬만 체인을 걸 수 있음.
			if (battleData.SelectedSkill.GetSkillApplyType()
				!= SkillApplyType.Damage)
			{
				isPossible = false;
			}

			battleData.uiManager.EnableSkillCheckChainButton(isPossible);
		}

		private static IEnumerator ChainAndStandby(BattleData battleData, List<GameObject> selectedTiles)
		{
			battleData.tileManager.ChangeTilesFromSeletedColorToDefaultColor(selectedTiles);

			// 방향 돌리기.
			battleData.selectedUnitObject.GetComponent<Unit>().SetDirection(Utility.GetDirectionToTarget(battleData.selectedUnitObject, selectedTiles));
			// 스킬 시전에 필요한 ap만큼 선 차감.
			int requireAP = battleData.SelectedSkill.GetRequireAP();
			battleData.selectedUnitObject.GetComponent<Unit>().UseActionPoint(requireAP);
			// 체인 목록에 추가.
			ChainList.AddChains(battleData.selectedUnitObject, selectedTiles, battleData.indexOfSeletedSkillByUser);
			battleData.indexOfSeletedSkillByUser = 0; // return to init value.
			yield return new WaitForSeconds(0.5f);

			Camera.main.transform.position = new Vector3(battleData.selectedUnitObject.transform.position.x, battleData.selectedUnitObject.transform.position.y, -10);
			battleData.currentState = CurrentState.Standby;
			battleData.alreadyMoved = false;
			BattleManager battleManager = battleData.battleManager;
			yield return battleManager.StartCoroutine(BattleManager.Standby()); // 이후 대기.
		}

		// 체인 가능 스킬일 경우의 스킬 시전 코루틴. 체인 정보와 배수를 받는다.
		private static IEnumerator ApplySkill(BattleData battleData, ChainInfo chainInfo, int chainCombo)
		{
			GameObject unitObjectInChain = chainInfo.GetUnit();
			Unit unitInChain = unitObjectInChain.GetComponent<Unit>();
			Skill appliedSkill = unitInChain.GetSkillList()[chainInfo.GetSkillIndex() - 1];
			List<GameObject> selectedTiles = chainInfo.GetTargetArea();

			// 시전 방향으로 유닛의 바라보는 방향을 돌림.
			if (appliedSkill.GetSkillType() != SkillType.Area)
				unitInChain.SetDirection(Utility.GetDirectionToTarget(unitInChain.gameObject, selectedTiles));

			// 자신의 체인 정보 삭제.
			ChainList.RemoveChainsFromUnit(unitObjectInChain);

			// 이펙트 임시로 비활성화.
			// yield return StartCoroutine(ApplySkillEffect(appliedSkill, unitInChain.gameObject, selectedTiles));

			List<GameObject> targets = new List<GameObject>();

			foreach (var tileObject in selectedTiles)
			{
				Tile tile = tileObject.GetComponent<Tile>();
				if (tile.IsUnitOnTile())
				{
					targets.Add(tile.GetUnitOnTile());
				}
			}

			foreach (var target in targets)
			{
				// 방향 체크.
				Utility.GetDegreeAtAttack(unitObjectInChain, target);
				BattleManager battleManager = battleData.battleManager;
				if (appliedSkill.GetSkillApplyType() == SkillApplyType.Damage)
				{
					// 방향 보너스.
					float directionBouns = Utility.GetDirectionBonus(unitObjectInChain, target);

					// 천체속성 보너스.
					float celestialBouns = Utility.GetCelestialBouns(unitObjectInChain, target);
					if (celestialBouns == 1.2f) unitObjectInChain.GetComponent<Unit>().PrintCelestialBouns();
					else if (celestialBouns == 0.8f) target.GetComponent<Unit>().PrintCelestialBouns();

					var damageAmount = (int)((chainCombo * battleData.chainDamageFactor) * directionBouns * celestialBouns * unitInChain.GetActualPower() * appliedSkill.GetPowerFactor());
					var damageCoroutine = target.GetComponent<Unit>().Damaged(unitInChain.GetUnitClass(), damageAmount, false);

					if (target == targets[targets.Count-1])
					{
						yield return battleManager.StartCoroutine(damageCoroutine);
					}
					else
					{
						battleManager.StartCoroutine(damageCoroutine);
					}
					Debug.Log("Apply " + damageAmount + " damage to " + target.GetComponent<Unit>().GetName() + "\n" +
								"ChainCombo : " + chainCombo);
				}
				else if (appliedSkill.GetSkillApplyType() == SkillApplyType.Heal)
				{
					var recoverAmount = (int)(unitInChain.GetActualPower() * appliedSkill.GetPowerFactor());
					var recoverHealthCoroutine = target.GetComponent<Unit>().RecoverHealth(recoverAmount);

					if (target == targets[targets.Count-1])
					{
						yield return battleManager.StartCoroutine(recoverHealthCoroutine);
					}
					else
					{
						battleManager.StartCoroutine(recoverHealthCoroutine);
					}

					Debug.Log("Apply " + recoverAmount + " heal to " + target.GetComponent<Unit>().GetName());
				}

				// FIXME : 버프, 디버프는 아직 미구현. 데미지/힐과 별개일 때도 있고 같이 들어갈 때도 있으므로 별도의 if문으로 구현할 것.
				else
				{
					Debug.Log("Apply additional effect to " + target.GetComponent<Unit>().name);
				}
			}

			// tileManager.ChangeTilesFromSeletedColorToDefaultColor(selectedTiles);

			int requireAP = appliedSkill.GetRequireAP();
			if (unitInChain.gameObject == battleData.selectedUnitObject)
				unitInChain.UseActionPoint(requireAP); // 즉시시전 대상만 ap를 차감. 나머지는 선차감되었으므로 패스.
			battleData.indexOfSeletedSkillByUser = 0; // return to init value.

			yield return new WaitForSeconds(0.5f);

			battleData.alreadyMoved = false;
		}

		// 체인 불가능 스킬일 경우의 스킬 시전 코루틴. 스킬 적용 범위만 받는다.
		private static IEnumerator ApplySkill(BattleData battleData, List<GameObject> selectedTiles)
		{
			Unit selectedUnit = battleData.selectedUnitObject.GetComponent<Unit>();
			Skill appliedSkill = battleData.SelectedSkill;
			BattleManager battleManager = battleData.battleManager;

			// 시전 방향으로 유닛의 바라보는 방향을 돌림.
			if (appliedSkill.GetSkillType() != SkillType.Area)
				selectedUnit.SetDirection(Utility.GetDirectionToTarget(selectedUnit.gameObject, selectedTiles));

			// 이펙트 임시로 비활성화.
			// yield return battleManager.StartCoroutine(ApplySkillEffect(appliedSkill, battleData.selectedUnitObject, selectedTiles));

			List<GameObject> targets = new List<GameObject>();

			foreach (var tileObject in selectedTiles)
			{
				Tile tile = tileObject.GetComponent<Tile>();
				if (tile.IsUnitOnTile())
				{
					targets.Add(tile.GetUnitOnTile());
				}
			}

			foreach (var target in targets)
			{
				if (appliedSkill.GetSkillApplyType() == SkillApplyType.Damage)
				{
					// 방향 보너스.
					float directionBouns = Utility.GetDirectionBonus(battleData.selectedUnitObject, target);

					// 천체속성 보너스.
					float celestialBouns = Utility.GetCelestialBouns(battleData.selectedUnitObject, target);
					if (celestialBouns == 1.2f) battleData.selectedUnitObject.GetComponent<Unit>().PrintCelestialBouns();
					else if (celestialBouns == 0.8f) target.GetComponent<Unit>().PrintCelestialBouns();

					var damageAmount = (int)(directionBouns * celestialBouns * selectedUnit.GetActualPower() * appliedSkill.GetPowerFactor());
					var damageCoroutine = target.GetComponent<Unit>().Damaged(selectedUnit.GetUnitClass(), damageAmount, false);

					if (target == targets[targets.Count-1])
					{
						yield return battleManager.StartCoroutine(damageCoroutine);
					}
					else
					{
						battleManager.StartCoroutine(damageCoroutine);
					}
					Debug.Log("Apply " + damageAmount + " damage to " + target.GetComponent<Unit>().GetName());
				}
				else if (appliedSkill.GetSkillApplyType() == SkillApplyType.Heal)
				{
					var recoverAmount = (int)(selectedUnit.GetActualPower() * appliedSkill.GetPowerFactor());
					var recoverHealthCoroutine = target.GetComponent<Unit>().RecoverHealth(recoverAmount);

					if (target == targets[targets.Count-1])
					{
						yield return battleManager.StartCoroutine(recoverHealthCoroutine);
					}
					else
					{
						battleManager.StartCoroutine(recoverHealthCoroutine);
					}

					Debug.Log("Apply " + recoverAmount + " heal to " + target.GetComponent<Unit>().GetName());
				}

				// FIXME : 버프, 디버프는 아직 미구현. 데미지/힐과 별개일 때도 있고 같이 들어갈 때도 있으므로 별도의 if문으로 구현할 것.
				else
				{
					Debug.Log("Apply additional effect to " + target.GetComponent<Unit>().name);
				}
			}

			battleData.tileManager.ChangeTilesFromSeletedColorToDefaultColor(selectedTiles);

			int requireAP = appliedSkill.GetRequireAP();
			selectedUnit.UseActionPoint(requireAP);
			battleData.indexOfSeletedSkillByUser = 0; // return to init value.

			yield return new WaitForSeconds(0.5f);

			Camera.main.transform.position = new Vector3(battleData.selectedUnitObject.transform.position.x, battleData.selectedUnitObject.transform.position.y, -10);
			battleData.currentState = CurrentState.FocusToUnit;
			battleData.alreadyMoved = false;
			// yield return battleManager.StartCoroutine(BattleManager.FocusToUnit(battleData));
		}

		private static IEnumerator ApplySkillEffect(Skill appliedSkill, GameObject unitObject, List<GameObject> selectedTiles)
		{
			string effectName = appliedSkill.GetEffectName();
			EffectVisualType effectVisualType = appliedSkill.GetEffectVisualType();
			EffectMoveType effectMoveType = appliedSkill.GetEffectMoveType();

			if ((effectVisualType == EffectVisualType.Area) && (effectMoveType == EffectMoveType.Move))
			{
				// 투사체, 범위형 이펙트.
				Vector3 startPos = unitObject.transform.position;
				Vector3 endPos = new Vector3(0, 0, 0);
				foreach (var tile in selectedTiles)
				{
					endPos += tile.transform.position;
				}
				endPos = endPos / (float)selectedTiles.Count;

				GameObject particle = GameObject.Instantiate(Resources.Load("Particle/" + effectName)) as GameObject;
				particle.transform.position = startPos - new Vector3(0, 0, 0.01f);
				yield return new WaitForSeconds(0.2f);
				iTween.MoveTo(particle, endPos - new Vector3(0, 0, 0.01f) - new Vector3(0, 0, 5f), 0.5f); // 타일 축 -> 유닛 축으로 옮기기 위해 z축으로 5만큼 앞으로 빼준다.
				yield return new WaitForSeconds(0.3f);
				GameObject.Destroy(particle, 0.5f);
				yield return null;
			}
			else if ((effectVisualType == EffectVisualType.Area) && (effectMoveType == EffectMoveType.NonMove))
			{
				// 고정형, 범위형 이펙트.
				Vector3 targetPos = new Vector3(0, 0, 0);
				foreach (var tile in selectedTiles)
				{
					targetPos += tile.transform.position;
				}
				targetPos = targetPos / (float)selectedTiles.Count;
				targetPos = targetPos - new Vector3(0, 0, 5f); // 타일 축 -> 유닛 축으로 옮기기 위해 z축으로 5만큼 앞으로 빼준다.

				GameObject particle = GameObject.Instantiate(Resources.Load("Particle/" + effectName)) as GameObject;
				particle.transform.position = targetPos - new Vector3(0, 0, 0.01f);
				yield return new WaitForSeconds(0.5f);
				GameObject.Destroy(particle, 0.5f);
				yield return null;
			}
			else if ((effectVisualType == EffectVisualType.Individual) && (effectMoveType == EffectMoveType.NonMove))
			{
				// 고정형, 개별 대상 이펙트.
				List<Vector3> targetPosList = new List<Vector3>();
				foreach (var tileObject in selectedTiles)
				{
					Tile tile = tileObject.GetComponent<Tile>();
					if (tile.IsUnitOnTile())
					{
						targetPosList.Add(tile.GetUnitOnTile().transform.position);
					}
				}

				foreach (var targetPos in targetPosList)
				{
					GameObject particle = GameObject.Instantiate(Resources.Load("Particle/" + effectName)) as GameObject;
					particle.transform.position = targetPos - new Vector3(0, 0, 0.01f);
					GameObject.Destroy(particle, 0.5f + 0.3f); // 아랫줄에서의 지연시간을 고려한 값이어야 함.
				}
				if (targetPosList.Count == 0) // 대상이 없을 경우. 일단 가운데 이펙트를 띄운다.
				{
					Vector3 midPos = new Vector3(0, 0, 0);
					foreach (var tile in selectedTiles)
					{
						midPos += tile.transform.position;
					}
					midPos = midPos / (float)selectedTiles.Count;

					GameObject particle = GameObject.Instantiate(Resources.Load("Particle/" + effectName)) as GameObject;
					particle.transform.position = midPos - new Vector3(0, 0, 0.01f);
					GameObject.Destroy(particle, 0.5f + 0.3f); // 아랫줄에서의 지연시간을 고려한 값이어야 함.
				}

				yield return new WaitForSeconds(0.5f);
			}
		}
	}
}
