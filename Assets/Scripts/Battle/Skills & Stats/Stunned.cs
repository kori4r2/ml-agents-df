﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stunned : StatusEffect {
    public Stunned(Unit src, Unit trgt, int duration=3):
        base(src, trgt, "Stunned", duration) {
    }

    public override bool Countdown() {
        target.acted = true;
        TurnsLeft--;
        if (TurnsLeft <= 0) {
          //  Remove();
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
