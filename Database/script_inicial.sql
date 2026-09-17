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
  PRIMARY KEY (`Id`),
  KEY `IX_Reserva_InquilinoId` (`InquilinoId`),
  KEY `IX_Reserva_InmuebleId` (`InmuebleId`),
  CONSTRAINT `fk_reserva_inquilino` FOREIGN KEY (`InquilinoId`) REFERENCES `inquilino` (`Id`),
  CONSTRAINT `fk_reserva_inmueble` FOREIGN KEY (`InmuebleId`) REFERENCES `inmueble` (`Id`)
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
  PRIMARY KEY (`Id`),
  KEY `IX_Pago_ReservaId` (`ReservaId`),
  CONSTRAINT `fk_pago_reserva` FOREIGN KEY (`ReservaId`) REFERENCES `reserva` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- Datos mínimos de ejemplo
-- (para un set más grande, correr Database/datos_prueba.sql)
-- =====================================================
INSERT INTO `rol` (`Id`, `Nombre`) VALUES
(1, 'Administrador'),
(2, 'Empleado');

INSERT INTO `tipoinmueble` (`Id`, `Tipo`, `Activo`) VALUES
(1, 'Departamento', 1),
(2, 'Casa', 1),
(3, 'Monoambiente', 1);

INSERT INTO Propietario (Dni, Nombre, Apellido, Telefono, Email, Activo) VALUES
('30111222', 'Marcelo', 'Fernandez', '3814001122', 'marcelo.fernandez@mail.com', 1),
('28555666', 'Laura', 'Gimenez', '3814003344', 'laura.gimenez@mail.com', 1),
('29887766', 'Silvina', 'Castro', '3814991122', 's.castro@outlook.com', 1);

INSERT INTO Inquilino (Dni, Nombre, Apellido, Telefono, Email, Activo) VALUES
('32444555', 'Ana', 'Lopez', '3814007788', 'ana.lopez1@mail.com', 1),
('29777111', 'Diego', 'Martinez', '3814009900', 'diego.martinez@mail.com', 1),
('40123456', 'Carla', 'Rojas', '3814012345', 'carla.rojas@mail.com', 1);

INSERT INTO inmueble (PropietarioId, ImgPortadaURL, Cupo, Direccion, Tipo, Latitud, Longitud, Activo, PrecioXDia, Estado, PorcentajeReserva) VALUES
(1, 'https://picsum.photos/400/300?id=1', 4, 'Av. Illia 120', 'Departamento', -33.2980, -66.3350, 1, 15000.00, 'Disponible', 20.00),
(2, 'https://picsum.photos/400/300?id=2', 6, 'Calle Rivadavia 450', 'Casa', -33.2991, -66.3361, 1, 28000.00, 'Disponible', 30.00),
(3, 'https://picsum.photos/400/300?id=3', 2, 'San Martín 780', 'Monoambiente', -33.3010, -66.3375, 1, 10000.00, 'Disponible', 15.00);

INSERT INTO reserva (InquilinoId, InmuebleId, FechaDesde, FechaHasta, MontoPorDia, Estado, FechaCreacion) VALUES
(1, 1, '2026-09-01', '2026-09-05', 15000.00, 'Vigente', NOW()),
(2, 2, '2026-09-10', '2026-09-15', 28000.00, 'Vigente', NOW());

INSERT INTO `pago` (`ReservaId`, `Monto`, `Concepto`, `Estado`, `Activo`) VALUES
(1, 15000.00, 'Seña de reserva', 'Aprobado', 1);
