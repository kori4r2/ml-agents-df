using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefUp : StatusEffect {
    private int boost = 70;
    public DefUp(Unit src, Unit trgt, int duration=3):
        base(src, trgt, "DefUp", duration){
    }

    public override bool Countdown() {
        TurnsLeft--;
        return (TurnsLeft > 0);
    }

    public override void Remove() {
        target.Def -= boost;
    }

    protected override void Apply() {
        TurnsLeft = duration;
        target.Def += boost;
    }
}
