using UnityEngine;

public class CollectWoodAction : GoapAction
{
    public GameObject woodPile;
    private bool done = false;

    void Awake()
    {
        AddPrecondition("treeChopped", true);
        AddEffect("hasWood", true);
        cost = 1f;
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        target = woodPile;
        return target != null && target.activeSelf;
    }

    public override bool Perform(GameObject agent)
    {
        done = true;
        woodPile.SetActive(false);
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
