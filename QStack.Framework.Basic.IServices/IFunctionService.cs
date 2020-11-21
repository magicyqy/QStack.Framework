using QStack.Framework.Basic.ViewModel.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.IServices
{
    public interface IFunctionService : IBaseService
    {
       
        Task<bool> Delete(int id);
        Task<List<FunctionDto>> GetByRoles(params int[] roldeId);
    }
}
