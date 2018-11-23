using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour {
    // Fixed stats
    [SerializeField]
    private int maxHP;
    [SerializeField]
    private int maxMP;
    [SerializeField]
    private int baseDef;
    [SerializeField]
    private int baseBth;
    [SerializeField]
    private int baseInitiative;
    [SerializeField]
    // Changeable stats
    private int curHP;
    private int curMP;
    private int baseAtk;
    private int maxAtk;
    private int def;
    private int bth;
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
    public Button[] skillButtons; // to do
    // Properties (getters/setters)
    public int CurHP {
        get {
            return curHP;
        }
        private set {
            curHP = (value <= 0)? 0 : value;
            hpUI.text = "HP =             "+curHP.ToString().PadLeft(3)+" /100";
            if (curHP <= 0) {
                MakeUnclickable();
                selectButton.interactable = false;
                BattleManager.Kill(this);
            }
        }
    }
    public int CurMP { get; private set; } //not gonna use this now
    public int MaxHP { get; }
    public int MaxMP { get; }
    public int BaseAtk { get; private set; }
    public int MaxAtk { get; private set; }
    public int Def { get; set; }
    public int Bth { get; set; }
    public int Dmg {
        get { return DmgRoll(); }
    }
    public int Atk {
        get { return AtkRoll(); }
    }
    // Aux variables
    public List<StatusEffect> effects;
    public List<Skill> skillList;
    // Aux functions
    private int AtkRoll() {
        return baseBth + Random.Range(0, 100);
    }
    private int DmgRoll(){
        return Random.Range(BaseAtk, MaxAtk);
    }
    private float GetMultiplier(string element) {
        return 1.0f;
    }
    private string updateStatusUI() {
        string newStatusUI = "";
        foreach(StatusEffect status in effects) {
            newStatusUI += status.text + "(" + status.TurnsLeft + ")" + System.Environment.NewLine;
        }
        statusUI.text = newStatusUI;
        return newStatusUI;
    }
    // Public functions
    public bool CheckHit(int bth) {
        return bth > Def;
    }
    public void Purge() {
        foreach (StatusEffect status in effects)
            status.Remove();
    }
    public void CountdownEffects() {
        foreach (StatusEffect status in effects)
            status.Countdown();
    }
    public void DealDamage(int value, string element = "None") {
        CurHP -= (int)(value * GetMultiplier(element));
    }
    public void MakeClickable() {
        selectButton.gameObject.GetComponent<Image>().raycastTarget = true;
    }
    public void MakeUnclickable() {
        selectButton.gameObject.GetComponent<Image>().raycastTarget = false;
    }

    // Use this for initialization
    void Start () {
        nameUI.text = name;
        CurHP = maxHP;
        CurMP = maxMP;
        effects = new List<StatusEffect>();
        skillList = new List<Skill>();
        updateStatusUI();
        //Adds all skills to the list
        skillList.Add(new Attack(this));
        skillList.Add(new Double(this));
        skillList.Add(new Poison(this));
        skillList.Add(new Stun(this));
        skillList.Add(new Blind(this));
        skillList.Add(new Block(this));
        skillList.Add(new Heal(this));
        MakeUnclickable();
        outline.SetActive(false);
        BattleManager.Add(this);
	}
}