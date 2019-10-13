using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : Skill {
    private int damage;
    public Poison(Unit unit) :
        base(unit, "Poison", 7, TARGETS.ENEMIES) {
    }

    public override bool Available { get; protected set; }

    public override void Use() {
        base.Use();
        // check hit on target
        if (thisUnit.lastAtkHit = thisUnit.battleManager.targetUnit.CheckHit(thisUnit.Atk)) {
            // deal damage as needed - 1 30% hit
            damage += thisUnit.battleManager.targetUnit.DealDamage((int)(thisUnit.Dmg * 0.3f));
            // apply the Poisoned status effect
            thisUnit.battleManager.targetUnit.AddEffect(new Poisoned(thisUnit, thisUnit.battleManager.targetUnit));
        }
        thisUnit.battleManager.BattleLog += thisUnit.name + "->" + name + "->" + thisUnit.battleManager.targetUnit.name + " (" + damage + ")";
    }
}
