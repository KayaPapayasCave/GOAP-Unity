using System.Collections.Generic;
using System.Linq;

public class SimpleGoapPlanner
{
    public Queue<GoapAction> Plan(List<GoapAction> availableActions, WorldState currentState, Dictionary<string, bool> goal)
    {
        List<GoapAction> usable = new List<GoapAction>(availableActions);

        Queue<GoapAction> plan = new Queue<GoapAction>();
        WorldState simulated = new WorldState();

        foreach (var kv in currentState.GetAllStates())
            simulated.SetState(kv.Key, kv.Value);

        int safety = 20;

        while (!GoalMet(goal, simulated) && safety-- > 0)
        {
            GoapAction best = usable
                .Where(a => PreconditionsMet(a.Preconditions, simulated))
                .OrderBy(a => a.cost)
                .FirstOrDefault(a => ProducesUsefulEffect(a, simulated, goal));

            if (best == null)
                return null;

            plan.Enqueue(best);

            foreach (var effect in best.Effects)
                simulated.SetState(effect.Key, effect.Value);

            usable.Remove(best);
        }

        return GoalMet(goal, simulated) ? plan : null;
    }

    private bool PreconditionsMet(Dictionary<string, bool> preconditions, WorldState state)
    {
        foreach (var p in preconditions)
        {
            if (state.GetState(p.Key) != p.Value)
                return false;
        }
        return true;
    }

    private bool GoalMet(Dictionary<string, bool> goal, WorldState state)
    {
        foreach (var g in goal)
        {
            if (state.GetState(g.Key) != g.Value)
                return false;
        }
        return true;
    }

    private bool ProducesUsefulEffect(GoapAction action, WorldState state, Dictionary<string, bool> goal)
    {
        foreach (var effect in action.Effects)
        {
            if (goal.ContainsKey(effect.Key) && state.GetState(effect.Key) != goal[effect.Key])
                return true;

            if (!state.GetState(effect.Key) && effect.Value)
                return true;
        }
        return false;
    }
}