@echo off
cd /d "%~dp0.."
start "" http://localhost:8081/Generator/index.html
python "%~dp0szerver.py"
