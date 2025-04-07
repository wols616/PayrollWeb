create database payroll_web1

use payroll_web1

CREATE TABLE Empleado (
    id_empleado INT IDENTITY(1,1) PRIMARY KEY,
    dui VARCHAR(10) NOT NULL UNIQUE,
    nombre VARCHAR(50) NOT NULL,
    apellidos VARCHAR(50) NOT NULL,
    telefono VARCHAR(15) NOT NULL,
    direccion VARCHAR(255) NOT NULL,
    cuenta_corriente VARCHAR(30) NOT NULL UNIQUE,
	estado VARCHAR(20) NOT NULL,
	correo VARCHAR(50) NOT NULL UNIQUE,
	contrasena VARCHAR(50) NOT NULL
); 

CREATE TABLE Administrador (
    id_administrador INT IDENTITY(1,1) PRIMARY KEY,
    dui VARCHAR(10) NOT NULL UNIQUE,
    nombre VARCHAR(50) NOT NULL,
    apellidos VARCHAR(50) NOT NULL,
    telefono VARCHAR(15) NOT NULL,
	correo VARCHAR(50) NOT NULL UNIQUE,
	contrasena VARCHAR(20) NOT NULL
);

CREATE TABLE Categoria (
    id_categoria INT IDENTITY(1,1) PRIMARY KEY,
    nombre_categoria VARCHAR(50) NOT NULL,
    sueldo_base DECIMAL(10,2) NOT NULL
);

CREATE TABLE Puesto (
    id_puesto INT IDENTITY(1,1) PRIMARY KEY,
    nombre_puesto VARCHAR(50) NOT NULL,
    id_categoria INT NOT NULL,
	sueldo_base DECIMAL(10,2),
    FOREIGN KEY (id_categoria) REFERENCES Categoria(id_categoria) ON DELETE CASCADE
);

CREATE TABLE Complemento_puesto (
	id_complemento_puesto INT IDENTITY(1,1) PRIMARY KEY,
	nombre_complemento VARCHAR(50) NOT NULL,
	monto DECIMAL(10,2) NOT NULL,
	id_puesto INT NOT NULL,
	FOREIGN KEY (id_puesto) REFERENCES Puesto(id_puesto)
);

CREATE TABLE Contrato (
    id_contrato INT IDENTITY(1,1) PRIMARY KEY,
    id_empleado INT NOT NULL,
    fecha_alta DATE NOT NULL,
    fecha_baja DATE NULL,  -- Permite valores nulos para contratos indefinidos
    id_puesto INT NOT NULL,
	tipo_contrato VARCHAR(15) NOT NULL,
	vigente VARCHAR(1) NOT NULL,
    FOREIGN KEY (id_empleado) REFERENCES Empleado(id_empleado) ON DELETE CASCADE,
    FOREIGN KEY (id_puesto) REFERENCES Puesto(id_puesto) ON DELETE CASCADE
);  

CREATE TABLE Deduccion (
    id_deduccion INT IDENTITY(1,1) PRIMARY KEY,
    nombre_deduccion VARCHAR(50) NOT NULL,
    porcentaje DECIMAL(5,2) NOT NULL CHECK (porcentaje >= 0 AND porcentaje <= 100),
	fija VARCHAR(1) NOT NULL
);

CREATE TABLE Deduccion_Personal (
	id_deduccion_personal INT IDENTITY(1,1) PRIMARY KEY,
	id_deduccion INT NOT NULL,
	id_empleado INT NOT NULL,
	porcentaje_personal DECIMAL(5,2),
    FOREIGN KEY (id_deduccion) REFERENCES Deduccion(id_deduccion) ON DELETE CASCADE,
	FOREIGN KEY (id_empleado) REFERENCES Empleado(id_empleado) ON DELETE CASCADE
);

CREATE TABLE KPI (
	id_kpi INT IDENTITY(1,1) PRIMARY KEY,
	nombre VARCHAR(50) NOT NULL
);

CREATE TABLE Evaluacion_Desempeno(
	id_evaluacion_desempeno INT IDENTITY(1,1) PRIMARY KEY,
	id_empleado INT,
	fecha DATETIME,
	id_kpi INT,
	puntuacion INT,
	FOREIGN KEY (id_empleado) REFERENCES Empleado(id_empleado),
	FOREIGN KEY (id_kpi) REFERENCES KPI(id_kpi)
);

