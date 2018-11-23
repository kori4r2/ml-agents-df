using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poisoned : StatusEffect {
    private float dotMultiplier;
    public Poisoned(Unit src, Unit trgt, int dur=5, float dmgMult=.2f) :
        base(src, trgt, "Poisoned", dur) {
        dotMultiplier = dmgMult;
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
        target.effects.Remove(this);
    }

    protected override void Apply() {
        TurnsLeft = duration;
    }
}
