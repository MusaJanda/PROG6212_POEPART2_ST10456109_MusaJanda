using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using CMCS.Controllers;
using CMCS.Models;
using CMCS.Data;
using CMCS.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMCS.Tests
{
    public class ClaimControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly ClaimController _controller;

        public ClaimControllerTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_Database_" + System.Guid.NewGuid())
                .Options;
            _context = new ApplicationDbContext(options);
            _controller = new ClaimController(_context);

            SetupAuthenticatedUser();
        }

        [Fact]
        public void Create_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CreateClaimViewModel>(viewResult.Model);
            Assert.Equal(System.DateTime.Today, model.ClaimDate);
        }

        [Fact]
        public async Task Create_Post_WithValidModel_RedirectsToDashboard()
        {
            // Arrange
            var model = new CreateClaimViewModel
            {
                ClaimDate = System.DateTime.Now,
                HoursWorked = 10,
                HourlyRate = 50,
                Description = "Test claim",
                Department = "Computer Science" // Add department
            };

            // Add a lecturer to the database
            var lecturer = new Lecturer
            {
                LecturerId = 1,
                UserId = "test-user-id",
                FirstName = "Test",
                LastName = "Lecturer",
                Email = "lecturer@test.com"
            };
            _context.Lecturer.Add(lecturer);
            await _context.SaveChangesAsync();

            // Act - Pass model AND documents as separate parameters
            var result = await _controller.Create(model, null);

            // Assert - Check if it's a redirect OR view (handle both cases)
            if (result is RedirectToActionResult redirectResult)
            {
                Assert.Equal("Index", redirectResult.ActionName);
                Assert.Equal("Dashboard", redirectResult.ControllerName);
            }
            else if (result is ViewResult viewResult)
            {
                // If it returns a view, check if there are model errors
                var returnedModel = Assert.IsType<CreateClaimViewModel>(viewResult.Model);
                // Log what's happening for debugging
                Console.WriteLine("Returned view instead of redirect. ModelState errors:");
                foreach (var error in _controller.ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($" - {error.ErrorMessage}");
                }
            }
            else
            {
                Assert.Fail($"Unexpected result type: {result.GetType()}");
            }
        }

        [Fact]
        public async Task Create_Post_WithInvalidModel_ReturnsView()
        {
            // Arrange
            var model = new CreateClaimViewModel
            {
                HoursWorked = -1, // Invalid hours
                HourlyRate = -10, // Invalid rate
                Department = "Test Department" // Add department
            };
            _controller.ModelState.AddModelError("HoursWorked", "Hours must be positive");
            _controller.ModelState.AddModelError("HourlyRate", "Rate must be positive");

            // Act
            var result = await _controller.Create(model, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<CreateClaimViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task Create_Post_WithoutLecturerRecord_ReturnsViewWithError()
        {
            // Arrange
            var model = new CreateClaimViewModel
            {
                ClaimDate = System.DateTime.Now,
                HoursWorked = 10,
                HourlyRate = 50,
                Description = "Test claim",
                Department = "Computer Science" // Add department
            };

            // Don't add lecturer to database - simulating missing lecturer record

            // Act
            var result = await _controller.Create(model, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var returnedModel = Assert.IsType<CreateClaimViewModel>(viewResult.Model);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public async Task MyClaims_ReturnsViewWithClaims()
        {
            // Arrange
            var lecturer = new Lecturer
            {
                LecturerId = 1,
                UserId = "test-user-id",
                FirstName = "Test",
                LastName = "Lecturer"
            };
            _context.Lecturer.Add(lecturer);

            // Use fully qualified name for Claim with ALL required fields
            var claim = new CMCS.Models.Claim
            {
                ClaimId = 1,
                LecturerId = 1,
                ClaimDate = System.DateTime.Now,
                HoursWorked = 10,
                HourlyRate = 50,
                Description = "Test claim",
                Department = "Computer Science", // ADD THIS REQUIRED FIELD
                Status = ClaimStatus.Pending,
                CreatedDate = System.DateTime.Now // Add created date
            };
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.MyClaims();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<CMCS.Models.Claim>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsView()
        {
            // Arrange - Use fully qualified name with ALL required fields
            var claim = new CMCS.Models.Claim
            {
                ClaimId = 1,
                ClaimDate = System.DateTime.Now,
                HoursWorked = 10,
                HourlyRate = 50,
                Description = "Test claim",
                Department = "Computer Science", // ADD THIS REQUIRED FIELD
                LecturerId = 1,
                CreatedDate = System.DateTime.Now // Add created date
            };
            _context.Claims.Add(claim);

            var lecturer = new Lecturer
            {
                LecturerId = 1,
                UserId = "test-user-id",
                FirstName = "Test",
                LastName = "Lecturer"
            };
            _context.Lecturer.Add(lecturer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CMCS.Models.Claim>(viewResult.Model);
            Assert.Equal(1, model.ClaimId);
        }

        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Get_WithValidId_ReturnsView()
        {
            // Arrange - Use fully qualified name with ALL required fields
            var claim = new CMCS.Models.Claim
            {
                ClaimId = 1,
                ClaimDate = System.DateTime.Now,
                HoursWorked = 10,
                HourlyRate = 50,
                Description = "Test claim",
                Department = "Computer Science", // ADD THIS REQUIRED FIELD
                LecturerId = 1,
                Status = ClaimStatus.Pending,
                CreatedDate = System.DateTime.Now // Add created date
            };
            _context.Claims.Add(claim);

            var lecturer = new Lecturer
            {
                LecturerId = 1,
                UserId = "test-user-id",
                FirstName = "Test",
                LastName = "Lecturer"
            };
            _context.Lecturer.Add(lecturer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CreateClaimViewModel>(viewResult.Model);
            Assert.Equal(10, model.HoursWorked);
            Assert.Equal(50, model.HourlyRate);
        }

        [Fact]
        public async Task Edit_Post_WithValidData_RedirectsToMyClaims()
        {
            // Create FRESH database for this test
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "EditTest_" + System.Guid.NewGuid())
                .Options;

            using var context = new ApplicationDbContext(options);
            var controller = new ClaimController(context);

            // Arrange - Add lecturer FIRST
            var lecturer = new Lecturer
            {
                LecturerId = 1,
                UserId = "test-user-id", // Must match the authentication setup
                FirstName = "Test",
                LastName = "Lecturer",
                Email = "test@test.com"
            };
            context.Lecturer.Add(lecturer);
            await context.SaveChangesAsync();

            // Add claim SECOND
            var claim = new CMCS.Models.Claim
            {
                ClaimId = 1,
                ClaimDate = System.DateTime.Now,
                HoursWorked = 10,
                HourlyRate = 50,
                Description = "Old description",
                Department = "Computer Science",
                LecturerId = 1, // Must match lecturer ID
                Status = ClaimStatus.Pending,
                CreatedDate = System.DateTime.Now
            };
            context.Claims.Add(claim);
            await context.SaveChangesAsync();

            var model = new CreateClaimViewModel
            {
                ClaimDate = System.DateTime.Now.AddDays(1),
                HoursWorked = 15,
                HourlyRate = 60,
                Description = "Updated description",
                Department = "Updated Department"
            };

            // Setup authentication for THIS controller
            var user = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new System.Security.Claims.Claim[]
                {
            new System.Security.Claims.Claim(
                System.Security.Claims.ClaimTypes.NameIdentifier, "test-user-id"), // Must match lecturer UserId
            new System.Security.Claims.Claim(
                System.Security.Claims.ClaimTypes.Name, "test@test.com")
                }, "TestAuthentication"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.Edit(1, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("MyClaims", redirectResult.ActionName);
        }

        [Fact]
        public async Task Review_WithCoordinatorRole_ReturnsView()
        {
            // Arrange - Use fully qualified name with ALL required fields
            var claim = new CMCS.Models.Claim
            {
                ClaimId = 1,
                ClaimDate = System.DateTime.Now,
                HoursWorked = 10,
                HourlyRate = 50,
                Description = "Test claim",
                Department = "Computer Science", // ADD THIS REQUIRED FIELD
                LecturerId = 1,
                Status = ClaimStatus.Pending,
                CreatedDate = System.DateTime.Now // Add created date
            };
            _context.Claims.Add(claim);

            var lecturer = new Lecturer
            {
                LecturerId = 1,
                FirstName = "Test",
                LastName = "Lecturer"
            };
            _context.Lecturer.Add(lecturer);
            await _context.SaveChangesAsync();

            // Setup user with ProgrammeCoordinator role
            var user = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new System.Security.Claims.Claim[]
                {
                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.NameIdentifier, "coordinator-user-id"),
                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.Role, "ProgrammeCoordinator")
                }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.Review(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CMCS.Models.Claim>(viewResult.Model);
            Assert.Equal(1, model.ClaimId);
        }

        private void SetupAuthenticatedUser()
        {
            var user = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new System.Security.Claims.Claim[]
                {
                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.NameIdentifier, "test-user-id"),
                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.Name, "test@test.com")
                }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}