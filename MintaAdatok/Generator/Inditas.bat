@echo off
cd /d "%~dp0.."
start "" http://localhost:8080/Generator/index.html
python -m http.server 8080
