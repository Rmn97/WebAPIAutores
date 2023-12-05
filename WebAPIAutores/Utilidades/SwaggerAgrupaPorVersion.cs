using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebAPIAutores.Utilidades
{
    public class SwaggerAgrupaPorVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var nameSpaceControlador = controller.ControllerType.Namespace;

            var versionApi = nameSpaceControlador.Split('.').Last().ToLower();

            controller.ApiExplorer.GroupName = versionApi;
        }
    }
}
