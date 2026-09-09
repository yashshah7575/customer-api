FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "./"]
COPY ["Customer.Common/Customer.Common.csproj", "Customer.Common/"]
COPY ["Customer.Repository/Customer.Repository.csproj", "Customer.Repository/"]
COPY ["Customer.Services/Customer.Services.csproj", "Customer.Services/"]
COPY ["Customer.Api/Customer.Api.csproj", "Customer.Api/"]
RUN dotnet restore "./Customer.Api/Customer.Api.csproj"
COPY . .
WORKDIR "/src/Customer.Api"
RUN dotnet build "./Customer.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Customer.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Customer.Api.dll"]
