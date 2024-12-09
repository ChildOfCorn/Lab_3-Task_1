using System;
using System.Collections.Generic;

public abstract class LivingOrganism
{
    public double Energy { get; protected set; }
    public int Age { get; protected set; }
    public double Size { get; protected set; }

    public LivingOrganism(double energy, int age, double size)
    {
        Energy = energy;
        Age = age;
        Size = size;
    }

    public abstract void Live();
}

public interface IReproducible
{
    LivingOrganism Reproduce();
}

public interface IPredator
{
    void Hunt(LivingOrganism prey);
}

public class Animal : LivingOrganism, IReproducible, IPredator
{
    public double Speed { get; private set; }

    public Animal(double energy, int age, double size, double speed)
        : base(energy, age, size)
    {
        Speed = speed;
    }

    public override void Live()
    {
        Energy -= 1;
        Age++;
    }

    public LivingOrganism Reproduce()
    {
        if (Energy > 10)
        {
            Energy -= 5;
            return new Animal(10, 0, Size * 0.8, Speed);
        }
        return null;
    }

    public void Hunt(LivingOrganism prey)
    {
        if (prey != null && Energy > 2)
        {
            Energy += prey.Size;
            prey = null;
        }
    }
}

public class Plant : LivingOrganism, IReproducible
{
    public double GrowthRate { get; private set; }

    public Plant(double energy, int age, double size, double growthRate)
        : base(energy, age, size)
    {
        GrowthRate = growthRate;
    }

    public override void Live()
    {
        Energy += GrowthRate;
        Age++;
    }

    public LivingOrganism Reproduce()
    {
        if (Energy > 15)
        {
            Energy -= 10;
            return new Plant(5, 0, Size * 0.5, GrowthRate);
        }
        return null;
    }
}

public class Microorganism : LivingOrganism
{
    public bool IsPathogenic { get; private set; }

    public Microorganism(double energy, int age, double size, bool isPathogenic)
        : base(energy, age, size)
    {
        IsPathogenic = isPathogenic;
    }

    public override void Live()
    {
        Energy -= 0.1;
        Age++;
    }
}

public class Ecosystem
{
    private List<LivingOrganism> organisms;

    public Ecosystem()
    {
        organisms = new List<LivingOrganism>();
    }

    public void AddOrganism(LivingOrganism organism)
    {
        organisms.Add(organism);
    }

    public void SimulateStep()
    {
        for (int i = 0; i < organisms.Count; i++)
        {
            LivingOrganism organism = organisms[i];
            organism.Live();

            if (organism is IReproducible reproducible)
            {
                var offspring = reproducible.Reproduce();
                if (offspring != null)
                {
                    organisms.Add(offspring);
                }
            }

            if (organism is IPredator predator)
            {
                var prey = organisms.Find(o => o != organism && o.Size < organism.Size);
                predator.Hunt(prey);
            }
        }

        organisms.RemoveAll(o => o.Energy <= 0);
    }

    public void DisplayEcosystem()
    {
        foreach (var organism in organisms)
        {
            Console.WriteLine($"{organism.GetType().Name} - Energy: {organism.Energy}, Age: {organism.Age}, Size: {organism.Size}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Ecosystem ecosystem = new Ecosystem();

        ecosystem.AddOrganism(new Animal(20, 0, 5, 10));
        ecosystem.AddOrganism(new Plant(30, 0, 2, 3));
        ecosystem.AddOrganism(new Microorganism(5, 0, 0.1, false));

        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine($"Step {i + 1}");
            ecosystem.SimulateStep();
            ecosystem.DisplayEcosystem();
            Console.WriteLine();
        }
    }
}
