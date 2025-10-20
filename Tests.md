# Plan de Pruebas - Testing Combinatorio PAIRWISE

## Objetivo
Este documento define los casos de prueba basados en la técnica **PAIRWISE** para las validaciones de las tablas Cliente, Huésped y Reserva del sistema de gestión hotelera.

---

## 1. TABLA CLIENTE

### Factores y Niveles

| Factor | Niveles |
|--------|---------|
| **Razon_Social** | Válida (1-20 chars), Vacía, Excede límite (>20) |
| **NIT** | Válido (1-20 chars), Vacío, Excede límite (>20) |
| **Email** | Válido y único, Formato inválido, Vacío, Duplicado |

### Casos de Prueba PAIRWISE - Cliente

| # | Razon_Social | NIT | Email | Resultado Esperado |
|---|--------------|-----|-------|-------------------|
| **TC-C01** | Válida: "Hotel Luna" | Válido: "1234567890" | Válido: "cliente1@mail.com" | ✅ **ÉXITO** - Cliente creado |
| **TC-C02** | Válida: "Hotel Sol" | Válido: "9876543210" | Formato inválido: "cliente@" | ❌ **ERROR** - Email con formato inválido |
| **TC-C03** | Válida: "Hotel Mar" | Vacío: "" | Válido: "cliente2@mail.com" | ❌ **ERROR** - NIT obligatorio |
| **TC-C04** | Vacía: "" | Válido: "5555555555" | Válido: "cliente3@mail.com" | ❌ **ERROR** - Razón Social obligatoria |
| **TC-C05** | Válida: "Hotel Paz" | Excede: "123456789012345678901" | Válido: "cliente4@mail.com" | ❌ **ERROR** - NIT excede 20 caracteres |
| **TC-C06** | Excede: "Hotel Gran Majestuoso *****" | Válido: "7777777777" | Válido: "cliente5@mail.com" | ❌ **ERROR** - Razón Social excede 20 caracteres |
| **TC-C07** | Válida: "Hotel Real" | Válido: "8888888888" | Vacío: "" | ❌ **ERROR** - Email obligatorio |
| **TC-C08** | Válida: "Hotel Plus" | Válido: "6666666666" | Duplicado: "cliente1@mail.com" | ❌ **ERROR** - Email ya existe |
| **TC-C09** | Vacía: "" | Vacío: "" | Formato inválido: "invalido" | ❌ **ERROR** - Múltiples campos obligatorios vacíos |
| **TC-C10** | Excede: "Hotel Internacional 5 Estrellas" | Excede: "999999999999999999999" | Vacío: "" | ❌ **ERROR** - Múltiples validaciones fallidas |

### Pruebas de Valores Límite - Cliente

| # | Campo | Valor | Longitud | Resultado Esperado |
|---|-------|-------|----------|-------------------|
| **TC-C11** | Razon_Social | "A" | 1 char | ✅ Válido (límite inferior) |
| **TC-C12** | Razon_Social | "12345678901234567890" | 20 chars | ✅ Válido (límite superior) |
| **TC-C13** | Razon_Social | "123456789012345678901" | 21 chars | ❌ Excede límite |
| **TC-C14** | NIT | "1" | 1 char | ✅ Válido (límite inferior) |
| **TC-C15** | NIT | "12345678901234567890" | 20 chars | ✅ Válido (límite superior) |
| **TC-C16** | NIT | "123456789012345678901" | 21 chars | ❌ Excede límite |
| **TC-C17** | Email | "a@b.c" | 5 chars | ✅ Válido (mínimo formato) |
| **TC-C18** | Email | "a@b.commmmmmmmmmmmmmmmmmmmmm" | 30 chars | ✅ Válido (límite superior) |
| **TC-C19** | Email | "a@b.commmmmmmmmmmmmmmmmmmmmmm" | 31 chars | ❌ Excede límite |

---

## 2. TABLA HUÉSPED

### Factores y Niveles

| Factor | Niveles |
|--------|---------|
| **Nombre** | Válido (1-30), Vacío, Excede (>30) |
| **Apellido** | Válido (1-30), Vacío, Excede (>30) |
| **Segundo_Apellido** | Válido (0-30), Excede (>30), Nulo |
| **Documento_Identidad** | Válido (1-20), Vacío, Excede (>20) |
| **Telefono** | Válido (0-20), Excede (>20), Nulo |
| **Fecha_Nacimiento** | Válida, Formato inválido, Nula |

### Casos de Prueba PAIRWISE - Huésped

