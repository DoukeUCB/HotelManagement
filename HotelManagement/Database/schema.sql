DROP DATABASE IF EXISTS HotelDB;
CREATE DATABASE IF NOT EXISTS HotelDB;
USE HotelDB;

-- Usuario (Empleado)
CREATE TABLE `Usuario` (
  `ID` BINARY(16) NOT NULL,
  `Usuario` VARCHAR(50) NOT NULL UNIQUE,
  `Contrasenia` VARCHAR(255) NOT NULL,
  `Nombre` VARCHAR(50) NOT NULL,
  `Apellido` VARCHAR(50) NOT NULL,
  `Segundo_Apellido` VARCHAR(50),
  `Documento_Identidad` VARCHAR(20) NOT NULL,
  `Rol` ENUM('Administrador', 'Recepcionista') NOT NULL DEFAULT 'Recepcionista',
  `Activo` BOOLEAN DEFAULT 1,
  PRIMARY KEY (`ID`)
);

CREATE TABLE `Cliente` (
  `ID` BINARY(16) NOT NULL,
  `Razon_Social` VARCHAR(20) NOT NULL,
  `NIT` VARCHAR(20) NOT NULL,
  `Email` VARCHAR(30) NOT NULL UNIQUE,
  `Activo` BOOLEAN DEFAULT 1,
  -- Campos de auditoría
  `Fecha_Creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Fecha_Actualizacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Usuario_Creacion_ID` BINARY(16) NULL,
  `Usuario_Actualizacion_ID` BINARY(16) NULL,
  PRIMARY KEY (`ID`),
  CONSTRAINT `fk_Cliente_Usuario_Creacion` FOREIGN KEY (`Usuario_Creacion_ID`) REFERENCES `Usuario`(`ID`) ON DELETE SET NULL,
  CONSTRAINT `fk_Cliente_Usuario_Actualizacion` FOREIGN KEY (`Usuario_Actualizacion_ID`) REFERENCES `Usuario`(`ID`) ON DELETE SET NULL
);

CREATE TABLE `Tipo_Habitacion` (
  `ID` BINARY(16) NOT NULL,
  `Nombre` VARCHAR(50) NOT NULL UNIQUE,
  `Descripcion` TEXT NULL,
  `Capacidad_Maxima` TINYINT UNSIGNED NOT NULL,
  `Precio_Base` DECIMAL(10, 2) NOT NULL,
  `Activo` BOOLEAN DEFAULT 1,
  -- Campos de auditoría
  `Fecha_Creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Fecha_Actualizacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Usuario_Creacion_ID` BINARY(16) NULL,
  `Usuario_Actualizacion_ID` BINARY(16) NULL,
  PRIMARY KEY (`ID`),
  CONSTRAINT `fk_TipoHabitacion_Usuario_Creacion` FOREIGN KEY (`Usuario_Creacion_ID`) REFERENCES `Usuario`(`ID`) ON DELETE SET NULL,
  CONSTRAINT `fk_TipoHabitacion_Usuario_Actualizacion` FOREIGN KEY (`Usuario_Actualizacion_ID`) REFERENCES `Usuario`(`ID`) ON DELETE SET NULL
);

CREATE TABLE `Huesped` (
  `ID` BINARY(16) NOT NULL,
  `Nombre` VARCHAR(30) NOT NULL,
  `Apellido` VARCHAR(30) NOT NULL,
  `Segundo_Apellido` VARCHAR(30), -- Columna renombrada
  `Documento_Identidad` VARCHAR(20) NOT NULL,
  `Telefono` VARCHAR(20) NULL,
  `Fecha_Nacimiento` DATE NULL,
  `Activo` BOOLEAN DEFAULT 1,
  -- Campos de auditoría
  `Fecha_Creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Fecha_Actualizacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Usuario_Creacion_ID` BINARY(16) NULL,
  `Usuario_Actualizacion_ID` BINARY(16) NULL,
  PRIMARY KEY (`ID`),
  CONSTRAINT `fk_Huesped_Usuario_Creacion` FOREIGN KEY (`Usuario_Creacion_ID`) REFERENCES `Usuario`(`ID`) ON DELETE SET NULL,
  CONSTRAINT `fk_Huesped_Usuario_Actualizacion` FOREIGN KEY (`Usuario_Actualizacion_ID`) REFERENCES `Usuario`(`ID`) ON DELETE SET NULL
);

CREATE TABLE `Reserva` (
  `ID` BINARY(16) NOT NULL,
  `Cliente_ID` BINARY(16) NOT NULL,
  `Estado_Reserva` ENUM('Pendiente', 'Confirmada', 'Cancelada') NOT NULL DEFAULT 'Pendiente',
  `Monto_Total` DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
  -- Campos de auditoría
  `Fecha_Creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Usuario_Creacion_ID` BINARY(16) NULL,
  PRIMARY KEY (`ID`),
  INDEX `fk_Reservas_Clientes_idx` (`Cliente_ID` ASC),
  CONSTRAINT `fk_Reservas_Clientes`
    FOREIGN KEY (`Cliente_ID`)
    REFERENCES `Cliente` (`ID`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_Reserva_Usuario_Creacion` FOREIGN KEY (`Usuario_Creacion_ID`) REFERENCES `Usuario`(`ID`) ON DELETE SET NULL
) ;

CREATE TABLE `Habitacion` (
  `ID` BINARY(16) NOT NULL,
  `Tipo_Habitacion_ID` BINARY(16) NOT NULL,
  `Numero_Habitacion` VARCHAR(10) NOT NULL UNIQUE,
  `Piso` SMALLINT NOT NULL,
  `Estado_Habitacion` ENUM('Libre', 'Reservada', 'Ocupada', 'Fuera de Servicio') NOT NULL DEFAULT 'Libre',
  -- Campos de auditoría
  `Fecha_Creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Fecha_Actualizacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Usuario_Creacion_ID` BINARY(16) NULL,
  `Usuario_Actualizacion_ID` BINARY(16) NULL,
  PRIMARY KEY (`ID`),
  INDEX `fk_Habitaciones_Tipos_idx` (`Tipo_Habitacion_ID` ASC),
  CONSTRAINT `fk_Habitaciones_Tipos`
    FOREIGN KEY (`Tipo_Habitacion_ID`)
    REFERENCES `Tipo_Habitacion` (`ID`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `fk_Habitacion_Usuario_Creacion` FOREIGN KEY (`Usuario_Creacion_ID`) REFERENCES `Usuario`(`ID`) ON DELETE SET NULL,
  CONSTRAINT `fk_Habitacion_Usuario_Actualizacion` FOREIGN KEY (`Usuario_Actualizacion_ID`) REFERENCES `Usuario`(`ID`) ON DELETE SET NULL
);

CREATE TABLE `Detalle_Reserva` (
  `ID` BINARY(16) NOT NULL,
  `Reserva_ID` BINARY(16) NOT NULL,
  `Habitacion_ID` BINARY(16) NOT NULL,
  `Huesped_ID` BINARY(16) NOT NULL,
  `Fecha_Entrada` DATE NOT NULL,
  `Fecha_Salida` DATE NOT NULL,
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
);