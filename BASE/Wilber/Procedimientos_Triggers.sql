use payroll_pruebas
--PROCEDIMIENTOS/TRIGGERS

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
--__________________________________________________________________________________________________________
--PROCEDIMIENTO PARA ACTUALIZAR EL PORCENTAJE PERSONAL DEL AFP, ISSS Y EL ISR
CREATE PROCEDURE ActualizarPorcentajesDeducciones 
AS
BEGIN
    -- Variables para almacenar valores temporales
    DECLARE @IdEmpleado INT;
    DECLARE @IdDeduccion INT;
    DECLARE @SueldoBase DECIMAL(10, 2);
    DECLARE @PorcentajePersonal DECIMAL(5, 2);
    DECLARE @PorcentajeDeduccion DECIMAL(5, 2);

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
        -- Calcular el porcentaje_personal según la deducción
        IF (SELECT nombre_deduccion FROM Deduccion WHERE id_deduccion = @IdDeduccion) = 'AFP'
        BEGIN
            -- AFP: 7.25% del sueldo base
            SET @PorcentajePersonal = 7.25;
        END
        ELSE IF (SELECT nombre_deduccion FROM Deduccion WHERE id_deduccion = @IdDeduccion) = 'ISSS'
        BEGIN
            -- ISSS: 3% del sueldo base (hasta un máximo de $1000)
            SET @PorcentajePersonal = 3.00;
        END
        ELSE IF (SELECT nombre_deduccion FROM Deduccion WHERE id_deduccion = @IdDeduccion) = 'ISR'
        BEGIN
            -- ISR: Aplicar escala progresiva (ejemplo simplificado)
            IF @SueldoBase <= 472.00
            BEGIN
                SET @PorcentajePersonal = 0.00;
            END
            ELSE IF @SueldoBase <= 895.24
            BEGIN
                SET @PorcentajePersonal = 10.00;
            END
            ELSE IF @SueldoBase <= 2038.10
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

