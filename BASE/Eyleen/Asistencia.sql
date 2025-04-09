
USE payroll_pruebas

CREATE TABLE Asistencia(
	id_asistencia INT IDENTITY (1,1) PRIMARY KEY,
	id_empleado INT NOT NULL,
	fecha DATE DEFAULT GETDATE() NOT NULL,
	hora_entrada TIME DEFAULT CAST(GETDATE() AS TIME) NOT NULL,
	hora_salida TIME DEFAULT CAST(GETDATE() AS TIME) NOT NULL,
	ausencia VARCHAR(200),
	FOREIGN KEY (id_empleado) REFERENCES Empleado(id_empleado),
);