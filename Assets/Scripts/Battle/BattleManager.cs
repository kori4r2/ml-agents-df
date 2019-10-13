using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void MyDelegate();

public class BattleManager : MonoBehaviour {
    public List<Unit> units = new List<Unit>();
    public Unit currentUnit;
    public Unit targetUnit;
    public Skill selectedAction;
    public int expectedNUnits = 4;
    public int turnCounter = -1;
    public char winningTeam;
    public DFAcademy academy;
    public bool rematchOverride = false;
    private bool waiting;
    private bool battleStarted = false;
    public List<DecisionLog> decisionLogs;
    private Text logObject;
    private string logString;
    public string BattleLog {
        get { return logObject.text; }
        set {
            string[] split = value.Split(new char[]{'\n'}, System.StringSplitOptions.RemoveEmptyEntries);
            if (split.Length <= 0){
                if(!academy.HasTrainingBrain())
                    logObject.text = "";
                logString = "";
            }
            else {
                if(!academy.HasTrainingBrain())
                    logObject.text = turnCounter +": "+ split[split.Length-1]+"\n";
                logString = turnCounter +": "+ split[split.Length-1]+"\n";
                for (int i = 0; i < split.Length - 1; i++) {
                    if(!academy.HasTrainingBrain())
                        logObject.text += split[i] + "\n";
                    logString += split[i] + "\n";
                }
            }
        }
    }
    private TurnQueue queue;

    public void Start(){
        queue = new TurnQueue(this);
        StartBattle();
    }
    
