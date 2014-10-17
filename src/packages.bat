@echo off
rem This batch will prepare ZIP packages with sources and binaries
rem Verify properties in Properties.xml and override them in UserProperties.xml
rem if you need
C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\msbuild.exe UnityRegistrationValidator.msbuild /Target:Packages /l:FileLogger,Microsoft.Build.Engine;logfile=Packages.log