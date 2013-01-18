REM ---
@ECHO OFF

ECHO DATABASE
P:\sqlbulkcopy\AppHarbor.SqlServerBulkCopy.exe --srcserver=926c7be5-4517-4cf6-94ff-a0b000e723b1.sqlserver.sequelizer.com --srcusername=hiamzfceeoitrveq --srcpassword=LELBhM3GDwwtrugH4Ys53DigSrQniWo5k78BGfCNAA7FXT4WAUZYEpThvESX5NxP --srcdatabasename=db926c7be545174cf694ffa0b000e723b1 --dstserver=.\SQLEXPRESS --dstdatabasename=MS.Katusha.Domain.DbContext --cleardstdatabase --checkidentityexists

ECHO RAVENDB

P:\NOSQL\ravendb\Smuggler\Raven.Smuggler out https://ec2-eu3.cloudbird.net/databases/7e377b2d-f438-4dfc-8254-d9b666472ad8.mskatushaeu P:\NOSQL\mskatusha.zip --key=62b36985-e77c-4b75-9c8a-424f1ee94156 
P:\NOSQL\ravendb\Smuggler\Raven.Smuggler in http://localhost:8080 P:\NOSQL\mskatusha.zip

REM curl -u mertiko:690514 -H "Content-type: application/json" -d filename https://mskatushaeu.apphb.com/Api/SetProfile

REM curl -u mertiko:690514 http://www.mskatusha.com/Api/GetConversations/{GUID}
REM curl -u mertiko:690514 http://www.mskatusha.com/Api/GetConversation/{FROMGUID}/{TOGUID}
REM curl -u mertiko:690514 http://www.mskatusha.com/Api/DeleteConversations/{GUID}
REM curl -u mertiko:690514 http://www.mskatusha.com/Api/DeleteConversation/{FROMGUID}/{TOGUID}
