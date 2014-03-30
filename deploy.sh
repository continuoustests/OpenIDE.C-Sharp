#!/bin/bash

ROOT=$(cd $(dirname "$0"); pwd)
BINARYDIR=$(cd $(dirname "$0"); pwd)/build_output
DEPLOYDIR=$(cd $(dirname "$0"); pwd)/ReleaseBinaries
LIB=$(cd $(dirname "$0"); pwd)/lib

if [ -d $BINARYDIR ]; then
{
    rm -r $BINARYDIR/
}
fi
if [ -d $DEPLOYDIR ]; then
{
    rm -r $DEPLOYDIR/
}
fi

mkdir $BINARYDIR
mkdir $DEPLOYDIR

mkdir $DEPLOYDIR/C#-files
mkdir $DEPLOYDIR/C#-files/bin
mkdir $DEPLOYDIR/C#-files/bin/AutoTest.Net
mkdir $DEPLOYDIR/C#-files/bin/ContinuousTests

chmod +x $LIB/ContinuousTests/AutoTest.*.exe
chmod +x $LIB/ContinuousTests/ContinuousTests.exe

xbuild CSharp.sln /target:rebuild /property:OutDir=$BINARYDIR/ /p:Configuration=Release;

cp $ROOT/resources/C#.oilnk $DEPLOYDIR/C#.oilnk
cp $ROOT/resources/language.oicfgoptions $DEPLOYDIR/C#-files/language.oicfgoptions
cp $ROOT/resources/package.json.CT $DEPLOYDIR/C#-files/package.json
cp $BINARYDIR/C#.exe $DEPLOYDIR/C#-files/C#.exe
cp $BINARYDIR/ICSharpCode.NRefactory.CSharp.dll $DEPLOYDIR/C#-files/ICSharpCode.NRefactory.CSharp.dll
cp $BINARYDIR/ICSharpCode.NRefactory.dll $DEPLOYDIR/C#-files/ICSharpCode.NRefactory.dll
cp $BINARYDIR/Mono.Cecil.dll $DEPLOYDIR/C#-files/Mono.Cecil.dll
cp -r $ROOT/resources/templates/* $DEPLOYDIR/C#-files
cp $ROOT/resources/initialize.sh $DEPLOYDIR/C#-files
cp $ROOT/resources/initialize.bat $DEPLOYDIR/C#-files
cp -r $LIB/AutoTest.Net/* $DEPLOYDIR/C#-files/bin/AutoTest.Net
cp -r $LIB/ContinuousTests/* $DEPLOYDIR/C#-files/bin/ContinuousTests

# Building packages
echo "Building packages.."
oi package build $DEPLOYDIR/C\# $DEPLOYDIR
rm -r $DEPLOYDIR/C\#-files/bin
rm $DEPLOYDIR/C\#-files/initialize.*
rm $DEPLOYDIR/C\#-files/package.json
cp $ROOT/resources/package.json $DEPLOYDIR/C#-files/package.json
oi package build $DEPLOYDIR/C\# $DEPLOYDIR