# Trouve Prénom

A simple website to help future parents find a name for their kid.

## Data

The data is provided by the INSEE.  [Download link](https://www.insee.fr/fr/statistiques/2540004)

## Versions

- Visual Studio Community 2017
- ASP.NET Core 2.

## Docker

- Start in directory `TrouvePrenoms`
- In Powershell :
  - `docker build -t trouveprenoms .`
  - `docker run -d -p 8080:80 --name tp trouveprenoms`