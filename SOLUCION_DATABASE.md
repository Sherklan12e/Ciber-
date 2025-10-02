# 🔧 Solución al Error de Base de Datos

## ❌ Problema
El error indica que la tabla `historialalquiler` no existe en la base de datos:
```
Table '5to_ciber.historialalquiler' doesn't exist
```

## ✅ Solución

### Opción 1: Ejecutar Script SQL (Recomendado)

1. **Abre tu cliente MySQL** (phpMyAdmin, MySQL Workbench, o línea de comandos)

2. **Ejecuta el script** `Scripts/create_database.sql`:
   - Copia todo el contenido del archivo
   - Pégalo en tu cliente MySQL
   - Ejecuta el script

### Opción 2: Desde Línea de Comandos

Si tienes MySQL en el PATH:

```bash
mysql -u root -proot < Scripts/create_database.sql
```

### Opción 3: Ejecutar Manualmente

Si no puedes ejecutar el script completo, ejecuta estos comandos uno por uno:

```sql
CREATE DATABASE IF NOT EXISTS `5to_ciber` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `5to_ciber`;

CREATE TABLE IF NOT EXISTS `Cuenta` (
    `Ncuenta` INT AUTO_INCREMENT,
    `nombre` VARCHAR(45) CHARACTER SET utf8mb4, 
    `pass` CHAR(64),
    `dni` INT,
    `horaRegistrada` TIME,
    PRIMARY KEY (`Ncuenta`)
);

CREATE TABLE IF NOT EXISTS `Maquina` (
    `Nmaquina` INT AUTO_INCREMENT,
    `estado` BOOL,
    `caracteristicas` VARCHAR(45),
    PRIMARY KEY (`Nmaquina`)
);

CREATE TABLE IF NOT EXISTS `Tipo` (
    `idTipo` INT AUTO_INCREMENT,
    `tipo` VARCHAR(45),
    PRIMARY KEY (`idTipo`)
);

CREATE TABLE IF NOT EXISTS `Alquiler` (
    `idAlquiler` INT AUTO_INCREMENT,
    `Ncuenta` INT,
    `Nmaquina` INT,
    `tipo` INT,
    `cantidadTiempo` TIME,
    `pagado` BOOL NULL,
    PRIMARY KEY (`idAlquiler`),
    CONSTRAINT `FK_Reservacion_Cuenta` FOREIGN KEY (`Ncuenta`) REFERENCES `Cuenta` (`Ncuenta`),
    CONSTRAINT `FK_Reservacion_Tipo` FOREIGN KEY (`tipo`) REFERENCES `Tipo` (`idTipo`),
    CONSTRAINT `FK_Reservacion_Maquina` FOREIGN KEY (`Nmaquina`) REFERENCES `Maquina` (`Nmaquina`)
);

CREATE TABLE IF NOT EXISTS `HistorialdeAlquiler` (
    `idHistorial` INT AUTO_INCREMENT,
    `Ncuenta` INT,
    `Nmaquina` INT,
    `fechaInicio` DATETIME,
    `fechaFin` DATETIME,
    `TotalPagar` DECIMAL(10,2),
    PRIMARY KEY (`idHistorial`),
    CONSTRAINT `FK_HistorialdeAlquiler_Maquina` FOREIGN KEY (`Nmaquina`) REFERENCES `Maquina` (`Nmaquina`),
    CONSTRAINT `FK_HistorialdeAlquiler_Cuenta` FOREIGN KEY (`Ncuenta`) REFERENCES `Cuenta` (`Ncuenta`)
);

INSERT IGNORE INTO `Tipo` (`tipo`) VALUES ('Libre'), ('Hora Definida');
```

## 🔍 Verificación

Después de ejecutar el script, verifica que las tablas se crearon:

```sql
USE 5to_ciber;
SHOW TABLES;
```

Deberías ver:
- Cuenta
- Maquina
- Tipo
- Alquiler
- HistorialdeAlquiler

## 🚀 Reiniciar la Aplicación

1. **Detén la aplicación** (Ctrl+C en la terminal)
2. **Reinicia** con `dotnet run`
3. **Prueba** accediendo al dashboard y al historial

## 📝 Notas

- ✅ **Corregido**: Los nombres de tabla en el código Dapper ahora coinciden con el script DDL
- ✅ **Incluido**: Datos de ejemplo para probar la aplicación
- ✅ **Optimizado**: Script con `CREATE TABLE IF NOT EXISTS` para evitar errores si ya existen

## 🆘 Si Sigues Teniendo Problemas

1. Verifica que MySQL esté ejecutándose
2. Confirma que la cadena de conexión en `appsettings.json` sea correcta
3. Asegúrate de que el usuario `root` tenga permisos para crear bases de datos
4. Revisa que no haya errores de sintaxis en el script SQL
