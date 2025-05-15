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
    password VARCHAR(50) NOT NULL,
    frase_perfil TEXT,
    creditos_disponibles INT DEFAULT 5,
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ultimo_acceso TIMESTAMP NULL,
    rol ENUM('usuario', 'admin') NOT NULL DEFAULT 'usuario',
    FOREIGN KEY (genero_id) REFERENCES generos(id),
    FOREIGN KEY (carrera_id) REFERENCES carreras(id)
);

-- Tabla de accesos de usuario (historial de accesos)
CREATE TABLE accesos_usuario (
    id INT PRIMARY KEY AUTO_INCREMENT,
    usuario_id INT NOT NULL,
    fecha_acceso TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (usuario_id) REFERENCES usuarios(id)
);

-- Tabla de log de interacciones (like/dislike)
CREATE TABLE log_interacciones (
    id INT PRIMARY KEY AUTO_INCREMENT,
    usuario_origen_id INT NOT NULL,
    usuario_destino_id INT NOT NULL,
    tipo ENUM('like', 'dislike') NOT NULL,
    fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (usuario_origen_id) REFERENCES usuarios(id),
    FOREIGN KEY (usuario_destino_id) REFERENCES usuarios(id)
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
INSERT INTO generos (descripcion) VALUES ('Masculino'), ('Femenino'), ('Otro');

-- Carreras
INSERT INTO carreras (nombre) VALUES 
('Ingeniería en Sistemas'),
('Psicología'),
('Administración de Empresas'),
('Derecho'),
('Medicina'),
('Arquitectura');

-- Intereses
INSERT INTO intereses (nombre) VALUES 
('Deportes'),
('Música'),
('Lectura'),
('Viajar'),
('Cine'),
('Tecnología');

-- Usuarios
INSERT INTO usuarios (nombre, edad, genero_id, carrera_id, email, password, frase_perfil, creditos_disponibles, rol)
VALUES
('Juan', 22, 1, 1, 'juan@mail.com', '1234', 'Amante de la tecnología y el fútbol', 5, 'usuario'),
('Ana', 21, 2, 2, 'ana@mail.com', 'abcd', 'Me encanta leer y viajar', 5, 'usuario'),
('Carlos', 23, 1, 3, 'carlos@mail.com', 'pass', 'Apasionado por la música', 5, 'usuario'),
('Lucía', 20, 2, 4, 'lucia@mail.com', 'lucia', 'Fan del cine y la arquitectura', 5, 'usuario'),
('Admin', 30, 1, 1, 'admin@mail.com', 'admin', 'Administrador del sistema', 5, 'admin');

-- Usuario_intereses
INSERT INTO usuario_intereses (usuario_id, interes_id) VALUES
(1, 1), (1, 6),
(2, 3), (2, 4),
(3, 2), (3, 6),
(4, 5), (4, 6);

-- Puedes agregar más datos de prueba según lo necesites
