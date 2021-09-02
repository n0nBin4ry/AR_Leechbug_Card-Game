using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoloSteer", menuName = "FishAI/Actions/SoloSteer")]
public class AIA_SoloSteer : AIAction {
	public float RayLength = 10f;

	public override void Act(BaseAIController controller) {
		if (controller == null || controller.ParentFish == null)
			return;

		// check if we will run into a wall, which is the default physics layer 0
		RaycastHit hitInfo;
		if (Physics.Raycast(controller.ParentFish.transform.position, controller.ParentFish.TargetForward, out hitInfo, RayLength, 0)) {
			// new direction is set to avoid collision
			//controller.ParentFish.TargetForward = Vector3.Normalize(/*controller.ParentFish.TargetForward +*/ Vector3.Reflect(controller.ParentFish.TargetForward, hitInfo.normal));

			controller.ParentFish.TargetForward = Vector3.Normalize(hitInfo.point + (hitInfo.normal * RayLength) - controller.ParentFish.transform.position);
		}
		controller.ParentFish.TargetVelocity = controller.ParentFish.Data.MaxSpeed * controller.ParentFish.TargetForward;
	}
}