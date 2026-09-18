FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "src/Services/LoanCore/LoanPlatform.LoanCore.Api/LoanPlatform.LoanCore.Api.csproj"

RUN dotnet publish "src/Services/LoanCore/LoanPlatform.LoanCore.Api/LoanPlatform.LoanCore.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "LoanPlatform.LoanCore.Api.dll"]
