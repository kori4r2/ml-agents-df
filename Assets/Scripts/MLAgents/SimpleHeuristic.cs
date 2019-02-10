using UnityEngine;
using MLAgents;
using System.Collections.Generic;

public enum BEHAVIOUR{
    AutoAttack,
    SimpleLogic
};

public class SimpleHeuristic : MonoBehaviour, Decision {
    public BEHAVIOUR logic;
    public float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory) {
        float[] decision = new float[1] { -1 }; // The decision variable has to be float[], but the value will be an integer
        // Define standard action to avoid invalid return value
        decision[0] = (vectorObs[(int)OBSERVATION.Enemy1HP] > 0.0f) ? (int)DECISION.AttackEnemy1 : (int)DECISION.AttackEnemy2;
        if (logic == BEHAVIOUR.AutoAttack) {
            if (vectorObs[(int)OBSERVATION.Enemy1HP] > 0.0f && vectorObs[(int)OBSERVATION.Enemy2HP] > 0.0f)
                decision[0] = (Random.Range(0, 2) == 0) ? (int)DECISION.AttackEnemy1 : (int)DECISION.AttackEnemy2;
            else
                decision[0] = (vectorObs[(int)OBSERVATION.Enemy1HP] > 0.0f) ? (int)DECISION.AttackEnemy1 : (int)DECISION.AttackEnemy2;
        } else {
            /*
            Debug.Log("=================================");
            for (int i = 0; i < vectorObs.Count; i++) {
                Debug.Log(vectorObs[i]);
            }
            Debug.Log("=================================");
            */
            bool enemy1Alive = vectorObs[(int)OBSERVATION.Enemy1HP] > 0.0f;
            bool enemy2Alive = vectorObs[(int)OBSERVATION.Enemy2HP] > 0.0f;
            bool allyAlive = vectorObs[(int)OBSERVATION.AllyHP] > 0.0f;
            bool allyVulnerable = allyAlive && vectorObs[(int)OBSERVATION.AllyHP] < 0.65f && vectorObs[(int)OBSERVATION.AllyDefUp] <= 0.0f;
            bool allyCriticalHP = allyAlive && vectorObs[(int)OBSERVATION.AllyHP] <= 0.133f;
            bool selfVulnerable = vectorObs[(int)OBSERVATION.SelfHP] < 0.65f && vectorObs[(int)OBSERVATION.SelfDefUp] <= 0.0f;
            bool selfCriticalHP = vectorObs[(int)OBSERVATION.SelfHP] <= 0.133f;
            bool enemy1CanHit = enemy1Alive && vectorObs[(int)OBSERVATION.Enemy1Blinded] < 0.26f && vectorObs[(int)OBSERVATION.Enemy1Stunned] < 0.34f;
            bool enemy2CanHit = enemy2Alive && vectorObs[(int)OBSERVATION.Enemy2Blinded] < 0.26f && vectorObs[(int)OBSERVATION.Enemy2Stunned] < 0.34f;
            bool enemiesCanHit = (enemy1CanHit || enemy2CanHit) && vectorObs[(int)OBSERVATION.SelfDefUp] <= 0.0f;
            bool canHitEnemy1 = enemy1Alive && vectorObs[(int)OBSERVATION.Enemy1DefUp] <= 0.0f;
            bool canHitEnemy2 = enemy2Alive && vectorObs[(int)OBSERVATION.Enemy2DefUp] <= 0.0f;
            // Emergency heal
            if (vectorObs[(int)OBSERVATION.HealCooldown] <= 0.0f && selfCriticalHP)
                decision[0] = (int)DECISION.HealSelf;
            else if (vectorObs[(int)OBSERVATION.HealCooldown] <= 0.0f && allyCriticalHP)
                decision[0] = (int)DECISION.HealAlly;
            else {
                // Stun is OP, if you can stun someone, DO IT
                if (vectorObs[(int)OBSERVATION.StunCooldown] <= 0.0f && vectorObs[(int)OBSERVATION.SelfBlinded] <= 0.0f &&
                   ((canHitEnemy1 && vectorObs[(int)OBSERVATION.Enemy1Stunned] <= 0.34f) ||
                    (canHitEnemy2 && vectorObs[(int)OBSERVATION.Enemy2Stunned] <= 0.34f))) {
                    // If enemy1 is a valid target, stun them
                    if (canHitEnemy1 && vectorObs[(int)OBSERVATION.Enemy1Stunned] <= 0.34f)
                        decision[0] = (int)DECISION.StunEnemy1;
                    // If not, stun the other (one of them must be a valid target to get here)
                    else
                        decision[0] = (int)DECISION.StunEnemy2;
                // If you or your friend needs a heal and have no shields up, they should be healed
                } else if (vectorObs[(int)OBSERVATION.HealCooldown] <= 0.0f && (selfVulnerable || allyVulnerable)) {
                    // Heal the one with lower hp
                    if (vectorObs[(int)OBSERVATION.SelfHP] <= vectorObs[(int)OBSERVATION.AllyHP])
                        decision[0] = (int)DECISION.HealSelf;
                    else
                        decision[0] = (int)DECISION.HealAlly;
                // If there is risk of being attacked and you can reduce it, it should be done
                } else if (enemiesCanHit && vectorObs[(int)OBSERVATION.SelfHP] <= 0.8f &&
                           (vectorObs[(int)OBSERVATION.BlockCooldown] <= 0.0f || (vectorObs[(int)OBSERVATION.BlindCooldown] <= 0.0f && vectorObs[(int)OBSERVATION.SelfBlinded] <= 0.0f && (canHitEnemy1 || canHitEnemy2)))) {
                    // Blind or block
                    if (vectorObs[(int)OBSERVATION.BlockCooldown] <= 0.0f)
                        decision[0] = (int)DECISION.BlockSelf;
                    else if (canHitEnemy1)
                        decision[0] = (int)DECISION.BlindEnemy1;
                    else
                        decision[0] = (int)DECISION.BlindEnemy2;
                // If any of the damage skills can be reliably used, its better than attack
                } else if ((canHitEnemy1 || canHitEnemy2) && vectorObs[(int)OBSERVATION.SelfBlinded] <= 0.0f &&
                           (vectorObs[(int)OBSERVATION.PoisonCooldown] <= 0.0f || (vectorObs[(int)OBSERVATION.DoubleCooldown] <= 0.0f && vectorObs[(int)OBSERVATION.LastAtkHit] == 1.0f)) ) {
                    // Offensive skills
                    if (vectorObs[(int)OBSERVATION.PoisonCooldown] <= 0.0f)
                        decision[0] = (canHitEnemy1) ? (int)DECISION.PoisonEnemy1 : (int)DECISION.PoisonEnemy2;
                    else
                        decision[0] = (canHitEnemy1) ? (int)DECISION.DoubleEnemy1 : (int)DECISION.DoubleEnemy2;
                } else {
                    // Attack
                    if (enemy1Alive && (!enemy2Alive || (canHitEnemy1 && !canHitEnemy2)))
                        decision[0] = (int)DECISION.AttackEnemy1;
                    else if (enemy2Alive && (!enemy1Alive || (canHitEnemy2 && !canHitEnemy1)))
                        decision[0] = (int)DECISION.AttackEnemy2;
                    else
                        decision[0] = (vectorObs[(int)OBSERVATION.Enemy1HP] <= vectorObs[(int)OBSERVATION.Enemy2HP]) ? (int)DECISION.AttackEnemy1 : (int)DECISION.AttackEnemy2;
                }
            }
        }
        return decision;
    }

    public List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory) {
        return new List<float>();
    }
}