| # | Nombre | Apellido | Segundo_Apellido | Documento_Identidad | Telefono | Fecha_Nacimiento | Resultado Esperado |
|---|--------|----------|------------------|---------------------|----------|------------------|-------------------|
| **TC-H01** | "Carlos" | "Gómez" | "Pérez" | "7894561" | "71234567" | "1998-05-12" | ✅ **ÉXITO** - Huésped creado |
| **TC-H02** | "María" | "López" | null | "9876543" | "72345678" | "1990-03-20" | ✅ **ÉXITO** - Segundo apellido opcional |
| **TC-H03** | "Juan" | "Martínez" | "Rodríguez" | "5555555" | null | null | ✅ **ÉXITO** - Teléfono y fecha opcionales |
| **TC-H04** | "" | "Fernández" | "García" | "1234567" | "73456789" | "1985-07-15" | ❌ **ERROR** - Nombre obligatorio |
| **TC-H05** | "Pedro" | "" | "Sánchez" | "8888888" | "74567890" | "1992-11-30" | ❌ **ERROR** - Apellido obligatorio |
| **TC-H06** | "Ana" | "Ruiz" | "Díaz" | "" | "75678901" | "1988-01-05" | ❌ **ERROR** - Documento obligatorio |
| **TC-H07** | "CarlosAndresFernandezRamiresPerez" | "Gómez" | "López" | "7777777" | "76789012" | "1995-06-25" | ❌ **ERROR** - Nombre excede 30 caracteres |
| **TC-H08** | "Luis" | "FernandezFernandezFernandez" | "Pérez" | "6666666" | "77890123" | "1987-09-18" | ❌ **ERROR** - Apellido excede 30 caracteres |
| **TC-H09** | "Rosa" | "Torres" | "Perezzzzzzzzzzzzzzzzzzzzzzzzzzz" | "4444444" | "78901234" | "1991-12-08" | ❌ **ERROR** - Segundo apellido excede 30 caracteres |
| **TC-H10** | "Jorge" | "Vargas" | "Morales" | "123456789012345678901" | "79012345" | "1993-04-22" | ❌ **ERROR** - Documento excede 20 caracteres |
| **TC-H11** | "Elena" | "Castro" | "Silva" | "3333333" | "999999999999999999999" | "1989-08-14" | ❌ **ERROR** - Teléfono excede 20 caracteres |
| **TC-H12** | "Miguel" | "Rojas" | "Vega" | "2222222" | "70123456" | "12/1998/05" | ❌ **ERROR** - Formato de fecha inválido |
| **TC-H13** | "" | "" | null | "" | null | null | ❌ **ERROR** - Campos obligatorios vacíos |
| **TC-H14** | "Sofía" | "Méndez" | "Ortiz" | "1111111" | "71112223" | "2025-12-31" | ✅ **ÉXITO** - Fecha futura válida |
| **TC-H15** | "Diego" | "Herrera" | null | "9999999" | "72223334" | "1900-01-01" | ✅ **ÉXITO** - Fecha muy antigua pero válida |

### Pruebas de Valores Límite - Huésped

| # | Campo | Valor | Longitud | Resultado Esperado |
|---|-------|-------|----------|-------------------|
| **TC-H16** | Nombre | "A" | 1 char | ✅ Válido (límite inferior) |
| **TC-H17** | Nombre | "123456789012345678901234567890" | 30 chars | ✅ Válido (límite superior) |
| **TC-H18** | Nombre | "1234567890123456789012345678901" | 31 chars | ❌ Excede límite |
| **TC-H19** | Apellido | "B" | 1 char | ✅ Válido (límite inferior) |
| **TC-H20** | Apellido | "123456789012345678901234567890" | 30 chars | ✅ Válido (límite superior) |
| **TC-H21** | Apellido | "1234567890123456789012345678901" | 31 chars | ❌ Excede límite |
| **TC-H22** | Segundo_Apellido | "" | 0 chars | ✅ Válido (opcional) |
| **TC-H23** | Segundo_Apellido | "123456789012345678901234567890" | 30 chars | ✅ Válido (límite superior) |
| **TC-H24** | Segundo_Apellido | "1234567890123456789012345678901" | 31 chars | ❌ Excede límite |
| **TC-H25** | Documento_Identidad | "1" | 1 char | ✅ Válido (límite inferior) |
| **TC-H26** | Documento_Identidad | "12345678901234567890" | 20 chars | ✅ Válido (límite superior) |
| **TC-H27** | Documento_Identidad | "123456789012345678901" | 21 chars | ❌ Excede límite |
| **TC-H28** | Telefono | "" | 0 chars | ✅ Válido (opcional) |
| **TC-H29** | Telefono | "12345678901234567890" | 20 chars | ✅ Válido (límite superior) |
| **TC-H30** | Telefono | "123456789012345678901" | 21 chars | ❌ Excede límite |

