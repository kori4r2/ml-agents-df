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
        return (TurnsLeft > 0);
    }

    public override void Remove() {
        target.Bth += negativeBth;
    }

    protected override void Apply() {
        TurnsLeft = duration;
        target.Bth -= negativeBth;
    }
}
