using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stunned : StatusEffect {
    public Stunned(Unit src, Unit trgt, int duration=2):
        base(src, trgt, "Stunned", duration) {
    }

    public override bool Countdown() {
        target.acted = true;
        TurnsLeft--;
        return (TurnsLeft > 0);
    }

    public override void Remove() {
    }

    protected override void Apply() {
        TurnsLeft = duration;
    }
}
