
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

public class Liquid : Container, IHazardExc
{
    public bool IsHazardous { get; private set; }

    public Liquid(double depth, double height, double maxPayload, double tare, bool isHazard)
        : base(depth, height, maxPayload, "L", tare)
    {
        IsHazardous = isHazard;
    }

    public override void LoadCargo(double mass)
    {
        double limit = IsHazardous ? MaxPayLoad * 0.5 : MaxPayLoad * 0.9;
        if (mass > limit)
        {
            Hazard($"Dangerous overfill {sNumber}");
            throw new HazardExc("Hazard overload!");
        }
        base.LoadCargo(mass);
    }

    public void Hazard(string message) => Console.WriteLine($"[Hazard Alert] {message}");
}

public class Gas : Container, IHazardExc
{
    public double Pressure { get; private set; }

    public Gas(double depth, double height, double maxPayload, double tare, double pressure)
        : base(depth, height, maxPayload, "G", tare)
    {
        Pressure = pressure;
    }

    public override void Empty()
    {
        CargoMass *= 0.05;
    }

    public void Hazard(string message) => Console.WriteLine($"[Hazard Alert] {message}");
}

