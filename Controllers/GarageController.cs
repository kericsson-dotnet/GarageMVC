﻿using System;
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
        public async Task<IActionResult> Index()
        {
            return View(await _context.ParkedVehicle.ToListAsync());
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
            ViewBag.RegNumberSelectList = new SelectList(vehicles, "Id", "RegNumber");

            ViewBag.SelectedId = -1;

            return View();
        }

        [HttpPost]
        public ActionResult SeedVehicles()
        {
            var seed = new[]
            {
                new ParkedVehicle {
                    VehicleType = "Car", RegNumber = "ABC123", Color = "Red", Make = "Reliant", Model = "Robin", NumberOfWheels = 3, ParkingTime = DateTime.Now.AddHours(-1), IsParked = true
                },
                new ParkedVehicle {
                    VehicleType = "Bus", RegNumber = "DEF456", Color = "Black", Make = "Scania", Model = "Citywide", NumberOfWheels = 8, ParkingTime = DateTime.Now.AddHours(-4), IsParked = true
                },
                new ParkedVehicle {
                    VehicleType = "Car", RegNumber = "ZXY666", Color = "Yellow", Make = "Pagani", Model = "Zonda", NumberOfWheels = 4, ParkingTime = DateTime.Now.AddHours(-8), IsParked = true
                },
            };

            _context.ParkedVehicle.AddRange(seed);
            _context.SaveChanges();

            return RedirectToAction("Index");
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
