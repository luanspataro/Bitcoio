FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Bitcoio/Bitcoio.csproj", "Bitcoio/"]
RUN dotnet restore "./Bitcoio/Bitcoio.csproj"
COPY . .
WORKDIR "/src/Bitcoio"
RUN dotnet build "./Bitcoio.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Bitcoio.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["Bitcoio/Data/BitcoinDatePriceBRL.xlsx", "Data/"]
ENTRYPOINT ["dotnet", "Bitcoio.dll"]
