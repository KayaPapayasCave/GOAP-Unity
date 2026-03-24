using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class GoapAgent : MonoBehaviour
{
    public DemoMode demoMode = DemoMode.GoapOnly;
    public float moveSpeed = 2f;
    public float stopDistance = 1.0f;

    private NavMeshAgent navAgent;
    private WorldState beliefs;
    private List<GoapAction> actions;
    private Queue<GoapAction> currentPlan;
    private GoapAction currentAction;
    private SimpleGoapPlanner planner;
    private GameObject lastLoggedTarget;
    private bool goalCompletedLogged = false;

    public Dictionary<string, bool> goal = new() { { "hasWood", true } };

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();

        if (navAgent != null)
            navAgent.enabled = (demoMode == DemoMode.GoapPlusNavMesh);

        planner = new SimpleGoapPlanner();

        beliefs = new WorldState();
        beliefs.SetState("hasAxe", false);
        beliefs.SetState("treeChopped", false);
        beliefs.SetState("hasWood", false);

        actions = new List<GoapAction>(GetComponents<GoapAction>());
    }

    void Update()
    {
        if (GoalAlreadyMet())
        {
            if (!goalCompletedLogged)
            {
                Debug.Log("Mål fuldført");
                goalCompletedLogged = true;
            }
            return;
        }

        if (currentAction == null && (currentPlan == null || currentPlan.Count == 0))
        {
            BuildPlan();
        }

        if (currentAction == null && currentPlan != null && currentPlan.Count > 0)
        {
            currentAction = currentPlan.Dequeue();
            currentAction.ResetAction();

            string cleanName = currentAction.GetType().Name.Replace("Action", "");
            Debug.Log("Starter action: " + cleanName);

            if (!currentAction.CheckProceduralPrecondition(gameObject))
            {
                currentAction = null;
                currentPlan = null;
                return;
            }
        }

        if (currentAction == null)
            return;

        if (currentAction.RequiresInRange())
        {
            MoveTowardsTarget(currentAction.target);

            if (!InRange(currentAction.target))
                return;
        }

        bool success = currentAction.Perform(gameObject);

        if (!success)
        {
            currentAction = null;
            currentPlan = null;
            return;
        }

        if (currentAction.IsDone())
        {
            ApplyEffects(currentAction);
            currentAction = null;
        }
    }

    bool GoalAlreadyMet()
    {
        foreach (var g in goal)
        {
            if (beliefs.GetState(g.Key) != g.Value)
                return false;
        }
        return true;
    }

    void BuildPlan()
    {
        currentAction = null;
        currentPlan = planner.Plan(actions, beliefs, goal);

        if (currentPlan == null)
        {
            Debug.Log("Ingen plan fundet");
        }
        else if (currentPlan.Count > 0)
        {
            Debug.Log("Plan: " + string.Join(" -> ", currentPlan.Select(a => a.GetType().Name.Replace("Action", ""))));
        }
    }

    void ApplyEffects(GoapAction action)
    {
        foreach (var effect in action.Effects)
            beliefs.SetState(effect.Key, effect.Value);
    }

    void MoveTowardsTarget(GameObject target)
    {
        if (target == null)
            return;

        if (demoMode == DemoMode.GoapPlusNavMesh && navAgent != null && navAgent.enabled)
        {
            navAgent.SetDestination(target.transform.position);
        }
        else
        {
            Vector3 toTarget = target.transform.position - transform.position;
            toTarget.y = 0f;

            if (toTarget.sqrMagnitude < 0.001f)
                return;

            Vector3 direction = toTarget.normalized;
            float moveStep = moveSpeed * Time.deltaTime;

            Vector3 castOrigin = transform.position + Vector3.up * 0.5f;
            float castRadius = 0.35f;
            float castDistance = 0.8f;

            RaycastHit hit;
            bool blocked = Physics.SphereCast(castOrigin, castRadius, direction, out hit, castDistance);

            if (blocked)
            {
                GameObject hitObject = hit.collider.gameObject;

                bool hitIsTarget =
                    hitObject == target ||
                    hit.collider.transform.IsChildOf(target.transform);

                if (!hitIsTarget)
                    return;
            }

            transform.position += direction * moveStep;
        }
    }

    bool InRange(GameObject target)
    {
        if (target == null)
            return false;

        return Vector3.Distance(transform.position, target.transform.position) <= stopDistance;
    }
}