﻿DROP PROCEDURE IF EXISTS [dbo].[SP_GET_PTFS_WMS_INTEGRATION_DATA]
GO

CREATE PROCEDURE [dbo].[SP_GET_PTFS_WMS_INTEGRATION_DATA]
	@Record AS NVARCHAR(MAX) = 'READY'
AS
BEGIN
	SELECT ID.REFERENCE_ID, ID.MSG_TYPE, ID.DIRECTION, ID.RECORD_STATUS, ID.CALL_TYPE, ID.APPLICATION_NAME,
		CNF.USER2VALUE AS 'STORED_PROCEDURE_NAME'
	FROM [SCALE_IHB].[dbo].[PTFS_WMS_INTEGRATION_DATA] AS ID WITH(NOLOCK) 
	LEFT JOIN GENERIC_CONFIG_DETAILS AS CNF WITH(NOLOCK)
		ON CNF.USER1VALUE = ID.MSG_TYPE AND CNF.RECORD_TYPE = 'PTFSWMSFabricIntegration' AND CNF.ACTIVE = 'Y'
	WHERE ID.RECORD_STATUS = @Record
	ORDER BY ID.MSG_TYPE
END
GO

