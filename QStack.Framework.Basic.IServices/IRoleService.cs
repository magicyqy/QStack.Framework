﻿using QStack.Framework.Basic.ViewModel.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.IServices
{
    public interface IRoleService : IBaseService
    {
     
        Task<bool> Delete(int id);
        Task<List<RoleFunctionDto>> GetPermissions(int id);
        Task<bool> HasCascadeData(int roldeId);
        Task UpdateRolePermission(int id,List<RoleFunctionDto> permissions);
    }
}
