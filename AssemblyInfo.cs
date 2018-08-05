using NRAKO_IvanCicek.AOP;

[assembly: ExceptionAspect(AttributeTargetTypes = "NRAKO_IvanCicek.Controllers.*")]
[assembly: ExceptionAspect(AttributeTargetTypes = "NRAKO_IvanCicek.DAL.*")]
//[assembly: RequestMetricsAspect(AttributeTargetTypes = "NRAKO_IvanCicek.Controllers.*")]
namespace NRAKO_IvanCicek
{
    public class AssemblyInfo
    {
    }
}