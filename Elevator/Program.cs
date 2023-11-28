using System;
using System.Threading;
using System.Threading.Tasks;

class Agent
{
    public string Name { get; set; }
    public string SecurityLevel { get; set; }

    public Agent(string name, string securityLevel)
    {
        Name = name;
        SecurityLevel = securityLevel;
    }

    public void CallElevator(Elevator elevator, int currentFloor, int destinationFloor)
    {
        Task.Run(() =>
        {
            elevator.ReceiveCall(currentFloor, destinationFloor, this);
            EnterElevator();
            ExitElevator();
        });
    }

    private void EnterElevator()
    {
        Console.WriteLine($"{Name} enters the elevator.");
        Thread.Sleep(1000);
    }

    private void ExitElevator()
    {
        Console.WriteLine($"{Name} exits the elevator.");
    }
}

class Elevator
{
    public int CurrentFloor { get; set; }
    public string Direction { get; set; }
    public bool IsAvailable { get; set; }
    private object doorLock = new object();

    public Elevator()
    {
        CurrentFloor = 1;
        Direction = "Stationary";
        IsAvailable = true;
    }

    public void ReceiveCall(int currentFloor, int destinationFloor, Agent agent)
    {
        if (!IsAvailable)
        {
            Console.WriteLine("Elevator is currently busy. Please wait.");
            return;
        }

        IsAvailable = false;
        Console.WriteLine($"Elevator called from floor {currentFloor} to go to floor {destinationFloor}.");

        if (destinationFloor > CurrentFloor)
            Direction = "Up";
        else if (destinationFloor < CurrentFloor)
            Direction = "Down";
        else
            Direction = "Stationary";

        while (CurrentFloor != destinationFloor)
        {
            if (Direction == "Up")
                CurrentFloor++;
            else if (Direction == "Down")
                CurrentFloor--;

            Console.WriteLine($"Elevator is at floor {CurrentFloor}.");
            Thread.Sleep(1000);
        }

        Console.WriteLine("Elevator has arrived.");
        PerformSecurityCheck(agent);
    }

    private void PerformSecurityCheck(Agent agent)
    {
        lock (doorLock)
        {
            if (agent.SecurityLevel == "Top-secret")
            {
                Console.WriteLine("Agent has top-secret clearance. Door opening...");
                OpenDoor();
            }
            else if (agent.SecurityLevel == "Secret")
            {
                Console.WriteLine("Agent has secret clearance. Door opening...");
                OpenDoor();
            }
            else
            {
                Console.WriteLine("Agent has lower clearance. Access denied.");
                IsAvailable = true;
            }
        }
    }

    private void OpenDoor()
    {
        Console.WriteLine("Door opens.");
        Thread.Sleep(1000);
        CloseDoor();
    }

    private void CloseDoor()
    {
        Console.WriteLine("Door closes.");
        IsAvailable = true;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Elevator elevator = new Elevator();

        Agent agent1 = new Agent("Agent 1", "Confidential");
        Agent agent2 = new Agent("Agent 2", "Secret");
        Agent agent3 = new Agent("Agent 3", "Top-secret");

        Task.Run(() => agent1.CallElevator(elevator, 1, 4));
        Task.Run(() => agent2.CallElevator(elevator, 3, 2));
        Task.Run(() => agent3.CallElevator(elevator, 2, 1));

        Console.ReadLine();
    }
}
