using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GarageMVC.Models;

namespace GarageMVC.Controllers
{
    public class GarageController : Controller
    {
        private readonly GarageContext _context;

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

        // GET: Garage
        //public async Task<IActionResult> Index( string sortOrder)
        //{
        //    ViewData["RegNumberSortParam"] = string.IsNullOrEmpty(sortOrder) ? "regNumber_desc" : "";
        //    ViewData["ColorSortParam"] = sortOrder == "color" ? "color_desc" : "color";
        //    ViewData["MakeSortParam"] = sortOrder == "make" ? "make_desc" : "make";

        //    var vehicles = from v in _context.ParkedVehicle
        //                   select v;

        //    switch (sortOrder)
        //    {
        //        case "regNumber_desc":
        //            vehicles = vehicles.OrderByDescending(v => v.RegNumber);
        //            break;
        //        case "color":
        //            vehicles = vehicles.OrderBy(v => v.Color);
        //            break;
        //        case "color_desc":
        //            vehicles = vehicles.OrderByDescending(v => v.Color);
        //            break;
        //        case "make":
        //            vehicles = vehicles.OrderBy(v => v.Make);
        //            break;
        //        case "make_desc":
        //            vehicles = vehicles.OrderByDescending(v => v.Make);
        //            break;
        //        default:
        //            vehicles = vehicles.OrderBy(v => v.RegNumber);
        //            break;
        //    }
        //    return View(await vehicles.ToListAsync());
        //}

        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["VehicleTypeSort"] = string.IsNullOrEmpty(sortOrder) ? "vehicleType_desc" : "";
            ViewData["RegNumberSort"] = sortOrder == "regNumber" ? "regNumber_desc" : "regNumber";
            ViewData["ColorSort"] = sortOrder == "color" ? "color_desc" : "color";
            ViewData["MakeSort"] = sortOrder == "make" ? "make_desc" : "make";
            ViewData["ModelSort"] = sortOrder == "model" ? "model_desc" : "model";
            ViewData["NumberOfWheelsSort"] = sortOrder == "numberOfWheels" ? "numberOfWheels_desc" : "numberOfWheels";
            ViewData["CheckInTimeSort"] = sortOrder == "checkInTime" ? "checkInTime_desc" : "checkInTime";

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

        // GET: Garage/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Garage/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VehicleType,RegNumber,Color,Make,Model,NumberOfWheels,ParkingTime,IsParked")] ParkedVehicle parkedVehicle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(parkedVehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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

        // POST: Garage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var parkedVehicle = await _context.ParkedVehicle.FindAsync(id);
            if (parkedVehicle != null)
            {
                _context.ParkedVehicle.Remove(parkedVehicle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ParkedVehicleExists(int id)
        {
            return _context.ParkedVehicle.Any(e => e.Id == id);
        }
    }
}
