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
mkdir $DEPLOYDIR/C#-files/scripts
mkdir $DEPLOYDIR/C#-files/snippets
mkdir $DEPLOYDIR/C#-files/preserved-data
mkdir $DEPLOYDIR/C#-files/preserved-data/new

xbuild CSharp.sln /target:rebuild /property:OutDir=$BINARYDIR/ /p:Configuration=Release;

cp $ROOT/resources/C#.oilnk $DEPLOYDIR/C#.oilnk
cp $ROOT/resources/language.oicfgoptions $DEPLOYDIR/C#-files/language.oicfgoptions
cp $ROOT/resources/package.json $DEPLOYDIR/C#-files/package.json
cp $BINARYDIR/C#.exe $DEPLOYDIR/C#-files/C#.exe
cp $BINARYDIR/ICSharpCode.NRefactory.CSharp.dll $DEPLOYDIR/C#-files/ICSharpCode.NRefactory.CSharp.dll
cp $BINARYDIR/ICSharpCode.NRefactory.dll $DEPLOYDIR/C#-files/ICSharpCode.NRefactory.dll
cp $BINARYDIR/Mono.Cecil.dll $DEPLOYDIR/C#-files/Mono.Cecil.dll
cp -r $ROOT/resources/templates/new/* $DEPLOYDIR/C#-files/preserved-data/new
cp -r $ROOT/resources/templates/scripts/* $DEPLOYDIR/C#-files/scripts
cp -r $ROOT/resources/templates/snippets/* $DEPLOYDIR/C#-files/snippets

# Building packages
echo "Building packages.."
oi package build $DEPLOYDIR/C\# $DEPLOYDIR
