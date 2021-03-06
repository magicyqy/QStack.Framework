#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

#COPY ["QStack.Web/QStack.Web.csproj", "QStack.Web/"]
#RUN dotnet restore "QStack.Web/QStack.Web.csproj"
COPY . .

#WORKDIR "/src/QStack.Web.DemoPlugin.Mvc"
#RUN dotnet build "QStack.Web.DemoPlugin.Mvc.csproj" -c Release
WORKDIR "/src/QStack.Web"
RUN dotnet build "QStack.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QStack.Web.csproj" -c Release -o /app/publish

FROM base AS final
# install System.Drawing native dependencies
RUN apt-get update \
    && apt-get install -y --allow-unauthenticated \
        libc6-dev \
        libgdiplus \
        libx11-dev \
        postgresql-client \
     && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=publish /app/publish .
RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
RUN echo 'Asia/Shanghai' >/etc/timezone
ENTRYPOINT ["dotnet", "QStack.Web.dll"]
