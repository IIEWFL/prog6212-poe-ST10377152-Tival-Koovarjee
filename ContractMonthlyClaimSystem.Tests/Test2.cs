using ContractMonthlyClaimSystem.Controllers;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractMonthlyClaimSystem.Tests
{
    internal class Test2
    {
        private ApplicationDbContext _context;
        private Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private ClaimsController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockWebHostEnvironment.Setup(m => m.WebRootPath).Returns("wwwroot");

            _controller = new ClaimsController(_context, _mockWebHostEnvironment.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task Index_ReturnsViewWithAllClaims()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult.Model);
            var model = viewResult.Model as IEnumerable<Claim>;
            Assert.AreEqual(3, model.Count());
        }

        [TestMethod]
        public void Submit_Get_ReturnsView()
        {
            // Act
            var result = _controller.Submit();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Submit_Post_ValidClaim_RedirectsToIndex()
        {
            // Arrange
            var newClaim = new Claim
            {
                LecturerName = "New Test Lecturer",
                ClaimMonth = new DateTime(2025, 4, 1),
                HoursWorked = 38,
                HourlyRate = 550,
                Notes = "New test claim"
            };

            // Act
            var result = await _controller.Submit(newClaim, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [TestMethod]
        public async Task Submit_Post_InvalidClaim_ReturnsView()
        {
            // Arrange
            var invalidClaim = new Claim
            {
                // Missing required fields
                LecturerName = "",
                HoursWorked = -5, // Invalid hours
                HourlyRate = -100 // Invalid rate
            };
            _controller.ModelState.AddModelError("LecturerName", "Required");

            // Act
            var result = await _controller.Submit(invalidClaim, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Review_ReturnsOnlyPendingClaims()
        {
            // Act
            var result = await _controller.Review();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            var model = viewResult.Model as IEnumerable<Claim>;
            Assert.AreEqual(1, model.Count()); // Only one pending claim in test data
            Assert.IsTrue(model.All(c => c.Status == "Pending"));
        }

        [TestMethod]
        public async Task Approve_ValidId_ChangesStatusToApproved()
        {
            // Arrange
            var claimId = 1; // This claim has status "Pending" in test data

            // Act
            var result = await _controller.Approve(claimId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var updatedClaim = await _context.Claims.FindAsync(claimId);
            Assert.AreEqual("Approved", updatedClaim.Status);
        }

        [TestMethod]
        public async Task Approve_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidId = 999;

            // Act
            var result = await _controller.Approve(invalidId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Reject_ValidId_ChangesStatusToRejected()
        {
            // Arrange
            var claimId = 1; // This claim has status "Pending" in test data

            // Act
            var result = await _controller.Reject(claimId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var updatedClaim = await _context.Claims.FindAsync(claimId);
            Assert.AreEqual("Rejected", updatedClaim.Status);
        }

        [TestMethod]
        public async Task Details_ValidId_ReturnsViewWithClaim()
        {
            // Arrange
            var claimId = 1;

            // Act
            var result = await _controller.Details(claimId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            var model = viewResult.Model as Claim;
            Assert.IsNotNull(model);
            Assert.AreEqual(claimId, model.ClaimId);
        }

        [TestMethod]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidId = 999;

            // Act
            var result = await _controller.Details(invalidId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Details_NullId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Details(null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
   

