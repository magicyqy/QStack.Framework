﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QStack.Framework.AspNetCore.Plugin.Core
{
    public interface IReferenceContainer
    {
        List<CachedReferenceItemKey> GetAll();

        bool Exist(string name, string version);

        void SaveStream(string name, string version, Stream stream);

        Stream GetStream(string name, string version);
    }
}
