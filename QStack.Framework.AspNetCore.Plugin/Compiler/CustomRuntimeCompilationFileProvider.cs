﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
{
    internal class CustomRuntimeCompilationFileProvider
    {
        private readonly MvcRazorRuntimeCompilationOptions _options;
        private IFileProvider _compositeFileProvider;

        public CustomRuntimeCompilationFileProvider(IOptions<MvcRazorRuntimeCompilationOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;
        }

        public IFileProvider FileProvider
        {
            get
            {
                if (_compositeFileProvider == null)
                {
                    _compositeFileProvider = GetCompositeFileProvider(_options);
                }

                return _compositeFileProvider;
            }
        }

        private static IFileProvider GetCompositeFileProvider(MvcRazorRuntimeCompilationOptions options)
        {
            var fileProviders = options.FileProviders;
            if (fileProviders.Count == 0)
            {
                var message = string.Format("'{0}.{1}' must not be empty. At least one '{2}' is required to locate a view for rendering.",
                    typeof(MvcRazorRuntimeCompilationOptions).FullName,
                    nameof(MvcRazorRuntimeCompilationOptions.FileProviders),
                    typeof(IFileProvider).FullName);
                throw new InvalidOperationException(message);
            }
            else if (fileProviders.Count == 1)
            {
                return fileProviders[0];
            }

            return new CompositeFileProvider(fileProviders);
        }
    }
}
