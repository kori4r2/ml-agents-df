using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poisoned : StatusEffect {
    private float dotMultiplier = .2f;
    public Poisoned(Unit src, Unit trgt, int dur=5) :
        base(src, trgt, "Poisoned", dur) {
    }

    public override bool Countdown() {
        target.DealDamage((int)(source.Dmg * dotMultiplier), "Poison");
        TurnsLeft--;
        if (TurnsLeft <= 0) {
            Remove();
            return false;
        }
        return true;
    }

    public override void Remove() {
    }

    protected override void Apply() {
        TurnsLeft = duration;
    }
}
