
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

INSERT INTO Asistencia (id_empleado, fecha, hora_entrada, hora_salida, ausencia) VALUES
-- Empleado 1
(1, '2024-03-25', '08:00:00', '17:00:00', NULL),
(1, '2024-03-26', '08:05:00', '17:10:00', NULL),
(1, '2024-03-27', '08:02:00', '16:50:00', NULL),
(1, '2024-03-28', '08:10:00', '17:15:00', NULL),
(1, '2024-03-29', '08:00:00', '17:00:00', NULL),

-- Empleado 2
(2, '2024-03-25', '07:50:00', '16:55:00', NULL),
(2, '2024-03-26', '08:00:00', '17:05:00', NULL),
(2, '2024-03-27', '07:55:00', '16:45:00', NULL),
(2, '2024-03-28', '08:05:00', '17:10:00', NULL),
(2, '2024-03-29', '08:00:00', '17:00:00', NULL),

-- Empleado 3
(3, '2024-03-25', '08:15:00', '17:20:00', NULL),
(3, '2024-03-26', '08:10:00', '17:00:00', NULL),
(3, '2024-03-27', '08:20:00', '16:55:00', NULL),
(3, '2024-03-28', '08:00:00', '17:05:00', NULL),
(3, '2024-03-29', '08:05:00', '17:10:00', NULL),

-- Empleado 4
(4, '2024-03-25', '08:00:00', '17:00:00', NULL),
(4, '2024-03-26', '08:05:00', '17:10:00', NULL),
(4, '2024-03-27', '08:10:00', '17:15:00', NULL),
(4, '2024-03-28', '08:00:00', '17:00:00', NULL),
(4, '2024-03-29', '08:02:00', '16:50:00', NULL),

-- Empleado 5
(5, '2024-03-25', '08:10:00', '17:15:00', NULL),
(5, '2024-03-26', '08:00:00', '17:00:00', NULL),
(5, '2024-03-27', '08:05:00', '17:10:00', NULL),
(5, '2024-03-28', '08:00:00', '17:00:00', NULL),
(5, '2024-03-29', '08:15:00', '17:20:00', NULL);


SELECT fecha, hora_entrada, hora_entrada FROM Asistencia where id_empleado = 1

select * from Empleado




SELECT * from Asistencia
Select * from Empleado


SELECT Asistencia.fecha, Asistencia.hora_entrada, Asistencia.hora_salida FROM Asistencia
JOIN Empleado on Asistencia.id_empleado = Empleado.id_empleado
WHERE Empleado.dui = '11223344-5';

SELECT Asistencia.fecha, Asistencia.hora_entrada, Asistencia.hora_salida
FROM Asistencia
JOIN Empleado ON Asistencia.id_empleado = Empleado.id_empleado
WHERE Empleado.dui = '11223344-5';



SELECT Asistencia.fecha, Asistencia.hora_entrada, Asistencia.hora_salida
FROM Asistencia
JOIN Empleado ON Asistencia.id_empleado = Empleado.id_empleado
WHERE Empleado.nombre LIKE '%ju%'

 SELECT Asistencia.fecha, Asistencia.hora_entrada, Asistencia.hora_salida FROM Asistencia 
 JOIN Empleado on Asistencia.id_empleado = Empleado.id_empleado 
 WHERE Asistencia.fecha = '2024-03-25' and Empleado.dui = '22334455-6'


 SELECT Asistencia.fecha, Empleado.nombre, Asistencia.hora_entrada, Asistencia.hora_salida from Asistencia
 JOIN Empleado ON Asistencia.id_empleado = Empleado.id_empleado
 WHERE Asistencia.fecha = '2024-03-25'

 SELECT * FROM Asistencia


 SELECT Empleado.nombre, Empleado.apellidos FROM Asistencia
 JOIN Empleado ON Asistencia.id_empleado = Empleado.id_empleado
 WHERE Asistencia.fecha = '2025-03-25'

SELECT Empleado.nombre, Empleado.apellidos 
FROM Empleado
LEFT JOIN Asistencia ON Asistencia.id_empleado = Empleado.id_empleado 
    AND Asistencia.fecha = '2025-03-25'
WHERE Asistencia.id_empleado IS NULL;






 
