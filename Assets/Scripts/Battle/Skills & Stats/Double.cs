using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Double : Skill {
    private int damage;
    public override bool Available {
        get {
            return available && thisUnit.lastAtkHit;
        }
        protected set {
            available = value;
        }
    }
    public Double(Unit unit) :
        base(unit, "Double", 8, TARGETS.ENEMIES) {
        Available = false;
    }
    public override void Use() {
        damage = 0;
        base.Use();
        // two hits
        for (int i = 0; i < 2; i++){
            // check hit on target
            if (thisUnit.lastAtkHit = thisUnit.battleManager.targetUnit.CheckHit(thisUnit.Atk)){
                // deal damage as needed - 1 75%% hit
                damage += thisUnit.battleManager.targetUnit.DealDamage((int)(thisUnit.Dmg * 0.75f));
            }
        }
        thisUnit.battleManager.BattleLog += thisUnit.name + "->" + name + "->" + thisUnit.battleManager.targetUnit.name + " (" + damage + ")";
    }
}
