using AutoMapper;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.Model.Auth;
using QStack.Framework.Core.Persistent;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Basic.Services.Auth
{
    [SessionInterceptor]
    public class GroupService:AbstractService<Group>,IGroupService
    {
        public GroupService(IMapper mapper)
        {
            Mapper = mapper;
        }
    }
}
