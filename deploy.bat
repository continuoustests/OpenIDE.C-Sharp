@echo off

SET ROOT=%~d0%~p0%
SET BINARYDIR="%ROOT%build_output"
SET DEPLOYDIR="%ROOT%ReleaseBinaries"
SET LIB="%ROOT%lib"

IF EXIST %BINARYDIR% (
  rmdir /Q /S %BINARYDIR%
)
mkdir %BINARYDIR%

IF EXIST %DEPLOYDIR% (
  rmdir /Q /S %DEPLOYDIR%
)
mkdir %DEPLOYDIR%

mkdir %DEPLOYDIR%\C#-files
mkdir %DEPLOYDIR%\C#-files\bin
mkdir %DEPLOYDIR%\C#-files\bin\AutoTest.Net
mkdir %DEPLOYDIR%\C#-files\bin\ContinuousTests

%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe %ROOT%CSharp.sln  /property:OutDir=%BINARYDIR%\;Configuration=Release /target:rebuild

copy %ROOT%\resources\C#.oilnk %DEPLOYDIR%\C#.oilnk
copy %ROOT%\resources\language.oicfgoptions %DEPLOYDIR%\C#-files\language.oicfgoptions
copy %ROOT%\resources\package.json.CT %DEPLOYDIR%\C#-files\package.json
copy %BINARYDIR%\C#.exe %DEPLOYDIR%\C#-files\C#.exe
copy %BINARYDIR%\ICSharpCode.NRefactory.CSharp.dll %DEPLOYDIR%\C#-files\ICSharpCode.NRefactory.CSharp.dll
copy %BINARYDIR%\ICSharpCode.NRefactory.dll %DEPLOYDIR%\C#-files\ICSharpCode.NRefactory.dll
copy %BINARYDIR%\Mono.Cecil.dll %DEPLOYDIR%\C#-files\Mono.Cecil.dll
xcopy /S /I /E %ROOT%\resources\templates %DEPLOYDIR%\C#-files
copy %ROOT%\resources\initialize.bat %DEPLOYDIR%\C#-files
copy %ROOT%\resources\initialize.sh %DEPLOYDIR%\C#-files
xcopy /S /I /E %LIB%\AutoTest.Net %DEPLOYDIR%\C#-files\bin\AutoTest.Net
xcopy /S /I /E %LIB%\ContinuousTests %DEPLOYDIR%\C#-files\bin\ContinuousTests

REM Building packages
ECHO Building packages

%DEPLOYDIR%\oi package build "Packages\C#" %PACKAGEDIR%/oipkg
rmdir /Q /S %DEPLOYDIR%\C#-files\bin
del %DEPLOYDIR%\C#-files\initialize.*
del %DEPLOYDIR%\C#-files\package.json
copy %ROOT%\resources\package.json %DEPLOYDIR%\C#-files\package.json
oi package build "ReleaseBinaries\C#" %DEPLOYDIR%