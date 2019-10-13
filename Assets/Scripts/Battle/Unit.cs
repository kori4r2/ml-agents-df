using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour , System.IComparable<Unit> {
    // Fixed stats
    [SerializeField] private int maxHP;
    [SerializeField] private int maxMP;
    [SerializeField] private int baseDef;
    [SerializeField] private int baseBth;
    [SerializeField] private int baseInitiative;
    // Changeable stats
    [SerializeField] private int curHP;
    private int curMP;
    [SerializeField] private int baseAtk;
    [SerializeField] private int maxAtk;
    [SerializeField] private int def;
    [SerializeField] private int bth;
    [SerializeField] private int initiative;
    private int lastInitRoll;
    public char team;
    public bool acted;
    public bool lastAtkHit;
    public new string name;
    // UI Elements
    public GameObject outline;
    public Text nameUI;
    public Text hpUI;
    public Text statusUI;
    public Button selectButton;
    // Name of skills must be the same as the button game object names
    public Button[] skillButtons;
    // Properties (getters/setters)
    public int CurHP {
        get { return curHP; }
        private set {
            if (curHP > 0) {
                curHP = Mathf.Clamp(value, 0, MaxHP);
                if (!battleManager.academy.HasTrainingBrain())
                    hpUI.text = "HP =             " + curHP.ToString().PadLeft((int)System.Math.Floor(System.Math.Log10(MaxHP)+1)) + " /"+MaxHP;
                if (curHP <= 0) {
                    battleManager.Kill(this);
                    Purge();
                }
            }
        }
    }
    public int CurMP { get { return curMP; } private set { curMP = value; } } //not gonna use this now
    public int MaxHP { get { return maxHP; } }
    public int MaxMP { get { return maxMP; } }
    public int BaseAtk { get { return baseAtk; } }
    public int MaxAtk { get { return maxAtk; } }
    public int Def { get { return def; } set { def = value; } }
    public int Bth { get { return bth; } set { bth = value; } }
    public int Initiative { get { return initiative; } set { initiative = value; } }
    public int LastInitRoll { get { return lastInitRoll; } }
    public int Dmg { get { return DmgRoll(); } }
    public int Atk { get { return AtkRoll(); } }
    // Aux variables
    public List<StatusEffect> effects;
    public List<Skill> skillList;
    public BattleManager battleManager;
    // Aux functions
    public int RollInitiative() {
        lastInitRoll = Random.Range(1, 100) + Initiative;
        return lastInitRoll;
    }
    public int CompareTo(Unit unit) {
        return unit.LastInitRoll.CompareTo(LastInitRoll);
    }
    private int AtkRoll() {
        return Bth + Random.Range(0, 100);
    }
    private int DmgRoll(){
        return Random.Range(BaseAtk, MaxAtk);
    }
    private float GetMultiplier(string element) {
        return 1.0f;
    }
    // Public functions
    // Battle related functions
    public void AddEffect(StatusEffect effect) {
        effects.Add(effect);
        effects.Sort();
        UpdateStatusUI();
    }
    public bool CheckHit(int bonus) {
        Debug.Log("Checking hit, bth="+bonus+", def="+Def);
        return bonus > Def;
    }
    public int DealDamage(int value, string element = "None") {
        Debug.Log("Dealing "+value+" damage");
        CurHP -= (int)(value * GetMultiplier(element));
        return (int)(value * GetMultiplier(element));
    }
    public void Purge() {
        foreach (StatusEffect status in effects)
            status.Remove();
        effects.Clear();
    }
    public void CountdownEffects() {
        foreach (StatusEffect status in effects.ToArray()){
            if (CurHP > 0 && !status.Countdown())
                effects.Remove(status);
        }
        effects.Sort();
        UpdateStatusUI();
    }
    // UI related functions
    public void UpdateStatusUI() {
        if (battleManager.academy.HasTrainingBrain())
            return;
        string newStatusUI = "";
        foreach (StatusEffect status in effects) {
            newStatusUI += status.text + "(" + status.TurnsLeft + ")" + System.Environment.NewLine;
        }
        statusUI.text = newStatusUI;
    }
    public void SetupTurn() {
        // Update all skill buttons
        for (int i = 0; i < skillList.Count; i++) {
            skillList[i].CurCD--;
            if (!battleManager.academy.HasTrainingBrain()) {
                skillButtons[i].gameObject.GetComponentInChildren<Text>().text =
                (skillList[i].CurCD <= 0) ? skillList[i].name : skillList[i].CurCD.ToString();
                skillButtons[i].interactable = skillList[i].Available;
            }
        }
        Debug.Log("Setting up skills");
    }
    public void MakeClickable() {
        if (battleManager.academy.HasTrainingBrain())
            return;
        selectButton.gameObject.GetComponent<Image>().raycastTarget = true;
    }
    public void MakeUnclickable() {
        if (battleManager.academy.HasTrainingBrain())
            return;
        selectButton.gameObject.GetComponent<Image>().raycastTarget = false;
    }
    public void FadeOut() {
        if (battleManager.academy.HasTrainingBrain())
            return;
        selectButton.interactable = false;
    }
    public void FadeIn() {
        if (battleManager.academy.HasTrainingBrain())
            return;
        selectButton.interactable = true;
    }

    // Use this for initialization
    void Awake () {
        if (!battleManager.academy.HasTrainingBrain())
            nameUI.text = name;
        CurHP = maxHP;
        CurMP = maxMP;
        Bth = baseBth;
        Def = baseDef;
        Initiative = baseInitiative;
        acted = false;
        lastAtkHit = false;
        effects = new List<StatusEffect>();
        skillList = new List<Skill>();
        UpdateStatusUI();
        //Adds all skills to the list
        skillList.Add(new Attack(this));
        skillList.Add(new Double(this));
        skillList.Add(new Poison(this));
        skillList.Add(new Stun(this));
        skillList.Add(new Blind(this));
        skillList.Add(new Block(this));
        skillList.Add(new Heal(this));
        MakeUnclickable();
        FadeIn();
        if (!battleManager.academy.HasTrainingBrain())
            outline.SetActive(false);
	}

    public void ResetStats() {
        FadeIn();
        Purge();
        foreach (Skill skill in skillList)
            skill.ResetCooldown();
        curHP = 1;
        CurHP = MaxHP;
        curMP = 1;
        CurMP = MaxMP;
        Bth = baseBth;
        Def = baseDef;
        Initiative = baseInitiative;
        acted = false;
        lastAtkHit = false;
        MakeUnclickable();
        if (!battleManager.academy.HasTrainingBrain())
            outline.SetActive(false);
        Debug.Log("Resetting " + name + ", CurHP = " + CurHP);
    }
}