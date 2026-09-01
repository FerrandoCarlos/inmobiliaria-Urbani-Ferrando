-- =====================================================
-- InmobiliariaApp - Datos de prueba (opcional)
-- Correr DESPUÉS de script_inicial.sql para cargar un
-- volumen mayor de datos (20 registros por entidad),
-- útil para probar paginación, filtros y listados.
-- =====================================================

USE inmobiliaria_db;

-- Limpia los datos mínimos de ejemplo antes de cargar el set grande
DELETE FROM reserva;
DELETE FROM imagenesInmueble;
DELETE FROM inmueble;
DELETE FROM Inquilino;
DELETE FROM Propietario;
ALTER TABLE Propietario AUTO_INCREMENT = 1;
ALTER TABLE Inquilino AUTO_INCREMENT = 1;
ALTER TABLE inmueble AUTO_INCREMENT = 1;
ALTER TABLE reserva AUTO_INCREMENT = 1;

-- 1. 20 Propietarios
INSERT INTO Propietario (Dni, Nombre, Apellido, Telefono, Email, Activo) VALUES
('10000001', 'Juan', 'Pérez', '2664000001', 'juan.perez@email.com', 1),
('10000002', 'María', 'Gómez', '2664000002', 'maria.gomez@email.com', 1),
('10000003', 'Carlos', 'López', '2664000003', 'carlos.lopez@email.com', 1),
('10000004', 'Ana', 'Martínez', '2664000004', 'ana.martinez@email.com', 1),
('10000005', 'Luis', 'Rodríguez', '2664000005', 'luis.rodriguez@email.com', 1),
('10000006', 'Sofia', 'Fernández', '2664000006', 'sofia.fernandez@email.com', 1),
('10000007', 'Diego', 'González', '2664000007', 'diego.gonzalez@email.com', 1),
('10000008', 'Laura', 'Sánchez', '2664000008', 'laura.sanchez@email.com', 1),
('10000009', 'Javier', 'Díaz', '2664000009', 'javier.diaz@email.com', 1),
('10000010', 'Lucía', 'Álvarez', '2664000010', 'lucia.alvarez@email.com', 1),
('10000011', 'Martín', 'Romero', '2664000011', 'martin.romero@email.com', 1),
('10000012', 'Elena', 'Sosa', '2664000012', 'elena.sosa@email.com', 1),
('10000013', 'Pablo', 'Torres', '2664000013', 'pablo.torres@email.com', 1),
('10000014', 'Valeria', 'Ruiz', '2664000014', 'valeria.ruiz@email.com', 1),
('10000015', 'Gonzalo', 'Ramírez', '2664000015', 'gonzalo.ramirez@email.com', 1),
('10000016', 'Camila', 'Flores', '2664000016', 'camila.flores@email.com', 1),
('10000017', 'Nicolás', 'Benítez', '2664000017', 'nicolas.benitez@email.com', 1),
('10000018', 'Agustina', 'Acosta', '2664000018', 'agustina.acosta@email.com', 1),
('10000019', 'Federico', 'Medina', '2664000019', 'federico.medina@email.com', 1),
('10000020', 'Valentina', 'Herrera', '2664000020', 'valentina.herrera@email.com', 1);

-- 2. 20 Inquilinos
INSERT INTO Inquilino (Dni, Nombre, Apellido, Telefono, Email, Activo) VALUES
('20000001', 'Roberto', 'Castillo', '2664110001', 'roberto.c@email.com', 1),
('20000002', 'Patricia', 'Castro', '2664110002', 'patricia.c@email.com', 1),
('20000003', 'Fernando', 'Suárez', '2664110003', 'fernando.s@email.com', 1),
('20000004', 'Daniela', 'Blanco', '2664110004', 'daniela.b@email.com', 1),
('20000005', 'Gabriel', 'Morales', '2664110005', 'gabriel.m@email.com', 1),
('20000006', 'Carolina', 'Ortega', '2664110006', 'carolina.o@email.com', 1),
('20000007', 'Adrian', 'Delgado', '2664110007', 'adrian.d@email.com', 1),
('20000008', 'Mariana', 'Castro', '2664110008', 'mariana.c@email.com', 1),
('20000009', 'Esteban', 'Ortiz', '2664110009', 'esteban.o@email.com', 1),
('20000010', 'Florencia', 'Marín', '2664110010', 'florencia.m@email.com', 1),
('20000011', 'Ignacio', 'Rubio', '2664110011', 'ignacio.r@email.com', 1),
('20000012', 'Natalia', 'Núñez', '2664110012', 'natalia.n@email.com', 1),
('20000013', 'Hugo', 'Iglesias', '2664110013', 'hugo.i@email.com', 1),
('20000014', 'Sabrina', 'Sanz', '2664110014', 'sabrina.s@email.com', 1),
('20000015', 'Joaquín', 'Molina', '2664110015', 'joaquin.m@email.com', 1),
('20000016', 'Victoria', 'Vidal', '2664110016', 'victoria.v@email.com', 1),
('20000017', 'Matías', 'Cano', '2664110017', 'matias.c@email.com', 1),
('20000018', 'Paula', 'Gil', '2664110018', 'paula.g@email.com', 1),
('20000019', 'Ezequiel', 'Vázquez', '2664110019', 'ezequiel.v@email.com', 1),
('20000020', 'Sol', 'Ramos', '2664110020', 'sol.ramos@email.com', 1);

