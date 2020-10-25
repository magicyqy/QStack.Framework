using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Hosting;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using QStack.Framework.Core.Persistent;
using QStack.Framework.Persistent.EFCore.External;
using Microsoft.Extensions.Logging;
using QStack.Framework.Core;

namespace QStack.Framework.Persistent.EFCore
{
    public class AutoMigration:IDisposable
	{
		ILogger<AutoMigration> _logger;
		private MigrationAssemblyLoadContext _assemblyLoadContext;
		private readonly IServiceProvider _serviceProvider;

		IHostEnvironment _env;

		readonly IEnumerable<IDaoFactory> daoFactories;

		private Dictionary<string,List<Assembly>> _providerAssemblies = new Dictionary<string, List<Assembly>>();

		readonly MigrationOptions _migrationOptions;

		public AutoMigration(IServiceProvider serviceProvider, IHostEnvironment env, MigrationOptions migrationOptions)
		{
			_logger = serviceProvider.GetService<ILogger<AutoMigration>>();
			_assemblyLoadContext = new MigrationAssemblyLoadContext();
			this._migrationOptions = migrationOptions;
			_serviceProvider = serviceProvider;
			
			_env = env;
			daoFactories = _serviceProvider.GetServices<IDaoFactory>();
			foreach(var item in daoFactories)
			{
				var factory = item as EFCoreDaoFactory;
				var extensions = factory.dbContextOptions.Extensions;
				var extension = extensions.FirstOrDefault(e => typeof(RelationalOptionsExtension).IsAssignableFrom(e.GetType()));
				if (extension == null)
					throw new ServiceFrameworkException($"DbContextOptions error,could not find RelationalOptionsExtension for {factory.FactoryName}");
				if (!_providerAssemblies.ContainsKey(factory.FactoryName))
				{
					_providerAssemblies.Add(factory.FactoryName, new List<Assembly>());

				}
				_providerAssemblies[factory.FactoryName].Add(extension.GetType().Assembly);
			}
		}

		private EFCoreDao CreateDao(EFCoreDaoFactory factory, string assembly)
		{
			
			assembly = assembly ?? Assembly.GetEntryAssembly().GetName().Name;
			return (EFCoreDao)factory.CreateDao(
				options => 
 				{
					if (!string.IsNullOrWhiteSpace(assembly))
					{
						var extensions = factory.dbContextOptions.Extensions;
						var extension = extensions.FirstOrDefault(e => typeof(RelationalOptionsExtension).IsAssignableFrom(e.GetType()));
						extension = ((RelationalOptionsExtension)extension).WithMigrationsAssembly(assembly);
						var builder = new DbContextOptionsBuilder<EFCoreDao>();
						var newOptions= builder.Options.WithExtension(extension);
						return newOptions;
					}
					return options;
				},
				//强制输入sessioncontext的环境变量，让dbcontext刷新OnModelCreating
				new SessionContext(_serviceProvider) 
			);
		}
		public void GenerateMigrations(bool ignoreFK=true,Action<IServiceCollection> designTimeServicesAction=null)
		{
	
			foreach (var item   in daoFactories)
			{
				var factory = item as EFCoreDaoFactory;
				if (!factory.DaoFactoryOption.EnableAutoMigration)
					continue;
				_logger.LogInformation($"start migrations of  {item.FactoryName} ...");
				//操作前强制备份数据库
				item.Backup(_migrationOptions).ConfigureAwait(false).GetAwaiter().GetResult();
				string accemblyName = null;
				var basePath = Path.Combine(_env.ContentRootPath, _migrationOptions.MigrationPath);
				if (!Directory.Exists(basePath))
				{
					Directory.CreateDirectory(basePath);
				}
				var path = Path.Combine(basePath, $"{factory.FactoryName}");
				var migrationName = $"{DateTimeOffset.Now.ToString("yyyyMMddHHmmss")}_{factory.FactoryName}.Migrations";
				var rootNameSpace = $"QStack.Framework.AutoMigration.{factory.FactoryName}";
			
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}

				var projectDir = _env.ContentRootPath;

				//生成之前的migration的程序集
				accemblyName = LoadScaffolderMigrations(path, $"Last_{factory.FactoryName}.Migrations", rootNameSpace,factory.FactoryName);

				using (var context = CreateDao(factory, accemblyName))
				{

					var designTimeServiceCollection = new ServiceCollection()
						.AddEntityFrameworkDesignTimeServices()
						.AddDbContextDesignTimeServices(context);
					if (ignoreFK)
					{
						//不生成外键，覆盖自带的生成类
						designTimeServiceCollection.AddSingleton<ICSharpSnapshotGenerator, IgnoreFKCSharpSnapshotGenerator>();
						designTimeServiceCollection.AddSingleton<ICSharpMigrationOperationGenerator, IgnoreFKCSharpMigrationOperationGenerator>();

					}
					designTimeServiceCollection.AddSingleton<IMigrationsCodeGenerator, MutipleContextCSharpMigrationsGenerator>();
					designTimeServicesAction?.Invoke(designTimeServiceCollection);
					//根据数据库类型创建,查找对应的IDesignTimeServices
					var designTimeService= FindIDesignTimeServices(factory);
					if (designTimeService == null)
						throw new ServiceFrameworkException($"could not find DesignTimeServices for the DaoFactory \"{factory.FactoryName}\"");
					designTimeService.ConfigureDesignTimeServices(designTimeServiceCollection);

					var designTimeServicesProvider = designTimeServiceCollection.BuildServiceProvider();

					var scaffolder = designTimeServicesProvider.GetRequiredService<IMigrationsScaffolder>();

					//var denpensies = designTimeServicesProvider.GetService<MigrationsScaffolderDependencies>();
					var csharpMigrationsGenerator = designTimeServicesProvider.GetService<IMigrationsCodeGenerator>() as MutipleContextCSharpMigrationsGenerator;
					csharpMigrationsGenerator.ModelSnapshotNamespace = rootNameSpace + ".Migrations";

					var migration = scaffolder.ScaffoldMigration(migrationName, rootNameSpace);
					scaffolder.Save(projectDir, migration, path);

					//另一种保存文件方式
					//File.WriteAllText(
					//	Path.Combine(path, migration.MigrationId + migration.FileExtension),
					//	migration.MigrationCode);
					//File.WriteAllText(
					//	Path.Combine(path, migration.MigrationId + ".Designer" + migration.FileExtension),
					//	migration.MetadataCode);
					//File.WriteAllText(
					//	Path.Combine(path, migration.SnapshotName + migration.FileExtension),
					//	migration.SnapshotCode);


				}
				accemblyName = LoadScaffolderMigrations(path, migrationName, rootNameSpace, factory.FactoryName);
				using (var context = CreateDao(factory, accemblyName))
				{
					//Console.WriteLine($"migrate {migrationName} from dll {accemblyName}");
					//var migrations = context.Database.GetMigrations();
					context.Database.Migrate();
					context.Database.EnsureCreated();
				}
				_logger.LogInformation($"complete migrations of  {item.FactoryName} ...");
			}
		}

