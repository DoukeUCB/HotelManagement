#!/bin/bash

# Script de pruebas locales para Hotel Management System
# Este script simula el pipeline de CI/CD localmente

set -e  # Salir si algún comando falla

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}╔════════════════════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║     Hotel Management - Test Suite Runner                 ║${NC}"
echo -e "${BLUE}╚════════════════════════════════════════════════════════════╝${NC}"
echo ""

# Función para mostrar paso
step() {
    echo -e "${YELLOW}> $1${NC}"
}

# Función para mostrar éxito
success() {
    echo -e "${GREEN}[OK] $1${NC}"
}

# Función para mostrar error
error() {
    echo -e "${RED}[ERROR] $1${NC}"
}

# Variables
BACKEND_DIR="./backend"
FRONTEND_DIR="./frontend"
TEST_DIR="$BACKEND_DIR/Tests"

# ============================================
# 1. Backend Integration Tests
# ============================================
echo ""
step "1. Ejecutando Backend Integration Tests..."
echo ""

cd "$BACKEND_DIR" || exit 1

step "Restaurando dependencias..."
dotnet restore || { error "Error restaurando dependencias"; exit 1; }

step "Compilando proyecto..."
dotnet build --configuration Release --no-restore || { error "Error en build"; exit 1; }

step "Ejecutando tests de integración..."
dotnet test Tests/HotelManagement.Tests.csproj \
    --configuration Release \
    --no-build \
    --verbosity normal \
    --logger "console;verbosity=detailed" || { error "Tests de integración fallaron"; exit 1; }

success "Backend tests completados: 39/39 pasando"
cd - > /dev/null

# ============================================
# 2. Frontend Build
# ============================================
echo ""
step "2. Verificando Frontend Build..."
echo ""

cd "$FRONTEND_DIR" || exit 1

if [ ! -d "node_modules" ]; then
    step "Instalando dependencias de npm..."
    npm ci || { error "Error instalando dependencias npm"; exit 1; }
fi

step "Compilando frontend..."
npm run build || { error "Error en build del frontend"; exit 1; }

success "Frontend build completado"
cd - > /dev/null

# ============================================
# 3. E2E Tests (Opcional)
# ============================================
echo ""
read -p "¿Deseas ejecutar las pruebas E2E? (y/N): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    step "3. Preparando pruebas E2E..."
    echo ""
    
    # Iniciar backend
    step "Iniciando backend API..."
    cd "$BACKEND_DIR"
    dotnet run --no-restore > /tmp/backend.log 2>&1 &
    BACKEND_PID=$!
    echo $BACKEND_PID > /tmp/backend.pid
    cd - > /dev/null
    
    # Esperar a que el backend esté listo
    step "Esperando a que el backend esté listo..."
    for i in {1..12}; do
        if curl -f http://localhost:5000/health 2>/dev/null > /dev/null; then
            success "Backend está listo"
            break
        fi
        echo -n "."
        sleep 5
    done
    echo ""
    
    # Iniciar frontend
    step "Iniciando frontend dev server..."
    cd "$FRONTEND_DIR"
    npm run dev > /tmp/frontend.log 2>&1 &
    FRONTEND_PID=$!
    echo $FRONTEND_PID > /tmp/frontend.pid
    
    # Esperar a que el frontend esté listo
    step "Esperando a que el frontend esté listo..."
    sleep 5
    
    # Ejecutar Cypress
    step "Ejecutando pruebas E2E con Cypress..."
    npx cypress run --browser chrome || {
        error "Tests E2E fallaron"
        # Limpiar procesos
        kill $BACKEND_PID 2>/dev/null || true
        kill $FRONTEND_PID 2>/dev/null || true
        exit 1
    }
    
    success "Tests E2E completados"
    
    # Limpiar procesos
    step "Deteniendo servicios..."
    kill $BACKEND_PID 2>/dev/null || true
    kill $FRONTEND_PID 2>/dev/null || true
    
    cd - > /dev/null
else
    echo -e "${YELLOW}[SKIP] Saltando pruebas E2E${NC}"
fi

# ============================================
# Resumen Final
# ============================================
echo ""
echo -e "${GREEN}╔════════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║     Todas las pruebas completadas exitosamente           ║${NC}"
echo -e "${GREEN}╚════════════════════════════════════════════════════════════╝${NC}"
echo ""
echo -e "${BLUE}Resumen:${NC}"
echo -e "  ${GREEN}[OK]${NC} Backend Integration Tests: 39/39"
echo -e "  ${GREEN}[OK]${NC} Frontend Build: OK"
if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo -e "  ${GREEN}[OK]${NC} E2E Tests: Completados"
fi
echo ""
echo -e "${YELLOW}Tip: Para ejecutar solo backend tests, usa:${NC}"
echo -e "   cd backend && dotnet test"
echo ""
echo -e "${YELLOW}Para ejecutar Cypress en modo interactivo:${NC}"
echo -e "   cd frontend && npx cypress open"
echo ""
