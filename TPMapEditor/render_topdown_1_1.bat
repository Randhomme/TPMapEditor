@echo off

set BLENDER_DIR=Your/Blender/Folder
set INPUT_DIR=Your/Input/Folder
set OUTPUT_DIR=Your/Output/Folder

for %%f in ("%INPUT_DIR%\*.obj") do (
    %BLENDER_DIR%\blender -b -P render_topdown.py -- "%%f" "%OUTPUT_DIR%\%%~nf.png" 1
)

pause
