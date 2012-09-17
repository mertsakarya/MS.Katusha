@echo off
git add -A
git commit -am %1
git push origin master:refs/heads/test
echo Bitti
pause 10