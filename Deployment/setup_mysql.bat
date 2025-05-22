@echo off
REM -- MySQL Configuration Script After Unattended Install --
REM Assumes MySQL is installed in the default location and added to PATH

set MYSQL_PATH="C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe"

echo.
echo Attempting to connect with known password 'yajidev'...
%MYSQL_PATH% -u root -pyajidev -e "SELECT 'Connection successful!' as Status;" 2>nul

if %ERRORLEVEL% EQU 0 (
    echo Connection successful with existing password.
    set ROOT_PASSWORD=yajidev
) else (
    echo Root password might not be set or is different.
    set /p ROOT_PASSWORD=Enter MySQL root password: 
    
    echo Testing connection...
    %MYSQL_PATH% -u root -p%ROOT_PASSWORD% -e "SELECT 'Connection test' as Status;"
    
    if %ERRORLEVEL% NEQ 0 (
        echo ERROR: Failed to connect to MySQL. Please check your password.
        pause
        exit /b 1
    )
)

echo.
echo Setting up user privileges from init_db.sql...
%MYSQL_PATH% -u root -p%ROOT_PASSWORD% < "%~dp0init_db.sql"

if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Failed to execute init_db.sql
    pause
    exit /b 1
)

echo.
echo Creating database if it doesn't exist...
%MYSQL_PATH% -u root -p%ROOT_PASSWORD% -e "CREATE DATABASE IF NOT EXISTS food_orderingsystem;"

echo.
echo Restoring ByteBiteDB.sql into food_orderingsystem...
%MYSQL_PATH% -u root -p%ROOT_PASSWORD% food_orderingsystem < "%~dp0ByteBiteDB.sql"

if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Failed to restore ByteBiteDB.sql
    pause
    exit /b 1
)

echo.
echo MySQL setup and database restoration complete successfully!
pause