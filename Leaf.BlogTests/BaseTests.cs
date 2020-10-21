using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leaf.BlogTests
{
    public class BaseTests
    {
        public IHost host;
        [SetUp]
        public void Init()
        {

            host = InitTestEnv.Init();

        }
    }
}
