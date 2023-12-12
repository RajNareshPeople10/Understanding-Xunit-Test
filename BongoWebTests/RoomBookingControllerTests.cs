using Bongo.Core.Services.IServices;
using Bongo.Models.Model;
using Bongo.Models.Model.VM;
using Bongo.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BongoWebTests
{
    [TestFixture]
    public class RoomBookingControllerTests
    {
        //we need to mock the Istudyroombookingservice.
        private Mock<IStudyRoomBookingService> _studyRoomBookingService;
        // if we want to invoke something we will have to create object.
        private RoomBookingController _bookingController;

        [SetUp]
        public void Setup()
        {
            _studyRoomBookingService = new Mock<IStudyRoomBookingService>();
            _bookingController = new RoomBookingController(_studyRoomBookingService.Object);
        }

        [Test]
        public void IndexPage_CallRequest_VerifyGetAllInvoked()
        {
            _bookingController.Index();
            _studyRoomBookingService.Verify(x => x.GetAllBooking(), Times.Once);
        }
        //This is testing a http post.
        [Test]
        public void BookRoomCheck_ModelStateInvalid_ReturnView()
        {
            _bookingController.ModelState.AddModelError("test", "test");
            //because book method expects of type studyroombooking.
            var result = _bookingController.Book(new StudyRoomBooking());
            //converting to view result
            ViewResult viewResult = result as ViewResult;
            Assert.AreEqual("Book", viewResult.ViewName);
        }

        // if the result code is no room available.
        [Test]
        public void BookRoomCheck_NotSuccessful_NoRoomCode()
        {
            _studyRoomBookingService.Setup(x => x.BookStudyRoom(It.IsAny<StudyRoomBooking>()))
                .Returns(new StudyRoomBookingResult()
                {
                    Code = StudyRoomBookingCode.NoRoomAvailable
                }) ;
            //Invoking book method.
            var result = _bookingController.Book(new StudyRoomBooking());
            // How to check if the type of result we received is of the type ViewResult.
            Assert.IsInstanceOf<ViewResult>(result);
            // Checking if the error message we receive is same as the error message we have provided.
            ViewResult viewResult = result as ViewResult;
            Assert.AreEqual("No Study Room available for selected date", viewResult.ViewData["Error"]);
        }

        [Test]
        public void BookRoomCheck_Successful_SuccessCodeAndRedirect()
        {
            //Arrange
            _studyRoomBookingService.Setup(x => x.BookStudyRoom(It.IsAny<StudyRoomBooking>()))
                .Returns((StudyRoomBooking booking) => new StudyRoomBookingResult()
                {
                    Code = StudyRoomBookingCode.Success,
                    FirstName = booking.FirstName,
                    LastName = booking.LastName,
                    Date = booking.Date,
                    Email = booking.Email
                });
            //Act
            var result = _bookingController.Book(new StudyRoomBooking()
            {
                Date = DateTime.Now, 
                Email = "hello@naresh.com",
                FirstName = "Hello",
                LastName = "DotNetMastery",
                StudyRoomId = 1
            });
            //Assert
            // Assert that return type will be redirect to action.
            // How to check if the type of result we received is of the type ViewResult.
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            // Checking if the error message we receive is same as the error message we have provided.
            RedirectToActionResult actionResult = result as RedirectToActionResult;
            //Assereting that the first name is Hello.
            // routeValues is the one we use to check the data of this.
            Assert.AreEqual("Hello", actionResult.RouteValues["FirstName"]);
            Assert.AreEqual(StudyRoomBookingCode.Success, actionResult.RouteValues["Code"]);
        }
    }

}
