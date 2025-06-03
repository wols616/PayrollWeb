use payroll_web1

--PROCEDIMIENTO PARA ACTUALIZAR EL PORCENTAJE PERSONAL DEL AFP, ISSS Y EL ISR
/*CREATE PROCEDURE ActualizarPorcentajesDeducciones 
AS
BEGIN
    -- Variables para almacenar valores temporales
    DECLARE @IdEmpleado INT;
    DECLARE @IdDeduccion INT;
    DECLARE @SueldoBase DECIMAL(10, 2);
    DECLARE @PorcentajePersonal DECIMAL(5, 2);
    DECLARE @PorcentajeDeduccion DECIMAL(5, 2);
    DECLARE @MontoTrienios DECIMAL(10, 2) = 0;
    DECLARE @SueldoParaISR DECIMAL(10, 2);

    -- Cursor para recorrer los empleados y sus deducciones
    DECLARE EmpleadoCursor CURSOR FOR
    SELECT 
        DP.id_empleado, 
        DP.id_deduccion, 
        P.sueldo_base,
        D.porcentaje
    FROM 
        Deduccion_Personal DP 
    INNER JOIN 
        Contrato C ON DP.id_empleado = C.id_empleado
    INNER JOIN 
        Puesto P ON C.id_puesto = P.id_puesto
    INNER JOIN 
        Deduccion D ON DP.id_deduccion = D.id_deduccion
    WHERE 
        C.vigente = 'S';  -- Solo contratos vigentes

    -- Abrir el cursor
    OPEN EmpleadoCursor;

    -- Recorrer el cursor
    FETCH NEXT FROM EmpleadoCursor INTO @IdEmpleado, @IdDeduccion, @SueldoBase, @PorcentajeDeduccion;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Inicializar variables para cada empleado
        SET @MontoTrienios = 0;
        SET @SueldoParaISR = @SueldoBase;

        -- Verificar si el empleado tiene trienios vigentes
        SELECT @MontoTrienios = ISNULL(SUM(monto), 0)
        FROM Trienios
        WHERE id_empleado = @IdEmpleado 
          AND estado = 'S';

        -- Calcular el sueldo base + trienios para el ISR
        IF (SELECT nombre_deduccion FROM Deduccion WHERE id_deduccion = @IdDeduccion) = 'ISR'
        BEGIN
            SET @SueldoParaISR = @SueldoBase + @MontoTrienios;
        END

        -- Calcular el porcentaje_personal según la deducción
        IF (SELECT nombre_deduccion FROM Deduccion WHERE id_deduccion = @IdDeduccion) = 'AFP'
        BEGIN
            -- AFP: 7.25% del sueldo base (no incluye trienios)
            SET @PorcentajePersonal = 7.25;
        END
        ELSE IF (SELECT nombre_deduccion FROM Deduccion WHERE id_deduccion = @IdDeduccion) = 'ISSS'
        BEGIN
            -- ISSS: 3% del sueldo base (hasta un máximo de $1000, no incluye trienios)
            SET @PorcentajePersonal = 3.00;
        END
        ELSE IF (SELECT nombre_deduccion FROM Deduccion WHERE id_deduccion = @IdDeduccion) = 'ISR'
        BEGIN
            -- ISR: Aplicar escala progresiva sobre sueldo base + trienios
            IF @SueldoParaISR <= 472.00
            BEGIN
                SET @PorcentajePersonal = 0.00;
            END
            ELSE IF @SueldoParaISR <= 895.24
            BEGIN
                SET @PorcentajePersonal = 10.00;
            END
            ELSE IF @SueldoParaISR <= 2038.10
            BEGIN
                SET @PorcentajePersonal = 20.00;
            END
            ELSE
            BEGIN
                SET @PorcentajePersonal = 30.00;
            END
        END
        ELSE
        BEGIN
            -- Para otras deducciones, usar el porcentaje de la deducción
            SET @PorcentajePersonal = @PorcentajeDeduccion;
        END

        -- Actualizar el porcentaje_personal en Deduccion_Personal
        UPDATE Deduccion_Personal
        SET porcentaje_personal = @PorcentajePersonal
        WHERE id_empleado = @IdEmpleado
          AND id_deduccion = @IdDeduccion;

        -- Obtener el siguiente registro
        FETCH NEXT FROM EmpleadoCursor INTO @IdEmpleado, @IdDeduccion, @SueldoBase, @PorcentajeDeduccion;
    END
    -- Cerrar y liberar el cursor
    CLOSE EmpleadoCursor;
    DEALLOCATE EmpleadoCursor;
END;*/


