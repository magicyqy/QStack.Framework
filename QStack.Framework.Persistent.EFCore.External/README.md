<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="3.1.3">
      <PrivateAssets>all</PrivateAssets>
		<IncludeAssets>compile;runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
说明：引用时IncludeAssets要新增compile 项,.csproj;
同时注释掉<PrivateAssets>all</PrivateAssets>