--OJO: el procedimiento es funcional pero, no funcionara bien si tenes datos sin sentido en contratos, 
--por ejemplo si insertas la serie de contratos para empleado 2 o 1, deberia crear un trienio, pero no lo hara si ya 
--tenes otros contratos tu en ese rango de fechas, es como que se bloquea si un empleado tiene varios contratos en
--el mismo intervalo de tiempo, por ejemplo para empleado 2, que en las inserciones de abajo de ejemplo tiene contratos desde el 2009
--pero como en la base de por si ya habian contratos del empleado 2, en ese intervalo de tiempo, el proc ya no puede calcular bien el trienio
--esto no representa un problema serio pues en el sistema no deberia permitirse crear otro contrato sin cancelar el primero
--es mas como un detalle de la base por si tenes datos que chocan o contratos en el mismo intervalo de tiempo

--Cuando canceles un contrato tendras que ejecutar ese proc para poner el trienio con estado N


--COMANDOS DE PRUEBA
--EXEC CalcularTrienios;
--Select * from trienios
--select * from puesto;
--select * from Contrato where id_empleado = 2
--select * from contrato where id_empleado = 1
--delete from contrato where id_empleado = 1
--delete puesto_historico
--delete Historial_Contrato
--DELETE trienios
--DELETE Contrato

--Datos de prueba:
--Contratos para empleado 1(sueldo base $4500)
INSERT INTO Contrato (id_empleado, fecha_alta, fecha_baja, id_puesto, tipo_contrato, vigente)
VALUES (33, '2022-01-03', NULL, 1, 'Indefinido', 'N');
INSERT INTO Contrato (id_empleado, fecha_alta, fecha_baja, id_puesto, tipo_contrato, vigente)
VALUES (33, '2018-01-03', '2022-01-03', 1, 'Indefinido', 'N');
INSERT INTO Contrato (id_empleado, fecha_alta, fecha_baja, id_puesto, tipo_contrato, vigente)
VALUES (1, '2014-01-03', '2018-01-03', 1, 'Indefinido', 'N');

-- Contrados para empleado 2, Sueldo base del puesto 4(puesto actual) = 2800
INSERT INTO Contrato (id_empleado, fecha_alta, fecha_baja, id_puesto, tipo_contrato, vigente)
VALUES 
(2, '2009-04-01', '2014-04-01', 3, 'Indefinido', 'N'),
(2, '2014-04-01', '2019-04-01', 2, 'Indefinido', 'N'),
(2, '2019-04-01', NULL,          4, 'Indefinido', 'S');

select * from Contrato
WHERE Contrato.id_empleado = 35

select * from Trienios

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



