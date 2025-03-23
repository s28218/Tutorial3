
using System;
using System.Collections.Generic;
using System.Linq;

Console.WriteLine("Hello");

public class HazardExc : Exception
{
    public HazardExc(string message) : base(message) { }
}

public class OverfillExc : Exception
{
    public OverfillExc(string message) : base(message) { }
}

public interface IHazardExc
{
    void Hazard(string messsage);
}

public abstract class Container
{
    public double Depth { get; protected set; }
    public double Height { get; protected set; }
    public double CargoMass { get; protected set; }
    public double TareWeight { get; protected set; }
    public double MaxPayLoad { get; protected set; }
    public string sNumber { get; private set; }
    public static int counter = 1;

    protected Container(double depth, double height, double maxPayLoad, string type, double tareWeight)
    {
        sNumber = $"KON -{type}-{counter++}";
        TareWeight = tareWeight;
        Depth = depth;
        Height = height;
        MaxPayLoad = maxPayLoad;
    }

    public virtual void Empty()
    {
        CargoMass = 0;
    }

    public virtual void LoadCargo(double mass)
    {
        if(mass + CargoMass > MaxPayLoad)
            throw new OverfillExc($"Container {sNumber} overload");
        CargoMass += mass;
    }
    
    public override string ToString() =>
        $"{sNumber}: CargoMass={CargoMass}kg, MaxPayLoad={MaxPayLoad}kg";
}

