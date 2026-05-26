#!/usr/bin/env bash
set -euo pipefail

missing=0

check_cmd() {
  local cmd="$1"
  local name="$2"
  if command -v "$cmd" >/dev/null 2>&1; then
    echo "✅ $name: $(command -v "$cmd")"
  else
    echo "❌ $name no está instalado o no está en PATH"
    missing=1
  fi
}

check_cmd dotnet ".NET SDK (dotnet)"

if [ "$missing" -eq 1 ]; then
  echo
  echo "Instala el .NET SDK 8+ y vuelve a ejecutar este script."
  exit 1
fi

echo
echo "Versión de dotnet detectada:"
dotnet --version
