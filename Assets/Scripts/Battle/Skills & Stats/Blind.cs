using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blind : Skill {
    private int damage;
    public Blind(Unit unit) :
        base(unit, "Blind", 8, TARGETS.ENEMIES) {
    }

    public override bool Available { get; protected set; }

    public override void Use() {
        base.Use();
        // check hit on target
        if (thisUnit.lastAtkHit = thisUnit.battleManager.targetUnit.CheckHit(thisUnit.Atk)) {
            // deal damage as needed - 1 90% hit
            damage += thisUnit.battleManager.targetUnit.DealDamage((int)(thisUnit.Dmg * 0.9f));
            // apply the Blinded status effect
            thisUnit.battleManager.targetUnit.AddEffect(new Blinded(thisUnit, thisUnit.battleManager.targetUnit));
        }
        thisUnit.battleManager.BattleLog += thisUnit.name + "->" + name + "->" + thisUnit.battleManager.targetUnit.name + " (" + damage + ")";
    }
}
