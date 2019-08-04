FROM microsoft/aspnetcore-build:2.0.3-stretch AS build-env

WORKDIR /app
COPY . ./
RUN dotnet build TrouvePrenoms.csproj
RUN dotnet publish TrouvePrenoms.csproj -c Release -r linux-x64 -o out

FROM microsoft/aspnetcore:2.0.3-stretch
RUN mkdir /app
COPY --from=build-env /app/out /app

ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001

ENTRYPOINT ["/app/TrouvePrenoms"]