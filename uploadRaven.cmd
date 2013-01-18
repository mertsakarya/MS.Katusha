REM ---
@ECHO OFF
REM Url=https://ec2-eu3.cloudbird.net/databases/7e377b2d-f438-4dfc-8254-d9b666472ad8.mskatushaeu;ApiKey=62b36985-e77c-4b75-9c8a-424f1ee94156


ECHO RAVENDB
P:\NOSQL\ravendb\Smuggler\Raven.Smuggler out http://localhost:8080 P:\NOSQL\mskatusha_out.zip
P:\NOSQL\ravendb\Smuggler\Raven.Smuggler in https://ec2-eu3.cloudbird.net/Databases/7e377b2d-f438-4dfc-8254-d9b666472ad8.mskatushaeu P:\NOSQL\mskatusha_out.zip --key=62b36985-e77c-4b75-9c8a-424f1ee94156 
