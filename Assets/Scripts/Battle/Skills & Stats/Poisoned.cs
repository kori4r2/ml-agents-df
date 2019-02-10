using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poisoned : StatusEffect {
    private float dotMultiplier = .3f;
    public Poisoned(Unit src, Unit trgt, int dur=5) :
        base(src, trgt, "Poisoned", dur) {
    }

    public override bool Countdown() {
        int damage = target.DealDamage((int)(source.Dmg * dotMultiplier), "Poison");
        if (!BattleManager.academy.HasTrainingBrain())
            BattleManager.BattleLog += target.name + "->" + text +" ("+ damage +")";
        TurnsLeft--;
        return (TurnsLeft > 0);
    }

    public override void Remove() {
    }

    protected override void Apply() {
        TurnsLeft = duration;
    }
}