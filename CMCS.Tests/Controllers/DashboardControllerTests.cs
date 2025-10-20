using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using CMCS.Controllers;
using CMCS.Models;
using CMCS.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMCS.Tests
{
    public class DashboardControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly DashboardController _controller;

        public DashboardControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_Dashboard_DB")
                .Options;
            _context = new ApplicationDbContext(options);
            _controller = new DashboardController(_context);
        }

        [Fact]
        public async Task Index_WithLecturerRole_ReturnsLecturerDashboard()
        {
            // Arrange - Use fully qualified names for System.Security.Claims.Claim
            var user = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new System.Security.Claims.Claim[]
                {
                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.NameIdentifier, "lecturer-user-id"),
                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.Role, "Lecturer")
                }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var lecturer = new Lecturer
            {
                LecturerId = 1,
                UserId = "lecturer-user-id",
                FirstName = "Test",
                LastName = "Lecturer"
            };
            _context.Lecturer.Add(lecturer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("LecturerDashboard", viewResult.ViewName);
        }

        [Fact]
        public async Task Index_WithProgrammeCoordinatorRole_ReturnsCoordinatorDashboard()
        {
            // Arrange - Use fully qualified names for System.Security.Claims.Claim
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
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("ProgrammeCoordinatorDashboard", viewResult.ViewName);
        }

        [Fact]
        public async Task Index_WithAcademicManagerRole_ReturnsManagerDashboard()
        {
            // Arrange - Use fully qualified names for System.Security.Claims.Claim
            var user = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new System.Security.Claims.Claim[]
                {
                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.NameIdentifier, "manager-user-id"),
                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.Role, "AcademicManager")
                }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("AcademicManagerDashboard", viewResult.ViewName);
        }

        [Fact]
        public async Task Index_WithNoRole_ReturnsUnauthorizedView()
        {
            // Arrange - Use fully qualified names for System.Security.Claims.Claim
            var user = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new System.Security.Claims.Claim[]
                {
                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.NameIdentifier, "basic-user-id")
                    // No roles assigned
                }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Unauthorized", viewResult.ViewName);
        }
    }
}