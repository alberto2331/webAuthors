using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Test.UnitTests
{
    [TestClass]
    public class FirstCapitalLetterAttributeTests
    {
        [TestMethod]
        public void FirstCapitalLetterReturnError()
        {
            //Preparation
            /*1) We instantiate the class that we are going to test. 
             * We instantiate it even though it is in another project*/
            var firstCapitalLetter = new FirstCapitalLetterAttribute();
            var value = "alberto";
            var context = new ValidationContext( new { Name = value });
            //Execution
            var result = firstCapitalLetter.GetValidationResult(value, context);

            //Check

            Assert.AreEqual("The first letter of this field must be capitalized", result.ErrorMessage);
        }

        [TestMethod]
        public void NullValueNotReturnError()
        {
            //Preparation
            var firstCapitalLetter = new FirstCapitalLetterAttribute();
            string value = null; //Shall be a string type var to accept "null";
            var context = new ValidationContext(new { Name = value });
            
            //Execution
            var result = firstCapitalLetter.GetValidationResult(value, context);

            //Check
            /*2) The different between the first case is in this case the 
             * method "GetValidationResult" returns a "null" when everithings is ok
             */
            Assert.IsNull(result);
        }

        [TestMethod]
        public void FirstCapitalLetterNotReturnError()
        {
            //Preparation
            /*1) We instantiate the class that we are going to test. 
             * We instantiate it even though it is in another project*/
            var firstCapitalLetter = new FirstCapitalLetterAttribute();
            var value = "Alberto";
            var context = new ValidationContext(new { Name = value });
            //Execution
            var result = firstCapitalLetter.GetValidationResult(value, context);

            //Check

            Assert.IsNull(result);
        }
    }
}