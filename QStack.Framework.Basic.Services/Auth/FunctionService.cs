using AutoMapper;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.Model.Auth;
using QStack.Framework.Basic.ViewModel.Auth;
using QStack.Framework.Core.Persistent;
using QStack.Framework.Core.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.Services.Auth
{
    [SessionInterceptor]
    public class FunctionService:AbstractService<Function>,IFunctionService
    {
        public FunctionService(IMapper mapper)
        {
            Mapper = mapper;
        }

        [TransactionInterceptor]
        public async Task<bool> Delete(int id)
        {
            var b = false;
            await Daos.CurrentDao.DeleteAsync<RoleFunction>(i => i.FunctionId == id);

            b =await Daos.CurrentDao.DeleteById<Function>(id);

            return b;
        }

        public async Task<List<FunctionDto>> GetByRoles(params int[] roldeIds)
        {
            if (roldeIds == null)
                return null;
            var data = await Daos.CurrentDao.QueryInclude<RoleFunction>(r => roldeIds.Contains(r.RoleId),r=>r.Function);

            return Mapper.Map<List<FunctionDto>>(data.Select(i => i.Function).ToList());
        }
    }
}
