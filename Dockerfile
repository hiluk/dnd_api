FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DndSolution.Core.Api/DndSolution.Core.Api.csproj", "DndSolution.Core.Api/"]
COPY ["DndSolution.Application.Abstractions/DndSolution.Application.Abstractions.csproj", "DndSolution.Application.Abstractions/"]
COPY ["DndSolution.Application.Models/DndSolution.Application.Models.csproj", "DndSolution.Application.Models/"]
COPY ["DndSolution.Application.Services/DndSolution.Application.Services.csproj", "DndSolution.Application.Services/"]
COPY ["DndSolution.Core.Sdk/DndSolution.Core.Sdk.csproj", "DndSolution.Core.Sdk/"]
COPY ["DndSolution.Data.Abstractions/DndSolution.Data.Abstractions.csproj", "DndSolution.Data.Abstractions/"]
COPY ["DndSolution.Data.Repositories/DndSolution.Data.Repositories.csproj", "DndSolution.Data.Repositories/"]
COPY ["DndSolution.Neccessary/DndSolution.Neccessary.csproj", "DndSolution.Neccessary/"]
RUN dotnet restore "DndSolution.Core.Api/DndSolution.Core.Api.csproj"
COPY . .
RUN dotnet publish "DndSolution.Core.Api/DndSolution.Core.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
USER $APP_UID
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DndSolution.Core.Api.dll"]
