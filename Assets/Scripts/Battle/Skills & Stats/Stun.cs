using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stun : Skill {
    public Stun(Unit unit) :
        base(unit, "Stun", 10, TARGETS.ENEMIES) {
    }

    public override bool Available { get; protected set; }

    public override void Use() {
        // check hit on target
        if (BattleManager.targetUnit.CheckHit(thisUnit.Atk)) {
            // deal damage as needed - 1 90% hit
            BattleManager.targetUnit.DealDamage((int)(thisUnit.Dmg * 0.9f));
            // apply the Stunned status effect
            BattleManager.targetUnit.effects.Add(new Stunned(thisUnit, BattleManager.targetUnit));
        }
    }
}
