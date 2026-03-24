using System.Collections.Generic;

public class WorldState
{
    private Dictionary<string, bool> states = new();

    public void SetState(string key, bool value)
    {
        states[key] = value;
    }

    public bool GetState(string key)
    {
        return states.TryGetValue(key, out bool value) && value;
    }

    public Dictionary<string, bool> GetAllStates()
    {
        return new Dictionary<string, bool>(states);
    }
}
