using AutoMapper;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.Model.Auth;
using QStack.Framework.Basic.ViewModel.Auth;
using QStack.Framework.Core.Persistent;
using QStack.Framework.Core.Transaction;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.Services.Auth
{
    [SessionInterceptor]
    public class RoleService : AbstractService<Role>, IRoleService
    {
        public RoleService(IMapper mapper)
        {

            Mapper = mapper;
        }

        public async Task<bool> HasCascadeData(int roleId)
        {
            return await Task.FromResult(Daos.CurrentDao.DbSet<UserRole>().Count(i => i.RoleId == roleId) > 0)
                || await Task.FromResult(Daos.CurrentDao.DbSet<RoleFunction>().Count(i => i.RoleId == roleId) > 0);
                   
        }

        [TransactionInterceptor]
       public async Task<bool> Delete(int id)
        {
            await Daos.CurrentDao.DeleteAsync<UserRole>(i => i.RoleId == id);
            await Daos.CurrentDao.DeleteAsync<RoleFunction>(i => i.RoleId == id);
            await Daos.CurrentDao.DeleteById<Role>(id);

            return true;
        }

        public async Task<List<RoleFunctionDto>> GetPermissions(int id)
        {
            var itmes=await Daos.CurrentDao.QueryInclude<RoleFunction>(i => i.RoleId == id,i=>i.Function);

            var result = Mapper.Map<List<RoleFunctionDto>>(itmes);
            return result;
        }

        [TransactionInterceptor]
        public async Task UpdateRolePermission(int id ,List<RoleFunctionDto> permissions)
        {
           
           
            await Daos.CurrentDao.DeleteAsync<RoleFunction>(i=>i.RoleId== id);
            //await Daos.CurrentDao.Flush();
            if (permissions != null)
            {
               var newPermissions = Mapper.Map<List<RoleFunction>>(permissions);

               await Daos.CurrentDao.AddRangeAsync(newPermissions);
            }
         

        }
    }
}