--------------------------------------------------------------------------------------
--PROCEDIMIENTO PARA ACTUALIZAR EL PORCENTAJE PERSONAL DEL AFP, ISSS Y EL ISR 2.0
CREATE PROCEDURE ActualizarPorcentajesDeducciones 
AS
BEGIN
    -- Variables para almacenar valores temporales
    DECLARE @TopeISSS DECIMAL(10, 2) = 30.00;
    DECLARE @BaseISSS DECIMAL(10, 2) = 1000.00;
    
    -- Tabla temporal para almacenar los montos de AFP e ISSS por empleado
    CREATE TABLE #DeduccionesEmpleado (
        IdEmpleado INT PRIMARY KEY,
        SueldoBase DECIMAL(10, 2),
        MontoAFP DECIMAL(10, 2),
        MontoISSS DECIMAL(10, 2),
        MontoTrienios DECIMAL(10, 2),
        BaseISR DECIMAL(10, 2) -- Añadimos esta columna
    );

    -- Primero: Calcular y guardar AFP e ISSS para cada empleado
    INSERT INTO #DeduccionesEmpleado (IdEmpleado, SueldoBase, MontoAFP, MontoISSS, MontoTrienios)
    SELECT 
        C.id_empleado,
        P.sueldo_base,
        -- Cálculo AFP (7.25% del sueldo base)
        P.sueldo_base * 0.0725,
        -- Cálculo ISSS (3% del sueldo base con tope de $30)
        CASE 
            WHEN P.sueldo_base > @BaseISSS THEN @BaseISSS * 0.03
            ELSE P.sueldo_base * 0.03
        END,
        -- Suma de trienios
        ISNULL((SELECT SUM(monto) FROM Trienios WHERE id_empleado = C.id_empleado AND estado = 'S'), 0)
    FROM 
        Contrato C
    INNER JOIN 
        Puesto P ON C.id_puesto = P.id_puesto
    WHERE 
        C.vigente = 'S';

    -- Aplicar tope máximo al ISSS
    UPDATE #DeduccionesEmpleado
    SET MontoISSS = CASE WHEN MontoISSS > @TopeISSS THEN @TopeISSS ELSE MontoISSS END;

    -- Calcular base para ISR (Sueldo + Trienios - AFP - ISSS)
    UPDATE #DeduccionesEmpleado
    SET BaseISR = SueldoBase + MontoTrienios - MontoAFP - MontoISSS;

    -- Segundo: Actualizar los porcentajes personales en Deduccion_Personal
    -- AFP (7.25%)
    UPDATE DP
    SET DP.porcentaje_personal = 7.25
    FROM Deduccion_Personal DP
    INNER JOIN #DeduccionesEmpleado DE ON DP.id_empleado = DE.IdEmpleado
    INNER JOIN Deduccion D ON DP.id_deduccion = D.id_deduccion
    WHERE D.nombre_deduccion = 'AFP';

    -- ISSS (Porcentaje equivalente al monto calculado)
    UPDATE DP
    SET DP.porcentaje_personal = CASE 
                                    WHEN DE.SueldoBase > 0 THEN (DE.MontoISSS / DE.SueldoBase) * 100 
                                    ELSE 0 
                                 END
    FROM Deduccion_Personal DP
    INNER JOIN #DeduccionesEmpleado DE ON DP.id_empleado = DE.IdEmpleado
    INNER JOIN Deduccion D ON DP.id_deduccion = D.id_deduccion
    WHERE D.nombre_deduccion = 'ISSS';

    -- ISR (Según escala progresiva)
    UPDATE DP
    SET DP.porcentaje_personal = CASE
                                    WHEN DE.BaseISR <= 472.00 THEN 0.00
                                    WHEN DE.BaseISR <= 895.24 THEN 10.00
                                    WHEN DE.BaseISR <= 2038.10 THEN 20.00
                                    ELSE 30.00
                                 END
    FROM Deduccion_Personal DP
    INNER JOIN #DeduccionesEmpleado DE ON DP.id_empleado = DE.IdEmpleado
    INNER JOIN Deduccion D ON DP.id_deduccion = D.id_deduccion
    WHERE D.nombre_deduccion = 'ISR';

    -- Otras deducciones (mantener su porcentaje original)
    --UPDATE DP
    --SET DP.porcentaje_personal = D.porcentaje
    --FROM Deduccion_Personal DP
    --INNER JOIN Deduccion D ON DP.id_deduccion = D.id_deduccion
    --WHERE D.nombre_deduccion NOT IN ('AFP', 'ISSS', 'ISR');

    -- Eliminar tabla temporal
    DROP TABLE #DeduccionesEmpleado;
