using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiAuthors.Controllers.v1;
using WebApiAuthors.Test.Mocks;

namespace WebApiAuthors.Test.UnitTests
{
    [TestClass]
    public class RootControllerTest
    {
        [TestMethod]
        public async Task IfUserIsAdminThenReturn4Links()
        {
            // Preparation:
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Result = AuthorizationResult.Success();
            var rootController = new RootController(authorizationService);

            rootController.Url = new URLHelperMock();
            // Execution:

            var result = await rootController.Get();

            // Check:
            Assert.AreEqual(4, result.Value.Count());
        }

        [TestMethod]
        public async Task IfUserIsNotAdminThenReturn2Links()
        {
            // Preparation:
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Result = AuthorizationResult.Failed();
            var rootController = new RootController(authorizationService);

            rootController.Url = new URLHelperMock();
            // Execution:

            var result = await rootController.Get();

            // Check:
            Assert.AreEqual(2, result.Value.Count());
        }
    }
}
