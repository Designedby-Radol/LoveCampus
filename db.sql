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
    password VARCHAR(100) NOT NULL,
    frase_perfil TEXT,
    creditos_disponibles INT DEFAULT 5,
    capcoins INT DEFAULT 0,
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
    usuario_id INT NOT NULL,
    interes_id INT NOT NULL,
    PRIMARY KEY (usuario_id, interes_id),
    FOREIGN KEY (usuario_id) REFERENCES usuarios(id),
    FOREIGN KEY (interes_id) REFERENCES intereses(id)
);

-- Tabla de matches
CREATE TABLE matches (
    id INT PRIMARY KEY AUTO_INCREMENT,
    usuario1_id INT NOT NULL,
    usuario2_id INT NOT NULL,
    fecha_match TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (usuario1_id) REFERENCES usuarios(id),
    FOREIGN KEY (usuario2_id) REFERENCES usuarios(id)
);

-- Tabla de tienda (para compras con capcoins)
CREATE TABLE tienda (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    precio_capcoins INT NOT NULL,
    tipo ENUM('token', 'like') NOT NULL,
    cantidad INT NOT NULL
);

-- Crear índices para mejorar el rendimiento
CREATE INDEX idx_usuarios_email ON usuarios(email);
CREATE INDEX idx_usuarios_ultimo_acceso ON usuarios(ultimo_acceso);
CREATE INDEX idx_matches_usuarios ON matches(usuario1_id, usuario2_id);

-- Insertar datos iniciales
INSERT INTO generos (descripcion) VALUES 
('Masculino'),
('Femenino'),
('No binario'),
('Prefiero no decir');

-- Insertar algunas carreras
INSERT INTO carreras (nombre) VALUES 
('Ingeniería en Sistemas'),
('Psicología'),
('Administración de Empresas'),
('Derecho'),
('Medicina'),
('Arquitectura'),
('Diseño Gráfico'),
('Ingeniería Civil'),
('Contabilidad'),
('Marketing');

-- Insertar algunos intereses
INSERT INTO intereses (nombre) VALUES 
('Deportes'),
('Música'),
('Lectura'),
('Viajar'),
('Cine'),
('Tecnología'),
('Arte'),
('Fotografía'),
('Cocina'),
('Videojuegos'),
('Naturaleza'),
('Fitness'),
('Animales'),
('Política'),
('Moda');

-- Insertar algunos productos en la tienda
INSERT INTO tienda (nombre, descripcion, precio_capcoins, tipo, cantidad) VALUES
('Paquete de 5 Tokens', '5 tokens para dar likes', 100, 'token', 5),
('Paquete de 10 Tokens', '10 tokens para dar likes', 180, 'token', 10),
('Paquete de 20 Tokens', '20 tokens para dar likes', 300, 'token', 20),
('Paquete de 5 Likes', '5 likes adicionales', 50, 'like', 5),
('Paquete de 10 Likes', '10 likes adicionales', 90, 'like', 10),
('Paquete de 20 Likes', '20 likes adicionales', 150, 'like', 20);

-- Insertar usuarios demo
INSERT INTO usuarios (nombre, edad, genero_id, carrera_id, email, password, frase_perfil, creditos_disponibles, capcoins) VALUES
('Juan Pérez', 22, 1, 1, 'juan@example.com', 'password123', 'Me gusta programar y escuchar música', 5, 100),
('María López', 21, 2, 2, 'maria@example.com', 'password123', 'Me encanta leer y el cine', 5, 100),
('Carlos Rodríguez', 23, 1, 3, 'carlos@example.com', 'password123', 'Apasionado del deporte y los videojuegos', 5, 100),
('Ana García', 20, 2, 4, 'ana@example.com', 'password123', 'Me gusta viajar y conocer nuevas culturas', 5, 100);

-- Asignar intereses a usuarios
INSERT INTO usuario_intereses (usuario_id, interes_id) VALUES
(1, 1), (1, 2), (1, 6),
(2, 3), (2, 4), (2, 5),
(3, 1), (3, 6), (3, 10),
(4, 2), (4, 4), (4, 7);