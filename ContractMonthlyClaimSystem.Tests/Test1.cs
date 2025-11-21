using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Tests
{
    [TestClass]
    public sealed class Test1
    {
      
            [TestMethod]
            public void TotalAmount_ShouldCalculateCorrectly()
            {
                // Arrange
                var claim = new Claim
                {
                    HoursWorked = 40,
                    HourlyRate = 600
                };

                // Act
                var totalAmount = claim.TotalAmount;

                // Assert
                Assert.AreEqual(24000, totalAmount);
            }

            [TestMethod]
            public void TotalAmount_WithZeroHours_ShouldReturnZero()
            {
                // Arrange
                var claim = new Claim
                {
                    HoursWorked = 0,
                    HourlyRate = 600
                };

                // Act
                var totalAmount = claim.TotalAmount;

                // Assert
                Assert.AreEqual(0, totalAmount);
            }

            [TestMethod]
            public void DefaultStatus_ShouldBePending()
            {
                // Arrange & Act
                var claim = new Claim();

                // Assert
                Assert.AreEqual("Pending", claim.Status);
            }

            [TestMethod]
            public void DefaultSubmissionDate_ShouldBeRecent()
            {
                // Arrange & Act
                var claim = new Claim();
                var timeDifference = DateTime.Now - claim.SubmissionDate;

                // Assert
                Assert.IsTrue(timeDifference.TotalSeconds < 5);
            }

            [TestMethod]
            public void Properties_ShouldBeSetCorrectly()
            {
                // Arrange
                var testDate = new DateTime(2025, 1, 1);

                // Act
                var claim = new Claim
                {
                    ClaimId = 1,
                    LecturerName = "Test Lecturer",
                    ClaimMonth = testDate,
                    HoursWorked = 40,
                    HourlyRate = 600,
                    Notes = "Test notes",
                    Status = "Pending",
                    FileName = "test.pdf"
                };

                // Assert
                Assert.AreEqual(1, claim.ClaimId);
                Assert.AreEqual("Test Lecturer", claim.LecturerName);
                Assert.AreEqual(testDate, claim.ClaimMonth);
                Assert.AreEqual(40, claim.HoursWorked);
                Assert.AreEqual(600, claim.HourlyRate);
                Assert.AreEqual("Test notes", claim.Notes);
                Assert.AreEqual("Pending", claim.Status);
                Assert.AreEqual("test.pdf", claim.FileName);
            }
        }
    }


