-- =====================================================
-- InmobiliariaApp - Script de creación e inicialización
-- Grupo: Urbani - Ferrando
-- =====================================================

CREATE DATABASE IF NOT EXISTS inmobiliaria_db;
USE inmobiliaria_db;

-- =====================================================
-- 1. Propietario
-- =====================================================
DROP TABLE IF EXISTS Reserva;
DROP TABLE IF EXISTS imagenesInmueble;
DROP TABLE IF EXISTS inmueble;
DROP TABLE IF EXISTS Inquilino;
DROP TABLE IF EXISTS Propietario;

CREATE TABLE Propietario (
    Id             INT AUTO_INCREMENT PRIMARY KEY,
    Dni            VARCHAR(15)   NOT NULL,
    Nombre         VARCHAR(100)  NOT NULL,
    Apellido       VARCHAR(100)  NOT NULL,
    Telefono       VARCHAR(20)   NULL,
    Email          VARCHAR(150)  NOT NULL,
    Activo         TINYINT(1)    NOT NULL DEFAULT 1,
    FechaCreacion  DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT UQ_Propietario_Dni UNIQUE (Dni)
);

-- =====================================================
-- 2. Inquilino
-- =====================================================
CREATE TABLE Inquilino (
    Id             INT AUTO_INCREMENT PRIMARY KEY,
    Dni            VARCHAR(15)   NOT NULL,
    Nombre         VARCHAR(100)  NOT NULL,
    Apellido       VARCHAR(100)  NOT NULL,
    Telefono       VARCHAR(20)   NULL,
    Email          VARCHAR(150)  NOT NULL,
    Activo         TINYINT(1)    NOT NULL DEFAULT 1,
    FechaCreacion  DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT UQ_Inquilino_Dni UNIQUE (Dni)
);

-- Aceleran búsquedas por apellido, típico filtro en listados ABM
CREATE INDEX IX_Propietario_Apellido ON Propietario (Apellido);
CREATE INDEX IX_Inquilino_Apellido ON Inquilino (Apellido);

-- =====================================================
-- 3. Inmueble
-- =====================================================
CREATE TABLE inmueble (
    Id                 INT AUTO_INCREMENT PRIMARY KEY,
    PropietarioId      INT NOT NULL,
    ImgPortadaURL      VARCHAR(255),
    Cupo               INT NOT NULL,
    Direccion          VARCHAR(255) NOT NULL,
    Tipo               VARCHAR(50) NOT NULL,
    Latitud            DECIMAL(18,2) NOT NULL,
    Longitud           DECIMAL(18,2) NOT NULL,
    Activo             TINYINT(1) NOT NULL DEFAULT 1,
    PrecioXDia         DECIMAL(18,2) NOT NULL,
    Estado             VARCHAR(50) NOT NULL DEFAULT 'Disponible',
    PorcentajeReserva  DECIMAL(18,2) NOT NULL,
    CONSTRAINT fk_inmueble_propietario FOREIGN KEY (PropietarioId) REFERENCES Propietario(Id)
);

-- =====================================================
-- 4. ImagenesInmueble
-- =====================================================
CREATE TABLE imagenesInmueble (
    Id          INT AUTO_INCREMENT PRIMARY KEY,
    InmuebleId  INT NOT NULL,
    ImgURL      VARCHAR(255),
    CONSTRAINT fk_imagen_inmueble FOREIGN KEY (InmuebleId) REFERENCES inmueble(Id)
);

-- =====================================================
-- 5. Reserva
-- =====================================================
CREATE TABLE reserva (
    Id                INT AUTO_INCREMENT PRIMARY KEY,
    InquilinoId       INT NOT NULL,
    InmuebleId        INT NOT NULL,
    FechaDesde        DATE NOT NULL,
    FechaHasta        DATE NOT NULL,
    FechaTerminacion  DATE NULL,
    MontoPorDia       DECIMAL(10,2) NOT NULL,
    Multa             DECIMAL(10,2) NULL,
    Estado            VARCHAR(20) NOT NULL DEFAULT 'Vigente',
    FechaCreacion     DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_reserva_inquilino FOREIGN KEY (InquilinoId) REFERENCES Inquilino(Id),
    CONSTRAINT fk_reserva_inmueble FOREIGN KEY (InmuebleId) REFERENCES inmueble(Id)
);

-- =====================================================
-- Datos mínimos de ejemplo
-- (para un set más grande, correr Database/datos_prueba.sql)
-- =====================================================

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
