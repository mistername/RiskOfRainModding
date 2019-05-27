pushd %~dp0
set /p location=<mono-cil-strip_location.txt
for /r %%v in (*.dll) do (%location% "%%v")
pause