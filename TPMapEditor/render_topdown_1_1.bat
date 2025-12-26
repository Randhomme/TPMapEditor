@echo off

set INPUT_DIR=Your/Input/Folder
set OUTPUT_DIR=Your/Output/Folder

for %%f in ("%INPUT_DIR%\*.obj") do (
    blender -b -P render_topdown.py -- "%%f" "%OUTPUT_DIR%\%%~nf.png" 1
)

pause
