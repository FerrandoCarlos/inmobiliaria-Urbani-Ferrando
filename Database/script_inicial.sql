-- =====================================================
-- InmobiliariaApp - Script de creación e inicialización
-- Grupo: Urbani - Ferrando
-- =====================================================

CREATE DATABASE IF NOT EXISTS inmobiliaria_db;
USE inmobiliaria_db;

-- Desactivar temporalmente el chequeo de claves foráneas para la recreación
SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS `pago`;
DROP TABLE IF EXISTS `reserva`;
DROP TABLE IF EXISTS `imagenesinmueble`;
DROP TABLE IF EXISTS `inmueble`;
DROP TABLE IF EXISTS `tipoinmueble`;
DROP TABLE IF EXISTS `usuario`;
DROP TABLE IF EXISTS `rol`;
DROP TABLE IF EXISTS `inquilino`;
DROP TABLE IF EXISTS `propietario`;

SET FOREIGN_KEY_CHECKS = 1;
-- =====================================================
-- 1. Roles y Usuarios
-- =====================================================
CREATE TABLE `rol` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Nombre` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `UX_Rol_Nombre` (`Nombre`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE `usuario` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Email` VARCHAR(150) NOT NULL,
  `PasswordHash` VARCHAR(255) NOT NULL,
  `Nombre` VARCHAR(100) NOT NULL,
  `Apellido` VARCHAR(100) NOT NULL,
  `Avatar` VARCHAR(255) NULL DEFAULT NULL,
  `RolId` INT NOT NULL,
  `Activo` TINYINT(1) NOT NULL DEFAULT 1,
  `FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `UX_Usuario_Email` (`Email`),
  KEY `IX_Usuario_RolId` (`RolId`),
  CONSTRAINT `fk_usuario_rol` FOREIGN KEY (`RolId`) REFERENCES `rol` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- 2. Propietario
-- =====================================================
CREATE TABLE `propietario` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Dni` VARCHAR(15) NOT NULL,
  `Nombre` VARCHAR(100) NOT NULL,
  `Apellido` VARCHAR(100) NOT NULL,
  `Telefono` VARCHAR(20) NULL DEFAULT NULL,
  `Email` VARCHAR(150) NULL DEFAULT NULL,
  `Activo` TINYINT(1) NOT NULL DEFAULT 1,
  `FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `UQ_Propietario_Dni` (`Dni`),
  KEY `IX_Propietario_Apellido` (`Apellido`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- 3. Inquilino
-- =====================================================
CREATE TABLE `inquilino` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Dni` VARCHAR(15) NOT NULL,
  `Nombre` VARCHAR(100) NOT NULL,
  `Apellido` VARCHAR(100) NOT NULL,
  `Telefono` VARCHAR(20) NULL DEFAULT NULL,
  `Email` VARCHAR(150) NULL DEFAULT NULL,
  `Activo` TINYINT(1) NOT NULL DEFAULT 1,
  `FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `UQ_Inquilino_Dni` (`Dni`),
  KEY `IX_Inquilino_Apellido` (`Apellido`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- 4. Tipo de Inmueble
-- =====================================================
CREATE TABLE `tipoinmueble` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Tipo` VARCHAR(50) NOT NULL,
  `Activo` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `UX_tipoInmueble_Tipo` (`Tipo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- 5. Inmueble
-- =====================================================
CREATE TABLE `inmueble` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `TipoInmuebleId` INT NOT NULL,
  `PropietarioId` INT NOT NULL,
  `ImgPortadaURL` VARCHAR(255) NULL DEFAULT NULL,
  `Cupo` INT NOT NULL,
  `Direccion` VARCHAR(255) NOT NULL,
  `PrecioXDia` DECIMAL(18,2) NOT NULL,
  `Estado` VARCHAR(50) NOT NULL DEFAULT 'Disponible',
  `PorcentajeReserva` DECIMAL(18,2) NOT NULL,
  `Latitud` DECIMAL(18,2) NOT NULL,
  `Longitud` DECIMAL(18,2) NOT NULL,
  `Activo` TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`),
  KEY `IX_Inmueble_PropietarioId` (`PropietarioId`),
  KEY `IX_Inmueble_TipoInmuebleId` (`TipoInmuebleId`),
  CONSTRAINT `fk_inmueble_propietario` FOREIGN KEY (`PropietarioId`) REFERENCES `propietario` (`Id`),
  CONSTRAINT `fk_inmueble_tipoinmueble` FOREIGN KEY (`TipoInmuebleId`) REFERENCES `tipoinmueble` (`Id`) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- 6. Imágenes de Inmueble
