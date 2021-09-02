using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoActiveAbility", menuName = "FishAI/Actions/DoActiveAbility")]
public class AIA_DoActiveAbility : AIAction {
	public override void Act(BaseAIController controller) {
		if (controller == null || controller.ParentFish.Defeated
		|| !(controller.ParentFish._activeAbility)
		|| !(controller.ParentFish._activeAbility.IsAbilityReady))
			return;

		// do a check depending on what type of fish
		bool goAhead = false;
		switch(controller.ParentFish._activeAbility.GetAbilityType()) {
			case AbilityType.SINGLE_TARGET_OFFENSE:
				goAhead = SingleTargetOffenseCheck(controller);
				break;
			case AbilityType.SINGLE_TARGET_SUPPORT:
				goAhead = SingleTargetSupportCheck(controller);
				break;
			case AbilityType.AREA_OFFENSE:
				goAhead = AreaOffenseCheck(controller);
				break;
			case AbilityType.AREA_SUPPORT:
				goAhead = AreaSupportCheck(controller);
				break;
			case AbilityType.MOBILITY:
				goAhead = MobilityCheck(controller);
				break;
			case AbilityType.SELF_EFFECT:
				goAhead = SelfEffectCheck(controller);
				break;
			default:
				Debug.Log("Error: AI: AIA_DoActiveAbility: Ability type of ability" + 
					controller.ParentFish._activeAbility.name + " in fish " +
					controller.ParentFish.name + " is not defined!");
				break;
		}

		if (goAhead)
			controller.ParentFish._activeAbility.Execute();
	}

	// single-target offense ability check
	bool SingleTargetOffenseCheck(BaseAIController controller) {
		// test if our current target can be affected
		var target = controller.FishAIStateInfo.EnemyTarget;
		var ability = controller.ParentFish._activeAbility;

		if (target && !target.Defeated
		&& ((target.transform.position - controller.ParentFish.transform.position).sqrMagnitude < ability.Range* ability.Range)) {
			return true;
		}

		// if not, see if other target can be
		target = null;
		if (controller.ParentFish.Faction == FishFaction.A)
			BattleUtils.GetClosestInRange(controller.ParentFish, ability.Range, out target, BattleUtils.IsB);
		else if (controller.ParentFish.Faction == FishFaction.B)
			BattleUtils.GetClosestInRange(controller.ParentFish, ability.Range, out target, BattleUtils.IsA);

		return (target);
	}
	
	// single-target support ability check
	bool SingleTargetSupportCheck(BaseAIController controller) {
		// test if our current target can be affected
		var target = controller.FishAIStateInfo.SupportTarget;
		var ability = controller.ParentFish._activeAbility;

		if (target && !target.Defeated
		&& ((target.transform.position - controller.ParentFish.transform.position).sqrMagnitude < ability.Range * ability.Range)) {
			return true;
		}

		// if not, see if other target can be
		target = null;
		if (controller.ParentFish.Faction == FishFaction.A)
			BattleUtils.GetClosestInRange(controller.ParentFish, ability.Range, out target, BattleUtils.IsA);
		else if (controller.ParentFish.Faction == FishFaction.B)
			BattleUtils.GetClosestInRange(controller.ParentFish, ability.Range, out target, BattleUtils.IsB);

		return (target);
	}

	// area offense ability check
	bool AreaOffenseCheck(BaseAIController controller) {
		// get all possible targets for the aoe ability
		List<Fish> possibleTargets = new List<Fish>();
		if (controller.ParentFish.Faction == FishFaction.A) {
			possibleTargets.AddRange(CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.B).allFish);
		}
		else {
			possibleTargets.AddRange(CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.A).allFish);
		}

		var ability = controller.ParentFish._activeAbility;
		int targetCount = 0;
		for (int i = possibleTargets.Count - 1; i > -1; i--) {
			Fish fish = possibleTargets[i];
			if (!fish || fish.Defeated) {
				possibleTargets.RemoveAt(i);
				continue;
			}
			// count every fish in ranges
			if ((fish.transform.position - controller.ParentFish.transform.position).sqrMagnitude < ability.Range * ability.Range) {
				possibleTargets.RemoveAt(i);
				targetCount++;
			}
			// if there are 3 or more targets in range, do ability
			if (targetCount == 3)
				return true;
		}

		// if there are 1-2 targets in range:
		// do ability if there is no target within (100 + (cooldown length * 2.5))% of the range
		if (targetCount > 0) {
			float newRange = ability.Range * ((100f + (ability.MaxCooldown * 2.5f)) / 100f);
			foreach (var fish in possibleTargets) {
				if ((fish.transform.position - controller.ParentFish.transform.position).sqrMagnitude < newRange * newRange) {
					return false;
				}
			}
		}
		// if no targets found at all; dont do
		else
			return false;

		// no other target in the new range, so do ability
		return true;
	}

	// area support ability check
	bool AreaSupportCheck(BaseAIController controller) {
		// get all possible targets for the aoe ability
		List<Fish> possibleTargets = new List<Fish>();
		if (controller.ParentFish.Faction == FishFaction.B) {
			possibleTargets.AddRange(CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.B).allFish);
		}
		else {
			possibleTargets.AddRange(CombatManager.Instance.GetTeamSwarm(CombatManager.TeamType.A).allFish);
		}

		var ability = controller.ParentFish._activeAbility;
		int targetCount = 0;
		for (int i = possibleTargets.Count - 1; i > -1; i--) {
			Fish fish = possibleTargets[i];
			if (!fish || fish.Defeated) {
				possibleTargets.RemoveAt(i);
				continue;
			}
			// count every fish in ranges
			if ((fish.transform.position - controller.ParentFish.transform.position).sqrMagnitude < ability.Range * ability.Range) {
				possibleTargets.RemoveAt(i);
				targetCount++;
			}
			// if there are 3 or more targets in range, do ability
			if (targetCount == 3)
				return true;
		}

		// if there are 1-2 targets in range:
		// do ability if there is no target within (100 + (cooldown length * 2.5))% of the range
		if (targetCount > 0) {
			float newRange = ability.Range * ((100f + (ability.MaxCooldown * 2.5f)) / 100f);
			foreach (var fish in possibleTargets) {
				if ((fish.transform.position - controller.ParentFish.transform.position).sqrMagnitude < newRange * newRange) {
					return false;
				}
			}
		}
		// if no targets found at all; dont do
		else
			return false;

		// no other target in the new range, so do ability
		return true;
	}

	// mobility ability check
	bool MobilityCheck(BaseAIController controller) {
		// dont use ability if you dont even have a valid target
		var target = controller.FishAIStateInfo.EnemyTarget;
		if (!target || target.Defeated)
			return false;

		// if your target is out of your regular attack's range, use ability
		var attack = controller.ParentFish._basicAttackAbility;
		return ((target.transform.position - controller.ParentFish.transform.position).sqrMagnitude >= attack.Range * attack.Range);
	}

	// self effect ability check
	bool SelfEffectCheck(BaseAIController controller) {
		// TODO: find a way to generalize this past just healing
		
		var ability = controller.ParentFish._activeAbility;
		// if there is no healing, it is probably a buff (which doesnt hurt to just cast)
		if (ability.Healing <= 0)
			return true;

		// if damaged, heal
		return (!controller.ParentFish.Health.IsFullHealth);
	}
}