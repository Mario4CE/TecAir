#!/usr/bin/env bash
set -euo pipefail

# Requires .NET SDK 8.x installed locally.

if ! command -v dotnet >/dev/null 2>&1; then
  echo "dotnet SDK no encontrado. Instala .NET 8 antes de ejecutar este script." >&2
  exit 1
fi

if [ ! -f "TecAir.sln" ]; then
  dotnet new sln -n TecAir
fi

dotnet sln TecAir.sln add \
  tecair-api/TecAir.Api.csproj \
  src/TecAir.Contracts/TecAir.Contracts.csproj \
  src/TecAir.Domain/TecAir.Domain.csproj \
  src/TecAir.Application/TecAir.Application.csproj \
  src/TecAir.Infrastructure/TecAir.Infrastructure.csproj \
  tests/TecAir.Api.Tests/TecAir.Api.Tests.csproj

echo "TecAir.sln actualizada correctamente."
