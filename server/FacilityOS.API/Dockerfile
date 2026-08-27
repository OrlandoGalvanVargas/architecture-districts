FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["FacilityOS.Domain/FacilityOS.Domain.csproj", "FacilityOS.Domain/"]
COPY ["FacilityOS.Application/FacilityOS.Application.csproj", "FacilityOS.Application/"]
COPY ["FacilityOS.API/FacilityOS.API.csproj", "FacilityOS.API/"]

RUN dotnet restore "FacilityOS.API/FacilityOS.API.csproj"

COPY . .

RUN dotnet publish "FacilityOS.API/FacilityOS.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

USER $APP_UID

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "FacilityOS.API.dll"]
