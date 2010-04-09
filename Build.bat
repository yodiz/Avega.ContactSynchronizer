@set Framework4Dir=c:\Windows\Microsoft.NET\Framework\v4.0.30128\
@set msBuild=%Framework4Dir%MsBuild.exe
@set outputDir=%cd%\bin\

@echo ===============================================================================
@echo Bulding Avega Contact Synchronizer...
@echo Output directory is %outputDir%
@echo ===============================================================================
@echo ===============================================================================
@echo -
@echo -


rmdir \S \Q %outputDir%
@%msBuild% Avega.ContactSynchronizer.sln /t:Build /p:Configuration=ReleaseWinApp /property:OutDir=%outputDir%3.5\ /verbosity:q /p:TargetFrameworkVersion=v3.5


