using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinded : StatusEffect {
    private int negativeBth;
    public Blinded(Unit src, Unit trgt, int duration=4, int bth=45) :
        base(src, trgt, "Blinded", duration) {
        negativeBth = bth;
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
        target.effects.Remove(this);
    }

    protected override void Apply() {
        TurnsLeft = duration;
        target.Def -= negativeBth;
    }
}
