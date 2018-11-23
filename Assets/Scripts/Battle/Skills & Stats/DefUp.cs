using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefUp : StatusEffect {
    private int boost;
    public DefUp(Unit src, Unit trgt, int duration=3, int bst=70):
        base(src, trgt, "DefUp", duration){
        boost = bst;
    }

    public override bool Countdown() {
        TurnsLeft--;
        if (TurnsLeft <= 0) {
            Remove();
            return false;
        }
        return true;
    }

    public override void Remove() {
        target.Def -= boost;
        target.effects.Remove(this);
    }

    protected override void Apply() {
        TurnsLeft = duration;
        target.Def += boost;
    }
}
