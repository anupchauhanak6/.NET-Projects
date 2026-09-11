# BasicAPIsPoints

A small ASP.NET Core Web API that demonstrates controller-based routing with
an in-memory list of products (fruits).

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Run the API

From the `FirstProject` directory, run:

```bash
dotnet run --project BasicAPIsPoints/BasicAPIsPoints.csproj
```

The application is available at:

- HTTP: `http://localhost:5054`
- HTTPS: `https://localhost:7224`

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/products` | Returns all products |
| `GET` | `/api/products/{id}` | Returns one product by its 1-based ID |

Example requests:

```bash
curl http://localhost:5054/api/products
curl http://localhost:5054/api/products/1
```

The available products are stored in memory and reset whenever the application
restarts.

## Build

```bash
dotnet build BasicAPIsPoints/BasicAPIsPoints.csproj
```
