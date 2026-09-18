FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "src/Services/FakeTaxAuthority/LoanPlatform.FakeTaxAuthority.Api/LoanPlatform.FakeTaxAuthority.Api.csproj"

RUN dotnet publish "src/Services/FakeTaxAuthority/LoanPlatform.FakeTaxAuthority.Api/LoanPlatform.FakeTaxAuthority.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "LoanPlatform.FakeTaxAuthority.Api.dll"]
