using AutoMapper;
using QStack.Framework.Basic.IServices;
using QStack.Framework.Basic.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace QStack.Framework.Basic.Services
{
    public class NavigationService:AbstractService<NavigationMenu>,INavigationService
    {
        public NavigationService(IMapper mapper)
        {
            Mapper = mapper;
        }
    }
}
