DROP DATABASE IF EXISTS 5to_Ciber;
CREATE DATABASE 5to_Ciber CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

USE 5to_Ciber;


-- Tabla Cuenta
CREATE TABLE Cuenta (
    Ncuenta INT AUTO_INCREMENT,
    nombre VARCHAR(45) CHARACTER SET utf8mb4, 
	pass CHAR(64),
    dni INT,
    horaRegistrada TIME,
    PRIMARY KEY (Ncuenta)
);

-- Tabla Maquina
CREATE TABLE Maquina (
    Nmaquina INT AUTO_INCREMENT,
    estado BOOL ,
    caracteristicas VARCHAR(45),
    PRIMARY KEY (Nmaquina)
);

-- Tabla Tipo
CREATE TABLE Tipo (
    idTipo INT AUTO_INCREMENT,
    tipo VARCHAR(45),
    PRIMARY KEY (idTipo)
);

-- Tabla ALquiler
CREATE TABLE Alquiler (
    idAlquiler INT AUTO_INCREMENT,
    Ncuenta INT,
	Nmaquina INT,
    tipo INT,
    cantidadTiempo TIME, -- si elije la opcion 2
    pagado BOOL NULL , -- Columna para estado de pago
    PRIMARY KEY (idAlquiler),
    CONSTRAINT FK_Reservacion_Cuenta FOREIGN KEY (Ncuenta) REFERENCES Cuenta (Ncuenta),
    CONSTRAINT FK_Reservacion_Tipo FOREIGN KEY (tipo) REFERENCES Tipo (idTipo),
	constraint FK_Reservacion_Maquina FOREIGN KEY (Nmaquina) REFERENCES Maquina (Nmaquina)
);

-- Tabla HistorialdeAlquiler
CREATE TABLE HistorialdeAlquiler (
    idHistorial INT AUTO_INCREMENT,
    Ncuenta INT,
    Nmaquina INT,
    fechaInicio DATETIME,
    fechaFin DATETIME,
    TotalPagar decimal,
    PRIMARY KEY (idHistorial),
    CONSTRAINT FK_HistorialdeAlquiler_Maquina FOREIGN KEY (Nmaquina) REFERENCES Maquina (Nmaquina),
	constraint FK_HistorialdeAlquiler_Cuenta FOREIGN KEY (Ncuenta) REFERENCES Cuenta (Ncuenta)
);




-- Los datos para el tipo si es libre o una hora ya definida IMPORTANTEEEEEE
INSERT INTO Tipo (tipo) VALUES ('Libre');
INSERT INTO Tipo (tipo) VALUES ('Hora Definida');

-- Ajustes no destructivos para compatibilidad con lógica mejorada
-- Maquina: ampliar caracteristicas y agregar precio/tipo si no existen
-- Maquina
-- Maquina
ALTER TABLE Maquina
    MODIFY caracteristicas VARCHAR(100),
    ADD COLUMN precioPorHora DECIMAL(10,2) DEFAULT 5.00 AFTER caracteristicas,
    ADD COLUMN tipoMaquina VARCHAR(20) DEFAULT 'Estándar' AFTER precioPorHora;

-- Alquiler
ALTER TABLE Alquiler
  ADD COLUMN fechaInicio DATETIME DEFAULT CURRENT_TIMESTAMP AFTER pagado,
  ADD COLUMN fechaFin DATETIME NULL AFTER fechaInicio,
  ADD COLUMN precioPorHora DECIMAL(10,2) AFTER fechaFin,
  ADD COLUMN totalAPagar DECIMAL(10,2) NULL AFTER precioPorHora,
  ADD COLUMN montoPagado DECIMAL(10,2) NULL AFTER totalAPagar;

-- HistorialdeAlquiler
ALTER TABLE HistorialdeAlquiler
  MODIFY TotalPagar DECIMAL(10,2),
  ADD COLUMN montoPagado DECIMAL(10,2) AFTER TotalPagar,
  ADD COLUMN precioPorHora DECIMAL(10,2) AFTER montoPagado;



-- Maquina
ALTER TABLE Maquina
  MODIFY caracteristicas VARCHAR(100);
ALTER TABLE Maquina
  ADD COLUMN precioPorHora DECIMAL(10,2) DEFAULT 5.00;
ALTER TABLE Maquina
  ADD COLUMN tipoMaquina VARCHAR(20) DEFAULT 'Estándar';

-- Alquiler
ALTER TABLE Alquiler
  ADD COLUMN fechaInicio DATETIME DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE Alquiler
  ADD COLUMN fechaFin DATETIME NULL;
ALTER TABLE Alquiler
  ADD COLUMN precioPorHora DECIMAL(10,2);
ALTER TABLE Alquiler
  ADD COLUMN totalAPagar DECIMAL(10,2) NULL;
ALTER TABLE Alquiler
  ADD COLUMN montoPagado DECIMAL(10,2) NULL;

-- HistorialdeAlquiler
ALTER TABLE HistorialdeAlquiler
  MODIFY TotalPagar DECIMAL(10,2);
ALTER TABLE HistorialdeAlquiler
  ADD COLUMN montoPagado DECIMAL(10,2);
ALTER TABLE HistorialdeAlquiler
  ADD COLUMN precioPorHora DECIMAL(10,2);
