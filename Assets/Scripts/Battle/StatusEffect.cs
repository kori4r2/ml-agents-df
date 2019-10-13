using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : System.IComparable<StatusEffect> {
    protected Unit source;
    protected Unit target;
    private int turnsLeft;
    public int TurnsLeft {
        get { return turnsLeft; }
        protected set {
            turnsLeft = value;
            if (TurnsLeft <= 0)
                Remove();
        }
    }
    public readonly int duration;
    public readonly string text;
    public readonly bool visible;
    // aux functions
    protected abstract void Apply();
    // public functions
    public StatusEffect(Unit src, Unit trgt, string txt, int dur, bool see=true) {
        source = src;
        target = trgt;
        text = txt;
        duration = dur;
        visible = see;
        Apply();
    }
    public abstract bool Countdown();
    public abstract void Remove();
    public int CompareTo(StatusEffect effect) {
        int nameComparison = text.CompareTo(effect.text);
        if(nameComparison == 0){
            return turnsLeft.CompareTo(effect.TurnsLeft);
        }
        return nameComparison;
    }
}
