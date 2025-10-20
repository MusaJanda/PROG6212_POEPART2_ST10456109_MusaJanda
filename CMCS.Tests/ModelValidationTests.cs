using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Xunit;

namespace CMCS.Tests
{
    public class ModelValidationTests
    {
        [Fact]
        public void Claim_WithValidData_ShouldPassValidation()
        {
            // Arrange - Use fully qualified name
            var claim = new CMCS.Models.Claim
            {
                ClaimDate = System.DateTime.Now,
                HoursWorked = 10,
                HourlyRate = 50.00m,
                Description = "Valid claim description"
            };

            var validationContext = new ValidationContext(claim);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(claim, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void Claim_WithInvalidHours_ShouldFailValidation()
        {
            // Arrange - Use fully qualified name
            var claim = new CMCS.Models.Claim
            {
                ClaimDate = System.DateTime.Now,
                HoursWorked = 300, // Invalid - exceeds maximum
                HourlyRate = 50.00m,
                Description = "Test claim"
            };

            var validationContext = new ValidationContext(claim);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(claim, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Single(validationResults);
        }

        [Fact]
        public void Claim_WithInvalidHourlyRate_ShouldFailValidation()
        {
            // Arrange - Use fully qualified name
            var claim = new CMCS.Models.Claim
            {
                ClaimDate = System.DateTime.Now,
                HoursWorked = 10,
                HourlyRate = 600.00m, // Invalid - exceeds maximum
                Description = "Test claim"
            };

            var validationContext = new ValidationContext(claim);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(claim, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Single(validationResults);
        }

        [Fact]
        public void Lecturer_WithValidData_ShouldPassValidation()
        {
            // Arrange
            var lecturer = new CMCS.Models.Lecturer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@university.com",
                HourlyRate = 45.50m
            };

            var validationContext = new ValidationContext(lecturer);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(lecturer, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void CreateClaimViewModel_WithValidData_ShouldPassValidation()
        {
            // Arrange
            var model = new CMCS.ViewModels.CreateClaimViewModel
            {
                ClaimDate = System.DateTime.Now,
                HoursWorked = 10,
                HourlyRate = 50.00m,
                Description = "Valid claim description"
            };

            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }
    }
}