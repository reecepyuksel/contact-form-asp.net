FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY aspnet-contact-form/aspnet-contact-form/*.csproj aspnet-contact-form/aspnet-contact-form/
RUN dotnet restore aspnet-contact-form/aspnet-contact-form/*.csproj

COPY aspnet-contact-form/ aspnet-contact-form/
WORKDIR /src/aspnet-contact-form/aspnet-contact-form
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "aspnet-contact-form.dll"]