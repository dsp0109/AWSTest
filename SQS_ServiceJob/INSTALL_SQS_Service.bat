sc create SQS_Service binPath= "%~dp0SQS_ServiceJob.exe"
sc failure SQS_Service actions= restart/600/restart/600/""/600 reset= 86400
sc start SQS_Service
sc config SQS_Service start=auto