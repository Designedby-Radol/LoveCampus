-- Eliminar base de datos si existe y crearla de nuevo
DROP DATABASE IF EXISTS campus_love;
CREATE DATABASE campus_love;
USE campus_love;

-- Tabla de géneros
CREATE TABLE generos (
    id INT PRIMARY KEY AUTO_INCREMENT,
    descripcion VARCHAR(50) NOT NULL UNIQUE
);

-- Tabla de carreras
CREATE TABLE carreras (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL UNIQUE
);

-- Tabla de intereses
CREATE TABLE intereses (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL UNIQUE
);

-- Tabla de usuarios
CREATE TABLE usuarios (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL,
    edad INT NOT NULL,
    genero_id INT NOT NULL,
    carrera_id INT NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    frase_perfil TEXT,
    creditos_disponibles INT DEFAULT 5,
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ultimo_acceso TIMESTAMP NULL,
    FOREIGN KEY (genero_id) REFERENCES generos(id),
    FOREIGN KEY (carrera_id) REFERENCES carreras(id)
);

-- Relación muchos a muchos entre usuarios e intereses
CREATE TABLE usuario_intereses (
    usuario_id INT,
    interes_id INT,
    PRIMARY KEY (usuario_id, interes_id),
    FOREIGN KEY (usuario_id) REFERENCES usuarios(id) ON DELETE CASCADE,
    FOREIGN KEY (interes_id) REFERENCES intereses(id) ON DELETE CASCADE
);

-- Tabla de interacciones (like/dislike)
CREATE TABLE interacciones (
    id INT PRIMARY KEY AUTO_INCREMENT,
    usuario_origen_id INT NOT NULL,
    usuario_destino_id INT NOT NULL,
    tipo ENUM('like', 'dislike') NOT NULL,
    fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (usuario_origen_id) REFERENCES usuarios(id),
    FOREIGN KEY (usuario_destino_id) REFERENCES usuarios(id)
);

-- Tabla de matches confirmados
CREATE TABLE matches (
    id INT PRIMARY KEY AUTO_INCREMENT,
    usuario1_id INT NOT NULL,
    usuario2_id INT NOT NULL,
    fecha_match TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    puntuacion_compatibilidad DECIMAL(5,2),
    FOREIGN KEY (usuario1_id) REFERENCES usuarios(id),
    FOREIGN KEY (usuario2_id) REFERENCES usuarios(id)
);

-- Tabla de factores y sus pesos
CREATE TABLE pesos_emparejamiento (
    id INT PRIMARY KEY AUTO_INCREMENT,
    factor VARCHAR(50) NOT NULL UNIQUE,
    peso DECIMAL(3,2) NOT NULL CHECK (peso >= 0 AND peso <= 1)
);

-- Crear índices para mejorar el rendimiento
CREATE INDEX idx_usuarios_email ON usuarios(email);
CREATE INDEX idx_usuarios_ultimo_acceso ON usuarios(ultimo_acceso);
CREATE INDEX idx_interacciones_usuarios ON interacciones(usuario_origen_id, usuario_destino_id);
CREATE INDEX idx_matches_usuarios ON matches(usuario1_id, usuario2_id);

-- Insertar datos de prueba

-- Géneros
INSERT INTO generos (descripcion) VALUES 
('Masculino'), 
('Femenino'), 
('Otro');

-- Carreras
INSERT INTO carreras (nombre) VALUES 
('Ingeniería de Sistemas'),
('Psicología'),
('Diseño Gráfico'),
('Medicina'),
('Derecho'),
('Administración de Empresas'),
('Arquitectura'),
('Contabilidad');

-- Intereses
INSERT INTO intereses (nombre) VALUES 
('Videojuegos'),
('Lectura'),
('Deportes'),
('Cine'),
('Música'),
('Fotografía'),
('Cocina'),
('Viajes'),
('Arte'),
('Tecnología'),
('Baile'),
('Yoga'),
('Programación'),
('Idiomas'),
('Voluntariado');

-- Pesos de emparejamiento
INSERT INTO pesos_emparejamiento (factor, peso) VALUES 
('edad', 0.25),
('genero', 0.10),
('carrera', 0.20),
('intereses', 0.30),
('frase', 0.15);

-- Usuarios de prueba (password: "test123")
INSERT INTO usuarios (nombre, edad, genero_id, carrera_id, email, password_hash, frase_perfil, creditos_disponibles) VALUES
('Juan Pérez', 20, 1, 1, 'juan@test.com', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Me gusta programar y jugar videojuegos', 5),
('María García', 21, 2, 2, 'maria@test.com', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Amante de la psicología y la música', 5),
('Carlos López', 22, 1, 3, 'carlos@test.com', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Diseñador gráfico y fotógrafo', 5),
('Ana Martínez', 19, 2, 4, 'ana@test.com', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Futura médica, amante de la vida', 5),
('Pedro Sánchez', 23, 1, 5, 'pedro@test.com', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Estudiante de derecho, buscando justicia', 5),
('Laura Torres', 20, 2, 6, 'laura@test.com', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Emprendedora y amante de los negocios', 5),
('Miguel Rodríguez', 21, 1, 7, 'miguel@test.com', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Arquitecto en formación, creativo y detallista', 5),
('Sofía Vargas', 22, 2, 8, 'sofia@test.com', '$2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', 'Contadora por vocación, organizada y precisa', 5);

-- Asignar intereses a usuarios
INSERT INTO usuario_intereses (usuario_id, interes_id) VALUES
-- Juan (Videojuegos, Programación, Tecnología)
(1, 1), (1, 13), (1, 10),
-- María (Música, Baile, Yoga)
(2, 5), (2, 11), (2, 12),
-- Carlos (Fotografía, Arte, Viajes)
(3, 6), (3, 9), (3, 8),
-- Ana (Cocina, Viajes, Voluntariado)
(4, 7), (4, 8), (4, 15),
-- Pedro (Lectura, Idiomas, Deportes)
(5, 2), (5, 14), (5, 3),
-- Laura (Tecnología, Programación, Deportes)
(6, 10), (6, 13), (6, 3),
-- Miguel (Arte, Fotografía, Viajes)
(7, 9), (7, 6), (7, 8),
-- Sofía (Música, Baile, Yoga)
(8, 5), (8, 11), (8, 12);

-- Crear algunos matches de prueba
INSERT INTO matches (usuario1_id, usuario2_id, puntuacion_compatibilidad) VALUES
(1, 6, 0.85), -- Juan y Laura (comparten interés en tecnología)
(2, 8, 0.90), -- María y Sofía (comparten interés en música y baile)
(3, 7, 0.80); -- Carlos y Miguel (comparten interés en arte y fotografía)

-- Crear algunas interacciones de prueba
INSERT INTO interacciones (usuario_origen_id, usuario_destino_id, tipo) VALUES
(1, 2, 'like'),
(1, 3, 'dislike'),
(2, 1, 'dislike'),
(2, 3, 'like'),
(3, 1, 'like'),
(3, 2, 'like'),
(4, 5, 'like'),
(5, 4, 'like'),
(6, 7, 'like'),
(7, 6, 'dislike'); 