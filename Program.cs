
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

public class Refrigirator : Container
{
    public string ProductType { get; private set; }
    public double Temperature { get; private set; }

    private static readonly Dictionary<string, double> ProductTemp = new()
    {
        { "Bananas", 13.3 },
        { "Chocolate", 18 },
        { "Fish", 2 },
        { "Meat", -15 },
        { "Ice cream", -18 },
        { "Frozen pizza", -30 },
        { "Cheese", 7.2 },
        { "Sausages", 5 },
        { "Butter", 20.5 },
        { "Eggs", 19 }
    };

    public Refrigirator(double depth, double height, double maxPayload, double tare, string product, double temp)
        : base(depth, height, maxPayload, "C", tare)
    {
        ProductType = product;
        Temperature = temp;

        if (!ProductTemp.ContainsKey(ProductType))
            throw new ArgumentException($"Wrong product: {ProductType}");

        if (Temperature < ProductTemp[ProductType])
            throw new ArgumentException($"Low temp for {ProductType}. Need: {ProductTemp[ProductType]}°C");
    }

    public class Ship
    {
        public string ShipName { get; private set; }
        public double MaxSpeed { get; private set; }
        public int MaxContainers { get; private set; }
        private List<Container> cargoList = new();

        public Ship(string name, double speed, int maxContainers)
        {
            ShipName = name;
            MaxSpeed = speed;
            MaxContainers = maxContainers;
        }

        public void LoadContainer(Container cont)
        {
            if (cargoList.Count >= MaxContainers)
                throw new InvalidOperationException("Ship is full!");
            cargoList.Add(cont);
        }

        public void LoadList(List<Container> list)
        {
            foreach (var c in list)
                LoadContainer(c);
        }

        public void RemoveContainer(string serial)
        {
            cargoList.RemoveAll(c => c.sNumber == serial);
        }

        public void ReplaceContainer(string serial, Container newC)
        {
            RemoveContainer(serial);
            LoadContainer(newC);
        }

        public void Transfer(string serial, Ship otherShip)
        {
            var cont = cargoList.FirstOrDefault(c => c.sNumber == serial);
            if (cont != null)
            {
                otherShip.LoadContainer(cont);
                RemoveContainer(serial);
            }
        }

        public double TotalWeight() => cargoList.Sum(c => c.CargoMass + c.TareWeight);
    }
}