---

## 3. TABLA RESERVA

### Factores y Niveles

| Factor | Niveles |
|--------|---------|
| **Cliente_ID** | GUID válido (existente), GUID formato inválido, Vacío |
| **Monto_Total** | Mayor a 0, Igual a 0, Negativo, No numérico |
| **Estado_Reserva** | "Pendiente", "Confirmada", "Cancelada", "Completada", "No-Show", Valor inválido, Vacío |

### Casos de Prueba PAIRWISE - Reserva

| # | Cliente_ID | Monto_Total | Estado_Reserva | Resultado Esperado |
|---|------------|-------------|----------------|-------------------|
| **TC-R01** | GUID válido: "550e8400-e29b-41d4-a716-446655440000" | 150.00 | "Pendiente" | ✅ **ÉXITO** - Reserva creada |
| **TC-R02** | GUID válido: "550e8400-e29b-41d4-a716-446655440001" | 250.50 | "Confirmada" | ✅ **ÉXITO** - Reserva confirmada |
| **TC-R03** | GUID válido: "550e8400-e29b-41d4-a716-446655440002" | 0.00 | "Cancelada" | ✅ **ÉXITO** - Monto cero válido |
| **TC-R04** | GUID válido: "550e8400-e29b-41d4-a716-446655440003" | 500.75 | "Completada" | ✅ **ÉXITO** - Reserva completada |
| **TC-R05** | GUID válido: "550e8400-e29b-41d4-a716-446655440004" | 100.00 | "No-Show" | ✅ **ÉXITO** - Estado No-Show |
| **TC-R06** | GUID válido: "550e8400-e29b-41d4-a716-446655440005" | 75.00 | "Rechazada" | ❌ **ERROR** - Estado inválido |
| **TC-R07** | GUID válido: "550e8400-e29b-41d4-a716-446655440006" | -10.50 | "Pendiente" | ❌ **ERROR** - Monto negativo |
| **TC-R08** | Formato inválido: "12345" | 200.00 | "Confirmada" | ❌ **ERROR** - GUID formato inválido |
| **TC-R09** | Vacío: "" | 300.00 | "Cancelada" | ❌ **ERROR** - Cliente_ID obligatorio |
| **TC-R10** | GUID válido: "550e8400-e29b-41d4-a716-446655440007" | No numérico: "abc" | "Completada" | ❌ **ERROR** - Monto no numérico |
| **TC-R11** | GUID válido: "550e8400-e29b-41d4-a716-446655440008" | 125.00 | Vacío: "" | ❌ **ERROR** - Estado obligatorio |
| **TC-R12** | Formato inválido: "XXXX-YYYY" | -5.00 | "Rechazada" | ❌ **ERROR** - Múltiples validaciones fallidas |
| **TC-R13** | Vacío: "" | 0.00 | Vacío: "" | ❌ **ERROR** - Campos obligatorios vacíos |
| **TC-R14** | GUID válido: "550e8400-e29b-41d4-a716-446655440009" | 0.01 | "Pendiente" | ✅ **ÉXITO** - Monto mínimo válido |

### Pruebas de Valores Límite - Reserva

| # | Campo | Valor | Descripción | Resultado Esperado |
|---|-------|-------|-------------|-------------------|
| **TC-R15** | Monto_Total | -1 | Límite inferior inválido | ❌ Error: negativo |
| **TC-R16** | Monto_Total | -0.01 | Justo debajo de cero | ❌ Error: negativo |
| **TC-R17** | Monto_Total | 0 | Límite inferior válido | ✅ Válido |
| **TC-R18** | Monto_Total | 0.01 | Justo encima de cero | ✅ Válido |
| **TC-R19** | Monto_Total | 999999.99 | Monto muy alto | ✅ Válido |

---

## 4. RESUMEN DE EJECUCIÓN

### Total de Casos de Prueba por Tabla

| Tabla | Casos PAIRWISE | Casos Límite | **Total** |
|-------|----------------|--------------|-----------|
| **Cliente** | 10 | 9 | **19** |
| **Huésped** | 15 | 15 | **30** |
| **Reserva** | 14 | 5 | **19** |
| **TOTAL GENERAL** | **39** | **29** | **68** |

---

