using System.Collections.Generic;

namespace MontyHall;

public class SimulationResult
{
    public int ChosenDoorByPlayer { get; set; }
    public int ChosenDoorByPresenter { get; set; }
    public List<bool> Simulation { get; set; }
    public bool Hit { get; set; }
}

public class MontyHallResponse
{
    public List<SimulationResult> Results { get; set; } = new();
    public double HitPercentage { get; set; }
}