CREATE TABLE Historial_Contrato(
	id_historial_contrato INT IDENTITY(1,1) PRIMARY KEY,
	id_contrato_anterior INT,
	id_contrato_nuevo INT NULL,
	fecha DATETIME DEFAULT GETDATE(),
	cambio VARCHAR(50),
	motivo VARCHAR(300),
	id_administrador INT,
	FOREIGN KEY (id_administrador) REFERENCES Administrador(id_administrador),
	FOREIGN KEY (id_contrato_anterior) REFERENCES Contrato(id_contrato),
	FOREIGN KEY (id_contrato_nuevo) REFERENCES Contrato(id_contrato)
); 
--ALTER TABLE Historial_Contrato
--ALTER COLUMN id_contrato_anterior INT NULL;

CREATE TABLE Cargo(
	id_cargo INT IDENTITY(1,1) PRIMARY KEY,
	nombre_cargo VARCHAR(50),
	descripcion VARCHAR(300)
);

CREATE TABLE Complemento_Cargo(
	id_complemento_cargo INT IDENTITY(1,1) PRIMARY KEY,
	id_cargo INT,
	id_empleado INT,
	fecha_inicio DATETIME DEFAULT GETDATE(),
	fecha_fin DATETIME DEFAULT GETDATE(),
	monto_complemento DECIMAL(10,2),
	FOREIGN KEY (id_cargo) REFERENCES Cargo(id_cargo),
	FOREIGN KEY (id_empleado) REFERENCES Empleado(id_empleado)
);

CREATE TABLE Puesto_Historico(
	id_puesto_historico INT IDENTITY(1,1) PRIMARY KEY,
	nombre_puesto VARCHAR(50),
	sueldo_base Decimal(10,2),
	nombre_categoria VARCHAR(50),
	id_contrato INT,
	FOREIGN KEY (id_contrato) REFERENCES Contrato(id_contrato)
);

CREATE TABLE Meta(
	id_meta INT IDENTITY(1,1) PRIMARY KEY,
	id_empleado INT,
	meta_descripcion VARCHAR(200),
	estado VARCHAR(50),
	FOREIGN KEY (id_empleado) REFERENCES Empleado(id_empleado)
); 

CREATE TABLE Habilidad(
	id_habilidad INT IDENTITY(1,1) PRIMARY KEY,
	nombre VARCHAR(100)
);
ALTER TABLE Habilidad ADD activo BIT DEFAULT 1;

CREATE TABLE Competencia(
	id_competencia INT IDENTITY(1,1) PRIMARY KEY,
	nombre VARCHAR(100)
); 
ALTER TABLE Competencia ADD activo BIT DEFAULT 1;

CREATE TABLE Habilidad_Empleado(
	id_habilidad_empleado INT IDENTITY(1,1) PRIMARY KEY,
	id_empleado INT,
	id_habilidad INT,
	FOREIGN KEY (id_empleado) REFERENCES Empleado(id_empleado),
	FOREIGN KEY (id_habilidad) REFERENCES Habilidad(id_habilidad)
);

CREATE TABLE Competencia_Empleado(
	id_competencia_empleado INT IDENTITY(1,1) PRIMARY KEY,
	id_empleado INT,
	id_competencia INT,
	FOREIGN KEY (id_empleado) REFERENCES Empleado(id_empleado),
	FOREIGN KEY (id_competencia) REFERENCES Competencia(id_competencia)
);

CREATE TABLE Asistencia(
	id_asistencia INT IDENTITY(1,1) PRIMARY KEY,
	id_empleado INT NOT NULL,
	fecha DATE DEFAULT GETDATE() NOT NULL,
	hora_entrada TIME DEFAULT CAST(GETDATE() AS TIME) NOT NULL,
	hora_salida TIME DEFAULT CAST(GETDATE() AS TIME) NOT NULL,
	ausencia VARCHAR(200),
	FOREIGN KEY (id_empleado) REFERENCES Empleado(id_empleado),
);

CREATE TABLE Trienios(
	id_trienio INT IDENTITY(1,1) PRIMARY KEY,
	id_empleado INT,
	fecha_inicio DATETIME,
	fecha_fin DATETIME,
	monto DECIMAL,
	FOREIGN KEY (id_empleado) REFERENCES Empleado(id_empleado)
);

