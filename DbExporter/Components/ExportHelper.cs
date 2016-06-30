using System.Data;
using System.Data.SqlClient;
using System.Web;
using DotNetNuke.Data;

namespace ICG.Modules.DbExporter.Components
{
    public class ExportHelper
    {
        /// <summary>
        /// Gets the connection string from the web.config
        /// </summary>
        /// <returns></returns>
        private static string ConnectionString()
        {
            return DataProvider.Instance().ConnectionString;
        }

        /// <summary>
        /// Return dataset
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataSet ReturnDataSet(string sql)
        {
            var ds = new DataSet();

            using (var objConnection = new SqlConnection(ConnectionString()))
            {
                using (var objCommand = new SqlCommand(sql, objConnection))
                {
                    objCommand.CommandType = CommandType.Text;
                    objConnection.Open();
                    var da = new SqlDataAdapter(objCommand);
                    da.Fill(ds);
                    da.Dispose();
                    return ds;
                }
            }
        }

        /// <summary>
        /// This method will take content and force it down to the client for viewing
        /// </summary>
        /// <param name="content"></param>
        /// <param name="filename"></param>
        /// <param name="contentType"></param>
        public static void ForceContentToUser(string content, string filename, string contentType)
        {
            var oContext = HttpContext.Current;

            //Prep for file write
            oContext.Response.ClearHeaders();
            oContext.Response.ClearContent();
            oContext.Response.AppendHeader("content-disposition", "attachment; filename=\"" + filename + "\"");
            oContext.Response.ContentType = contentType;

            //Write content, flush and end
            oContext.Response.Write(content);
            oContext.Response.Flush();
            oContext.Response.End();
        }
    }
}