using QStack.Framework.Basic.ViewModel.Auth;
using QStack.Framework.Core.Transaction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.IServices
{
    public interface IFunctionService : IBaseService
    {
       
        Task<bool> Delete(int id);
        Task<List<FunctionDto>> GetByRoles(params int[] roldeId);
    }
}
