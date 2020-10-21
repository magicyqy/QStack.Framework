using AutoMapper;
using QStack.Framework.Basic.Enum;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.Model.Auth;
using QStack.Framework.Basic.ViewModel.Auth;
using QStack.Framework.Core.Persistent;
using QStack.Framework.Core.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace QStack.Framework.Basic.Services.Auth
{
    [SessionInterceptor]

    public class UserService : AbstractService<User>, IUserService
    {
        IRoleService roleService;
        public UserService(IMapper mapper, IRoleService roleService)
        {
          
            Mapper = mapper;
            this.roleService = roleService;
        }
        public UserService(IDaoCollection daos, IMapper mapper)
        {
            Daos = daos;
            Mapper = mapper;
        }

       
        [IgnoreQueryFilters]
        public async Task<UserDto> GetUserRoles(int id)
        {
            
            var users = await Daos.CurrentDao.QueryInclude<User>(u => u.Id == id,u=>u.UserRoles.Select(r=>r.Role),u=>u.Group);
            if (users.Count == 0)
                return null;
            var user = users.First();
            return Mapper.Map<UserDto>(user);
        }
        [IgnoreQueryFilters]
        public async Task<UserDto> GetUserFunctions(int id)
        {

         
            var user = await GetUserRoles(id);
            if (user == null)
                return user;
          

            var roleIds = user.Roles.Select(r => r.Id);
            var isAdmin = user.Roles.Any(r => "admin".Equals(r.Name));
            var data = await Daos.CurrentDao.QueryInclude<RoleFunction>(r => isAdmin|| roleIds.Contains(r.RoleId), r => r.Function);

            user.Functions = Mapper.Map<List<FunctionDto>>(data.Select(i => i.Function).Distinct().ToList());
            return user;
           
        }
        public string Test()
        {
            return $"test {this.GetType().FullName}";
        }
        [TransactionInterceptor]
        public async Task TestMutipleTrans(UserDto userDto)
        {
            User entity = Mapper.Map<User>(userDto);
            User entity1 = Mapper.Map<User>(userDto);
            if (entity.Id>0)
            {
                await this.DeleteByIdAsync(entity.Id);
            }
            entity.LastModifyDate = DateTime.Now;
            await this.AddAsync(entity);

            await Daos["sfdb1"].AddAsync(entity1);

            await roleService.AddAsync(new RoleDto { Name = "role01", Code = "001", CreateDate = DateTime.Now });
            throw new Exception("ss");
        }
        [TransactionInterceptor]
        public  async Task<UserDto> Update(UserDto dto)
        {
            var users = await Daos.CurrentDao.QueryInclude<User>(u=>u.Id==dto.Id,u=>u.UserRoles);
            if (users == null || users.Count == 0)
                return null;
            var user = users.First();
            user = Mapper.Map<UserDto, User>(dto, user);

            user= await Daos.CurrentDao.AddOrUpdate<User,int>(user);

            return Mapper.Map<UserDto>(user);
        }

        [TransactionInterceptor]
        public async Task UpdateState(int id, UserState state)
        {
            await Daos.CurrentDao.Update<User>(u => u.Id == id, u => new User { State = state });
        }

        public async Task UpdateUserRoles(int id,List<UserRoleDto> userRoles)
        {
            await Daos.CurrentDao.DeleteAsync<UserRole>(i => i.UserId == id);
            //await Daos.CurrentDao.Flush();
            if (userRoles != null)
            {
                var newValues = Mapper.Map<List<UserRole>>(userRoles);
  
                await Daos.CurrentDao.AddRangeAsync(newValues);

            }
            
        }
    }
}
