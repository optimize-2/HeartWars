del HeartWars.dll
dotnet build HeartWars.csproj
copy bin\\HeartWars.dll %CelestePrefix%\\Mods\\HeartWars\\bin /Y
copy bin\\HeartWars.dll "E:\amusement\Celeste.v1.4.0.0\Celeste.v1.4.0.0_test\Mods\HeartWars\bin" /Y