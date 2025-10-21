# Script de Verificación - Pruebas Cypress
# Verifica que todo esté configurado correctamente antes de ejecutar las pruebas

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "  VERIFICACIÓN DE CONFIGURACIÓN CYPRESS  " -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

$errores = 0

# Verificar directorio actual
Write-Host "[1/8] Verificando directorio..." -ForegroundColor Yellow
if (Test-Path "cypress.config.ts") {
    Write-Host "  ✅ Directorio correcto (frontend)" -ForegroundColor Green
} else {
    Write-Host "  ❌ ERROR: Debes ejecutar este script desde el directorio 'frontend'" -ForegroundColor Red
    $errores++
}

# Verificar node_modules
Write-Host "[2/8] Verificando dependencias..." -ForegroundColor Yellow
if (Test-Path "node_modules") {
    Write-Host "  ✅ node_modules encontrado" -ForegroundColor Green
    
    if (Test-Path "node_modules\cypress") {
        Write-Host "  ✅ Cypress instalado" -ForegroundColor Green
    } else {
        Write-Host "  ❌ Cypress NO instalado. Ejecuta: npm install" -ForegroundColor Red
        $errores++
    }
} else {
    Write-Host "  ❌ node_modules NO encontrado. Ejecuta: npm install" -ForegroundColor Red
    $errores++
}

# Verificar archivos de prueba
Write-Host "[3/8] Verificando archivos de prueba..." -ForegroundColor Yellow
$archivos = @(
    "cypress\e2e\00-navigation.cy.ts",
    "cypress\e2e\01-cliente-pairwise.cy.ts",
    "cypress\e2e\02-huesped-pairwise.cy.ts",
    "cypress\e2e\03-reserva-pairwise.cy.ts"
)

foreach ($archivo in $archivos) {
    if (Test-Path $archivo) {
        Write-Host "  ✅ $archivo" -ForegroundColor Green
    } else {
        Write-Host "  ❌ $archivo NO encontrado" -ForegroundColor Red
        $errores++
    }
}

# Verificar comandos personalizados
Write-Host "[4/8] Verificando comandos personalizados..." -ForegroundColor Yellow
if (Test-Path "cypress\support\commands.ts") {
    $content = Get-Content "cypress\support\commands.ts" -Raw
    if ($content -match "fillClienteForm") {
        Write-Host "  ✅ Comando fillClienteForm encontrado" -ForegroundColor Green
    } else {
        Write-Host "  ⚠️  Comando fillClienteForm NO encontrado" -ForegroundColor Yellow
    }
    
    if ($content -match "fillHuespedForm") {
        Write-Host "  ✅ Comando fillHuespedForm encontrado" -ForegroundColor Green
    } else {
        Write-Host "  ⚠️  Comando fillHuespedForm NO encontrado" -ForegroundColor Yellow
    }
} else {
    Write-Host "  ❌ cypress\support\commands.ts NO encontrado" -ForegroundColor Red
    $errores++
}

# Verificar backend
Write-Host "[5/8] Verificando Backend..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/Cliente" -Method GET -TimeoutSec 3 -ErrorAction Stop
    Write-Host "  ✅ Backend respondiendo en http://localhost:5000" -ForegroundColor Green
} catch {
    Write-Host "  ❌ Backend NO responde en http://localhost:5000" -ForegroundColor Red
    Write-Host "     Inicia el backend con: dotnet run" -ForegroundColor Yellow
    $errores++
}

# Verificar frontend
Write-Host "[6/8] Verificando Frontend..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:4200" -Method GET -TimeoutSec 3 -ErrorAction Stop
    Write-Host "  ✅ Frontend respondiendo en http://localhost:4200" -ForegroundColor Green
} catch {
    Write-Host "  ❌ Frontend NO responde en http://localhost:4200" -ForegroundColor Red
    Write-Host "     Inicia el frontend con: npm start" -ForegroundColor Yellow
    $errores++
}

# Verificar configuración Cypress
Write-Host "[7/8] Verificando configuración Cypress..." -ForegroundColor Yellow
if (Test-Path "cypress.config.ts") {
    $config = Get-Content "cypress.config.ts" -Raw
    if ($config -match "baseUrl.*localhost:4200") {
        Write-Host "  ✅ baseUrl configurado correctamente" -ForegroundColor Green
    } else {
        Write-Host "  ⚠️  baseUrl podría necesitar revisión" -ForegroundColor Yellow
    }
} else {
    Write-Host "  ❌ cypress.config.ts NO encontrado" -ForegroundColor Red
    $errores++
}

# Verificar documentación
Write-Host "[8/8] Verificando documentación..." -ForegroundColor Yellow
$docs = @(
    "QUICK_START_CYPRESS.md",
    "TESTING.md",
    "cypress\README.md"
)

foreach ($doc in $docs) {
    if (Test-Path $doc) {
        Write-Host "  ✅ $doc" -ForegroundColor Green
    } else {
        Write-Host "  ⚠️  $doc NO encontrado" -ForegroundColor Yellow
    }
}

# Resumen
Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "  RESUMEN DE VERIFICACIÓN" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan

if ($errores -eq 0) {
    Write-Host ""
    Write-Host "✅ ¡TODO LISTO!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Puedes ejecutar las pruebas con:" -ForegroundColor White
    Write-Host "  npx cypress open     (Modo interactivo)" -ForegroundColor Cyan
    Write-Host "  npx cypress run      (Modo headless)" -ForegroundColor Cyan
    Write-Host "  .\run-tests.ps1      (Menú interactivo)" -ForegroundColor Cyan
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "❌ ERRORES ENCONTRADOS: $errores" -ForegroundColor Red
    Write-Host ""
    Write-Host "Por favor, corrige los errores antes de ejecutar las pruebas." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Checklist:" -ForegroundColor White
    Write-Host "  1. cd frontend" -ForegroundColor Cyan
    Write-Host "  2. npm install" -ForegroundColor Cyan
    Write-Host "  3. Backend: dotnet run (en otro terminal)" -ForegroundColor Cyan
    Write-Host "  4. Frontend: npm start (en otro terminal)" -ForegroundColor Cyan
    Write-Host ""
}

Write-Host "=========================================" -ForegroundColor Cyan

# Preguntar si desea abrir Cypress
if ($errores -eq 0) {
    Write-Host ""
    $respuesta = Read-Host "¿Deseas abrir Cypress ahora? (S/N)"
    if ($respuesta -eq "S" -or $respuesta -eq "s") {
        Write-Host "Abriendo Cypress..." -ForegroundColor Green
        npx cypress open
    }
}
