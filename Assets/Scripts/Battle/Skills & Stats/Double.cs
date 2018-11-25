using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Double : Skill {
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
        base.Use();
        // two hits
        for (int i = 0; i < 2; i++)
            // check hit on target
            if (thisUnit.lastAtkHit = BattleManager.targetUnit.CheckHit(thisUnit.Atk))
                // deal damage as needed - 1 75%% hit
                BattleManager.targetUnit.DealDamage((int)(thisUnit.Dmg * 0.75f));
    }
}
