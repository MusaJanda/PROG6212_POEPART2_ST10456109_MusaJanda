using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CMCS.Data;
using CMCS.Models;
using CMCS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    [Authorize]
    public class ClaimController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClaimController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Claim/Create
        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateClaimViewModel
            {
                ClaimDate = DateTime.Today // Set default to today
            };
            return View(model);
        }

        // POST: Claim/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateClaimViewModel model, List<IFormFile> documents)
        {
            Console.WriteLine("=== CLAIM SUBMISSION STARTED ===");

            if (ModelState.IsValid)
            {
                try
                {
                    Console.WriteLine("ModelState is valid");

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    Console.WriteLine($"User ID: {userId}");

                    var lecturer = await _context.Lecturer.FirstOrDefaultAsync(l => l.UserId == userId);
                    Console.WriteLine($"Lecturer found: {lecturer != null}");

                    if (lecturer == null)
                    {
                        ModelState.AddModelError("", "Lecturer record not found. Please contact administrator.");
                        Console.WriteLine("Lecturer not found error");
                        return View(model);
                    }

                    // Create the actual Claim entity from the ViewModel
                    var claim = new Models.Claim
                    {
                        LecturerId = lecturer.LecturerId,
                        ClaimDate = model.ClaimDate,
                        HoursWorked = model.HoursWorked,
                        HourlyRate = model.HourlyRate,
                        Description = model.Description,
                        Department = model.Department,
                        CreatedDate = DateTime.Now,
                        Status = ClaimStatus.Pending
                        // Don't set ManagerNotes, CoordinatorNotes, or approval fields - they stay null
                    };

                    Console.WriteLine($"Claim data - Date: {claim.ClaimDate}, Hours: {claim.HoursWorked}, Rate: {claim.HourlyRate}");

                    _context.Claims.Add(claim);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Claim saved with ID: {claim.ClaimId}");

                    // Handle file uploads
                    if (documents != null && documents.Count > 0)
                    {
                        Console.WriteLine($"Processing {documents.Count} documents");

                        foreach (var file in documents)
                        {
                            if (file.Length > 0 && file.Length <= 10 * 1024 * 1024) // 10MB limit
                            {
                                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "documents", fileName);

                                // Ensure directory exists
                                var directory = Path.GetDirectoryName(filePath);
                                if (!Directory.Exists(directory))
                                {
                                    Directory.CreateDirectory(directory);
                                }

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                var document = new Document
                                {
                                    ClaimId = claim.ClaimId,
                                    FileName = file.FileName,
                                    FilePath = fileName,
                                    ContentType = file.ContentType,
                                    FileSize = file.Length,
                                    UploadedDate = DateTime.Now
                                };
                                _context.Documents.Add(document);
                                Console.WriteLine($"Document saved: {file.FileName}");
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    TempData["Success"] = "Claim submitted successfully!";
                    Console.WriteLine("=== CLAIM SUBMISSION SUCCESS ===");
                    return RedirectToAction("Index", "Dashboard");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"=== ERROR: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    ModelState.AddModelError("", $"An error occurred while submitting your claim: {ex.Message}");
                }
            }
            else
            {
                // Log validation errors
                Console.WriteLine("=== MODEL STATE ERRORS ===");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    if (errors.Count > 0)
                    {
                        Console.WriteLine($"Field: {key}");
                        foreach (var error in errors)
                        {
                            Console.WriteLine($"  - {error.ErrorMessage}");
                        }
                    }
                }
            }

            return View(model);
        }

        // GET: Claim/MyClaims
        [HttpGet]
        public async Task<IActionResult> MyClaims()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lecturer = await _context.Lecturer.FirstOrDefaultAsync(l => l.UserId == userId);

            if (lecturer == null)
            {
                return View("Error", new ErrorViewModel { ErrorMessage = "Lecturer data not found." });
            }

            var claims = await _context.Claims
                .Where(c => c.LecturerId == lecturer.LecturerId)
                .Include(c => c.Documents)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();

            return View(claims);
        }

        // GET: Claim/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var claim = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.ClaimId == id);

            if (claim == null)
            {
                return NotFound();
            }

            // Verify ownership
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lecturer = await _context.Lecturer.FirstOrDefaultAsync(l => l.UserId == userId);

            if (claim.LecturerId != lecturer.LecturerId && !User.IsInRole("ProgrammeCoordinator") && !User.IsInRole("AcademicManager"))
            {
                return Forbid();
            }

            return View(claim);
        }

        // GET: Claim/Review/5 - For Programme Coordinators and Academic Managers
        [Authorize(Roles = "ProgrammeCoordinator,AcademicManager")]
        public async Task<IActionResult> Review(int id)
        {
            var claim = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.ClaimId == id);

            if (claim == null)
            {
                return NotFound();
            }

            return View(claim);
        }

        // POST: Claim/ApproveByCoordinator
        [HttpPost]
        [Authorize(Roles = "ProgrammeCoordinator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveByCoordinator(int id, string coordinatorNotes, bool isApproved)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var coordinator = await _context.ProgrammeCoordinator.FirstOrDefaultAsync(p => p.UserId == userId);

            if (coordinator == null)
            {
                return Forbid();
            }

            if (isApproved)
            {
                claim.Status = ClaimStatus.ApprovedByCoordinator;
                claim.ApprovedByCoordinatorId = coordinator.CoordinatorId;
                claim.CoordinatorApprovalDate = DateTime.Now;
                claim.CoordinatorNotes = coordinatorNotes;
                TempData["Success"] = "Claim approved successfully!";
            }
            else
            {
                claim.Status = ClaimStatus.Rejected;
                claim.CoordinatorNotes = coordinatorNotes;
                claim.CoordinatorApprovalDate = DateTime.Now;
                TempData["Success"] = "Claim rejected successfully!";
            }

            _context.Claims.Update(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard");
        }
        // POST: Claim/ApprovedByManager
        [HttpPost]
        [Authorize(Roles = "AcademicManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveByManager(int id, string managerNotes, bool isApproved, string returnToCoordinator)
        {
            // Debug output to see what's being received
            Console.WriteLine($"=== MANAGER ACTION ===");
            Console.WriteLine($"Claim ID: {id}");
            Console.WriteLine($"IsApproved: {isApproved}");
            Console.WriteLine($"ReturnToCoordinator: {returnToCoordinator}");
            Console.WriteLine($"ManagerNotes: {managerNotes}");

            var claim = await _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.ApprovedByCoordinator)
                .FirstOrDefaultAsync(c => c.ClaimId == id);

            if (claim == null)
            {
                return NotFound();
            }

            // Check if this is a return to coordinator action
            bool isReturnToCoordinator = returnToCoordinator?.ToLower() == "true";

            if (isApproved)
            {
                claim.Status = ClaimStatus.FullyApproved;
                claim.ApprovedByManagerId = (await _context.AcademicManager.FirstOrDefaultAsync(a => a.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)))?.ManagerId;
                claim.ManagerApprovalDate = DateTime.Now;
                claim.ManagerNotes = managerNotes;
                TempData["Success"] = "Claim fully approved! Payment can now be processed.";
            }
            else if (isReturnToCoordinator)
            {
                claim.Status = ClaimStatus.ReturnedToCoordinator;
                claim.ManagerNotes = managerNotes;
                claim.ManagerApprovalDate = DateTime.Now;
                TempData["Success"] = "Claim returned to coordinator for revision.";
            }
            else
            {
                claim.Status = ClaimStatus.RejectedByManager;
                claim.ManagerNotes = managerNotes;
                claim.ManagerApprovalDate = DateTime.Now;
                TempData["Success"] = "Claim rejected!";
            }

            _context.Claims.Update(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard");
        }


        // GET: Claim/Download/5 - File download
        [Authorize]
        public async Task<IActionResult> Download(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "documents", document.FilePath);
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, document.ContentType, document.FileName);
        }

        // GET: Claim/Edit/5 - For lecturers to edit their pending claims
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var claim = await _context.Claims
                .Include(c => c.Lecturer)
                .FirstOrDefaultAsync(c => c.ClaimId == id);

            if (claim == null)
            {
                return NotFound();
            }

            // Verify the claim belongs to the current user and is still pending
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var lecturer = await _context.Lecturer.FirstOrDefaultAsync(l => l.UserId == userId);

            if (claim.LecturerId != lecturer.LecturerId)
            {
                return Forbid();
            }

            // Only allow editing of pending or returned claims
            if (claim.Status != ClaimStatus.Pending && claim.Status != ClaimStatus.ReturnedToCoordinator)
            {
                TempData["Error"] = "You can only edit claims that are pending or returned for revision.";
                return RedirectToAction("MyClaims");
            }

            // Convert Claim to CreateClaimViewModel
            var model = new CreateClaimViewModel
            {
                ClaimDate = claim.ClaimDate,
                HoursWorked = claim.HoursWorked,
                HourlyRate = claim.HourlyRate,
                Description = claim.Description
            };

            // Pass the claim ID to the view
            ViewData["ClaimId"] = claim.ClaimId;

            return View(model);
        }

        // POST: Claim/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateClaimViewModel model)
        {
            Console.WriteLine($"=== EDIT CLAIM STARTED ===");
            Console.WriteLine($"Claim ID: {id}");

            if (ModelState.IsValid)
            {
                try
                {
                    var claim = await _context.Claims.FindAsync(id);
                    if (claim == null)
                    {
                        return NotFound();
                    }

                    // Verify ownership and status
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var lecturer = await _context.Lecturer.FirstOrDefaultAsync(l => l.UserId == userId);

                    if (claim.LecturerId != lecturer.LecturerId)
                    {
                        return Forbid();
                    }

                    // Only allow editing of pending or returned claims
                    if (claim.Status != ClaimStatus.Pending && claim.Status != ClaimStatus.ReturnedToCoordinator)
                    {
                        TempData["Error"] = "You can only edit claims that are pending or returned for revision.";
                        return RedirectToAction("MyClaims");
                    }

                    // If claim was returned to coordinator, reset status to pending after edit
                    if (claim.Status == ClaimStatus.ReturnedToCoordinator)
                    {
                        claim.Status = ClaimStatus.Pending;
                        claim.ManagerNotes = null; // Clear manager notes since we're making changes
                        Console.WriteLine("Reset claim status from ReturnedToCoordinator to Pending");
                    }

                    // Update claim fields
                    claim.ClaimDate = model.ClaimDate;
                    claim.HoursWorked = model.HoursWorked;
                    claim.HourlyRate = model.HourlyRate;
                    claim.Description = model.Description;
                    claim.CreatedDate = DateTime.Now; // Update the timestamp

                    Console.WriteLine($"Updated claim data - Date: {claim.ClaimDate}, Hours: {claim.HoursWorked}, Rate: {claim.HourlyRate}");

                    _context.Claims.Update(claim);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Claim updated successfully!";
                    Console.WriteLine("=== CLAIM UPDATE SUCCESS ===");
                    return RedirectToAction("MyClaims");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"=== ERROR: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    ModelState.AddModelError("", $"An error occurred while updating your claim: {ex.Message}");
                }
            }
            else
            {
                // Log validation errors
                Console.WriteLine("=== MODEL STATE ERRORS ===");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    if (errors.Count > 0)
                    {
                        Console.WriteLine($"Field: {key}");
                        foreach (var error in errors)
                        {
                            Console.WriteLine($"  - {error.ErrorMessage}");
                        }
                    }
                }
            }

            // If we got here, something went wrong - return to edit view
            ViewData["ClaimId"] = id;
            return View(model);
        }
    }
}