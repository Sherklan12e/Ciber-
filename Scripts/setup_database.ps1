# Script de PowerShell para crear la base de datos del Sistema Ciber
# Ejecutar como administrador si es necesario

Write-Host "🔧 Configurando base de datos para Sistema Ciber..." -ForegroundColor Green

# Verificar si MySQL está disponible
try {
    $mysqlVersion = mysql --version 2>$null
    if ($mysqlVersion) {
        Write-Host "✅ MySQL encontrado: $mysqlVersion" -ForegroundColor Green
    } else {
        Write-Host "❌ MySQL no encontrado en el PATH" -ForegroundColor Red
        Write-Host "💡 Solución: Ejecuta manualmente el script SQL desde tu cliente MySQL" -ForegroundColor Yellow
        Write-Host "📁 Archivo: Scripts/setup_database_improved.sql" -ForegroundColor Cyan
        exit 1
    }
} catch {
    Write-Host "❌ Error al verificar MySQL" -ForegroundColor Red
    Write-Host "💡 Solución: Ejecuta manualmente el script SQL desde tu cliente MySQL" -ForegroundColor Yellow
    Write-Host "📁 Archivo: Scripts/setup_database_improved.sql" -ForegroundColor Cyan
    exit 1
}

# Solicitar credenciales
$username = Read-Host "Usuario MySQL (por defecto: root)"
if ([string]::IsNullOrEmpty($username)) {
    $username = "root"
}

$password = Read-Host "Contraseña MySQL" -AsSecureString
$passwordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($password))

# Ejecutar el script SQL
Write-Host "🚀 Ejecutando script de creación de base de datos..." -ForegroundColor Yellow

try {
    $scriptPath = "Scripts\setup_database_improved.sql"
    if (Test-Path $scriptPath) {
        mysql -u $username -p$passwordPlain < $scriptPath
        Write-Host "✅ Base de datos creada exitosamente!" -ForegroundColor Green
        Write-Host "🎉 Ahora puedes ejecutar la aplicación con: dotnet run" -ForegroundColor Cyan
    } else {
        Write-Host "❌ No se encontró el archivo: $scriptPath" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Error al ejecutar el script SQL" -ForegroundColor Red
    Write-Host "💡 Ejecuta manualmente el script desde tu cliente MySQL" -ForegroundColor Yellow
}

Write-Host "`n📋 Pasos siguientes:" -ForegroundColor Cyan
Write-Host "1. Verifica que las tablas se crearon correctamente" -ForegroundColor White
Write-Host "2. Ejecuta: cd src/Ciber.MVC" -ForegroundColor White
Write-Host "3. Ejecuta: dotnet run" -ForegroundColor White
Write-Host "4. Abre: https://localhost:5001" -ForegroundColor White
