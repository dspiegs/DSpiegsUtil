using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSpiegsUtil.Database
{
    public static class DBHelper
    {
        public static List<DBTable> GetDBTables(string connectionString)
        {
            const string sql = @"
USE iodc_central; 

GO: 

SELECT o.NAME               AS [Table_Name], 
       Cast(CASE 
              WHEN EXISTS (SELECT TOP 1 1 
                           FROM   sys.all_objects 
                           WHERE  parent_object_id = o.object_id 
                                  AND type = 'tr') THEN 1 
              ELSE 0 
            END AS BIT)     AS [Has_Trigger], 
       c.NAME               AS [Column_Name], 
       t.NAME               AS [Type], 
       c.is_nullable        AS [Is_Nullable], 
       c.is_identity        AS [Is_Identity], 
       c.max_length         AS [Max_Length], 
       inf.ordinal_position AS [Index], 
       Isnull(t2.NAME, '')  AS [FK_Table_Name], 
       Isnull(c2.NAME, '')  AS [FK_Column_Name], 
       Cast(CASE 
              WHEN Object_definition(c.default_object_id) IS NOT NULL THEN 1 
              ELSE 0 
            END AS BIT)     AS [Has_Default_Value],
       Cast(CASE 
              WHEN EXISTS (SELECT TOP 1 1 
                           FROM   sys.indexes i 
                                  INNER JOIN sys.index_columns ic 
                                          ON i.index_id = ic.index_id 
                           WHERE  i.is_primary_key = 1 
                                  AND ic.object_id = o.object_id 
                                  AND ic.column_id = c.column_id) THEN 1 
              ELSE 0 
            END AS BIT)     AS [Is_Primary_Key]
FROM   sys.objects o 
       JOIN sys.columns c 
         ON o.object_id = c.object_id 
       JOIN sys.types t 
         ON c.system_type_id = t.system_type_id 
       JOIN information_schema.columns inf 
         ON o.NAME = inf.table_name 
            AND c.NAME = inf.column_name 
       LEFT OUTER JOIN sys.foreign_key_columns fkc 
                    ON c.column_id = fkc.parent_column_id 
                       AND o.object_id = fkc.parent_object_id 
       LEFT OUTER JOIN sys.columns c2 
                    ON fkc.referenced_column_id = c2.column_id 
                       AND fkc.referenced_object_id = c2.object_id 
       LEFT OUTER JOIN sys.tables t2 
                    ON c2.object_id = t2.object_id 
WHERE  o.type = 'u' 
       AND t.NAME <> 'sysname' 
ORDER  BY o.NAME, 
          inf.ordinal_position";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(sql, connection))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlCommand))
                    {
                        using (DataTable dbTablesWithColumns = new DataTable())
                        {
                            da.Fill(dbTablesWithColumns);
                            return dbTablesWithColumns.Rows.Cast<DataRow>()
                                .GroupBy(row => (string)row[0])
                                .Select(
                                    group =>
                                        new DBTable(@group.Key,
                                            @group.Select(
                                                row =>
                                                    new DBColumn((string)row[2], (string)row[3], (bool)row[4], (bool)row[5], (short)row[6],
                                                        (int)row[7], (string)row[8], (string)row[9], (bool)row[10], (bool)row[11])),
                                            (bool)@group.First()[1]))
                                .ToList();
                        }
                    }
                }
            }
        }
    }
}
