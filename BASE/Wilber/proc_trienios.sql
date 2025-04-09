--OJO: el procedimiento es funcional pero, no funcionara bien si tenes datos sin sentido en contratos, 
--por ejemplo si insertas la serie de contratos para empleado 2 o 1, deberia crear un trienio, pero no lo hara si ya 
--tenes otros contratos tu en ese rango de fechas, es como que se bloquea si un empleado tiene varios contratos en
--el mismo intervalo de tiempo, por ejemplo para empleado 2, que en las inserciones de abajo de ejemplo tiene contratos desde el 2009
--pero como en la base de por si ya habian contratos del empleado 2, en ese intervalo de tiempo, el proc ya no puede calcular bien el trienio
--esto no representa un problema serio pues en el sistema no deberia permitirse crear otro contrato sin cancelar el primero
--es mas como un detalle de la base por si tenes datos que chocan o contratos en el mismo intervalo de tiempo

--Cuando canceles un contrato tendras que ejecutar ese proc para poner el trienio con estado N


--COMANDOS DE PRUEBA
EXEC CalcularTrienios;
Select * from trienios
select * from puesto;
select * from Contrato where id_empleado = 2
select * from contrato where id_empleado = 1
delete from contrato where id_empleado = 1
delete puesto_historico
delete Historial_Contrato
DELETE trienios
DELETE Contrato

--Datos de prueba:
--Contratos para empleado 1(sueldo base $4500)
INSERT INTO Contrato (id_empleado, fecha_alta, fecha_baja, id_puesto, tipo_contrato, vigente)
VALUES (1, '2022-01-03', NULL, 1, 'Indefinido', 'N');
INSERT INTO Contrato (id_empleado, fecha_alta, fecha_baja, id_puesto, tipo_contrato, vigente)
VALUES (1, '2018-01-03', '2022-01-03', 1, 'Indefinido', 'N');
INSERT INTO Contrato (id_empleado, fecha_alta, fecha_baja, id_puesto, tipo_contrato, vigente)
VALUES (1, '2014-01-03', '2018-01-03', 1, 'Indefinido', 'N');

-- Contrados para empleado 2, Sueldo base del puesto 4(puesto actual) = 2800
INSERT INTO Contrato (id_empleado, fecha_alta, fecha_baja, id_puesto, tipo_contrato, vigente)
VALUES 
(2, '2009-04-01', '2014-04-01', 3, 'Indefinido', 'N'),
(2, '2014-04-01', '2019-04-01', 2, 'Indefinido', 'N'),
(2, '2019-04-01', NULL,          4, 'Indefinido', 'S');



--PROCEDIMIENTO
DROP PROCEDURE IF EXISTS CalcularTrienios;

CREATE PROCEDURE CalcularTrienios
AS
BEGIN
    SET NOCOUNT ON;

    -- Se usa un CTE para ordenar los contratos y detectar interrupciones
    ;WITH ContratosOrdenados AS (
        SELECT 
            id_empleado,
            fecha_alta,
            fecha_baja,
            vigente,
            id_puesto,
            ROW_NUMBER() OVER(PARTITION BY id_empleado ORDER BY fecha_alta) AS rn,
            LAG(fecha_baja) OVER(PARTITION BY id_empleado ORDER BY fecha_alta) AS fecha_baja_prev
        FROM Contrato
    ),
    -- Este CTE agrupa por empleado y verifica si hay interrupciones (más de un día de diferencia)
    EmpleadoContinuo AS (
        SELECT 
            id_empleado,
            MIN(fecha_alta) AS fecha_inicio,
            MAX(CASE 
                    WHEN fecha_baja IS NULL THEN GETDATE() 
                    ELSE fecha_baja 
                END) AS fecha_fin,
            SUM(CASE 
                    WHEN rn > 1 AND DATEDIFF(DAY, fecha_baja_prev, fecha_alta) > 1 THEN 1 
                    ELSE 0 
                END) AS interrupciones,
            -- Se determina si tiene algún contrato vigente
            MAX(CASE WHEN vigente = 'S' THEN 1 ELSE 0 END) AS activo,
            -- Se asume que el puesto del contrato actual es el del contrato con mayor fecha_alta
            MAX(id_puesto) AS id_puesto
        FROM ContratosOrdenados
        GROUP BY id_empleado
    )
    -- Se crea una tabla temporal para trabajar solo con empleados que cumplen la continuidad
    SELECT 
        e.id_empleado,
        DATEDIFF(YEAR, e.fecha_inicio, e.fecha_fin) AS anios_trabajados,
        e.fecha_inicio,
        e.fecha_fin,
        e.activo,
        p.sueldo_base
    INTO #EmpleadosTrienios
    FROM EmpleadoContinuo e
    INNER JOIN Puesto p ON e.id_puesto = p.id_puesto
    WHERE e.interrupciones = 0;

    -- Se usa MERGE para actualizar o insertar en la tabla de trienios
    MERGE trienios AS t
    USING #EmpleadosTrienios AS e
    ON t.id_empleado = e.id_empleado
    WHEN MATCHED THEN
        UPDATE SET
            t.monto = 
                CASE 
                    WHEN e.anios_trabajados >= 15 THEN 0.25 * e.sueldo_base
                    WHEN e.anios_trabajados >= 12 THEN 0.20 * e.sueldo_base
                    WHEN e.anios_trabajados >= 9  THEN 0.15 * e.sueldo_base
                    WHEN e.anios_trabajados >= 6  THEN 0.10 * e.sueldo_base
                    WHEN e.anios_trabajados >= 3  THEN 0.05 * e.sueldo_base
                    ELSE 0
                END,
            -- El estado se marca 'S' si el empleado tiene contrato vigente, 'N' en caso contrario
            t.estado = CASE WHEN e.activo = 1 THEN 'S' ELSE 'N' END,
            t.fecha_inicio = e.fecha_inicio,
            t.fecha_fin = CASE WHEN e.activo = 1 THEN NULL ELSE e.fecha_fin END
    WHEN NOT MATCHED AND e.anios_trabajados >= 3 THEN
        INSERT (id_empleado, fecha_inicio, fecha_fin, monto, estado)
        VALUES (
            e.id_empleado, 
            e.fecha_inicio, 
            CASE WHEN e.activo = 1 THEN NULL ELSE e.fecha_fin END,
            CASE 
                WHEN e.anios_trabajados >= 15 THEN 0.25 * e.sueldo_base
                WHEN e.anios_trabajados >= 12 THEN 0.20 * e.sueldo_base
                WHEN e.anios_trabajados >= 9  THEN 0.15 * e.sueldo_base
                WHEN e.anios_trabajados >= 6  THEN 0.10 * e.sueldo_base
                WHEN e.anios_trabajados >= 3  THEN 0.05 * e.sueldo_base
                ELSE 0
            END,
            CASE WHEN e.activo = 1 THEN 'S' ELSE 'N' END
        );

    DROP TABLE #EmpleadosTrienios;
END;