## 5. INSTRUCCIONES DE EJECUCIÓN

### Prerrequisitos
1. **Backend corriendo** en `https://localhost:5001` o `http://localhost:5000`
   ```bash
   dotnet run
   ```
2. **Frontend corriendo** en `http://localhost:4200`
   ```bash
   cd frontend
   npm start
   ```
3. Tener acceso a la base de datos de pruebas
4. Resetear la base de datos antes de ejecutar las pruebas para evitar conflictos con emails/GUIDs duplicados

### Rutas de la Aplicación

| Módulo | Ruta Frontend | API Backend |
|--------|---------------|-------------|
| Clientes (Listar) | `/clientes` | `GET /api/Cliente` |
| Cliente (Crear) | `/nuevo-cliente` | `POST /api/Cliente` |
| Cliente (Editar) | `/editar-cliente` | `PUT /api/Cliente/{id}` |
| Huéspedes (Listar) | `/huespedes` | `GET /api/Huesped` |
| Huésped (Crear) | `/nuevo-huesped` | `POST /api/Huesped` |
| Reservas (Listar) | `/reservas` | `GET /api/Reserva` |
| Reserva (Crear) | `/nueva-reserva` | `POST /api/Reserva` |
| Reserva (Detalle) | `/reservas/:id` | `GET /api/Reserva/{id}` |

### Orden de Ejecución Recomendado

1. **Primero**: Ejecutar pruebas de **Cliente** (TC-C01 a TC-C19)
   - Crear clientes válidos para usar en pruebas de Reserva
   - Anotar los GUIDs generados para clientes válidos

2. **Segundo**: Ejecutar pruebas de **Huésped** (TC-H01 a TC-H30)
   - Independiente de Cliente y Reserva

3. **Tercero**: Ejecutar pruebas de **Reserva** (TC-R01 a TC-R19)
   - Usar GUIDs de clientes creados en paso 1

### Formato de Registro de Resultados

Para cada caso de prueba, registrar:
- ✅ **PASS**: El resultado obtenido coincide con el esperado
- ❌ **FAIL**: El resultado obtenido NO coincide con el esperado
- ⚠️ **BLOCKED**: No se pudo ejecutar por dependencias
- 📝 **Comentarios**: Cualquier observación relevante

---

## 6. MATRIZ DE TRAZABILIDAD

| Requisito de Validación | Casos de Prueba Relacionados |
|--------------------------|------------------------------|
| Cliente - Razón Social obligatoria y longitud | TC-C04, TC-C06, TC-C11, TC-C12, TC-C13 |
| Cliente - NIT obligatorio y longitud | TC-C03, TC-C05, TC-C14, TC-C15, TC-C16 |
| Cliente - Email obligatorio, formato, único, longitud | TC-C02, TC-C07, TC-C08, TC-C17, TC-C18, TC-C19 |
| Huésped - Nombre obligatorio y longitud | TC-H04, TC-H07, TC-H16, TC-H17, TC-H18 |
| Huésped - Apellido obligatorio y longitud | TC-H05, TC-H08, TC-H19, TC-H20, TC-H21 |
| Huésped - Segundo apellido opcional y longitud | TC-H09, TC-H22, TC-H23, TC-H24 |
| Huésped - Documento obligatorio y longitud | TC-H06, TC-H10, TC-H25, TC-H26, TC-H27 |
| Huésped - Teléfono opcional y longitud | TC-H11, TC-H28, TC-H29, TC-H30 |
| Huésped - Fecha nacimiento formato válido | TC-H12, TC-H14, TC-H15 |
| Reserva - Cliente_ID obligatorio y formato GUID | TC-R08, TC-R09 |
| Reserva - Monto_Total mayor o igual a cero | TC-R07, TC-R10, TC-R15, TC-R16, TC-R17, TC-R18 |
| Reserva - Estado válido | TC-R06, TC-R11 |

---

## 7. NOTAS ADICIONALES

### Cobertura PAIRWISE
La técnica PAIRWISE garantiza que **cada par de valores** de diferentes factores se pruebe al menos una vez, reduciendo significativamente el número de casos de prueba mientras mantiene una cobertura efectiva de combinaciones.

### Campos No Probados
- `Activo` (booleano por defecto `true`)
- `Fecha_Creacion` y `Fecha_Actualizacion` (automáticas, gestionadas por el backend)

### Recomendaciones
- Automatizar estos casos de prueba usando un framework de testing (ej: xUnit, NUnit para .NET)
- Implementar data-driven testing para los valores límite
- Crear scripts de reseteo de base de datos entre ejecuciones
- Documentar cualquier defecto encontrado con capturas y logs

