using Bongo.Core.Services;
using Bongo.DataAccess.Repository.IRepository;
using Bongo.Models.Model;
using Bongo.Models.Model.VM;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bongo.Core
{
    [TestFixture]
    public class StudyRoomBookingServiceTests
    {
        //Only thing we can test in this file is to check whether the getAll method is invoked or not.
        private StudyRoomBooking _request;
        //List of available study rooms
        private List<StudyRoom> _availableStudyRoom;
        private Mock<IStudyRoomBookingRepository> _studyRoomBookingRepoMock;
        private Mock<IStudyRoomRepository> _studyRoomRepoMock;
        private StudyRoomBookingService _bookingService;
        //Instantiating above objects.

        [SetUp]
        public void Setup()
        {
            //Adding dummy values for the availabe study room.
            _request = new StudyRoomBooking
            {
                FirstName = "Ben",
                LastName = "Spark",
                Email = "ben@gmail.com",
                Date = new DateTime(2022, 1, 1)

            };
            //Adding atlest one study room in availableStudy room.
            _availableStudyRoom = new List<StudyRoom>()
            {
                new StudyRoom
                {
                    Id = 10,
                    RoomName = "Michigan",
                    RoomNumber = "A202"
                }
            };
            _studyRoomBookingRepoMock = new Mock<IStudyRoomBookingRepository>();
            _studyRoomRepoMock = new Mock<IStudyRoomRepository>();
            //Get all method in studyroomrepositroy returns all the available rooms therefore.
            _studyRoomRepoMock.Setup(x => x.GetAll()).Returns(_availableStudyRoom);
            //To resolve not null here error add .object
            _bookingService = new StudyRoomBookingService(_studyRoomBookingRepoMock.Object, _studyRoomRepoMock.Object);
        }

        [Test]
        public void GetAllBooking_InvokedMedhod_CheckIfRepoIsCalled()
        {
            _bookingService.GetAllBooking();
            //Verify if the method has been called atleast once or once.
            _studyRoomBookingRepoMock.Verify(x => x.GetAll(null), Times.Once);
        }

        [Test]
        public void BookingException_NullRequest_ThrowsException()
        {
            //check if null exception is thrown when the method is invoked.
            var exception = Assert.Throws<ArgumentNullException>(
                // BookStudyRoom expects a request so we send it null for testing
                () => _bookingService.BookStudyRoom(null));

            //If you want to be specific about the exception like the string it will throw then do this.
            Assert.AreEqual("Value cannot be null. (Parameter 'request')", exception.Message);

            //If you want to just check for the parameter that is failing.
            Assert.AreEqual("request", exception.ParamName);
        }

        //Unit test for a successful booking.
        [Test]
        // We will need booking request and list of available rooms.
        public void StudyRoomBooking_SaveBookingWithAvailableRoom_ReturnsResultWithAllValues()
        {
            //Whenever booking is invoked we pass saved studyroombooking.
            // Here we are assigning value as null because savedStudyroombooking will give error at the one of the assert below.
            StudyRoomBooking savedStudyRoomBooking = null;
            _studyRoomBookingRepoMock.Setup(x => x.Book(It.IsAny<StudyRoomBooking>()))
                .Callback<StudyRoomBooking>(booking =>
                {
                    savedStudyRoomBooking = booking;
                });

            //Act
            _bookingService.BookStudyRoom(_request);

            //Assert
            // This must be invoked once.
            _studyRoomBookingRepoMock.Verify(x => x.Book(It.IsAny<StudyRoomBooking>()), Times.Once);
            //Here we will get error that savedstudyroombooking is not null therefore above we assign the value null.
            Assert.NotNull(savedStudyRoomBooking);
            //Request property must match with the received studyroombooking.
            Assert.AreEqual(_request.FirstName, savedStudyRoomBooking.FirstName);
            Assert.AreEqual(_request.LastName, savedStudyRoomBooking.LastName);
            Assert.AreEqual(_request.Date, savedStudyRoomBooking.Date);
            Assert.AreEqual(_request.Email, savedStudyRoomBooking.Email);
            //Id we can get from _availableStudyroom.
            Assert.AreEqual(_availableStudyRoom.First().Id, savedStudyRoomBooking.StudyRoomId);
        }

        [Test]
        public void StudyRoomBookingResultCheck_InputRequest_ValuesMatchInResult()
        {
            StudyRoomBookingResult result = _bookingService.BookStudyRoom(_request);
            Assert.NotNull(result);

            Assert.AreEqual(_request.FirstName, result.FirstName);
            Assert.AreEqual(_request.LastName, result.LastName);
            Assert.AreEqual(_request.Date, result.Date);
            Assert.AreEqual(_request.Email, result.Email);
        }

        //If rooms are not available the return code should be success.
        //here (true) testcase is room availability.
        [TestCase(true, ExpectedResult = StudyRoomBookingCode.Success)]
        [TestCase(false, ExpectedResult = StudyRoomBookingCode.NoRoomAvailable)]
        // Note :- here the return type is studyroombookingcode.
        public StudyRoomBookingCode ResultCodeSuccess_RoomAvailability_ReturnsSuccessResultCode(bool roomavailable)
        {
            // StudyRoomBookingCode is nothing but a enum that can be checked in StudyRoomBookingService.
            if (!roomavailable)
            {
                _availableStudyRoom.Clear();
            }
            return _bookingService.BookStudyRoom(_request).Code;

            // we don't need this line of code because we are using testcase.
            //Assert.AreEqual(StudyRoomBookingCode.Success, result.Code);
        }
}
}
