@echo off
set myParam=%1
set manageBLD=%2
rem Legal Values: ManageBLD | NO_ManageBLD
set projectPath=%3
set dllPath=%4
set configuration=%5
set info_buildconfig=%configuration%
set more_info=XXX
set targetScript=UpdateBuildLog.ps1
set targetScriptPath=%~dp0
set targetScriptFullPath=%targetScriptPath%%targetScript%

echo ------------------------------ Post-build event started -------------------
echo Configuration:
echo --------------
echo %myParam%
echo %manageBLD%
echo %projectPath%
echo -------------------------------------
if "%myParam%"=="ValueA" GOTO :doForValueA
if "%myParam%"=="ValueB" GOTO :doForValueB
echo Updating Build information...
rem If you wish to auto-increment build for RELEASE configuration only, uncomment following comment and comment the line after it.
rem if "%manageBLD%"=="ManageBLD" if "%configuration%"=="Release" GOTO :manageBuild
if "%manageBLD%"=="ManageBLD" GOTO :manageBuild
echo ------------------------------ Post-build event ended -------------------


:doForValueA
echo ValueA task: Start...
rem: do something here related to ValueA Param
if %errorlevel% neq 0 Exit %errorlevel%
echo ValueA task: Over...
echo --------------

:doForValueB
echo ValueB task: Start...
rem: do something here related to ValueB Param
if %errorlevel% neq 0 Exit %errorlevel%
echo ValueB task: Over...
echo --------------

:manageBuild
echo Managing build...
echo --------------
echo Configuration:
echo --------------
set projPath=%projectPath:\"=\\"%
echo %projPath%
echo %dllPath%
echo %info_buildconfig%
echo %targetScriptFullPath%
echo -------------------------------------
PowerShell.exe -ExecutionPolicy ByPass -File %targetScriptFullPath% -projectPath %projPath% -targetDLLFullPath %dllPath% -extraAttribute %more_info% -buildConfig %info_buildconfig%
if %errorlevel% neq 0 Exit %errorlevel%
echo Build log updated
echo -----------------