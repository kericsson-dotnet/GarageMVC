using GarageMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
            //ViewBag.ShowButton = false;
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

        //public ActionResult ShowReceiptButton(int id)
        //{
        //    //CalculateParkingDuration(id);
        //    ViewBag.VehicleId = id;
        //    ViewBag.ShowButton = true;

        //    return View("Unpark");
        //}

        //private void CalculateParkingDuration(int id)
        //{
        //    var parkedVehicle = _context.ParkedVehicle.Find(id);
        //    if (parkedVehicle != null)
        //    {
        //        TimeSpan duration = DateTime.Now - parkedVehicle.CheckInTime;
        //        ViewBag.ParkingDuration = duration!;
        //    }
        //}
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

        private bool ParkedVehicleExists(int id)
        {
            return _context.ParkedVehicle.Any(e => e.Id == id);
        }
    }
}
