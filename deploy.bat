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
mkdir %DEPLOYDIR%\C#-files\scripts
mkdir %DEPLOYDIR%\C#-files\snippets
mkdir %DEPLOYDIR%\C#-files\preserved-data
mkdir %DEPLOYDIR%\C#-files\preserved-data\new

%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe %ROOT%CSharp.sln  /property:OutDir=%BINARYDIR%\;Configuration=Release /target:rebuild

copy %ROOT%\resources\C#.oilnk %DEPLOYDIR%\C#.oilnk
copy %ROOT%\resources\language.oicfgoptions %DEPLOYDIR%\C#-files\language.oicfgoptions
copy %ROOT%\resources\package.json %DEPLOYDIR%\C#-files\package.json
copy %BINARYDIR%\C#.exe %DEPLOYDIR%\C#-files\C#.exe
copy %BINARYDIR%\ICSharpCode.NRefactory.CSharp.dll %DEPLOYDIR%\C#-files\ICSharpCode.NRefactory.CSharp.dll
copy %BINARYDIR%\ICSharpCode.NRefactory.dll %DEPLOYDIR%\C#-files\ICSharpCode.NRefactory.dll
copy %BINARYDIR%\Mono.Cecil.dll %DEPLOYDIR%\C#-files\Mono.Cecil.dll
xcopy /S /I /E %ROOT%\resources\templates\new %DEPLOYDIR%\C#-files\preserved-data\new
xcopy /S /I /E %ROOT%\resources\templates\scripts %DEPLOYDIR%\C#-files\scripts
xcopy /S /I /E %ROOT%\resources\templates\snippets %DEPLOYDIR%\C#-files\snippets

REM Building packages
ECHO Building packages

oi package build "ReleaseBinaries\C#" %DEPLOYDIR%
