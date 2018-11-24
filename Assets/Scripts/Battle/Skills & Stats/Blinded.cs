using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinded : StatusEffect {
    private int negativeBth = 45;
    public Blinded(Unit src, Unit trgt, int duration=4) :
        base(src, trgt, "Blinded", duration) {
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
        target.Def += negativeBth;
    }

    protected override void Apply() {
        TurnsLeft = duration;
        target.Def -= negativeBth;
    }
}
