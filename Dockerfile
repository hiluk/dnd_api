FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DndSolution.Core.Api/DndSolution.Core.Api.csproj", "DndSolution.Core.Api/"]
COPY ["DndSolution.Application.Abstractions/DndSolution.Application.Abstractions.csproj", "DndSolution.Application.Abstractions/"]
COPY ["DndSolution.Data.Abstractions/DndSolution.Data.Abstractions.csproj", "DndSolution.Data.Abstractions/"]
COPY ["DndSolution.Application.Models/DndSolution.Application.Models.csproj", "DndSolution.Application.Models/"]
COPY ["DndSolution.Core.Sdk/DndSolution.Core.Sdk.csproj", "DndSolution.Core.Sdk/"]
COPY ["DndSolution.Neccessary/DndSolution.Neccessary.csproj", "DndSolution.Neccessary/"]
COPY ["DndSolution.Data.Repositories/DndSolution.Data.Repositories.csproj", "DndSolution.Data.Repositories/"]
COPY ["DndSolution.Application.Services/DndSolution.Application.Services.csproj", "DndSolution.Application.Services/"]
RUN dotnet restore "DndSolution.Core.Api/DndSolution.Core.Api.csproj"
COPY . .
WORKDIR "/src/DndSolution.Core.Api"
RUN dotnet build "DndSolution.Core.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DndSolution.Core.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DndSolution.Core.Api.dll"]
