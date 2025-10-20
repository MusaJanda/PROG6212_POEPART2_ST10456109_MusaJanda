using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMCS.Controllers;
using CMCS.Models;
using CMCS.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CMCS.Tests
{
    public class AdminControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            // Setup mock UserManager
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            // Setup mock RoleManager
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                roleStore.Object, null, null, null, null);

            _controller = new AdminController(_mockUserManager.Object, _mockRoleManager.Object);

            // Setup admin user for authorization
            var user = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new System.Security.Claims.Claim[]
                {
                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.NameIdentifier, "admin-user-id"),
                    new System.Security.Claims.Claim(
                        System.Security.Claims.ClaimTypes.Role, "Admin")
                }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task Index_ReturnsViewWithUsers()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "1", Email = "user1@test.com" },
                new ApplicationUser { Id = "2", Email = "user2@test.com" }
            };

            // Create a mock that supports async operations
            var mockSet = new Mock<DbSet<ApplicationUser>>();
            mockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(users.AsQueryable().Provider);
            mockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(users.AsQueryable().Expression);
            mockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(users.AsQueryable().ElementType);
            mockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(users.AsQueryable().GetEnumerator());
            mockSet.As<IAsyncEnumerable<ApplicationUser>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<ApplicationUser>(users.GetEnumerator()));

            _mockUserManager.Setup(x => x.Users).Returns(mockSet.Object);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<ApplicationUser>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task ManageRoles_WithValidUserId_ReturnsView()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", Email = "test@test.com" };
            _mockUserManager.Setup(x => x.FindByIdAsync("1"))
                          .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                          .ReturnsAsync(new List<string> { "Lecturer" });

            var roles = new List<IdentityRole>
            {
                new IdentityRole("Lecturer"),
                new IdentityRole("ProgrammeCoordinator")
            };

            _mockRoleManager.Setup(x => x.Roles)
                          .Returns(roles.AsQueryable());

            // Act
            var result = await _controller.ManageRoles("1");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ManageUserRolesViewModel>(viewResult.Model);
            Assert.Equal("1", model.UserId);
            Assert.Equal("test@test.com", model.Email);
        }

        [Fact]
        public async Task ManageRoles_WithInvalidUserId_ReturnsNotFound()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByIdAsync("999"))
                          .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.ManageRoles("999");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateRoles_WithValidData_RedirectsToIndex()
        {
            // Arrange
            var user = new ApplicationUser { Id = "1", Email = "test@test.com" };
            _mockUserManager.Setup(x => x.FindByIdAsync("1"))
                          .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                          .ReturnsAsync(new List<string> { "OldRole" });
            _mockUserManager.Setup(x => x.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                          .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                          .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.UpdateRoles("1", new[] { "Lecturer", "ProgrammeCoordinator" });

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }

    // Add this helper class for async operations
    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public T Current => _inner.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}