		private IDesignTimeServices FindIDesignTimeServices(EFCoreDaoFactory factory)
		{
			var providerAssemblies = _providerAssemblies[factory.FactoryName];
			foreach (var assembly in providerAssemblies)
			{
				foreach (Type type in assembly.ExportedTypes)
				{
					if (type.IsClass && type != typeof(IDesignTimeServices) && typeof(IDesignTimeServices).IsAssignableFrom(type))
					{
						return (IDesignTimeServices)type.GetConstructor(new Type[] { }).Invoke(null);
					}
				}
			}
			return null;
		}

		private string LoadScaffolderMigrations(string outputDir, string migrationName, string rootNameSpace,string factoryName=null)
		{
			var sourceResult = CreateSyntaxTree(outputDir, migrationName);
			var syntaxTrees = sourceResult.Item1;
			if (syntaxTrees.Length <= 0)
				return null;
			var dotnetCoreDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
			var symbolsName = Path.ChangeExtension(migrationName, "pdb");
#if DEBUG
			var optimizationLevel = OptimizationLevel.Debug;
#else
			var optimizationLevel = OptimizationLevel.Release;
#endif
			var references = new List<MetadataReference> {
				    MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
					MetadataReference.CreateFromFile(typeof(Console).GetTypeInfo().Assembly.Location),
					MetadataReference.CreateFromFile(typeof(EFCoreDao).GetTypeInfo().Assembly.Location),
					MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "mscorlib.dll")),
					MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "netstandard.dll")),
					MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "System.Runtime.dll")),
					MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "System.Linq.Expressions.dll")),
					
					//MetadataReference.CreateFromFile(Path.Combine(AppContext.BaseDirectory, "Npgsql.EntityFrameworkCore.PostgreSQL.dll")),
					MetadataReference.CreateFromFile(Path.Combine(AppContext.BaseDirectory, "Microsoft.EntityFrameworkCore.Relational.dll")),
					MetadataReference.CreateFromFile(Path.Combine(AppContext.BaseDirectory, "Microsoft.EntityFrameworkCore.Design.dll")),
					MetadataReference.CreateFromFile(Path.Combine(AppContext.BaseDirectory, "Microsoft.EntityFrameworkCore.dll"))
			};
			//此处添加相应数据库的提供程序//MetadataReference.CreateFromFile(Path.Combine(AppContext.BaseDirectory, "Npgsql.EntityFrameworkCore.PostgreSQL.dll")),
			
			if (factoryName != null)
			{
				var provideAssemblies = _providerAssemblies[factoryName];
				foreach (var assembly in provideAssemblies)
				{
					references.Add(MetadataReference.CreateFromFile(assembly.Location));
				}
			}
			var compilation = CSharpCompilation.Create(migrationName)
				.WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
				.AddReferences(references)
				.AddSyntaxTrees(syntaxTrees)
				.WithOptions(
				    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
							.WithOptimizationLevel(optimizationLevel)
							.WithPlatform(Platform.AnyCpu)
				 );
				

			// Debug output. In case your environment is different it may show some messages.
			foreach (var compilerMessage in compilation.GetDiagnostics())
				Console.WriteLine(compilerMessage);

			//output to dll file and load 可以生成程序集，然后加载到内存
			//var fileName = Path.Combine(outputDir, migrationName + ".dll");
			//var emitResult = compilation.Emit(fileName);
			//if (emitResult.Success)
			//{
			//	var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(fileName));

			//	return assembly.GetName().Name;

			//}

			///or to memory stream 或者直接生成到内存
			//using (var memoryStream = new MemoryStream())
			//{
			//	var emitResult = compilation.Emit(memoryStream);
			//	if (emitResult.Success)
			//	{
			//		memoryStream.Seek(0, SeekOrigin.Begin);

			//		var assembly = AssemblyLoadContext.Default.LoadFromStream(memoryStream);
			//		return assembly.GetName().Name;
			//	}
			//}
			//return null;
			//生成到内存，并支持debug的版本
			using (var assemblyStream = new MemoryStream())
			using (var symbolsStream = new MemoryStream())
			{
				var emitOptions = new EmitOptions(
						debugInformationFormat: DebugInformationFormat.PortablePdb,
						pdbFilePath: symbolsName);

				var embeddedTexts = sourceResult.Item2;

				EmitResult result = compilation.Emit(
					peStream: assemblyStream,
					pdbStream: symbolsStream,
					embeddedTexts: embeddedTexts,
					options: emitOptions);

				if (!result.Success)
				{
					var errors = new List<string>();

					IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
						diagnostic.IsWarningAsError ||
						diagnostic.Severity == DiagnosticSeverity.Error);

					foreach (Diagnostic diagnostic in failures)
						errors.Add($"{diagnostic.Id}: {diagnostic.GetMessage()}");

					throw new Exception(String.Join("\n", errors));
				}



				assemblyStream.Seek(0, SeekOrigin.Begin);
				symbolsStream?.Seek(0, SeekOrigin.Begin);

				var assembly = _assemblyLoadContext.LoadFromStream(assemblyStream, symbolsStream);
				return assembly.GetName().Name;
			}


		}

		private Tuple<SyntaxTree[], List<EmbeddedText>> CreateSyntaxTree(string outputDir, string migrationName)
		{

			string[] sourceFilePaths = Directory.GetFiles(outputDir, "*.cs", SearchOption.TopDirectoryOnly);

			var encoding = Encoding.UTF8; 
			var syntaxTreeList = new List<SyntaxTree>();
			var embeddedTexts = new List<EmbeddedText>();

			///添加一个空类，防止首次生成返回null导致上下文转向查找当前运行程序集
			var emptyTypeText ="namespace _"+ Guid.NewGuid().ToString("N") + "{class Class1{}}";
			syntaxTreeList.Add(CSharpSyntaxTree.ParseText(emptyTypeText));
			foreach (var path in sourceFilePaths)
			{
				var text = File.ReadAllText(path);
				var syntaxTree = CSharpSyntaxTree.ParseText(
					text,
					new CSharpParseOptions(),
					path: path);

				var syntaxRootNode = syntaxTree.GetRoot() as CSharpSyntaxNode;
				var encoded = CSharpSyntaxTree.Create(syntaxRootNode, null, path, encoding);
				syntaxTreeList.Add(encoded);
				var buffer = encoding.GetBytes(text);
				embeddedTexts.Add(EmbeddedText.FromSource(path, SourceText.From(buffer,buffer.Length, encoding,canBeEmbedded:true)));


			}
			return Tuple.Create(syntaxTreeList.ToArray(), embeddedTexts);

		}

        public void Dispose()
        {
			_assemblyLoadContext?.Unload();

		}
    }
}
