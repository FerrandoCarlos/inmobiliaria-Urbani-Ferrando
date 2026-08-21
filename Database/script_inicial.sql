CREATE DATABASE IF NOT EXISTS inmobiliaria_db
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE inmobiliaria_db;
DROP TABLE IF EXISTS Propietario;

CREATE TABLE Propietario (
    Id             INT AUTO_INCREMENT PRIMARY KEY,
    Dni            VARCHAR(15)   NOT NULL,
    Nombre         VARCHAR(100)  NOT NULL,
    Apellido       VARCHAR(100)  NOT NULL,
    Telefono       VARCHAR(20)   NULL,
    Email          VARCHAR(150)  NULL,
    Activo         TINYINT(1)    NOT NULL DEFAULT 1,
    FechaCreacion  DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT UQ_Propietario_Dni UNIQUE (Dni)
) ENGINE = INNODB;

DROP TABLE IF EXISTS Inquilino;

CREATE TABLE Inquilino (
    Id             INT AUTO_INCREMENT PRIMARY KEY,
    Dni            VARCHAR(15)   NOT NULL,
    Nombre         VARCHAR(100)  NOT NULL,
    Apellido       VARCHAR(100)  NOT NULL,
    Telefono       VARCHAR(20)   NULL,
    Email          VARCHAR(150)  NULL,
    Activo         TINYINT(1)    NOT NULL DEFAULT 1,
    FechaCreacion  DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT UQ_Inquilino_Dni UNIQUE (Dni)
) ENGINE = InnoDB;
-- Aceleran búsquedas por apellido, típico filtro en listados ABM
CREATE INDEX IX_Propietario_Apellido ON Propietario (Apellido);
CREATE INDEX IX_Inquilino_Apellido ON Inquilino (Apellido);
 INSERT INTO Propietario (Dni, Nombre, Apellido, Telefono, Email, Activo) VALUES
('30111222', 'Marcelo', 'Fernandez', '3814001122', 'marcelo.fernandez@mail.com', 1),
('28555666', 'Laura',   'Gimenez',   '3814003344', 'laura.gimenez@mail.com', 1),
('35999888', 'Ricardo', 'Suarez',    '3814005566', 'ricardo.suarez@mail.com', 0);

INSERT INTO Inquilino (Dni, Nombre, Apellido, Telefono, Email, Activo) VALUES
('32444555', 'Ana',     'Lopez',   '3814007788', 'ana.lopez@mail.com', 1),
('29777111', 'Diego',   'Martinez','3814009900', 'diego.martinez@mail.com', 1),
('40123456', 'Carla',   'Rojas',   '3814012345', 'carla.rojas@mail.com', 1);
