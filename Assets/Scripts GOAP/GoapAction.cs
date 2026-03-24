using UnityEngine;
using System.Collections.Generic;

public abstract class GoapAction : MonoBehaviour
{
    public float cost = 1f;
    public GameObject target;

    protected Dictionary<string, bool> preconditions = new();
    protected Dictionary<string, bool> effects = new();

    public Dictionary<string, bool> Preconditions => preconditions;
    public Dictionary<string, bool> Effects => effects;

    public void AddPrecondition(string key, bool value)
    {
        preconditions[key] = value;
    }

    public void AddEffect(string key, bool value)
    {
        effects[key] = value;
    }

    public virtual void ResetAction() { }

    public virtual bool RequiresInRange()
    {
        return true;
    }

    public abstract bool CheckProceduralPrecondition(GameObject agent);
    public abstract bool Perform(GameObject agent);
    public abstract bool IsDone();
}