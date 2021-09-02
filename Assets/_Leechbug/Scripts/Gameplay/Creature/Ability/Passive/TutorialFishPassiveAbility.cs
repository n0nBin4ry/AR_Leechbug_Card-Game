using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFishPassiveAbility : APassiveAbility
{
    [SerializeField] private int dmgModifier = 3;

    public override void Initialize(Fish parentFish)
    {
        base.Initialize(parentFish);
        ParentFish.InterceptDamageDealt += Modify;
    }

    private void Modify(HealthModification healthMod)
    {
        if (ParentFish.Controller.ControllerType == ControllerType.Player)
        {
            healthMod.Multiplier += dmgModifier;
        }
    }

    public override void Terminate()
    {
    ParentFish.InterceptDamageDealt -= Modify;
    }
}
