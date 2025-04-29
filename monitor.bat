@echo off

set JAVA_HOME=C:\Program Files (x86)\Java\jre-1.8
set PATH=%JAVA_HOME%\bin;%PATH%
set filePath=%ProgramFiles(x86)%\Android\android-sdk\tools\lib\monitor-x86\monitor.exe

if exist "%filePath%" goto :startProgram
echo O arquivo monitor.exe não foi encontrado em %filePath%.
echo Pressione qualquer tecla para fechar...
pause > nul
goto :eof

:startProgram
echo Iniciando o Monitor do Android SDK...
start "" "%filePath%"

:eof
exit
