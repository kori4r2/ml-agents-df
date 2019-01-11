using MLAgents;
using System.Collections.Generic;
using UnityEngine;

public class DFAcademy : Academy {
    public List<UnitAgent> agents;

    public bool HasTrainingBrain() {
        foreach (UnitAgent agent in agents) {
            if (agent.brain.brainType == BrainType.External)
                return true;
        }
        return false;
    }
    public override void InitializeAcademy() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            agents.Add(player.GetComponent<UnitAgent>());
        }
    }
    public override void AcademyReset() {
        /*
        foreach(UnitAgent agent in agents) {
            agent.AgentReset();
            agent.agentParameters.onDemandDecision = true;
        }
        */
        BattleManager.BattleLog = "";
    }
}
