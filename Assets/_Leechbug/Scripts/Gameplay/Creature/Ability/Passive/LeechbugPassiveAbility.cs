using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeechbugPassiveAbility : APassiveAbility
{
    [SerializeField] private ModifyDamageReceivedEffect _statusEffect;
    [SerializeField] private float _duration;

    public override void Initialize(Fish parentFish)
    {
        base.Initialize(parentFish);

        parentFish.OnPossessFish += MakeItInvincible;
    }

    public void MakeItInvincible(Fish leechbug)
    {
        var effectInst = Instantiate(_statusEffect);
        effectInst.Initialize(leechbug, _duration);
    }
}
