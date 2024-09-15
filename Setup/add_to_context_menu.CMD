@echo off
setlocal

:: Récupérer le chemin courant du répertoire où se trouve le batch
set "currentDir=%~dp0"
set "programPath=%currentDir%GitCleanProject.exe"

:: Remplacer les simples antislashs par des doubles antislashs
set "programPath=%programPath:\=\\%"

:: Utilisation de l'encodeur CHCP pour UTF-8
chcp 65001 >nul

:: Créer le fichier .reg avec le chemin dynamique
(
echo Windows Registry Editor Version 5.00
echo.
echo [HKEY_CLASSES_ROOT\Directory\shell\NettoyerProjet]
echo @="Nettoyer le projet"
echo "Icon"="\"%programPath%\""
echo.
echo [HKEY_CLASSES_ROOT\Directory\shell\NettoyerProjet\command]
REM echo @="cmd.exe /c start /min \"%programPath%\" \"%%1\""
echo @="\"%programPath%\" \"%%1\""
) > add_to_context_menu_utf8.reg

iconv -f UTF-8 -t CP1252 add_to_context_menu_utf8.reg > add_to_context_menu.reg
del add_to_context_menu_utf8.reg

echo Le fichier .reg a été crée avec succès.
echo Exécutez le fichier .reg pour pouvoir exécuter GitCleanProject depuis l'Explorateur de fichier.

pause

:: Restaurer l'encodage par défaut (ANSI - 850 ou autre selon votre système)
chcp 850 >nul
