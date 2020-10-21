using AutoMapper;
using QStack.Blog.DemoPlugin.Mvc.Models;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.Services;
using QStack.Framework.Basic.ViewModel.Auth;
using QStack.Framework.Core.Persistent;
using QStack.Framework.Core.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.DemoPlugin.Mvc
{
    [SessionInterceptor]
    public class TestService : AbstractService<TestModel>, ITestService
    {
        IUserService _userService;
        public TestService(IMapper mapper,IUserService userService)
        {
            Mapper = mapper;
            _userService = userService;
        }
        public async Task<TestModelDto> GetMessage()
        {
            var user = await _userService.Get<UserDto>(u => u.Name == "admin");

            return new TestModelDto { Message = user.Name };
        }
        [TransactionInterceptor]
        public async Task SaveMessage(TestModelDto dto)
        {
            var model = Mapper.Map<TestModel>(dto);
            await Daos.CurrentDao.AddOrUpdate<TestModel,int>(model);
        }
    }
}
