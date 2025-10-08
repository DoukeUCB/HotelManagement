-- Crear base de datos si no existe
DROP DATABASE IF EXISTS HotelDB;
CREATE DATABASE HotelDB;
USE HotelDB;

-- Tabla Cliente
CREATE TABLE `Cliente` (
  `ID` BINARY(16) NOT NULL,
  `Razon_Social` VARCHAR(20) NOT NULL,
  `NIT` VARCHAR(20) NOT NULL,
  `Email` VARCHAR(30) NOT NULL UNIQUE,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabla Tipo_Habitacion
CREATE TABLE `Tipo_Habitacion` (
  `ID` BINARY(16) NOT NULL,
  `Nombre_Tipo` VARCHAR(50) NOT NULL UNIQUE,
  `Descripcion` TEXT NULL,
  `Capacidad_Maxima` TINYINT UNSIGNED NOT NULL,
  `Precio_Base` DECIMAL(10, 2) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabla Huesped
CREATE TABLE `Huesped` (
  `ID` BINARY(16) NOT NULL,
  `Nombre_Completo` VARCHAR(100) NOT NULL,
  `Documento_Identidad` VARCHAR(20) NOT NULL,
  `Telefono` VARCHAR(20) NULL,
  `Email` VARCHAR(100) NULL,
  `Fecha_Nacimiento` DATE NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabla Reserva
CREATE TABLE `Reserva` (
  `ID` BINARY(16) NOT NULL,
  `Cliente_ID` BINARY(16) NOT NULL,
  `Fecha_Reserva` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Fecha_Entrada` DATE NOT NULL,
  `Fecha_Salida` DATE NOT NULL,
  `Estado_Reserva` ENUM('Pendiente', 'Confirmada', 'Cancelada', 'Completada', 'No-Show') NOT NULL DEFAULT 'Pendiente',
  `Monto_Total` DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
  PRIMARY KEY (`ID`),
  INDEX `fk_Reservas_Clientes_idx` (`Cliente_ID` ASC),
  CONSTRAINT `fk_Reservas_Clientes`
    FOREIGN KEY (`Cliente_ID`)
    REFERENCES `Cliente` (`ID`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabla Habitacion
CREATE TABLE `Habitacion` (
  `ID` BINARY(16) NOT NULL,
  `Tipo_Habitacion_ID` BINARY(16) NOT NULL,
  `Numero_Habitacion` VARCHAR(10) NOT NULL UNIQUE,
  `Piso` SMALLINT NOT NULL,
  `Estado_Habitacion` ENUM('Libre', 'Reservada', 'Ocupada', 'Mantenimiento', 'Fuera de Servicio') NOT NULL DEFAULT 'Libre',
  PRIMARY KEY (`ID`),
  INDEX `fk_Habitaciones_Tipos_idx` (`Tipo_Habitacion_ID` ASC),
  CONSTRAINT `fk_Habitaciones_Tipos`
    FOREIGN KEY (`Tipo_Habitacion_ID`)
    REFERENCES `Tipo_Habitacion` (`ID`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabla Detalle_Reserva
CREATE TABLE `Detalle_Reserva` (
  `ID` BINARY(16) NOT NULL,
  `Reserva_ID` BINARY(16) NOT NULL,
  `Habitacion_ID` BINARY(16) NOT NULL,
  `Huesped_ID` BINARY(16) NOT NULL,
  `Precio_Total` DECIMAL(10, 2) NOT NULL,
  `Cantidad_Huespedes` TINYINT UNSIGNED NOT NULL,
  PRIMARY KEY (`ID`),
  INDEX `fk_Detalle_Reservas_idx` (`Reserva_ID` ASC),
  INDEX `fk_Detalle_Habitaciones_idx` (`Habitacion_ID` ASC),
  INDEX `fk_Detalle_Huespedes_idx` (`Huesped_ID` ASC),
  CONSTRAINT `fk_Detalle_Reservas`
    FOREIGN KEY (`Reserva_ID`)
    REFERENCES `Reserva` (`ID`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_Detalle_Habitaciones`
    FOREIGN KEY (`Habitacion_ID`)
    REFERENCES `Habitacion` (`ID`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_Detalle_Huespedes`
    FOREIGN KEY (`Huesped_ID`)
    REFERENCES `Huesped` (`ID`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Datos de prueba

-- Insertar tipos de habitación de ejemplo
INSERT INTO `Tipo_Habitacion` (`ID`, `Nombre_Tipo`, `Descripcion`, `Capacidad_Maxima`, `Precio_Base`) VALUES
(UUID_TO_BIN('11111111-1111-1111-1111-111111111111'), 'Simple', 'Habitación individual con cama simple', 1, 100.00),
(UUID_TO_BIN('22222222-2222-2222-2222-222222222222'), 'Doble', 'Habitación con cama matrimonial', 2, 150.00),
(UUID_TO_BIN('33333333-3333-3333-3333-333333333333'), 'Suite', 'Suite de lujo con sala de estar', 4, 350.00);

-- Insertar habitaciones de ejemplo
INSERT INTO `Habitacion` (`ID`, `Tipo_Habitacion_ID`, `Numero_Habitacion`, `Piso`, `Estado_Habitacion`) VALUES
(UUID_TO_BIN('44444444-4444-4444-4444-444444444444'), UUID_TO_BIN('11111111-1111-1111-1111-111111111111'), '101', 1, 'Libre'),
(UUID_TO_BIN('55555555-5555-5555-5555-555555555555'), UUID_TO_BIN('22222222-2222-2222-2222-222222222222'), '201', 2, 'Libre'),
(UUID_TO_BIN('66666666-6666-6666-6666-666666666666'), UUID_TO_BIN('33333333-3333-3333-3333-333333333333'), '301', 3, 'Libre');

-- Insertar clientes de ejemplo
INSERT INTO `Cliente` (`ID`, `Razon_Social`, `NIT`, `Email`) VALUES
(UUID_TO_BIN('77777777-7777-7777-7777-777777777777'), 'Hotel Corp', '1234567890', 'cliente1@example.com'),
(UUID_TO_BIN('88888888-8888-8888-8888-888888888888'), 'Turismo SA', '0987654321', 'cliente2@example.com');

-- Insertar huéspedes de ejemplo
INSERT INTO `Huesped` (`ID`, `Nombre_Completo`, `Documento_Identidad`, `Telefono`, `Email`, `Fecha_Nacimiento`) VALUES
(UUID_TO_BIN('99999999-9999-9999-9999-999999999999'), 'Juan Pérez', '12345678', '555-1234', 'juan@example.com', '1990-01-15'),
(UUID_TO_BIN('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'), 'María García', '87654321', '555-5678', 'maria@example.com', '1985-05-20');

-- Insertar reservas de ejemplo
INSERT INTO `Reserva` (`ID`, `Cliente_ID`, `Fecha_Reserva`, `Fecha_Entrada`, `Fecha_Salida`, `Estado_Reserva`, `Monto_Total`) VALUES
(UUID_TO_BIN('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'), UUID_TO_BIN('77777777-7777-7777-7777-777777777777'), NOW(), '2025-11-01', '2025-11-05', 'Confirmada', 600.00),
(UUID_TO_BIN('cccccccc-cccc-cccc-cccc-cccccccccccc'), UUID_TO_BIN('88888888-8888-8888-8888-888888888888'), NOW(), '2025-12-15', '2025-12-20', 'Pendiente', 750.00);

-- Insertar detalles de reserva de ejemplo
INSERT INTO `Detalle_Reserva` (`ID`, `Reserva_ID`, `Habitacion_ID`, `Huesped_ID`, `Precio_Total`, `Cantidad_Huespedes`) VALUES
(UUID_TO_BIN('dddddddd-dddd-dddd-dddd-dddddddddddd'), UUID_TO_BIN('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb'), UUID_TO_BIN('44444444-4444-4444-4444-444444444444'), UUID_TO_BIN('99999999-9999-9999-9999-999999999999'), 400.00, 1),
(UUID_TO_BIN('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee'), UUID_TO_BIN('cccccccc-cccc-cccc-cccc-cccccccccccc'), UUID_TO_BIN('55555555-5555-5555-5555-555555555555'), UUID_TO_BIN('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa'), 750.00, 2);