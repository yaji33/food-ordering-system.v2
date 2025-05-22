; -- Inno Setup Script for Byte Bite Food Ordering System C# Demo --

[Setup]
AppName=Byte Bite Food Ordering System
AppVersion=1.0
DefaultDirName={pf}\Byte_Bite_Food_Ordering_System
DefaultGroupName=Byte Bite Food Ordering System
OutputDir=.
OutputBaseFilename=Byte_Bite_Food_Ordering_System_Installer
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "installmysql"; Description: "Install MySQL Server"; GroupDescription: "Database Setup"; Flags: checkedonce

[Files]
; Application executable
Source: "food-ordering-system.v2.exe"; DestDir: "{app}"; Flags: ignoreversion

; DLL dependencies
Source: "System.Buffers.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "System.Memory.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "System.Numerics.Vectors.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "System.Threading.Tasks.Extensions.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "MySql.Data.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "BouncyCastle.Cryptography.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Google.Protobuf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "K4os.Compression.LZ4.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "K4os.Compression.LZ4.Streams.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "K4os.Hash.xxHash.dll"; DestDir: "{app}"; Flags: ignoreversion

Source: "ZstdSharp.dll"; DestDir: "{app}"; Flags: ignoreversion

; MySQL setup files
Source: "init_db.sql"; DestDir: "{app}"; Flags: ignoreversion
Source: "ByteBiteDB.sql"; DestDir: "{app}"; Flags: ignoreversion
Source: "setup_mysql.bat"; DestDir: "{app}"; Flags: ignoreversion

; Configuration file
Source: "app.config"; DestDir: "{app}"; DestName: "food-ordering-system.v2.exe.config"; Flags: ignoreversion

; MySQL Installer (must be manually placed in the build directory)
Source: "mysql-installer-community-8.0.41.0.msi"; DestDir: "{tmp}"; Flags: ignoreversion

[Run]
; Silent installation of MySQL (only if task is selected)
Filename: "msiexec.exe"; Parameters: "/i ""{tmp}\mysql-installer-community-8.0.41.0.msi"" /qn /norestart"; Flags: runhidden waituntilterminated; Tasks: installmysql

; Execute MySQL configuration and DB restoration
Filename: "cmd.exe"; Parameters: "/C ""{app}\setup_mysql.bat"""; Flags: runhidden waituntilterminated

; Launch the C# application
Filename: "{app}\food-ordering-system.v2.exe"; Description: "Launch Byte Bite Application"; Flags: nowait postinstall skipifsilent



[Icons]
Name: "{group}\Byte Bite Food Ordering System"; Filename: "{app}\food-ordering-system.v2.exe"
Name: "{group}\Database Setup"; Filename: "{app}\setup_mysql.bat"
Name: "{group}\{cm:UninstallProgram,Byte Bite}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\Byte Bite"; Filename: "{app}\food-ordering-system.v2.exe"; Tasks: desktopicon