---

---

## 8. INFORMACIÓN TÉCNICA DEL BACKEND

### DTOs y Endpoints

#### Cliente
**Endpoint**: `/api/Cliente`

**POST - ClienteCreateDTO**:
```json
{
  "razon_Social": "string (max 20)",
  "nit": "string (max 20)",
  "email": "string (max 30, formato email)"
}
```

**Validaciones Backend**:
- `Razon_Social`: Required, MaxLength(20)
- `NIT`: Required, MaxLength(20)
- `Email`: Required, MaxLength(30), EmailAddress, Único

**Códigos de Respuesta**:
- 201 Created: Cliente creado exitosamente
- 400 Bad Request: Validación fallida
- 409 Conflict: Email duplicado

#### Huésped
**Endpoint**: `/api/Huesped`

**POST - HuespedCreateDTO**:
```json
{
  "Nombre": "string (max 30)",
  "Apellido": "string (max 30)",
  "Segundo_Apellido": "string (max 30, opcional)",
  "Documento_Identidad": "string (max 20)",
  "Telefono": "string (max 20, opcional)",
  "Fecha_Nacimiento": "string (ISO 8601, opcional)"
}
```

**Validaciones Backend**:
- `Nombre`: Required, MaxLength(30)
- `Apellido`: Required, MaxLength(30)
- `Segundo_Apellido`: Optional, MaxLength(30)
- `Documento_Identidad`: Required, MaxLength(20)
- `Telefono`: Optional, MaxLength(20)
- `Fecha_Nacimiento`: Optional, formato ISO 8601

**Códigos de Respuesta**:
- 201 Created: Huésped creado exitosamente
- 400 Bad Request: Validación fallida o formato de fecha inválido

#### Reserva
**Endpoint**: `/api/Reserva`

**POST - ReservaCreateDTO**:
```json
{
  "Cliente_ID": "GUID (string)",
  "Estado_Reserva": "string (Pendiente|Confirmada|Cancelada|Completada|No-Show)",
  "Monto_Total": "decimal (>= 0)"
}
```

**Validaciones Backend**:
- `Cliente_ID`: Required, debe ser GUID válido y existente
- `Estado_Reserva`: Required, MaxLength(20), valores permitidos específicos
- `Monto_Total`: Required, Decimal(10,2), >= 0

**Códigos de Respuesta**:
- 201 Created: Reserva creada exitosamente
- 400 Bad Request: Cliente_ID inválido o no existe, monto negativo
- 404 Not Found: Cliente no encontrado

### Modelos de Base de Datos

#### Cliente
```csharp
- ID: BINARY(16) - GUID
- Razon_Social: VARCHAR(20) - NOT NULL
- NIT: VARCHAR(20) - NOT NULL
- Email: VARCHAR(30) - NOT NULL, UNIQUE
- Activo: BOOLEAN - DEFAULT TRUE
- Fecha_Creacion: DATETIME - NOT NULL
- Fecha_Actualizacion: DATETIME - NOT NULL
```

#### Huesped
```csharp
- ID: BINARY(16) - GUID
- Nombre: VARCHAR(30) - NOT NULL
- Apellido: VARCHAR(30) - NOT NULL
- Segundo_Apellido: VARCHAR(30) - NULL
- Documento_Identidad: VARCHAR(20) - NOT NULL
- Telefono: VARCHAR(20) - NULL
- Fecha_Nacimiento: DATETIME - NULL
- Activo: BOOLEAN - DEFAULT TRUE
- Fecha_Creacion: DATETIME - NOT NULL
- Fecha_Actualizacion: DATETIME - NOT NULL
```

#### Reserva
```csharp
- ID: BINARY(16) - GUID
- Cliente_ID: BINARY(16) - NOT NULL, FK
- Estado_Reserva: VARCHAR(20) - NOT NULL, DEFAULT 'Pendiente'
- Monto_Total: DECIMAL(10,2) - NOT NULL, DEFAULT 0.00
- Fecha_Creacion: DATETIME - NOT NULL
```

### Configuración de CORS

El backend debe tener CORS configurado para permitir peticiones desde `http://localhost:4200`:

```csharp
app.UseCors(policy => policy
    .WithOrigins("http://localhost:4200")
    .AllowAnyMethod()
    .AllowAnyHeader()
);
```

---

**Documento generado el**: 20 de octubre de 2025  
**Versión**: 1.0  
**Proyecto**: HotelManagement - Sistema de Gestión Hotelera
