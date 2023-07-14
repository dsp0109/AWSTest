sc stop SQS_Service
timeout /t 5 /nobreak > NUL
sc delete SQS_Service