CREATE PROCEDURE usp_GenerarNominaMensual
    @Fecha DATE  -- cualquier día del mes para el cual queremos generar nómina
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @Anio            INT,
        @Mes             INT,
        @PrimerDiaMes    DATE;

    -- 1) Calcular año, mes y primer día de mes
    SET @Anio = YEAR(@Fecha);
    SET @Mes  = MONTH(@Fecha);
    SET @PrimerDiaMes = DATEFROMPARTS(@Anio, @Mes, 1);


    ----------------------------------------------------------------
    -- 2) Cursor sobre todos los empleados con estado = 'Activo'
    ----------------------------------------------------------------
    DECLARE @EmpleadoId       INT;

    DECLARE Cur_Activos CURSOR FOR
        SELECT E.id_empleado
        FROM Empleado E
        WHERE E.estado = 'Activo';

    OPEN Cur_Activos;
    FETCH NEXT FROM Cur_Activos INTO @EmpleadoId;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        ------------
        -- 2.a) Obtener datos base para este empleado
        ------------
        DECLARE 
            @SueldoBase               DECIMAL(10,2)   = 0.00,
            @IdPuesto                 INT              = NULL,
            @TotalCompPuesto           DECIMAL(10,2)   = 0.00,
            @TotalTrienios             DECIMAL(10,2)   = 0.00,
            @TotalBonifCatGt1          DECIMAL(10,2)   = 0.00,
            @TotalNoSujeto             DECIMAL(10,2)   = 0.00,
            @TotalDevengos             DECIMAL(10,2)   = 0.00,
            @AFP                       DECIMAL(10,2)   = 0.00,
            @ISSS                      DECIMAL(10,2)   = 0.00,
            @BaseParaISR               DECIMAL(10,2)   = 0.00,
            @ISR                       DECIMAL(10,2)   = 0.00,
            @TotalOtrosDeducciones     DECIMAL(10,2)   = 0.00,
            @TotalDeducciones          DECIMAL(10,2)   = 0.00,
            @SalarioNeto               DECIMAL(10,2)   = 0.00,
            @AguinaldoExento           DECIMAL(10,2)   = 0.00,  -- Nueva variable para aguinaldo exento
            @AguinaldoSujeto           DECIMAL(10,2)   = 0.00;  -- Nueva variable para aguinaldo sujeto a renta

        -- 2.a.1) Obtener el contrato vigente y salario base
        SELECT 
            @IdPuesto    = C.id_puesto,
            @SueldoBase  = P.sueldo_base
        FROM Contrato C
        JOIN Puesto P ON C.id_puesto = P.id_puesto
        WHERE 
            C.id_empleado = @EmpleadoId
            AND C.vigente = 'S';  -- Se ajusta aquí: 'S' en vez de 'Y'

        IF @SueldoBase IS NULL
            SET @SueldoBase = 0.00;

        ------------
        -- 2.a.2) Sumar todos los complementos del puesto actual
        ------------
        SELECT 
            @TotalCompPuesto = ISNULL(SUM(cp.monto), 0.00)
        FROM Complemento_puesto cp
        WHERE cp.id_puesto = @IdPuesto;

        ------------
        -- 2.a.3) Sumar todos los trienios activos (donde estado = 'S')
        ------------
        SELECT
            @TotalTrienios = ISNULL(SUM(t.monto), 0.00)
        FROM Trienios t
        WHERE 
            t.id_empleado = @EmpleadoId
            AND t.estado = 'S';

        ------------
        -- 2.a.4) Sumar bonificaciones del mes con categoria_id > 1 (son "devengos sujetos de renta")
        ------------
        SELECT
            @TotalBonifCatGt1 = ISNULL(
                SUM(b.monto), 
                0.00
            )
        FROM Bonificacion b
        WHERE 
            b.id_empleado = @EmpleadoId
            AND MONTH(b.Fecha) = @Mes
            AND YEAR(b.Fecha) = @Anio
            AND b.categoria_id > 1;

        ------------
        -- 2.a.5) Sumar bonificaciones del mes con categoria_id = 1 ("aguinaldo", parcialmente sujeto a renta)
        -- MODIFICACIÓN: Solo los primeros $1,500 son no sujetos a renta
        ------------
        DECLARE @TotalAguinaldo DECIMAL(10,2) = 0.00;
        
        SELECT
            @TotalAguinaldo = ISNULL(
                SUM(b.monto),
                0.00
            )
        FROM Bonificacion b
        WHERE
            b.id_empleado = @EmpleadoId
            AND MONTH(b.Fecha) = @Mes
            AND YEAR(b.Fecha) = @Anio
            AND b.categoria_id = 1;
            
        -- Calcular la parte exenta y la parte sujeta a renta
        IF @TotalAguinaldo <= 1500.00
        BEGIN
            SET @AguinaldoExento = @TotalAguinaldo;
            SET @AguinaldoSujeto = 0.00;
        END
        ELSE
        BEGIN
            SET @AguinaldoExento = 1500.00;
            SET @AguinaldoSujeto = @TotalAguinaldo - 1500.00;
        END
        
        SET @TotalNoSujeto = @AguinaldoExento;
        -- La parte sujeta (@AguinaldoSujeto) se agregará a los devengos sujetos a renta

        ------------
        -- 2.b) Calcular total_devengos (ahora incluye la parte del aguinaldo sujeta a renta)
        ------------
        SET @TotalDevengos = 
            ISNULL(@SueldoBase,         0.00)
          + ISNULL(@TotalCompPuesto,   0.00)
          + ISNULL(@TotalTrienios,     0.00)
          + ISNULL(@TotalBonifCatGt1,  0.00)
          + ISNULL(@AguinaldoSujeto,   0.00);  -- Agregamos la parte del aguinaldo sujeta a renta

        ------------
        -- 2.c) Calcular AFP = 7.25% de sueldo base
        ------------
        SET @AFP = ROUND(@SueldoBase * 0.0725, 2);

        ------------
        -- 2.d) Calcular ISSS = 3% de sueldo base, pero máximo $30 si sueldo_base > 1000
        ------------
        IF @SueldoBase > 1000.00
            SET @ISSS = 30.00;
        ELSE
            SET @ISSS = ROUND(@SueldoBase * 0.03, 2);

        ------------
        -- 2.e) Calcular base para ISR = total_devengos - afp - isss
        ------------
        SET @BaseParaISR = @TotalDevengos - @AFP - @ISSS;

        ------------
        -- 2.f) Calcular ISR por rangos sobre @BaseParaISR
        ------------
        SET @ISR = 
            CASE 
                WHEN @BaseParaISR <= 472.00                 THEN 0.00
                WHEN @BaseParaISR BETWEEN 472.01 AND 895.24 THEN ROUND((@BaseParaISR - 472.00) * 0.10 + 17.67, 2)
                WHEN @BaseParaISR BETWEEN 895.25 AND 2038.10 THEN ROUND((@BaseParaISR - 895.24) * 0.20 + 60.00, 2)
                ELSE                                           ROUND((@BaseParaISR - 2038.10) * 0.30 + 288.57, 2)
            END;

        -- 2.g) Calcular "otras deducciones personales" (id_deduccion > 3)
