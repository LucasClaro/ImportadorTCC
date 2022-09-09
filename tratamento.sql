select Registro.id, setor, bssid, rssi from Registro inner join Conexao on Registro.id = Conexao.idRegistro 
where setor in ('A11', 'A12', 'A21', 'A22','B11','B21','C11','C12','C21','C22','D11','D21','E11','E21','F21', 'NASA')

select setor from Registro
where setor in ('A11', 'A12', 'A21', 'A22','B11','B21','C11','C12','C21','C22','D11','D21','E11','E21','F21', 'NASA')

SELECT DISTINCT CONCAT('@ATTRIBUTE ', [bssid], ' NUMERIC') as a FROM Conexao
ORDER BY a

------------------------

DECLARE @Columns as VARCHAR(MAX)

SELECT @Columns = COALESCE(@Columns + ', ','') + QUOTENAME([bssid])
FROM
    (SELECT DISTINCT [bssid] FROM Conexao) AS B
ORDER BY B.bssid

DECLARE @SQL as VARCHAR(MAX)
SET @SQL = 'SELECT setor, ' + @Columns + ' 
FROM
(
 select Registro.id, setor, bssid, rssi from Registro inner join Conexao on Registro.id = Conexao.idRegistro 
	where setor in (''A11'', ''A12'', ''A21'', ''A22'',''B11'',''B21'',''C11'',''C12'',''C21'',''C22'',''D11'',''D21'',''E11'',''E21'',''F21'', ''NASA'')
) as PivotData
PIVOT
(
   Max(rssi)
   FOR [bssid] IN (' + @Columns + ')
) AS PivotResult'


EXEC(@SQL);