-- 3. 20 Inmuebles
INSERT INTO inmueble (PropietarioId, ImgPortadaURL, Cupo, Direccion, Tipo, Latitud, Longitud, Activo, PrecioXDia, Estado, PorcentajeReserva) VALUES
(1, 'https://picsum.photos/400/300?id=1', 4, 'Av. Illia 120', 'Departamento', -33.2980, -66.3350, 1, 15000.00, 'Disponible', 20.00),
(2, 'https://picsum.photos/400/300?id=2', 6, 'Calle Rivadavia 450', 'Casa', -33.2991, -66.3361, 1, 28000.00, 'Disponible', 30.00),
(3, 'https://picsum.photos/400/300?id=3', 2, 'San Martín 780', 'Monoambiente', -33.3010, -66.3375, 1, 10000.00, 'Disponible', 15.00),
(4, 'https://picsum.photos/400/300?id=4', 5, 'Belgrano 1100', 'Casa', -33.3025, -66.3380, 1, 24000.00, 'Disponible', 25.00),
(5, 'https://picsum.photos/400/300?id=5', 3, 'Pringles 320', 'Departamento', -33.3032, -66.3392, 1, 13500.00, 'Disponible', 20.00),
(6, 'https://picsum.photos/400/300?id=6', 4, 'Chacabuco 890', 'Departamento', -33.3040, -66.3401, 1, 16000.00, 'Disponible', 20.00),
(7, 'https://picsum.photos/400/300?id=7', 8, 'Av. España 1500', 'Cabaña', -33.3051, -66.3415, 1, 35000.00, 'Disponible', 30.00),
(8, 'https://picsum.photos/400/300?id=8', 2, 'Mitre 540', 'Monoambiente', -33.3060, -66.3420, 1, 11000.00, 'Disponible', 15.00),
(9, 'https://picsum.photos/400/300?id=9', 4, 'Junín 230', 'Departamento', -33.3072, -66.3431, 1, 17500.00, 'Disponible', 20.00),
(10, 'https://picsum.photos/400/300?id=10', 6, 'Lavalle 670', 'Casa', -33.3080, -66.3440, 1, 29000.00, 'Disponible', 25.00),
(11, 'https://picsum.photos/400/300?id=11', 3, 'Colón 1050', 'Departamento', -33.3091, -66.3452, 1, 14000.00, 'Disponible', 20.00),
(12, 'https://picsum.photos/400/300?id=12', 5, 'Bolívar 410', 'Casa', -33.3100, -66.3460, 1, 23000.00, 'Disponible', 25.00),
(13, 'https://picsum.photos/400/300?id=13', 2, 'Ayacucho 930', 'Monoambiente', -33.3112, -66.3471, 1, 9500.00, 'Disponible', 15.00),
(14, 'https://picsum.photos/400/300?id=14', 4, 'Lafinur 1400', 'Departamento', -33.3120, -66.3480, 1, 18000.00, 'Disponible', 20.00),
(15, 'https://picsum.photos/400/300?id=15', 7, 'Av. del Viento 200', 'Cabaña', -33.3131, -66.3492, 1, 32000.00, 'Disponible', 30.00),
(16, 'https://picsum.photos/400/300?id=16', 3, 'Constitución 340', 'Departamento', -33.3140, -66.3501, 1, 13000.00, 'Disponible', 20.00),
(17, 'https://picsum.photos/400/300?id=17', 5, 'Ascasubi 510', 'Casa', -33.3152, -66.3510, 1, 26000.00, 'Disponible', 25.00),
(18, 'https://picsum.photos/400/300?id=18', 2, 'Caseros 1120', 'Monoambiente', -33.3160, -66.3522, 1, 10500.00, 'Disponible', 15.00),
(19, 'https://picsum.photos/400/300?id=19', 4, 'Sucre 760', 'Departamento', -33.3171, -66.3530, 1, 16500.00, 'Disponible', 20.00),
(20, 'https://picsum.photos/400/300?id=20', 6, 'San Luis 980', 'Casa', -33.3180, -66.3541, 1, 30000.00, 'Disponible', 30.00);

