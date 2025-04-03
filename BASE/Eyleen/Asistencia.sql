
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


DROP TABLE Asistencia

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


 SELECT Asistencia.hora_entrada, Asistencia.hora_salida from Asistencia
 WHERE Asistencia.fecha = '2024-03-25' and asistencia.id_empleado = 1

 SELECT * FROM Asistencia


 SELECT Empleado.nombre, Empleado.apellidos FROM Asistencia
 JOIN Empleado ON Asistencia.id_empleado = Empleado.id_empleado
 WHERE Asistencia.fecha = '2025-03-25'

SELECT Empleado.nombre, Empleado.apellidos 
FROM Empleado
LEFT JOIN Asistencia ON Asistencia.id_empleado = Empleado.id_empleado 
    AND Asistencia.fecha = '2025-03-25'
WHERE Asistencia.id_empleado IS NULL;






 




SELECT Empleado.id_empleado, Empleado.nombre, Empleado.apellidos 
FROM Empleado
INNER JOIN Asistencia ON Asistencia.id_empleado = Empleado.id_empleado 
WHERE Asistencia.fecha = '2025-04-02' 
AND Asistencia.hora_entrada <> '00:00'
AND Asistencia.hora_salida = '00:00'
AND Asistencia.ausencia IS NULL;


 UPDATE Asistencia SET hora_salida = '22:22' WHERE id_empleado = 1 AND fecha = '2025-04-03'

 select * from asistencia


 
INSERT INTO Asistencia (id_empleado, fecha, hora_entrada, hora_salida, ausencia) VALUES
-- Empleado 1
(2, '2025-03-2', '00:00:00', '0:00:00', 'Ha cometido ciertos delitos');



UPDATE Asistencia SET ausencia = 'ejemplo1'
WHERE id_empleado = 1 AND fecha = '2025-04-03'