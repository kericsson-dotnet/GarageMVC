﻿using GarageMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GarageMVC.Controllers
{
    public class GarageController : Controller
    {
        private readonly GarageContext _context;
        private static readonly int fixedParkNumber = 60;
        private int car;
        private int truck;
        private int bus;
        private int motorcycle;
        private int airplan;

        public GarageController(GarageContext context)
        {
            _context = context;
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


        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["VehicleTypeSort"] = string.IsNullOrEmpty(sortOrder) ? "vehicleType_desc" : "";
            ViewData["RegNumberSort"] = sortOrder == "regNumber" ? "regNumber_desc" : "regNumber";
            ViewData["ColorSort"] = sortOrder == "color" ? "color_desc" : "color";
            ViewData["MakeSort"] = sortOrder == "make" ? "make_desc" : "make";
            ViewData["ModelSort"] = sortOrder == "model" ? "model_desc" : "model";
            ViewData["NumberOfWheelsSort"] = sortOrder == "numberOfWheels" ? "numberOfWheels_desc" : "numberOfWheels";
            ViewData["CheckInTimeSort"] = sortOrder == "checkInTime" ? "checkInTime_desc" : "checkInTime";
            int parkNumber = CalcEmptyPark();
            ViewData["Airplan"] = parkNumber / 3;
            ViewData["Bus"] = parkNumber / 2;
            ViewData["Car"] = parkNumber;

            ViewData["CarAmount"] = car;
            ViewData["TruckAmount"] = truck;
            ViewData["BusAmount"] = bus;
            ViewData["MotorcycleAmount"] = motorcycle;
            ViewData["AirplanAmount"] = airplan;

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

        public IActionResult Unpark()
        {
            RegNumberList();
            return View();
        }

        // GET: Garage/Create
        public IActionResult Create()
        {
            return View();
        }

        public ActionResult RegNumberList()
        {
            var vehicles = _context.ParkedVehicle.ToList();

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

        public IActionResult UnparkReceipt(int id)
        {
            ViewBag.SelectedId = id;
            CalculateParkingDuration(id);
            return View();
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

                ViewBag.Days = days;
                ViewBag.Hours = hours;
                ViewBag.Minutes = minutes;
            }
        }


        [HttpPost]
        public ActionResult SeedVehicles()
        {
            var seed = new[]
            {
                new ParkedVehicle {
                    VehicleType = VehicleType.Car, RegNumber = "ABC123", Color = "Red", Make = "Reliant", Model = "Robin", NumberOfWheels = 3, CheckInTime = DateTime.Now.AddHours(-1)
                },
                new ParkedVehicle {
                    VehicleType = VehicleType.Bus, RegNumber = "DEF456", Color = "Black", Make = "Scania", Model = "Citywide", NumberOfWheels = 8, CheckInTime = DateTime.Now.AddHours(-4)
                },
                new ParkedVehicle {
                    VehicleType = VehicleType.Car, RegNumber = "ZXY666", Color = "Yellow", Make = "Pagani", Model = "Zonda", NumberOfWheels = 4, CheckInTime = DateTime.Now.AddHours(-8)
                },
            };

            foreach (var vehicle in seed)
            {
                // Seeda inte fordonet om det redan finns ett parkerat fordon i db med samma regnummer.
                if (!_context.ParkedVehicle.Any(v => v.RegNumber == vehicle.RegNumber))
                {
                    _context.ParkedVehicle.Add(vehicle);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: Garage/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VehicleType,RegNumber,Color,Make,Model,NumberOfWheels")] ParkedVehicle parkedVehicle)
        {
            TempData["Message"] = "";

            if (ModelState.IsValid && !ParkedVehicleExistsByReg(parkedVehicle.RegNumber))
            {
                _context.Add(parkedVehicle);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Vehicle with registration number " + parkedVehicle.RegNumber + " parked successfully!";
                string test = parkedVehicle.VehicleType.ToString();
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,VehicleType,RegNumber,Color,Make,Model,NumberOfWheels,ParkingTime,IsParked")] ParkedVehicle parkedVehicle)
        {
            if (id != parkedVehicle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(parkedVehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParkedVehicleExists(parkedVehicle.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(parkedVehicle);
        }

        // GET: Garage/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        public IActionResult Search(string searchValue)
        {
            List<ParkedVehicle> searchVehicles = new List<ParkedVehicle>();
            foreach (var vehicle in _context.ParkedVehicle)
            {
                string text = vehicle.VehicleType.ToString().ToLower();
                if (text.Contains(searchValue.ToLower())) searchVehicles.Add(vehicle);
            }
            searchVehicles.AddRange(_context.ParkedVehicle.Where(v => v.RegNumber.Contains(searchValue)));
            searchVehicles.AddRange(_context.ParkedVehicle.Where(v => v.Color.Contains(searchValue)));
            searchVehicles.AddRange(_context.ParkedVehicle.Where(v => v.Make.Contains(searchValue)));
            searchVehicles.AddRange(_context.ParkedVehicle.Where(v => v.Model.Contains(searchValue)));
            searchVehicles.AddRange(_context.ParkedVehicle.Where(v => v.NumberOfWheels.ToString().Contains(searchValue)));
            var searchList = searchVehicles.Distinct().ToArray();
            return View("Index", searchList);
        }

        // POST: Garage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete2(int id)
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
                if (typeVehicle.Equals("Car")) car = car + 1;
                else if (typeVehicle.Equals("Motorcycle")) motorcycle = motorcycle + 1;
                else if (typeVehicle.Equals("Bus")) bus = bus + 1;
                else if (typeVehicle.Equals("Truck")) truck = truck + 1;
                else airplan = airplan + 1;
            }
            return fixedParkNumber - car - motorcycle - (bus * 2) - (truck * 2) - (airplan * 3);
        }



        private bool ParkedVehicleExists(int id)
        {
            return _context.ParkedVehicle.Any(e => e.Id == id);
        }

        private bool ParkedVehicleExistsByReg(String RegNo)
        {
            return _context.ParkedVehicle.Any(v => v.RegNumber == RegNo);
        }
    }
}