END;
--------------------------------------------------------------------------------Calcular trienios
-----------------------------------------------------------------------------------------------
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

--__________________________________________________________________________________________________________
--PROCEDIMIENTO PARA CREAR UN PUESTO HISTORICO CADA QUE SE CANCELA UN CONTRATO
CREATE OR ALTER PROCEDURE sp_RegistrarPuestoHistorico
    @id_contrato INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @vigente CHAR(1);
    
    -- Verificar si el contrato está marcado como no vigente
    SELECT @vigente = vigente 
    FROM Contrato 
    WHERE id_contrato = @id_contrato;
    
    -- Solo proceder si el contrato es no vigente
    IF @vigente = 'N'
    BEGIN
        -- Insertar en Puesto_Historico con los datos del puesto asociado
        INSERT INTO Puesto_Historico (
            nombre_puesto,
            sueldo_base,
            nombre_categoria,
            id_contrato
        )
        SELECT 
            p.nombre_puesto,
            p.sueldo_base,
            c.nombre_categoria,
            @id_contrato
        FROM Puesto p
        INNER JOIN Categoria c ON p.id_categoria = c.id_categoria
        INNER JOIN Contrato ct ON p.id_puesto = ct.id_puesto
        WHERE ct.id_contrato = @id_contrato;
        
        PRINT 'Registro histórico creado para el puesto asociado al contrato: ' + CAST(@id_contrato AS VARCHAR);
    END
    ELSE
    BEGIN
        PRINT 'El contrato aún está vigente, no se creará registro histórico';
    END
END;
--_________________________________________________________________________________________________________
--TRIGGERS

--_________________________________________________________________________________________________________
--Trigger para actualizar el sueldo base cada que se agregue un puesto
CREATE TRIGGER trg_SetSueldoBasePuesto  
ON Puesto
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE p
    SET p.sueldo_base = c.sueldo_base
    FROM Puesto p
    INNER JOIN inserted i ON p.id_puesto = i.id_puesto
    INNER JOIN Categoria c ON i.id_categoria = c.id_categoria;

	EXEC ActualizarPorcentajesDeducciones;
END;
--_________________________________________________________________________________________________________

--_________________________________________________________________________________________________________
--Trigger para actualizar el sueldo de puesto cada que se agregue un nuevo complemento de sueldo
CREATE TRIGGER trg_UpdateSueldoBasePuesto 
ON Complemento_puesto
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Actualizar el sueldo_base de los puestos afectados
    UPDATE p
    SET p.sueldo_base = c.sueldo_base + ISNULL(cp.total_complemento, 0)
    FROM Puesto p
    INNER JOIN Categoria c ON p.id_categoria = c.id_categoria
    LEFT JOIN (
        SELECT id_puesto, SUM(monto) AS total_complemento
        FROM Complemento_puesto
        GROUP BY id_puesto
    ) cp ON p.id_puesto = cp.id_puesto;

	EXEC ActualizarPorcentajesDeducciones;
END;

--_________________________________________________________________________________________________________
-- Trigger para actualizar el sueldo base de un puesto después de modificar la categoría
CREATE TRIGGER trg_UpdateSueldoBasePuestoCategoria 
ON Puesto
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si la categoría ha sido actualizada
    IF UPDATE(id_categoria)
    BEGIN
        -- Actualizar el sueldo_base del puesto sumando el sueldo de la nueva categoría y los complementos
        UPDATE p
        SET p.sueldo_base = c.sueldo_base + ISNULL(cp.total_complemento, 0)
        FROM Puesto p
        INNER JOIN inserted i ON p.id_puesto = i.id_puesto
        INNER JOIN Categoria c ON i.id_categoria = c.id_categoria
        LEFT JOIN (
            SELECT id_puesto, SUM(monto) AS total_complemento
            FROM Complemento_puesto
            GROUP BY id_puesto
        ) cp ON p.id_puesto = cp.id_puesto
        WHERE p.id_puesto = i.id_puesto;
    END

	EXEC ActualizarPorcentajesDeducciones;
