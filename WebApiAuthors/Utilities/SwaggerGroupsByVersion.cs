using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApiAuthors.Utilities
{
    public class SwaggerGroupsByVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var nameSpaceController = controller.ControllerType.Namespace; // this will return the name of the controller: "Controller.v1"
            var versionApi = nameSpaceController.Split(".").Last().ToLower(); //this will return the name of the controller: "v1"
            controller.ApiExplorer.GroupName = versionApi;
        }
    }
}
