using QStack.Blog.DemoPlugin.Mvc.Models;
using QStack.Framework.Basic.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.DemoPlugin.Mvc
{
    public interface ITestService:IBaseService
    {
        Task SaveMessage(TestModelDto dto);
        Task<TestModelDto> GetMessage();
    }
}
