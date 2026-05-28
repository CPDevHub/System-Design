/*
ParkingLevel:
level:int(Pk)
parkingSlots:List<ParkingSlot>
Add_Slot(int totalSlots,VehicleType Slot)
Remove_Slot(int totalSlots,VehicleType Slot)

ParkingSlot:
id:(PK)
level:ParkingLevel(Fk)
type:[[Car,Bus,MotorCycle]
VehiclePark:Vehicle | null

EntryTicket
id:int
arrivalTime:Time
slot:ParkingSlot
vehicle:Vehicle

ParkingBill:
id:int
ticket:EntryTicket
totalFare:int
deptTime:Time

Vehicle:
licenseNum(PK)
type:[Car,Bus,MotorCycle]

ParkingLotService(Singleton):
#levels:List<ParkingLevels>
#Map<VehicleType, IPricingStrategy> pricingStrategies;
#IAllocationService allocationStrategy
ParkingLostService(List<ParkingLevels> _parkingLevels, Map<VehicleType, IPricingStrategy> _pricingStrategies, IAllocationService _allocationStrategy){}
Add_Level(totalMotorCycleSlots,totalCarSlots,totalBusSlots)
ParkVehicle(licenseNum,VehicleType):EntryTicket
ExitVehicle(EntryTicket):Ticket

IAllocationService: FirstFreeSlotAllocationStrategy
FirstFreeSlotAllocationStrategy(parkingLevel:ParkingLevel){}
allocateSlot(vehicle):ParkingSlot | null

IPricingService:BusPricingStrategy, MotorCyclePricingStrategy, CarPricingStrategy
getTicket(EntryTicket):Ticket

*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ParkingLotLLD
{
    // =======================
    // ENUMS
    // =======================

    public enum VehicleType
    {
        Car,
        Bike,
        Bus
    }

    // =======================
    // DOMAIN ENTITIES
    // =======================

    public class Vehicle
    {
        public string LicenseNumber { get; private set; }
        public VehicleType Type { get; private set; }

        public Vehicle(string licenseNumber, VehicleType type)
        {
            LicenseNumber = licenseNumber;
            Type = type;
        }
    }


    public class ParkingSlot
    {
        public int Id { get; private set; }

        public int Level { get; private set; }

        public VehicleType Type { get; private set; }

        public Vehicle? ParkedVehicle { get; private set; }
        private readonly object _slotLock = new object();
        private bool _isRemoved = false; //Mark slot temporary deleted

        public ParkingSlot(
            int id,
            int level,
            VehicleType type
        )
        {
            Id = id;
            Level = level;
            Type = type;
        }

        public bool IsAvailable()
        {
            return ParkedVehicle==null;
        }

        public bool TryAssignVehicle(Vehicle vehicle)
        {
            lock (_slotLock)
            {
                if (ParkedVehicle != null || _isRemoved) return false;
                ParkedVehicle = vehicle;
                return true;
            }
        }
 
        public void RemoveVehicle()
        {
            lock (_slotLock)
            {
                ParkedVehicle = null;
            }
        }
    }


    public class ParkingLevel
    {
        public int LevelNumber { get; private set; }

        private readonly List<ParkingSlot> _slots;

        public ParkingLevel(int levelNumber)
        {
            LevelNumber = levelNumber;
            _slots = new List<ParkingSlot>();
        }

        public void AddSlots(int totalSlots, VehicleType type)
        {
            for(int i=0;i<totalSlots;i++) _slots.Add(new ParkingSlot(_slots.Count,this.LevelNumber,type));
        }

        public void RemoveSlots(int totalSlots, VehicleType type)
        {
            for(int i = _slots.Count-1; i>0 && totalSlots>0; i--)
            {
                if(_slots[i].IsAvailable() && _slots[i].Type == type)
                {
                    _slots.Remove(_slots[i]);
                    totalSlots--;
                }
            }
        }

        public ReadOnlyCollection<ParkingSlot> GetAllSlots()
        {
            return _slots.AsReadOnly();
        }
    }

    public class EntryTicket
    {
        public string Id { get; private set; }

        public DateTime ArrivalTime { get; private set; }

        public ParkingSlot Slot { get; private set; }

        public Vehicle Vehicle { get; private set; }

        public EntryTicket(
            string id,
            Vehicle vehicle,
            ParkingSlot slot
        )
        {
            Id = id;
            Vehicle = vehicle;
            Slot = slot;
            ArrivalTime = DateTime.Now;
        }
    }


    public class ParkingBill
    {
        public long Id { get; private set; }

        public EntryTicket Ticket { get; private set; }

        public decimal TotalFare { get; private set; }

        public DateTime DepartureTime { get; private set; }

        public ParkingBill(
            long id,
            EntryTicket ticket,
            decimal totalFare
        )
        {
            Id = id;
            Ticket = ticket;
            TotalFare = totalFare;
            DepartureTime = DateTime.Now;
        }
    }


    // =======================
    // STRATEGIES
    // =======================

    public interface ISlotAllocationStrategy
    {
        ParkingSlot? AllocateSlot(
            Vehicle vehicle,
            ReadOnlyCollection<ParkingLevel> levels
        );
    }


    public class FirstFreeSlotAllocationStrategy
        : ISlotAllocationStrategy
    {
        public ParkingSlot? AllocateSlot(
            Vehicle vehicle,
            ReadOnlyCollection<ParkingLevel> levels
        )
        {
            for(int i = 0; i < levels.Count; i++)
            {
                ReadOnlyCollection<ParkingSlot> slots=levels[i].GetAllSlots();
                for(int j = 0; j < slots.Count; j++)
                {
                    if (slots[j].Type == vehicle.Type && slots[j].TryAssignVehicle(vehicle))
                        return slots[j];
                }
            }
            return null;
        }
    }


    public abstract class PricingStrategy
    {
        public abstract decimal CalculateFare(EntryTicket ticket);
        protected decimal ParkingDuration(DateTime ArrivalTime)
        {
            return (decimal)(DateTime.Now - ArrivalTime).TotalHours;
        }
    }


    public class CarPricingStrategy
        : PricingStrategy
    {
        public override decimal CalculateFare(EntryTicket ticket)
        {
            return 200m * ParkingDuration(ticket.ArrivalTime);
        }
    }


    public class BikePricingStrategy
        : PricingStrategy
    {
        public override decimal CalculateFare(EntryTicket ticket)
        {
            return 100m * ParkingDuration(ticket.ArrivalTime);
        }
    }


    public class BusPricingStrategy
        : PricingStrategy
    {
        public override decimal CalculateFare(EntryTicket ticket)
        {
            return 500m * ParkingDuration(ticket.ArrivalTime);
        }
    }


    // =======================
    // SERVICES
    // =======================

    public class ParkingLotService
    {
        
        private List<ParkingLevel> _parkingLevels;
        private readonly ISlotAllocationStrategy _allocationStrategy;
        private readonly object _lockObj = new object();

        private readonly Dictionary<
            VehicleType,
            PricingStrategy
        > _pricingStrategies;

        public ParkingLotService(
            List<ParkingLevel> parkingLevels,
            ISlotAllocationStrategy allocationStrategy,
            Dictionary<VehicleType, PricingStrategy> pricingStrategies
        )
        {
            _parkingLevels=parkingLevels;
            _allocationStrategy = allocationStrategy;
            _pricingStrategies = pricingStrategies;
        }

        public void AddLevel(
            int totalBikeSlots,
            int totalCarSlots,
            int totalBusSlots
        )
        {
            var newLevel = new ParkingLevel(_parkingLevels.Count);
                newLevel.AddSlots(totalBikeSlots, VehicleType.Bike);
                newLevel.AddSlots(totalCarSlots,  VehicleType.Car);
                newLevel.AddSlots(totalBusSlots,  VehicleType.Bus);
                _parkingLevels.Add(newLevel);
        }

        public EntryTicket? ParkVehicle(
            string licenseNumber,
            VehicleType vehicleType
        )
        {
            //Check Avialability and assign
            Vehicle vehicle=new Vehicle(licenseNumber,vehicleType);
           ParkingSlot slotAssign=_allocationStrategy.AllocateSlot(vehicle,_parkingLevels.AsReadOnly<ParkingLevel>());
           if(slotAssign==null) return null;

            //Generate entry ticket and return it
            return new EntryTicket(DateTime.UtcNow.ToString("yyyyMMddHHmmss"),vehicle,slotAssign);
        }

        public ParkingBill ExitVehicle(
            EntryTicket ticket
        )
        {
            //unpark vehicle
           ticket.Slot.RemoveVehicle();

           //return bill
           return new ParkingBill(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),ticket,_pricingStrategies[ticket.Vehicle.Type].CalculateFare(ticket));

        }
    }
}

