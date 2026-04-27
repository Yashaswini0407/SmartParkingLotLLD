using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public enum VehicleType
{
    Motorcycle,
    Car,
    Bus
}

public enum SpotType
{
    Small,
    Medium,
    Large
}

public class Vehicle
{
    public string LicenseNumber { get; set; }
    public VehicleType Type { get; set; }

    public Vehicle(string licenseNumber, VehicleType type)
    {
        LicenseNumber = licenseNumber;
        Type = type;
    }
}

public class ParkingSpot
{
    public int SpotId { get; private set; }
    public SpotType SpotType { get; private set; }
    public bool IsOccupied { get; private set; }

    public ParkingSpot(int id, SpotType type)
    {
        SpotId = id;
        SpotType = type;
        IsOccupied = false;
    }

    public void AssignVehicle()
    {
        IsOccupied = true;
    }

    public void RemoveVehicle()
    {
        IsOccupied = false;
    }
}

public class ParkingFloor
{
    public int FloorNumber { get; private set; }
    public List<ParkingSpot> Spots { get; private set; }

    public ParkingFloor(int floorNumber, List<ParkingSpot> spots)
    {
        FloorNumber = floorNumber;
        Spots = spots;
    }

    public ParkingSpot GetAvailableSpot(SpotType type)
    {
        return Spots.FirstOrDefault(x => !x.IsOccupied && x.SpotType == type);
    }
}

public class ParkingTicket
{
    public string TicketId { get; private set; }
    public Vehicle Vehicle { get; private set; }
    public ParkingSpot Spot { get; private set; }

    public DateTime EntryTime { get; private set; }
    public DateTime ExitTime { get; private set; }

    public ParkingTicket(string id, Vehicle vehicle, ParkingSpot spot)
    {
        TicketId = id;
        Vehicle = vehicle;
        Spot = spot;
        EntryTime = DateTime.Now;
    }

    public void CloseTicket()
    {
        ExitTime = DateTime.Now;
    }
}

public class FeeCalculator
{
    public double CalculateFee(ParkingTicket ticket)
    {
        ticket.CloseTicket();

        double hours = (ticket.ExitTime - ticket.EntryTime).TotalHours;

        if (hours < 1)
            hours = 1;

        switch (ticket.Vehicle.Type)
        {
            case VehicleType.Motorcycle:
                return hours * 10;
            case VehicleType.Car:
                return hours * 20;
            case VehicleType.Bus:
                return hours * 50;
            default:
                return 0;
        }
    }
}

public class SpotAllocator
{
    public ParkingSpot Allocate(List<ParkingFloor> floors, Vehicle vehicle)
    {
        SpotType required = vehicle.Type switch
        {
            VehicleType.Motorcycle => SpotType.Small,
            VehicleType.Car => SpotType.Medium,
            VehicleType.Bus => SpotType.Large,
            _ => SpotType.Small
        };

        foreach (var floor in floors)
        {
            var spot = floor.GetAvailableSpot(required);

            if (spot != null)
            {
                spot.AssignVehicle();
                return spot;
            }
        }

        return null;
    }
}

public class ParkingLot
{
    private List<ParkingFloor> floors;
    private Dictionary<string, ParkingTicket> activeTickets;

    private SpotAllocator allocator;
    private FeeCalculator feeCalculator;

    private static readonly object lockObj = new object();

    public ParkingLot(List<ParkingFloor> floors)
    {
        this.floors = floors;
        activeTickets = new Dictionary<string, ParkingTicket>();

        allocator = new SpotAllocator();
        feeCalculator = new FeeCalculator();
    }

    public ParkingTicket CheckIn(Vehicle vehicle)
    {
        lock (lockObj)
        {
            var spot = allocator.Allocate(floors, vehicle);

            if (spot == null)
            {
                Console.WriteLine("Parking Full");
                return null;
            }

            string ticketId = Guid.NewGuid().ToString();

            var ticket = new ParkingTicket(ticketId, vehicle, spot);

            activeTickets.Add(ticketId, ticket);

            Console.WriteLine($"Vehicle parked. Ticket: {ticketId}");

            return ticket;
        }
    }

    public double CheckOut(string ticketId)
    {
        lock (lockObj)
        {
            if (!activeTickets.ContainsKey(ticketId))
            {
                Console.WriteLine("Invalid Ticket");
                return 0;
            }

            var ticket = activeTickets[ticketId];

            ticket.Spot.RemoveVehicle();

            double fee = feeCalculator.CalculateFee(ticket);

            activeTickets.Remove(ticketId);

            Console.WriteLine($"Fee: {fee}");

            return fee;
        }
    }
}

class Program
{
    static void Main()
    {
        var floor = new ParkingFloor(1, new List<ParkingSpot>
        {
            new ParkingSpot(1, SpotType.Small),
            new ParkingSpot(2, SpotType.Medium),
            new ParkingSpot(3, SpotType.Large)
        });

        var lot = new ParkingLot(new List<ParkingFloor> { floor });

        var vehicle = new Vehicle("KA01AB1234", VehicleType.Car);

        var ticket = lot.CheckIn(vehicle);

        Thread.Sleep(2000);

        lot.CheckOut(ticket.TicketId);
    }
}
