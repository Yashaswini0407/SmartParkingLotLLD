# Smart Parking Lot Backend System (Low Level Design)

## Objective
Design a backend system for managing vehicle entry, parking allocation, exit tracking, and fee calculation in a multi-floor smart parking lot.

## Functional Requirements Covered

✔ Automatic parking spot allocation  
✔ Vehicle check-in and check-out tracking  
✔ Fee calculation based on duration and vehicle type  
✔ Real-time availability update  
✔ Concurrency handling for multiple vehicles  

---

## Data Model (Database Schema)

Vehicles
- LicenseNumber (PK)
- VehicleType

ParkingSpots
- SpotId (PK)
- FloorNumber
- SpotType
- IsOccupied

ParkingTickets
- TicketId (PK)
- LicenseNumber (FK)
- SpotId (FK)
- EntryTime
- ExitTime
- Fee

---

## Spot Allocation Algorithm

1. Detect vehicle type
2. Map vehicle to required spot size
3. Scan floors sequentially
4. Assign first available matching spot
5. Generate ticket

Time Complexity: O(n)

---

## Fee Calculation Logic

Motorcycle → ₹10/hour  
Car → ₹20/hour  
Bus → ₹50/hour  

Minimum billing: 1 hour

---

## Concurrency Handling

Thread-safe locking ensures:

- No duplicate spot allocation
- Safe parallel entry/exit
- Consistent availability tracking

Implemented using lock() in C#.

---

## Tech Stack

Language: C#
Architecture: Object-Oriented Low-Level Design
