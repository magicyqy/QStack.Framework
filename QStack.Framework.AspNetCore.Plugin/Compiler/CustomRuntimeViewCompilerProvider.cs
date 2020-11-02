// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
{
    internal class CustomRuntimeViewCompilerProvider : IViewCompilerProvider
    {
        private readonly RazorProjectEngine _razorProjectEngine;
        private readonly ApplicationPartManager _applicationPartManager;
        private readonly CustomCSharpCompiler _csharpCompiler;
        private readonly CustomRuntimeCompilationFileProvider _fileProvider;
        private readonly ILogger<CustomRuntimeViewCompiler> _logger;
        private readonly Func<IViewCompiler> _createCompiler;

        private object _initializeLock = new object();
        private bool _initialized;
        private IViewCompiler _compiler;

        public CustomRuntimeViewCompilerProvider(
            ApplicationPartManager applicationPartManager,
            RazorProjectEngine razorProjectEngine,
            CustomRuntimeCompilationFileProvider fileProvider,
            CustomCSharpCompiler csharpCompiler,
            ILoggerFactory loggerFactory)
        {
            _applicationPartManager = applicationPartManager;
            _razorProjectEngine = razorProjectEngine;
            _csharpCompiler = csharpCompiler;
            _fileProvider = fileProvider;

            _logger = loggerFactory.CreateLogger<CustomRuntimeViewCompiler>();
            _createCompiler = CreateCompiler;
        }

        public IViewCompiler GetCompiler()
        {
            return LazyInitializer.EnsureInitialized(
                ref _compiler,
                ref _initialized,
                ref _initializeLock,
                _createCompiler);
        }

        private IViewCompiler CreateCompiler()
        {
            var feature = new ViewsFeature();
            _applicationPartManager.PopulateFeature(feature);

            return new CustomRuntimeViewCompiler(_applicationPartManager,
                _fileProvider.FileProvider,
                _razorProjectEngine,
                _csharpCompiler,
                feature.ViewDescriptors,
                _logger);
        }
    }
}