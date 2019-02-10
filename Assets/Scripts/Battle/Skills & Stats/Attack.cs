using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Skill {
    private int damage;
    public Attack(Unit unit) :
        base(unit, "Attack", 0, TARGETS.ENEMIES){
        Available = true;
    }

    public override bool Available {
        get {
            return true;
        }
        protected set {
            available = true;
        }
    }

    public override void Use() {
        damage = 0;
        base.Use();
        // check hit on target
        if (thisUnit.lastAtkHit = BattleManager.targetUnit.CheckHit(thisUnit.Atk))
            // deal damage as needed - 1 100% hit
            damage += BattleManager.targetUnit.DealDamage(thisUnit.Dmg);
        if(!BattleManager.academy.HasTrainingBrain())
            BattleManager.BattleLog += thisUnit.name +"->"+ name +"->"+ BattleManager.targetUnit.name +" ("+ damage +")";
    }
}
