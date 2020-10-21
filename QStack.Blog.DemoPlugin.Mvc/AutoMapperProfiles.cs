using AutoMapper;
using QStack.Blog.DemoPlugin.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStack.Blog.DemoPlugin.Mvc
{
    public class TestModelProfiles:Profile
    {
        public TestModelProfiles()
        {
            base.CreateMap<TestModel, TestModelDto>().ReverseMap();
        }
    }
}
