﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blind : Skill {
    private int damage;
    public Blind(Unit unit) :
        base(unit, "Blind", 10, TARGETS.ENEMIES) {
    }

    public override bool Available { get; protected set; }

    public override void Use() {
        base.Use();
        // check hit on target
        if (thisUnit.lastAtkHit = BattleManager.targetUnit.CheckHit(thisUnit.Atk)) {
            // deal damage as needed - 1 90% hit
            damage += BattleManager.targetUnit.DealDamage((int)(thisUnit.Dmg * 0.9f));
            // apply the Stunned status effect
            BattleManager.targetUnit.AddEffect(new Blinded(thisUnit, BattleManager.targetUnit));
        }
        BattleManager.BattleLog += thisUnit.name + "->" + name + "->" + BattleManager.targetUnit.name + " (" + damage + ")";
    }
}
