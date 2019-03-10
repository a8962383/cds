#!/bin/bash

# this script builds our software

pushd mopp
make clean
make
popd


# For Dotnet Core application, I didn't use the makefile.
# Apps are directly built from the build script and it's working fine.
pushd mopp/t0-harmonic-progression-sum/
dotnet clean -o . -r ubuntu.16.04-x64
dotnet build -o . -r ubuntu.16.04-x64
popd

pushd mopp/t1-transitive-closure/
dotnet clean -o . -r ubuntu.16.04-x64
dotnet build -o . -r ubuntu.16.04-x64
popd

pushd mopp/t3-mandelbrot-set/
dotnet clean -o . -r ubuntu.16.04-x64
dotnet build -o . -r ubuntu.16.04-x64
popd

pushd mopp/t4-k-means-clustering/
dotnet clean -o . -r ubuntu.16.04-x64
dotnet build -o . -r ubuntu.16.04-x64
popd

pushd mopp/t5-shortest-superstring/
dotnet clean -o . -r ubuntu.16.04-x64
dotnet build -o . -r ubuntu.16.04-x64
popd