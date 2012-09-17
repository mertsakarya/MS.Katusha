@echo off
git add -A
git commit -am %1
git push
echo Bitti
pause 10