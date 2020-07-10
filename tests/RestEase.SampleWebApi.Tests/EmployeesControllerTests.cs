using Microsoft.AspNetCore.Mvc.Testing;
using RestEase.Implementation;
using RestEase.SampleWebApi.Contracts;
using RestEase.Serialization.Extensions;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RestEase.SampleWebApi.Tests
{
	public class EmployeesControllerTests
	{
		private static readonly WebApplicationFactory<Startup> _factory = new WebApplicationFactory<Startup>();
		private readonly IEmployeesController _controller;

		public EmployeesControllerTests()
		{
			var client = _factory.CreateClient();
			var requester = new Requester(client)
			{
				RequestQueryParamSerializer = new ComplexTypeRequestQueryParamSerializer(),
			};
			_controller = RestClient.For<IEmployeesController>(requester);
		}

		[Fact]
		public async Task GetByEnumFromQuery_Success()
		{
			var response = await _controller.GetByEnumFromQuery(MyEnum.ExistingValue);
			Assert.Equal(MyEnum.ExistingValue, response);
		}

		[Fact]
		public async Task GetByEnumFromRequest_Success()
		{
			var enumRequest = new EnumRequest
			{
				MyEnum = MyEnum.ExistingValue
			};
			var response = await _controller.GetByEnumFromRequest(enumRequest);
			Assert.Equal(MyEnum.ExistingValue, response);
		}

		[Fact]
		public async Task GetEmployeeByRequest_Success()
		{
			var request = new BulkRequest { Ids = new[] { 1, 2, 3 }, Value = "Qwerty" };
			var response = await _controller.GetEmployeeByRequest(request);
			Assert.Equal(request.Value, response.Value);
			Assert.True(request.Ids.SequenceEqual(response.Ids));
		}

		[Fact]
		public async Task DeleteEmployeeFromQuery_Success()
		{
			var employee = new Employee
			{
				Id = 1,
				FullName = "John Doe"
			};
			var response = await _controller.DeleteEmployeeFromQuery(employee);
			Assert.Equal(employee.Id, response.Id);
			Assert.Equal(employee.FullName, response.FullName);
		}
	}
}
