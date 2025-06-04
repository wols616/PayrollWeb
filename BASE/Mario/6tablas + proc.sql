-- Crear tabla Categoria_bonificación
CREATE TABLE Categoria_bonificacion (
    id_categoria_bono INT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL
);

-- Crear tabla Bonificación
CREATE TABLE Bonificacion (
    id_bonificacion INT PRIMARY KEY,
    id_empleado INT NOT NULL,
    categoria_id INT NOT NULL,
    monto DECIMAL(10, 2) NOT NULL,
    Fecha DATE NOT NULL,
    FOREIGN KEY (categoria_id) REFERENCES Categoria_bonificacion(id_categoria_bono),
    FOREIGN KEY (id_empleado) REFERENCES Empleado(id_empleado)
);

-- Tabla principal: Nomina
CREATE TABLE Nomina (
    id_nomina INT IDENTITY(1,1) PRIMARY KEY,
    id_empleado INT NOT NULL,
    fecha_emision DATE NOT NULL,
    total_deducciones DECIMAL(10, 2),
    total_devengos DECIMAL(10, 2),
    tota_no_sujetos_de_renta DECIMAL(10, 2),
    salario_neto DECIMAL(10, 2),
    FOREIGN KEY (id_empleado) REFERENCES Empleado(id_empleado)
);

-- Tabla: Nomina_devengos
CREATE TABLE Nomina_devengos (
    id_nomina_cargo INT IDENTITY(1,1) PRIMARY KEY,
    id_nomina INT NOT NULL,
    nombre_devengo VARCHAR(100) NOT NULL,
    monto DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (id_nomina) REFERENCES Nomina(id_nomina)
);

-- Tabla: Nomina_no_sujetos_de_renta
CREATE TABLE Nomina_no_sujetos_de_renta (
    id_nomina_cargo INT IDENTITY(1,1) PRIMARY KEY,
    id_nomina INT NOT NULL,
    nombre_devengo VARCHAR(100) NOT NULL,
    monto DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (id_nomina) REFERENCES Nomina(id_nomina)
);

-- Tabla: Nomina_Deduccion
CREATE TABLE Nomina_Deduccion (
    id_nomina_deduccion INT IDENTITY(1,1) PRIMARY KEY,
    id_nomina INT NOT NULL,
    nombre_deduccion VARCHAR(100) NOT NULL,
    monto_deduccion DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (id_nomina) REFERENCES Nomina(id_nomina)
);

--PRCEDIMIOENTOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO--

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
            @SalarioNeto               DECIMAL(10,2)   = 0.00;

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
        -- 2.a.5) Sumar bonificaciones del mes con categoria_id = 1 ("aguinaldo", no sujeto a renta)
        ------------
        SELECT
            @TotalNoSujeto = ISNULL(
                SUM(b.monto),
                0.00
            )
        FROM Bonificacion b
        WHERE
            b.id_empleado = @EmpleadoId
            AND MONTH(b.Fecha) = @Mes
            AND YEAR(b.Fecha) = @Anio
            AND b.categoria_id = 1;

        ------------
        -- 2.b) Calcular total_devengos
        ------------
        SET @TotalDevengos = 
            ISNULL(@SueldoBase,         0.00)
          + ISNULL(@TotalCompPuesto,   0.00)
          + ISNULL(@TotalTrienios,     0.00)
          + ISNULL(@TotalBonifCatGt1,  0.00);

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
                WHEN @BaseParaISR BETWEEN 472.01 AND 895.24 THEN ROUND(@BaseParaISR * 0.10, 2)
                WHEN @BaseParaISR BETWEEN 895.25 AND 2038.10 THEN ROUND(@BaseParaISR * 0.20, 2)
                ELSE                                           ROUND(@BaseParaISR * 0.30, 2)
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
        --      Salario Neto = total_devengos - total_deducciones + total_no_sujeto (aguinaldo)
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

        ----------------------------------------------------------------
        -- 5) Llenar Nomina_no_sujetos_de_renta (solo aguinaldo si existe)
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
                'Aguinaldo',
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
            ROUND(@BaseParaISR * (dp.porcentaje_personal / 100.0), 2)
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


