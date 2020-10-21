using QStack.Framework.Basic.IServices;
using System;
using QStack.Framework.Basic.ViewModel.Auth;
using System.Threading.Tasks;
using QStack.Framework.Basic.Enum;
using System.Collections.Generic;

namespace QStack.Framework.Basic.IServices
{
  
    public  interface IUserService:IBaseService
    {
        string Test();
       
        Task TestMutipleTrans(UserDto userDto);
        Task<UserDto> GetUserRoles(int id);
      
        Task UpdateState(int id,UserState state);
        Task<UserDto> Update(UserDto dto);
        Task UpdateUserRoles(int id,List<UserRoleDto> userRoles);
        Task<UserDto> GetUserFunctions(int id);
   
    }
}