-- 4. Imágenes de Inmuebles
INSERT INTO imagenesInmueble (InmuebleId, ImgURL) VALUES
(1, 'https://picsum.photos/800/600?id=101'), (1, 'https://picsum.photos/800/600?id=102'),
(2, 'https://picsum.photos/800/600?id=103'), (2, 'https://picsum.photos/800/600?id=104'),
(3, 'https://picsum.photos/800/600?id=105'), (4, 'https://picsum.photos/800/600?id=106'),
(5, 'https://picsum.photos/800/600?id=107'), (6, 'https://picsum.photos/800/600?id=108'),
(7, 'https://picsum.photos/800/600?id=109'), (8, 'https://picsum.photos/800/600?id=110'),
(9, 'https://picsum.photos/800/600?id=111'), (10, 'https://picsum.photos/800/600?id=112'),
(11, 'https://picsum.photos/800/600?id=113'), (12, 'https://picsum.photos/800/600?id=114'),
(13, 'https://picsum.photos/800/600?id=115'), (14, 'https://picsum.photos/800/600?id=116'),
(15, 'https://picsum.photos/800/600?id=117'), (16, 'https://picsum.photos/800/600?id=118'),
(17, 'https://picsum.photos/800/600?id=119'), (18, 'https://picsum.photos/800/600?id=120');

-- 5. 20 Reservas
INSERT INTO reserva (InquilinoId, InmuebleId, FechaDesde, FechaHasta, MontoPorDia, Multa, Estado, FechaCreacion) VALUES
(1, 1, '2026-09-01', '2026-09-05', 15000.00, NULL, 'Vigente', NOW()),
(2, 2, '2026-09-02', '2026-09-07', 28000.00, NULL, 'Vigente', NOW()),
(3, 3, '2026-09-05', '2026-09-10', 10000.00, NULL, 'Vigente', NOW()),
(4, 4, '2026-09-10', '2026-09-15', 24000.00, NULL, 'Vigente', NOW()),
(5, 5, '2026-09-12', '2026-09-14', 13500.00, NULL, 'Vigente', NOW()),
(6, 6, '2026-09-15', '2026-09-20', 16000.00, NULL, 'Vigente', NOW()),
(7, 7, '2026-09-18', '2026-09-25', 35000.00, NULL, 'Vigente', NOW()),
(8, 8, '2026-09-20', '2026-09-22', 11000.00, NULL, 'Vigente', NOW()),
(9, 9, '2026-09-21', '2026-09-26', 17500.00, NULL, 'Vigente', NOW()),
(10, 10, '2026-09-25', '2026-09-30', 29000.00, NULL, 'Vigente', NOW()),
(11, 11, '2026-10-01', '2026-10-05', 14000.00, NULL, 'Vigente', NOW()),
(12, 12, '2026-10-03', '2026-10-08', 23000.00, NULL, 'Vigente', NOW()),
(13, 13, '2026-10-05', '2026-10-07', 9500.00, NULL, 'Vigente', NOW()),
(14, 14, '2026-10-10', '2026-10-15', 18000.00, NULL, 'Vigente', NOW()),
(15, 15, '2026-10-12', '2026-10-19', 32000.00, NULL, 'Vigente', NOW()),
(16, 16, '2026-10-15', '2026-10-18', 13000.00, NULL, 'Vigente', NOW()),
(17, 17, '2026-10-20', '2026-10-25', 26000.00, NULL, 'Vigente', NOW()),
(18, 18, '2026-10-22', '2026-10-24', 10500.00, NULL, 'Vigente', NOW()),
(19, 19, '2026-10-25', '2026-10-30', 16500.00, NULL, 'Vigente', NOW()),
(20, 20, '2026-11-01', '2026-11-06', 30000.00, NULL, 'Vigente', NOW());
