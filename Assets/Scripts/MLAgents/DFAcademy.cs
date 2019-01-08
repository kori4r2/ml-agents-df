using MLAgents;

public class DFAcademy : Academy {
    public UnitAgent[] agents = new UnitAgent[4];
    public override void AcademyReset() {
        foreach(UnitAgent agent in agents) {
            agent.AgentReset();
        }
        BattleManager.BattleLog = "";
        base.AcademyReset();
    }
}
