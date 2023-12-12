using Bongo.Models.ModelValidations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BongoModelTests
{
    [TestFixture]
    public class DateInFutureAttributeTests
    {
        [TestCase(100, ExpectedResult = true)]
        [TestCase(0, ExpectedResult = false)]
        [TestCase(-100, ExpectedResult = false)]
        public bool DateValidator_InputExpectedDateRange_DateValidity(int addTime)
        {
            DateInFutureAttribute dateInFutureAttribute = new(() => DateTime.Now);

            return dateInFutureAttribute.IsValid(DateTime.Now.AddSeconds(addTime));
        }

        //As soon as we create the attribute what is the error message assigned is tested in this unit test case.
        [Test]
        public void DateValidator_AnyDate_ReturnAnyMessage()
        {
            var result = new DateInFutureAttribute();
            // Expected comes first and then actual in the parameters.
            Assert.AreEqual("Date must be in the future", result.ErrorMessage);
        }
    }
}
