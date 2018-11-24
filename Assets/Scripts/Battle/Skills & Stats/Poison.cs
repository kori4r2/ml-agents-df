using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : Skill {
    public Poison(Unit unit) :
        base(unit, "Poison", 8, TARGETS.ENEMIES) {
    }

    public override bool Available { get; protected set; }

    public override void Use() {
        base.Use();
        // check hit on target
        if (thisUnit.lastAtkHit = BattleManager.targetUnit.CheckHit(thisUnit.Atk)) {
            // deal damage as needed - 1 30% hit
            BattleManager.targetUnit.DealDamage((int)(thisUnit.Dmg * 0.3f));
            // apply the Stunned status effect
            BattleManager.targetUnit.AddEffect(new Poisoned(thisUnit, BattleManager.targetUnit));
        }
    }
}