END;
--_________________________________________________________________________________________________________
-- Trigger para actualizar el sueldo base de los puestos cuando el sueldo base de la categoría cambia
CREATE TRIGGER trg_UpdateSueldoBasePuestoCategoriaSueldo 
ON Categoria
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar si el sueldo_base de la categoría ha sido actualizado
    IF UPDATE(sueldo_base)
    BEGIN
        -- Actualizar el sueldo_base de los puestos relacionados sumando el nuevo sueldo base de la categoría y los complementos
        UPDATE p
        SET p.sueldo_base = c.sueldo_base + ISNULL(cp.total_complemento, 0)
        FROM Puesto p
        INNER JOIN inserted i ON p.id_categoria = i.id_categoria
        INNER JOIN Categoria c ON i.id_categoria = c.id_categoria
        LEFT JOIN (
            SELECT id_puesto, SUM(monto) AS total_complemento
            FROM Complemento_puesto
            GROUP BY id_puesto
        ) cp ON p.id_puesto = cp.id_puesto
        WHERE p.id_categoria = i.id_categoria;
    END

	EXEC ActualizarPorcentajesDeducciones;
END;
--__________________________________________________________________________________________________________
--Trigger para agregar las deducciones fijas a todos los empleados, cada que una nueva deducción se crea o actualiza
CREATE TRIGGER trg_InsertOrUpdateDeduccion 
ON Deduccion
AFTER INSERT, UPDATE
AS
BEGIN
    -- Insertar en Deduccion_Personal solo si la deducción es fija ('S')
    INSERT INTO Deduccion_Personal (id_deduccion, id_empleado)
    SELECT i.id_deduccion, e.id_empleado
    FROM inserted i
    CROSS JOIN Empleado e
    WHERE i.fija = 'S'
          AND NOT EXISTS (
              SELECT 1
              FROM Deduccion_Personal dp
              WHERE dp.id_deduccion = i.id_deduccion
              AND dp.id_empleado = e.id_empleado
          );

		  EXEC ActualizarPorcentajesDeducciones;
END;
--__________________________________________________________________________________________________________
--TRIGER PARA AGREGARLE A UN NUEVO EMPLEADO LAS DEDUCCIONES FIJAS
CREATE TRIGGER trg_InsertEmpleado 
ON Empleado
AFTER INSERT
AS
BEGIN
    -- Insertar las deducciones fijas para el nuevo empleado
    INSERT INTO Deduccion_Personal (id_deduccion, id_empleado, porcentaje_personal)
    SELECT d.id_deduccion, e.id_empleado, d.porcentaje
    FROM inserted e
    JOIN Deduccion d ON d.fija = 'S'
    WHERE NOT EXISTS (
        SELECT 1
        FROM Deduccion_Personal dp
        WHERE dp.id_deduccion = d.id_deduccion
        AND dp.id_empleado = e.id_empleado
    );
END;

--.............................................................................................................
----Procedimiento Almacenado para Actualizar Sueldos con Complementos de Cargo
--CREATE PROCEDURE ActualizarSueldosConComplementosCargo
--AS
--BEGIN
--    SET NOCOUNT ON;
    
--    -- Actualizar el sueldo base de todos los puestos con sus complementos de cargo vigentes
--    UPDATE p
--    SET p.sueldo_base = c.sueldo_base + 
--                        ISNULL((SELECT SUM(monto) 
--                               FROM Complemento_puesto cp 
--                               WHERE cp.id_puesto = p.id_puesto), 0) +
--                        ISNULL((SELECT SUM(cc.monto_complemento)
--                               FROM Complemento_Cargo cc
--                               INNER JOIN Contrato co ON cc.id_empleado = co.id_empleado
--                               WHERE co.id_puesto = p.id_puesto
--                               AND co.vigente = 'S'
--                               AND cc.fecha_inicio <= GETDATE()
--                               AND (cc.fecha_fin IS NULL OR cc.fecha_fin >= GETDATE())), 0)
--    FROM Puesto p
--    INNER JOIN Categoria c ON p.id_categoria = c.id_categoria;
    
