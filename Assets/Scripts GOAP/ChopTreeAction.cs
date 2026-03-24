using UnityEngine;

public class ChopTreeAction : GoapAction
{
    public GameObject treeObject;
    public GameObject woodPile;
    private bool done = false;

    void Awake()
    {
        AddPrecondition("hasAxe", true);
        AddEffect("treeChopped", true);
        cost = 2f;
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        target = treeObject;
        return target != null && target.activeSelf;
    }

    public override bool Perform(GameObject agent)
    {
        done = true;

        treeObject.SetActive(false);
        woodPile.SetActive(true);

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
