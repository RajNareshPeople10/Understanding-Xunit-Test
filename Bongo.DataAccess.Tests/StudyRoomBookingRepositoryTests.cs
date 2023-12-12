using Bongo.DataAccess.Repository;
using Bongo.Models.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bongo.DataAccess
{
    [TestFixture]
    public class StudyRoomBookingRepositoryTests
    {
        private StudyRoomBooking studyRoomBooking_One;
        private StudyRoomBooking studyRoomBooking_Two;
        //Creating private variable for options
        private DbContextOptions<ApplicationDbContext> options;

        public StudyRoomBookingRepositoryTests() 
        {
            studyRoomBooking_One = new StudyRoomBooking()
            {
                FirstName = "Ben1",
                LastName = "Spark1",
                Date = new DateTime(2023, 1, 1),
                Email = "ben1@gmail.coom",
                BookingId = 11,
                StudyRoomId = 1
            };

            studyRoomBooking_Two = new StudyRoomBooking()
            {
                FirstName = "Ben2",
                LastName = "Spark2",
                Date = new DateTime(2023, 2, 2),
                Email = "ben2@gmail.coom",
                BookingId = 22,
                StudyRoomId = 2
            };
        }

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "temp_Bongo").Options;
        }

        [Test]
        //[Order(1)]
        public void SaveBooking_Booking_One_CheckTheValuesFromDatabase()
        {
            //arrange
            

            //act
            using (var context = new  ApplicationDbContext(options)) 
            {
                var repository = new StudyRoomBookingRepository(context);
                repository.Book(studyRoomBooking_One);

            }

            //checking whether the booking was done successfully or not
            //Assert
            using (var context = new ApplicationDbContext(options))
            {
                //first or default is used because this will be the first entry in our database.
                //for being specific we are adding u=> u.bookingId == 11 doing so we are specific which id to test.
                var bookingFromDb = context.StudyRoomBookings.FirstOrDefault();
                //comparing if this booking is exactly similar to the one we did before the above booking.
                //repeating for checking for other properties as well.
                Assert.AreEqual(studyRoomBooking_One.FirstName, bookingFromDb.FirstName);
                Assert.AreEqual(studyRoomBooking_One.LastName, bookingFromDb.LastName);
                Assert.AreEqual(studyRoomBooking_One.Email, bookingFromDb.Email);
                Assert.AreEqual(studyRoomBooking_One.Date, bookingFromDb.Date);
                Assert.AreEqual(studyRoomBooking_One.BookingId, bookingFromDb.BookingId);
                Assert.AreEqual(studyRoomBooking_One.StudyRoomId, bookingFromDb.StudyRoomId);

            }
        }

        [Test]
        // we can use order attribute to make one test cases to run in particular order.
        //[Order(2)]
        public void GetAllBooking_Booking_OneAndTwo_CheckBoththebookingsFromDatabase()
        {
            //arrange
            //What we need now is expected result
            var expectedResult = new List<StudyRoomBooking> { studyRoomBooking_One, studyRoomBooking_Two };

            using (var context = new ApplicationDbContext(options))
            {
                // we use ensure deleted to make sure everything in the database is deleted before new test case is executed.
                context.Database.EnsureDeleted();
                var repository = new StudyRoomBookingRepository(context);
                repository.Book(studyRoomBooking_One);
                repository.Book(studyRoomBooking_Two);

            }
            //act
            //Retrieve all of the bookings from the database.
            List<StudyRoomBooking> actualList;
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new StudyRoomBookingRepository(context);
                actualList = repository.GetAll(null).ToList();

            }
            
            //Compare acual list with the expected result.
            //Assert
            //To implement custom comparer we can add third parameter to this.
            CollectionAssert.AreEqual(expectedResult, actualList, new BookingCompare());
        }
        //Usage of Icomparer interface which is used to compare two objects.
        public class BookingCompare : IComparer
        {
            public int Compare(object? x, object? y)
            {
                var booking1 = (StudyRoomBooking)x;
                var booking2 = (StudyRoomBooking)y;
                if (booking1.BookingId != booking2.BookingId)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
