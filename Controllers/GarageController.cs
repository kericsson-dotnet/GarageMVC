﻿using GarageMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace GarageMVC.Controllers
{
    public class GarageController : Controller
    {
        private readonly GarageContext _context;
        private const int fixedParkNumber = 24;
        private int car;
        private int truck;
        private int bus;
        private int motorcycle;
        private int airplan;

        private const decimal hourlyRate = 1.23m;
        private readonly object lockObject = new object();
        private List<List<ParkedVehicle?>> GarageSlots;
        // Nytillagd variabel
        private bool IsDbEmpty;

        public GarageController(GarageContext context)
        {
            _context = context;

            IsDbEmpty = !_context.ParkedVehicle.Any();
            GarageSlots = new List<List<ParkedVehicle?>>(new List<ParkedVehicle?>[fixedParkNumber]);

            foreach (var vehicle in _context.ParkedVehicle)
            {
                for (int i = 0; i < vehicle.VehicleType.GetNumberOfSlots(); i++)
                {
                    if (GarageSlots[vehicle.Slot - 1 + i] == null)
                    {
                        GarageSlots[vehicle.Slot - 1 + i] = new List<ParkedVehicle?> { vehicle };
                    }
                    else
                    {
                        GarageSlots[vehicle.Slot - 1 + i].Add(vehicle);
                    }
                }
            }
        }


        public async Task<IActionResult> CheckInVehicle(string regNumber)
        {
            // Retrieve the vehicle from the database based on the vehicleId
            var parkedvehicle = await _context.ParkedVehicle.FirstOrDefaultAsync(v => v.RegNumber == regNumber);
            if (parkedvehicle == null)
            {
                return NotFound(); // Handle not found scenario
            }

            // Set the check-in time to the current time
            parkedvehicle.CheckInTime = DateTime.Now;

            // Update the vehicle entity in the database
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home"); // Redirect to the home page after check-in

        }

        // Method to calculate parking fee for a specific vehicle
        private decimal CalculateParkingFee(ParkedVehicle vehicle)
        {
            int occupiedSlots = vehicle.VehicleType.GetNumberOfSlots();
            if (vehicle.CheckInTime != DateTime.MinValue)
            {
                TimeSpan parkingDuration = DateTime.Now - vehicle.CheckInTime;
                double totalHours = Math.Ceiling(parkingDuration.TotalHours);

                decimal parkingFee = (decimal)totalHours * occupiedSlots * hourlyRate;

                return parkingFee;
            }

            // Default to 0 fee if checkInTime is null (or any other handling logic)
            return 0;
        }

        private decimal CalculateTotalParkingFees()
        {
            var parkedVehicles = _context.ParkedVehicle.ToList();
            decimal totalFees = 0;

            foreach (var vehicle in parkedVehicles)
            {
                totalFees += CalculateParkingFee(vehicle);
            }

            return totalFees;
        }

        // GarageController.cs
        public async Task<IActionResult> Statistics()
        {
            if (!IsDbEmpty)
            {
                var totalVehicles = await _context.ParkedVehicle.CountAsync();

                var totalWheels = await _context.ParkedVehicle.SumAsync(v => v.NumberOfWheels);

                var totalFees = CalculateTotalParkingFees();

                var durations = await _context.ParkedVehicle
                    .Select(v => (DateTime.Now - v.CheckInTime).TotalHours)
                    .ToListAsync();

                var longestDuration = durations.Max();
                var shortestDuration = durations.Min();

                // Create a view model to pass data to the view
                var viewModel = new StatisticsViewModel
                {
                    TotalParkedVehicles = totalVehicles,
                    TotalWheels = (int)totalWheels,
                    TotalFees = totalFees,
                    LongestDurationHours = longestDuration,
                    ShortestDurationHours = shortestDuration
                };
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewBag.IsDbEmpty = IsDbEmpty;
            ViewBag.GarageSlots = GarageSlots;
            ViewData["VehicleTypeSort"] = string.IsNullOrEmpty(sortOrder) ? "vehicleType_desc" : "";
            ViewData["RegNumberSort"] = sortOrder == "regNumber" ? "regNumber_desc" : "regNumber";
            ViewData["ColorSort"] = sortOrder == "color" ? "color_desc" : "color";
            ViewData["MakeSort"] = sortOrder == "make" ? "make_desc" : "make";
            ViewData["ModelSort"] = sortOrder == "model" ? "model_desc" : "model";
            ViewData["NumberOfWheelsSort"] = sortOrder == "numberOfWheels" ? "numberOfWheels_desc" : "numberOfWheels";
            ViewData["CheckInTimeSort"] = sortOrder == "checkInTime" ? "checkInTime_desc" : "checkInTime";
            int parkNumber = CalcEmptyPark();
            ViewData["Airplane"] = parkNumber / 3;
            ViewData["Bus"] = parkNumber / 2;
            ViewData["Car"] = parkNumber;

            ViewData["CarAmount"] = car;
            ViewData["TruckAmount"] = truck;
            ViewData["BusAmount"] = bus;
            ViewData["MotorcycleAmount"] = motorcycle;
            ViewData["AirplaneAmount"] = airplan;

            var vehicles = from v in _context.ParkedVehicle
                           select v;

            switch (sortOrder)
            {
                case "vehicleType_desc":
                    vehicles = vehicles.OrderByDescending(v => v.VehicleType);
                    break;
                case "regNumber":
                    vehicles = vehicles.OrderBy(v => v.RegNumber);
                    break;
                case "regNumber_desc":
                    vehicles = vehicles.OrderByDescending(v => v.RegNumber);
                    break;
                case "color":
                    vehicles = vehicles.OrderBy(v => v.Color);
                    break;
                case "color_desc":
                    vehicles = vehicles.OrderByDescending(v => v.Color);
                    break;
                case "make":
                    vehicles = vehicles.OrderBy(v => v.Make);
                    break;
                case "make_desc":
                    vehicles = vehicles.OrderByDescending(v => v.Make);
                    break;
                case "model":
                    vehicles = vehicles.OrderBy(v => v.Model);
                    break;
                case "model_desc":
                    vehicles = vehicles.OrderByDescending(v => v.Model);
                    break;
                case "numberOfWheels":
                    vehicles = vehicles.OrderBy(v => v.NumberOfWheels);
                    break;
                case "numberOfWheels_desc":
                    vehicles = vehicles.OrderByDescending(v => v.NumberOfWheels);
                    break;
                case "checkInTime":
                    vehicles = vehicles.OrderBy(v => v.CheckInTime);
                    break;
                case "checkInTime_desc":
                    vehicles = vehicles.OrderByDescending(v => v.CheckInTime);
                    break;
                default:
                    vehicles = vehicles.OrderBy(v => v.RegNumber); // Default sorting by registration number
                    break;
            }

            return View(await vehicles.ToListAsync());
        }


        // GET: Garage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkedVehicle = await _context.ParkedVehicle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parkedVehicle == null)
            {
                return NotFound();
            }

            return View(parkedVehicle);
        }

        // Lagt till validering för om garaget är tom eller inte.
        public IActionResult Unpark()
        {
            if (!IsDbEmpty)
            {
                RegNumberList();
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Garage/Create
        public IActionResult Create()
        {
            return View();
        }

        public ActionResult RegNumberList()
        {
            var vehicles = _context.ParkedVehicle.OrderBy(v => v.RegNumber).ToList();

            var selectList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Select your vehicle" }
            };

            foreach (var vehicle in vehicles)
            {
                // Visa bara Regnummer i dropdownlistan
                selectList.Add(new SelectListItem { Value = vehicle.Id.ToString(), Text = vehicle.RegNumber });
            }

            ViewBag.UnparkSelectList = new SelectList(selectList, "Value", "Text");

            ViewBag.SelectedId = -1;

            return View();
        }

        /*
            Lagt till validering så att användare måste välja ett fordon i dropdown listan för att gå vidare.
            Tidigare kunde knappen klickas på om default Select your vehicle hjälptexten vad vald.
        */
        public IActionResult UnparkReceipt(int id)
        {
            ViewBag.SelectedId = id;

            if (id == 0)
            {
                RegNumberList();
                return View("Unpark");
            }
            else
            {
                CalculateParkingDuration(id);
                return View();
            }
        }

        private void CalculateParkingDuration(int id)
        {
            var parkedVehicle = _context.ParkedVehicle.Find(id);
            if (parkedVehicle != null)
            {
                TimeSpan duration = DateTime.Now - parkedVehicle.CheckInTime;

                int days = duration.Days;
                int hours = duration.Hours;
                int minutes = duration.Minutes;

                // Lagt till pris och totalkostnad (decimal datatyp)
                //decimal hourlyRate = 1.23M;
                //decimal totalTimeInHours = (days * 24) + hours + ((decimal)minutes / 60);
                //string totalSum = (totalTimeInHours * hourlyRate).ToString("0.###");

                string totalSum = CalculateParkingFee(parkedVehicle).ToString("0.###");

                ViewBag.Days = days;
                ViewBag.Hours = hours;
                ViewBag.Minutes = minutes;
                ViewBag.TotalSum = totalSum;
            }
        }

        public IActionResult InventoryCount()
        {
            int inventoryCount = _context.ParkedVehicle.Count();
            return PartialView("_InventoryCount", inventoryCount);
        }

        [HttpPost]
        public async Task<ActionResult> SeedVehicles()
        {
            var seed = new[]
            {
                new ParkedVehicle {
                    VehicleType = VehicleType.Truck, RegNumber = "FLY007", Color = "Red with logo", Make = "SAAB", Model = "SkrotSaab 900", NumberOfWheels = 4, CheckInTime = DateTime.Now.AddHours(-27)
                },
                new ParkedVehicle {
                    VehicleType = VehicleType.Car, RegNumber = "ABC123", Color = "Red", Make = "Reliant", Model = "Robin", NumberOfWheels = 3, CheckInTime = DateTime.Now.AddHours(-17)
                },
                new ParkedVehicle {
                    VehicleType = VehicleType.Bus, RegNumber = "DEF456", Color = "Black", Make = "Scania", Model = "Citywide", NumberOfWheels = 8, CheckInTime = DateTime.Now.AddHours(-4)
                },
                new ParkedVehicle {
                    VehicleType = VehicleType.Car, RegNumber = "ZXY666", Color = "Yellow", Make = "Pagani", Model = "Zonda", NumberOfWheels = 4, CheckInTime = DateTime.Now.AddHours(-8)
                },
                new ParkedVehicle {
                    VehicleType = VehicleType.Motorcycle, RegNumber = "ZZZ111", Color = "Yellow", Make = "Kawasaki", Model = "Mupp", NumberOfWheels = 2, CheckInTime = DateTime.Now.AddHours(-25)
                },
                new ParkedVehicle {
                    VehicleType = VehicleType.Airplane, RegNumber = "SNE111", Color = "White", Make = "Airbus", Model = "A-320", NumberOfWheels = 8, CheckInTime = DateTime.Now.AddHours(-58)
                },
            };

            foreach (var vehicle in seed)
            {
                // Seeda inte fordonet om det redan finns ett parkerat fordon i db med samma regnummer.
                if (!_context.ParkedVehicle.Any(v => v.RegNumber == vehicle.RegNumber))
                {
                    var result = Create(vehicle).Result;
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult UnparkAllVehicles()
        {
            var vehicles = _context.ParkedVehicle.ToList();

            _context.ParkedVehicle.RemoveRange(vehicles);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // POST: Garage/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VehicleType,RegNumber,Color,Make,Model,NumberOfWheels")] ParkedVehicle parkedVehicle)
        {
            TempData["Message"] = "";
            parkedVehicle.Slot = ClaimNextAvailableSlot(parkedVehicle.VehicleType) + 1;
            if (parkedVehicle.Slot == 0)
            {
                TempData["Message"] = "Garage is full!";
                return View(parkedVehicle);
            }

            if (ModelState.IsValid && !ParkedVehicleExistsByReg(parkedVehicle.RegNumber))
            {
                _context.Add(parkedVehicle);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Vehicle with registration number " + parkedVehicle.RegNumber + " parked successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Message"] = "Vehicle with registration number " + parkedVehicle.RegNumber + " already parked!";
            }
            return View(parkedVehicle);
        }

        // GET: Garage/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parkedVehicle = await _context.ParkedVehicle.FindAsync(id);
            if (parkedVehicle == null)
            {
                return NotFound();
            }
            return View(parkedVehicle);
        }

        // POST: Garage/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VehicleType,RegNumber,Color,Make,Model,NumberOfWheels,CheckInTime")] ParkedVehicle parkedVehicle)
        {
            TempData["Message"] = "";
            if (id != parkedVehicle.Id)
            {
                return NotFound();
            }

            var existingVehicle = await _context.ParkedVehicle.FindAsync(id);
            if (existingVehicle == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingVehicle.VehicleType = parkedVehicle.VehicleType;
                    existingVehicle.RegNumber = parkedVehicle.RegNumber;
                    existingVehicle.Color = parkedVehicle.Color;
                    existingVehicle.Make = parkedVehicle.Make;
                    existingVehicle.Model = parkedVehicle.Model;
                    existingVehicle.NumberOfWheels = parkedVehicle.NumberOfWheels;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParkedVehicleExistsByReg(existingVehicle.RegNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Message"] = "Vehicle with registration number " + parkedVehicle.RegNumber + " updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(parkedVehicle);
        }

        public IActionResult Search(string searchValue)
        {
            List<ParkedVehicle> searchVehicles = new List<ParkedVehicle>();
            var allVehicles = _context.ParkedVehicle;
            ViewBag.IsDbEmpty = IsDbEmpty;
            ViewBag.GarageSlots = GarageSlots;
            if (searchValue != null)
            {

                foreach (var vehicle in _context.ParkedVehicle)
                {
                    string text = vehicle.VehicleType.ToString().ToLower();
                    if (text.Contains(searchValue.ToLower()))
                        searchVehicles.Add(vehicle);
                }
                searchVehicles.AddRange(_context.ParkedVehicle.Where(v => v.RegNumber.Contains(searchValue)));
                searchVehicles.AddRange(_context.ParkedVehicle.Where(v => v.Color.Contains(searchValue)));
                searchVehicles.AddRange(_context.ParkedVehicle.Where(v => v.Make.Contains(searchValue)));
                searchVehicles.AddRange(_context.ParkedVehicle.Where(v => v.Model.Contains(searchValue)));
                searchVehicles.AddRange(_context.ParkedVehicle.Where(v => v.NumberOfWheels.ToString().Contains(searchValue)));

            }
            else
            {
                searchVehicles = allVehicles.ToList();
            }
            return View("Index", searchVehicles.Distinct().ToArray());
        }

        // POST: Garage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var parkedVehicle = await _context.ParkedVehicle.FindAsync(id);
            if (parkedVehicle != null)
            {
                _context.ParkedVehicle.Remove(parkedVehicle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private int CalcEmptyPark()
        {
            foreach (var vehicle in _context.ParkedVehicle)
            {
                String typeVehicle = vehicle.VehicleType.ToString();
                if (typeVehicle.Equals("Car"))
                    car = car + 1;
                else if (typeVehicle.Equals("Motorcycle"))
                    motorcycle = motorcycle + 1;
                else if (typeVehicle.Equals("Bus"))
                    bus = bus + 1;
                else if (typeVehicle.Equals("Truck"))
                    truck = truck + 1;
                else
                    airplan = airplan + 1;
            }
            return fixedParkNumber - car - motorcycle - (bus * 2) - (truck * 2) - (airplan * 3);
        }

        private bool ParkedVehicleExistsByReg(String RegNo)
        {
            return _context.ParkedVehicle.Any(v => v.RegNumber == RegNo);
        }

        private int ClaimNextAvailableSlot(VehicleType vehicleType)
        {
            var requiredSlots = vehicleType.GetNumberOfSlots();
            lock (lockObject)
            {
                if (vehicleType == VehicleType.Motorcycle)
                {
                    // Check for any slot with motorcycles and less than 3 motorcycles
                    for (int i = 0; i < GarageSlots.Count; i++)
                    {
                        if (GarageSlots[i] != null && GarageSlots[i].Count < 3 && GarageSlots[i][0].VehicleType == VehicleType.Motorcycle)
                        {
                            GarageSlots[i].Add(new ParkedVehicle()); // Add the motorcycle to the slot
                            return i;
                        }
                    }
                }

                for (int i = 0; i <= GarageSlots.Count - requiredSlots; i++)
                {
                    // Check if there are enough null slots in a row
                    if (Enumerable.Range(i, requiredSlots).All(j => GarageSlots[j] == null))
                    {
                        for (int j = i; j < i + requiredSlots; j++)
                        {
                            GarageSlots[j] = new List<ParkedVehicle?> { new ParkedVehicle() }; // Reserve the slot
                        }
                        return i;
                    }
                }
            }

            return -1;
        }
    }
}