--    -- Actualizar los porcentajes de deducción
--    EXEC ActualizarPorcentajesDeducciones;
    
--    PRINT 'Sueldos actualizados con complementos de cargo vigentes';
--END; 

----............................................................................................................................
----Trigger para Actualizar cuando se Modifican Complementos de Cargo
--CREATE TRIGGER trg_ComplementoCargo_Change
--ON Complemento_Cargo
--AFTER INSERT, UPDATE, DELETE
--AS
--BEGIN
--    SET NOCOUNT ON;
    
--    -- Identificar los puestos afectados
--    DECLARE @PuestosAfectados TABLE (id_puesto INT);
    
--    -- Insertar puestos afectados por cambios en complementos de cargo
--    INSERT INTO @PuestosAfectados (id_puesto)
--    SELECT DISTINCT co.id_puesto
--    FROM (
--        SELECT id_empleado FROM inserted
--        UNION
--        SELECT id_empleado FROM deleted
--    ) AS cambios
--    INNER JOIN Contrato co ON cambios.id_empleado = co.id_empleado
--    WHERE co.vigente = 'S';
    
--    -- Actualizar solo los puestos afectados
--    UPDATE p
--    SET p.sueldo_base = c.sueldo_base + 
--                        ISNULL((SELECT SUM(monto) 
--                               FROM Complemento_puesto cp 
--                               WHERE cp.id_puesto = p.id_puesto), 0) +
--                        ISNULL((SELECT SUM(cc.monto_complemento)
--                               FROM Complemento_Cargo cc
--                               INNER JOIN Contrato co ON cc.id_empleado = co.id_empleado
--                               WHERE co.id_puesto = p.id_puesto
--                               AND co.vigente = 'S'
--                               AND cc.fecha_inicio <= GETDATE()
--                               AND (cc.fecha_fin IS NULL OR cc.fecha_fin >= GETDATE())), 0)
--    FROM Puesto p
--    INNER JOIN Categoria c ON p.id_categoria = c.id_categoria
--    WHERE p.id_puesto IN (SELECT id_puesto FROM @PuestosAfectados);
    
--    -- Actualizar los porcentajes de deducción
--    EXEC ActualizarPorcentajesDeducciones;
--END;

----............................................................................................................................
----Trigger Adicional para Actualizar cuando Cambia la Vigencia de un Contrato
--CREATE TRIGGER trg_Contrato_Vigencia_Change
--ON Contrato
--AFTER UPDATE
--AS
--BEGIN
--    SET NOCOUNT ON;
    
--    -- Solo actuar si cambió el campo 'vigente'
--    IF UPDATE(vigente)
--    BEGIN
--        -- Identificar los puestos afectados
--        DECLARE @PuestosAfectados TABLE (id_puesto INT);
        
--        INSERT INTO @PuestosAfectados (id_puesto)
--        SELECT DISTINCT id_puesto
--        FROM inserted
--        WHERE id_puesto IN (SELECT id_puesto FROM deleted);
        
--        -- Actualizar solo los puestos afectados
--        UPDATE p
--        SET p.sueldo_base = c.sueldo_base + 
--                            ISNULL((SELECT SUM(monto) 
--                                   FROM Complemento_puesto cp 
--                                   WHERE cp.id_puesto = p.id_puesto), 0) +
--                            ISNULL((SELECT SUM(cc.monto_complemento)
--                                   FROM Complemento_Cargo cc
--                                   INNER JOIN Contrato co ON cc.id_empleado = co.id_empleado
--                                   WHERE co.id_puesto = p.id_puesto
--                                   AND co.vigente = 'S'
--                                   AND cc.fecha_inicio <= GETDATE()
--                                   AND (cc.fecha_fin IS NULL OR cc.fecha_fin >= GETDATE())), 0)
--        FROM Puesto p
--        INNER JOIN Categoria c ON p.id_categoria = c.id_categoria
--        WHERE p.id_puesto IN (SELECT id_puesto FROM @PuestosAfectados);
        
--        -- Actualizar los porcentajes de deducción
--        EXEC ActualizarPorcentajesDeducciones;
--    END
--END;
