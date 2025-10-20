
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CMCS.Data;
using CMCS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (User.IsInRole("Lecturer"))
            {
                var lecturer = await _context.Lecturer.FirstOrDefaultAsync(l => l.UserId == userId);
                if (lecturer == null)
                {
                    return View("Error", new ErrorViewModel { ErrorMessage = "Lecturer data not found." });
                }

                var lecturerClaims = await _context.Claims
                    .Where(c => c.LecturerId == lecturer.LecturerId)
                    .Include(c => c.Documents)
                    .OrderByDescending(c => c.CreatedDate)
                    .Take(5)
                    .ToListAsync();

                ViewData["Claims"] = lecturerClaims;
                ViewData["User"] = lecturer;
                return View("LecturerDashboard");
            }

            if (User.IsInRole("ProgrammeCoordinator"))
            {
                // Show both PENDING and RETURNED claims to Programme Coordinators
                var coordinatorClaims = await _context.Claims
                    .Where(c => c.Status == ClaimStatus.Pending || c.Status == ClaimStatus.ReturnedToCoordinator)
                    .Include(c => c.Lecturer)
                    .Include(c => c.Documents)
                    .Include(c => c.ApprovedByManager) 
                    .OrderByDescending(c => c.Status == ClaimStatus.ReturnedToCoordinator) // Show returned claims first
                    .ThenBy(c => c.CreatedDate)
                    .ToListAsync();

                return View("ProgrammeCoordinatorDashboard", coordinatorClaims);
            }

            if (User.IsInRole("AcademicManager"))
            {
                var approvedByCoordinatorClaims = await _context.Claims
                    .Where(c => c.Status == ClaimStatus.ApprovedByCoordinator)
                    .Include(c => c.Lecturer)
                    .Include(c => c.Documents)
                    .OrderBy(c => c.CoordinatorApprovalDate)
                    .ToListAsync();

                return View("AcademicManagerDashboard", approvedByCoordinatorClaims);
            }

            return View("Unauthorized");
        }
    }
}
    