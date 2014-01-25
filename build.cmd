@echo off
.nuget\nuget install packages.config -o packages
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe Pegasus.sln /p:Configuration=Release /t:Clean,Rebuild
packages\NUnit.Runners.2.6.3\tools\nunit-console Pegasus.Tests\bin\Release\Pegasus.Tests.dll