-- =====================================================
CREATE TABLE `imagenesinmueble` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `InmuebleId` INT NOT NULL,
  `ImgURL` VARCHAR(255) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ImagenesInmueble_InmuebleId` (`InmuebleId`),
  CONSTRAINT `fk_imagenesinmueble_inmueble` FOREIGN KEY (`InmuebleId`) REFERENCES `inmueble` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- 7. Reserva
-- =====================================================
CREATE TABLE `reserva` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `InquilinoId` INT NOT NULL,
  `InmuebleId` INT NOT NULL,
  `FechaDesde` DATE NOT NULL,
  `FechaHasta` DATE NOT NULL,
  `FechaTerminacion` DATE NULL DEFAULT NULL,
  `MontoPorDia` DECIMAL(10,2) NOT NULL,
  `Multa` DECIMAL(10,2) NULL DEFAULT NULL,
  `Estado` VARCHAR(20) NOT NULL DEFAULT 'Vigente',
  `FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreadoPorId` INT NOT NULL,
  `TerminadoPorId` INT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Reserva_InquilinoId` (`InquilinoId`),
  KEY `IX_Reserva_InmuebleId` (`InmuebleId`),
  KEY `IX_Reserva_CreadoPorId` (`CreadoPorId`),
  CONSTRAINT `fk_reserva_inquilino` FOREIGN KEY (`InquilinoId`) REFERENCES `inquilino` (`Id`),
  CONSTRAINT `fk_reserva_inmueble` FOREIGN KEY (`InmuebleId`) REFERENCES `inmueble` (`Id`),
  CONSTRAINT `fk_reserva_creado_por` FOREIGN KEY (`CreadoPorId`) REFERENCES `Usuario` (`Id`),
  CONSTRAINT `fk_reserva_terminado_por` FOREIGN KEY (`TerminadoPorId`) REFERENCES `Usuario` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- 8. Pago
-- =====================================================
CREATE TABLE `pago` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `ReservaId` INT NOT NULL,
  `Monto` DECIMAL(18,2) NOT NULL DEFAULT 0.00,
  `Concepto` VARCHAR(50) NOT NULL,
  `Estado` VARCHAR(50) NOT NULL,
  `Activo` TINYINT(1) NOT NULL DEFAULT 1,
  `Fecha` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `CreadoPorId` INT NOT NULL,
  `AnuladoPorId` INT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Pago_ReservaId` (`ReservaId`),
  KEY `IX_Pago_CreadoPorId` (`CreadoPorId`),
  CONSTRAINT `fk_pago_reserva` FOREIGN KEY (`ReservaId`) REFERENCES `reserva` (`Id`),
  CONSTRAINT `fk_pago_creado_por` FOREIGN KEY (`CreadoPorId`) REFERENCES `usuario` (`Id`),
  CONSTRAINT `fk_pago_anulado_por` FOREIGN KEY (`AnuladoPorId`) REFERENCES `usuario` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- Datos mínimos de ejemplo
-- (para un set más grande, correr Database/datos_prueba.sql)
-- =====================================================
INSERT INTO `rol` (`Id`, `Nombre`) VALUES
(1, 'Administrador'),
(2, 'Empleado');

INSERT INTO `usuario` (`Id`, `Email`, `PasswordHash`, `Nombre`, `Apellido`, `RolId`, `Activo`) VALUES
(1, 'admin@inmobiliaria.com', 'AQAAAAIAAYagAAAAEDVhH48zGJF825pq440yl/mDEyd0BqXVA8HimJQMP5uSJGRrZpNTVDY13j+WXYUglg==', 'Admin', 'Sistema', 1, 1),
(2, 'empleado@inmobiliaria.com', 'AQAAAAIAAYagAAAAEIW8p5wvA4GYEEFRysWZzlF8ph9ID9lUEB4CD+wyJSbOu2e+zsWLuitxNEoTbo3MpQ==', 'Empleado', 'Sistema', 2, 1);

INSERT INTO `tipoinmueble` (`Id`, `Tipo`, `Activo`) VALUES
(1, 'Departamento', 1),
(2, 'Casa', 1),
(3, 'Monoambiente', 1);