--      Monto_i = (baseParaISR - ISR) * (porcentaje_personal / 100)

		SELECT 
			@TotalOtrosDeducciones = ISNULL(
				SUM(
					ROUND(
						(@BaseParaISR - @ISR) * (dp.porcentaje_personal / 100.0),
						2
					)
				), 
				0.00
			)
		FROM Deduccion_Personal dp
		JOIN Deduccion d 
			ON dp.id_deduccion = d.id_deduccion
		WHERE 
			dp.id_empleado = @EmpleadoId
			AND d.id_deduccion > 3;


        ------------
        -- 2.h) Total de deducciones = afp + isss + isr + otras
        ------------
        SET @TotalDeducciones = 
            ISNULL(@AFP,                0.00)
          + ISNULL(@ISSS,               0.00)
          + ISNULL(@ISR,                0.00)
          + ISNULL(@TotalOtrosDeducciones, 0.00);

        ------------
        -- 2.i) Salario neto (según fórmula solicitada)
        --      Salario Neto = total_devengos - total_deducciones + total_no_sujeto (aguinaldo exento)
        ------------
        SET @SalarioNeto = 
            @TotalDevengos 
          - @TotalDeducciones 
          + ISNULL(@TotalNoSujeto, 0.00);

        ----------------------------------------------------------------
        -- 3) Insertar el registro en la tabla Nomina
        ----------------------------------------------------------------
        INSERT INTO Nomina
        (
            id_empleado, 
            fecha_emision, 
            total_deducciones, 
            total_devengos, 
            tota_no_sujetos_de_renta, 
            salario_neto
        )
        VALUES
        (
            @EmpleadoId,
            @Fecha,
            @TotalDeducciones,
            @TotalDevengos,
            @TotalNoSujeto,
            @SalarioNeto
        );

        DECLARE @IdNominaActual INT = SCOPE_IDENTITY();

        ----------------------------------------------------------------
        -- 4) Llenar Nomina_devengos
        ----------------------------------------------------------------
        -- 4.1) Salario Base
        INSERT INTO Nomina_devengos
        (
            id_nomina, 
            nombre_devengo, 
            monto
        )
        VALUES
        (
            @IdNominaActual,
            'Salario Base',
            @SueldoBase
        );

        -- 4.2) Complementos de Puesto
        INSERT INTO Nomina_devengos (id_nomina, nombre_devengo, monto)
        SELECT
            @IdNominaActual,
            cp.nombre_complemento,
            cp.monto
        FROM Complemento_puesto cp
        WHERE cp.id_puesto = @IdPuesto;

        -- 4.3) Trienios (estado = 'S')
        INSERT INTO Nomina_devengos (id_nomina, nombre_devengo, monto)
        SELECT
            @IdNominaActual,
            'Trienio',
            t.monto
        FROM Trienios t
        WHERE 
            t.id_empleado = @EmpleadoId
            AND t.estado = 'S';

        -- 4.4) Cargos vigentes: fecha_fin >= primer día del mes
        INSERT INTO Nomina_devengos (id_nomina, nombre_devengo, monto)
        SELECT
            @IdNominaActual,
            c.nombre_cargo,
            cc.monto_complemento
        FROM Complemento_Cargo cc
        JOIN Cargo c ON cc.id_cargo = c.id_cargo
        WHERE 
            cc.id_empleado = @EmpleadoId
            AND cc.fecha_fin >= @PrimerDiaMes;

        -- 4.5) Bonificaciones de categoría > 1 en ese mes
        INSERT INTO Nomina_devengos (id_nomina, nombre_devengo, monto)
        SELECT
            @IdNominaActual,
            cb.nombre AS nombre_devengo,
            b.monto
        FROM Bonificacion b
        JOIN Categoria_bonificacion cb 
            ON b.categoria_id = cb.id_categoria_bono
        WHERE
            b.id_empleado = @EmpleadoId
            AND MONTH(b.Fecha) = @Mes
            AND YEAR(b.Fecha) = @Anio
            AND b.categoria_id > 1;
            
        -- 4.6) Parte del aguinaldo sujeta a renta (si existe)
        IF @AguinaldoSujeto > 0.00
        BEGIN
            INSERT INTO Nomina_devengos (id_nomina, nombre_devengo, monto)
            VALUES (@IdNominaActual, 'Aguinaldo (sujeto a renta)', @AguinaldoSujeto);
        END

        ----------------------------------------------------------------
        -- 5) Llenar Nomina_no_sujetos_de_renta (solo la parte exenta del aguinaldo)
        ----------------------------------------------------------------
        IF @TotalNoSujeto > 0.00
        BEGIN
            INSERT INTO Nomina_no_sujetos_de_renta
            (
                id_nomina, 
                nombre_devengo, 
                monto
            )
            VALUES
            (
                @IdNominaActual,
                'Aguinaldo (exento)',
                @TotalNoSujeto
            );
        END

        ----------------------------------------------------------------
        -- 6) Llenar Nomina_Deduccion
        ----------------------------------------------------------------
        -- 6.1) AFP
        INSERT INTO Nomina_Deduccion (id_nomina, nombre_deduccion, monto_deduccion)
        VALUES (@IdNominaActual, 'AFP', @AFP);

        -- 6.2) ISSS
        INSERT INTO Nomina_Deduccion (id_nomina, nombre_deduccion, monto_deduccion)
        VALUES (@IdNominaActual, 'ISSS', @ISSS);

        -- 6.3) ISR
        INSERT INTO Nomina_Deduccion (id_nomina, nombre_deduccion, monto_deduccion)
        VALUES (@IdNominaActual, 'ISR', @ISR);

        -- 6.4) Otras deducciones personales (id_deduccion > 3)
        INSERT INTO Nomina_Deduccion (id_nomina, nombre_deduccion, monto_deduccion)
        SELECT
            @IdNominaActual,
            d.nombre_deduccion,
            ROUND((@BaseParaISR - @ISR) * (dp.porcentaje_personal / 100.0), 2)
        FROM Deduccion_Personal dp
        JOIN Deduccion d ON dp.id_deduccion = d.id_deduccion
        WHERE
            dp.id_empleado = @EmpleadoId
            AND d.id_deduccion > 3;

        ----------------------------------------------------------------
        -- Fin de procesamiento de este empleado
        ----------------------------------------------------------------
        FETCH NEXT FROM Cur_Activos INTO @EmpleadoId;
    END

    CLOSE Cur_Activos;
    DEALLOCATE Cur_Activos;

    SET NOCOUNT OFF;
END;
GO