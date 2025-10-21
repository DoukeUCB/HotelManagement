# Script PowerShell para ejecutar pruebas Cypress
# Uso: .\run-tests.ps1

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  HOTEL MANAGEMENT - CYPRESS TESTS  " -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si estamos en el directorio correcto
if (-not (Test-Path "cypress.config.ts")) {
    Write-Host "ERROR: Este script debe ejecutarse desde el directorio 'frontend'" -ForegroundColor Red
    Write-Host "Usa: cd frontend; .\run-tests.ps1" -ForegroundColor Yellow
    exit 1
}

# Verificar que node_modules existe
if (-not (Test-Path "node_modules")) {
    Write-Host "Instalando dependencias..." -ForegroundColor Yellow
    npm install
}

# Menú de opciones
Write-Host "Selecciona una opción:" -ForegroundColor Green
Write-Host "1. Ejecutar TODAS las pruebas (Headless)" -ForegroundColor White
Write-Host "2. Abrir Cypress (Modo Interactivo)" -ForegroundColor White
Write-Host "3. Ejecutar solo pruebas de Cliente" -ForegroundColor White
Write-Host "4. Ejecutar solo pruebas de Huésped" -ForegroundColor White
Write-Host "5. Ejecutar solo pruebas de Reserva" -ForegroundColor White
Write-Host "6. Ejecutar solo pruebas de Navegación" -ForegroundColor White
Write-Host "7. Salir" -ForegroundColor White
Write-Host ""

$opcion = Read-Host "Ingresa el número de opción"

switch ($opcion) {
    "1" {
        Write-Host "`nEjecutando TODAS las pruebas..." -ForegroundColor Cyan
        npx cypress run
    }
    "2" {
        Write-Host "`nAbriendo Cypress en modo interactivo..." -ForegroundColor Cyan
        npx cypress open
    }
    "3" {
        Write-Host "`nEjecutando pruebas de CLIENTE..." -ForegroundColor Cyan
        npx cypress run --spec "cypress/e2e/01-cliente-pairwise.cy.ts"
    }
    "4" {
        Write-Host "`nEjecutando pruebas de HUÉSPED..." -ForegroundColor Cyan
        npx cypress run --spec "cypress/e2e/02-huesped-pairwise.cy.ts"
    }
    "5" {
        Write-Host "`nEjecutando pruebas de RESERVA..." -ForegroundColor Cyan
        npx cypress run --spec "cypress/e2e/03-reserva-pairwise.cy.ts"
    }
    "6" {
        Write-Host "`nEjecutando pruebas de NAVEGACIÓN..." -ForegroundColor Cyan
        npx cypress run --spec "cypress/e2e/00-navigation.cy.ts"
    }
    "7" {
        Write-Host "`nSaliendo..." -ForegroundColor Yellow
        exit 0
    }
    default {
        Write-Host "`nOpción inválida" -ForegroundColor Red
        exit 1
    }
}

Write-Host "`n=====================================" -ForegroundColor Cyan
Write-Host "  PRUEBAS COMPLETADAS  " -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
