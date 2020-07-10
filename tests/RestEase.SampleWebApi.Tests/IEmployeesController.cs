using System.Threading.Tasks;
using RestEase.SampleWebApi.Contracts;

namespace RestEase.SampleWebApi.Tests
{
	[BasePath("Employees")]
	[SerializationMethods(Query = QuerySerializationMethod.Serialized)]
	public interface IEmployeesController
	{
		[Get("bulk")]
		Task<BulkRequest> GetEmployeeByRequest([Query] BulkRequest bulkRequest);

		[Delete("deleteEmployeeFromQuery")]
		Task<Employee> DeleteEmployeeFromQuery([Query] Employee employee);

		[Get("byEnumFromQuery")]
		Task<MyEnum> GetByEnumFromQuery([Query] MyEnum myEnum);

		[Get("ByEnumFromRequest")]
		Task<MyEnum> GetByEnumFromRequest([Query] EnumRequest myEnumRequest);
	}
}