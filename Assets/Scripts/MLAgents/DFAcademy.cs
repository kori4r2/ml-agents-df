using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DFAcademy : Academy {
    public List<UnitAgent> agents;
    public BattleManager battleManager;
    private bool changedLogic;
    private const float maxRewardValue = 50.0f;
    private int debug = 0;
    
    public bool HasTrainingBrain() {
        //return true;
        foreach (UnitAgent agent in agents) {
            if (agent.brain.brainType == BrainType.External)
                return true;
        }
        return false;
    }

    public override void InitializeAcademy() {
        changedLogic = false;
        debug = 0;
        // Debug.Log("Final list: " + agents[0].name + ", " + agents[1].name + ", " + agents[2].name + ", " + agents[3].name);
    }

    public void CalculateRewards() {
        float[] rewards = new float[agents.Count];
        float[] HPBonus = new float[agents.Count];
        // Calculate how much each unit has as reward value based on remaining health
        for (int i = 0, j = 0; i < agents.Count; i++) {
            Unit unit = agents[i].GetComponent<Unit>();
            if (battleManager.winningTeam == unit.team) {
                agents[i].AddReward(maxRewardValue); // victory reward

                // HPBonus[index unit1, bonus unit1, index unit2, bonus unit2(...)]
                HPBonus[j++] = i;
                HPBonus[j++] = (unit.MaxHP - unit.CurHP) / (unit.MaxHP * 1.0f); // % HP lost, from 0.0 to 1.0
                // Debug.Log("HPBonus of unit " + agents[(int)HPBonus[j-2]].name + " is " + HPBonus[j-1]+ "; j-1 = " + (j-1));
            }

            rewards[i] = 0.0f;
        }
        for(int i = 0; i < agents.Count; i++) {
            int index = (i == HPBonus[0]) ? 0 : (i == HPBonus[2]) ? 2 : -1;
            // In case of agent on losing team
            if (index == -1) {
                // Receives a reward of up to 1.0 based on how much health the winners lost
                for(int j = 1; j < agents.Count; j += 2){
                    rewards[i] += (HPBonus[j]) / (agents.Count / 2.0f);
                } 
            // In case of agent on the winning team
            } else {
                // Receives a punishment based on how much health agent (and ally, if cooperative) lost
                if(agents[i].type == UnitAgent.TYPEAGENT.Individualistic){
                    rewards[i] -= (HPBonus[index + 1]);
                }else{
                    for(int j = 1; j < agents.Count; j += 2){
                        rewards[i] -= (HPBonus[j]) / (agents.Count / 2.0f);
                    } 
                }
            }
            Debug.Log("Adding reward of " + (rewards[i] * maxRewardValue) + " to " + agents[i].name);
            agents[i].AddReward(rewards[i] * maxRewardValue / 2.0f);
        }
        // Print cumulative rewards
        foreach (UnitAgent agent in agents) {
            Debug.Log("agent " + agent.name + " has reward of " + agent.GetCumulativeReward());
        }
    }

    public override void AcademyReset() {
        //debug++;
        CalculateRewards();
        foreach(DecisionLog log in battleManager.decisionLogs){
            log.RecalculateStatistics();
        }
        // Clear battle log
        battleManager.BattleLog = "";
        // During curriculum training, changes the heuristic brain's logic if necessary
        if ((!changedLogic && resetParameters["logic"] == 1.0f) || debug >= 5) {
            gameObject.GetComponentInChildren<SimpleHeuristic>().logic = BEHAVIOUR.SimpleLogic;
            changedLogic = true;
        } else{
            resetParameters["logic"] = 0.0f;
        }
        // Resets all agents and the scene as well
        foreach (UnitAgent agent in agents) {
            Debug.Log("Told agent" + agent.name + " to be Done");
            agent.Done();
        }
        battleManager.StartBattle();
        // ResetAgentsInOrder();
    }

//     private bool currentAgentReset = false;
//     private IEnumerator WaitForCurrentAgent(UnitAgent agent){
//         while(!agent.IsFullHealth){
//             yield return null;
//         }

//         currentAgentReset = true;
//     }

//     private void ResetAgentsInOrder(){
//         foreach(UnitAgent agent in agents){
//             Debug.Log("Told agent" + agent.name + " to reset");
//             agent.Done();
//             currentAgentReset = false;
//             StartCoroutine(WaitForCurrentAgent(agent));

//             while(!currentAgentReset){
//                 Debug.Log("Waiting for reset of agent " + agent.name + "...");
//             }
//         }
//         Debug.Log("All units are reset");

//         battleManager.StartBattle();
//     }
}
