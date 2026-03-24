using UnityEngine;

public class GetAxeAction : GoapAction
{
    public GameObject axeObject;
    private bool done = false;

    void Awake()
    {
        AddEffect("hasAxe", true);
        cost = 1f;
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        target = axeObject;
        return target != null && target.activeSelf;
    }

    public override bool Perform(GameObject agent)
    {
        done = true;
        axeObject.SetActive(false);
        return true;
    }

    public override bool IsDone()
    {
        return done;
    }

    public override void ResetAction()
    {
        done = false;
    }
}