    public void Kill(Unit unit) {
        // Removes from lists and changes interface to reflect death
        unit.FadeOut();
        unit.MakeUnclickable();
        //units.Remove(unit);
        queue.Remove(unit);
        //NUnits--;
    }
    public void Add(Unit unit) {
        Debug.Log("Added unit: " + unit.name);
        //units.Add(unit);
        queue.Enqueue(unit);
        //NUnits++;
    }
    public void StartBattle() {
        foreach(Unit unit in units){
            unit.ResetStats();
            queue.Enqueue(unit);
        }
        
        battleStarted = true;
        academy = GameObject.Find("Academy").GetComponent<DFAcademy>();

        Debug.Log("Battle started");
        if(!academy.HasTrainingBrain()){
            Unit unit = units[0];
            // make sure skill buttons are clickable
            foreach (Button button in unit.skillButtons) {
                button.interactable = true;
            }
        }
        queue.FirstTurn();
        turnCounter = 0;
        logObject = GameObject.Find("BattleLog").GetComponentInChildren<Text>();
        StartTurn();
    }
    public void EndBattle(char winTeam) {
        Debug.Log("Battle ended");
        winningTeam = winTeam;
        if(!academy.HasTrainingBrain()){
            Unit survivor = queue.Dequeue();
            foreach(Button button in survivor.skillButtons) {
                button.interactable = false;
            }
        }
        queue.Clear();

        battleStarted = false;
        if (!academy.HasTrainingBrain() && !rematchOverride) {
            academy.CalculateRewards();
            foreach(DecisionLog log in decisionLogs){
                log.RecalculateStatistics();
            }
            GameObject message = GameObject.Instantiate(Resources.Load("WinMessage") as GameObject, GameObject.Find("Canvas").transform);
            message.GetComponentInChildren<Text>().text += winningTeam;
            message.GetComponent<Button>().onClick.AddListener(ButtonFunctions.EndGame);
        } else {
            academy.AcademyReset();
        }

    }
    public void StartTurn() {
        //Debug.Log("Turn started");
        waiting = false;
        currentUnit = queue.Dequeue();
        if (!academy.HasTrainingBrain())
            currentUnit.outline.SetActive(true);
        MyDelegate continuation = currentUnit.CountdownEffects;
        continuation += ContinueTurn;
        continuation();
    }
    private IEnumerator WaitSeconds(float dur) {
        //Debug.Log("waiting started");
        waiting = true;
        yield return new WaitForSeconds(dur);
        //Debug.Log("waiting ended");
        waiting = false;
    }
    public void Wait3Seconds() {
        if (!academy.HasTrainingBrain()) {
            waiting = true;
            currentUnit.StartCoroutine(WaitSeconds(3.0f));
        } else
            waiting = false;
    }
    private IEnumerator WaitingForWait() {
        while (waiting)
            yield return new WaitForSeconds(0.1f);
        //Debug.Log("requesting decision from " + currentUnit.name);
        currentUnit.GetComponent<UnitAgent>().RequestDecision();
    }
    private void WaitToGetAction() {
        currentUnit.StartCoroutine(WaitingForWait());
    }
    public void ContinueTurn() {
        //Debug.Log("continuing turn");
        if (currentUnit.CurHP > 0 && !currentUnit.acted) {
            // Update all skill buttons, and skill cooldowns
            MyDelegate orderedFunctions = currentUnit.SetupTurn;
            // If it is not the player, get decision of movement from brain
            if (currentUnit.GetComponent<UnitAgent>().brain.brainType != MLAgents.BrainType.Player) {
                orderedFunctions += Wait3Seconds;
                orderedFunctions += WaitToGetAction;
            }
            orderedFunctions();
        } else {
            // In case of stun, put a notification on the battle log
            if (currentUnit.statusUI.text.Contains("Stunned"))
                BattleLog += currentUnit.name+"->Stunned";
            //Debug.Log(currentUnit.name + " has " + ((currentUnit.acted) ? "" : "not ") + "acted, " + currentUnit.CurHP + " HP left");
            EndTurn();
        }
    }
    public void EndTurn() {
        /*
        foreach(UnitAgent agent in academy.agents) {
            Debug.Log("agent " + agent.name + " has reward of " + agent.GetReward());
        }
        */
        //Debug.Log("Turn ended");
        currentUnit.acted = false;
        if (!academy.HasTrainingBrain())
            currentUnit.outline.SetActive(false);
        int team1Count = 0;
        int team2Count = 0;
        //Debug.Log("units count = " + units.Count);
        foreach (Unit unit in units) {
            if (unit.CurHP > 0 && unit.team == '1')
                team1Count++;
            else if(unit.CurHP > 0)
                team2Count++;
            unit.MakeUnclickable();
            unit.FadeIn();
        }
        //Debug.Log("team1count = " + team1Count + "; team2count = " + team2Count);
        if (team1Count == 0 || team2Count == 0)
            EndBattle((team1Count == 0) ? '2' : '1');
        else
            StartTurn();
    }
    public void SkillSelected() {
        //Debug.Log("Skill selected");
        foreach(Unit unit in units) {
            switch (selectedAction.skillType) {
                case TARGETS.ALL:
                    unit.FadeIn();
                    unit.MakeClickable();
                    break;
                case TARGETS.ALLIES:
                    if(unit.team == currentUnit.team) {
                        unit.FadeIn();
                        unit.MakeClickable();
                    } else {
                        unit.FadeOut();
                        unit.MakeUnclickable();
                    }
                    break;
                case TARGETS.ENEMIES:
                    if (unit.team != currentUnit.team) {
                        unit.FadeIn();
                        unit.MakeClickable();
                    } else {
                        unit.FadeOut();
                        unit.MakeUnclickable();
                    }
                    break;
                case TARGETS.SELF:
                    if (unit == currentUnit) {
                        unit.FadeIn();
                        unit.MakeClickable();
                    } else {
                        unit.FadeOut();
                        unit.MakeUnclickable();
                    }
                    break;
            }
        }
    }
    public void TargetSelected() {
        Debug.Log("Target selected, "+currentUnit.name+" used "+selectedAction.name+" on "+targetUnit.name);
        MyDelegate aux = selectedAction.Use;
        // Give rewards for enemies killed and negative rewards for deaths
        aux += EndTurn;
        aux();
    }
}
