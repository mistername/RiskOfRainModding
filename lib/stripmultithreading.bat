set startTime=%time%
pushd %~dp0
set /p location=<mono-cil-strip_location.txt
for /r %%v in (*.dll) do start %location% "%%v"
echo Start Time: %startTime%
echo Finish Time: %time%
pause