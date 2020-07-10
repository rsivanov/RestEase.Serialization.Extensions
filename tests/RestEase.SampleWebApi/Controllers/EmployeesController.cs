using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestEase.SampleWebApi.Contracts;

namespace RestEase.SampleWebApi.Controllers
{
	[Route("[controller]")]
	public class EmployeesController : Controller
	{
		[HttpGet("bulk")]
		public Task<BulkRequest> GetEmployeeByRequest([FromQuery] BulkRequest bulkRequest) =>
			Task.FromResult(bulkRequest);

		[HttpDelete("deleteEmployeeFromQuery")]
		public Task<Employee> DeleteEmployeeFromQuery([FromQuery] Employee employee) => Task.FromResult(employee);

		[HttpGet("byEnumFromQuery")]
		public Task<MyEnum> GetByEnumFromQuery([FromQuery] MyEnum myEnum) => Task.FromResult(myEnum);

		[HttpGet("ByEnumFromRequest")]
		public Task<MyEnum> GetByEnumFromRequest([FromQuery] EnumRequest myEnumRequest) => Task.FromResult(myEnumRequest.MyEnum);
	}
}