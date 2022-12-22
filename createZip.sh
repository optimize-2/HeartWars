#!/bin/bash
rm HeartWars.zip HeartWars.dll
dotnet build HeartWars.csproj
zip HeartWars.zip -r LICENSE.txt everest.yaml Ahorn Dialog Graphics Maps HeartWars.dll debug.bin