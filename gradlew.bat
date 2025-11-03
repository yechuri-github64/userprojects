@echo off
REM Placeholder gradle wrapper batch file
if "%JAVA_HOME%"=="" (
  echo JAVA_HOME is not set
)
set BASEDIR=%~dp0
"%BASEDIR%gradle\wrapper\gradle-wrapper.jar" %*
