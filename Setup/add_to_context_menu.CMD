@echo off
setlocal

:: Get current path, where this file and .exe file are located
set "currentDir=%~dp0"
set "programPath=%currentDir%GitCleanProject.exe"

:: Remplace backslashes by double backlashes
set "programPath=%programPath:\=\\%"

:: Use CHCP encoding for UTF-8
chcp 65001 >nul

:: Create .reg file including current path
(
echo Windows Registry Editor Version 5.00
echo.
echo [HKEY_CLASSES_ROOT\Directory\shell\NettoyerProjet]
echo @="Clean the project"
echo "Icon"="\"%programPath%\""
echo.
echo [HKEY_CLASSES_ROOT\Directory\shell\CleanProjet\command]
:: echo @="cmd.exe /c start /min \"%programPath%\" \"%%1\""
echo @="\"%programPath%\" \"%%1\""
) > add_to_context_menu_utf8.reg

iconv -f UTF-8 -t CP1252 add_to_context_menu_utf8.reg > add_to_context_menu.reg
del add_to_context_menu_utf8.reg

echo The .reg file was created successfully.
echo Execute the .reg file to use GitCleanProject from File Explorer.

pause

:: Restore default encoding (ANSI - 850 or another depending of your system)
chcp 850 >nul