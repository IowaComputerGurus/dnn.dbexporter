using System;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Skins.Controls;
using ICG.Modules.DbExporter.Components;

namespace ICG.Modules.DbExporter
{
    public partial class ViewDbExporter : PortalModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (!UserInfo.IsSuperUser)
                    {
                        pnlViewer.Visible = false;
                        Skin.AddModuleMessage(this, Localization.GetString("NotHostUser", LocalResourceFile),
                                              ModuleMessage.ModuleMessageType.YellowWarning);
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }


        /// <summary>
        /// show all tables of the current database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void linkShowTables_Click(object sender, EventArgs e)
        {
            try
            {
                var ds =
                    ExportHelper.ReturnDataSet(
                        "SELECT table_name AS Name FROM INFORMATION_SCHEMA.Tables WHERE (TABLE_TYPE = 'BASE TABLE') Order By table_name");
                if (ds != null)
                {
                    GridDS.DataSource = ds;
                    GridDS.DataBind();
                }
            }
            catch (Exception ex)
            {
                Skin.AddModuleMessage(this, "Error In Getting Tables: " + ex.Message,
                                      ModuleMessage.ModuleMessageType.RedError);
            }
        }

        /// <summary>
        /// Users request to query the db
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExecuteQuery_Click(object sender, EventArgs e)
        {
            //Secondary security fix
            if (IsEditable && txtQuery.Text.ToLower().StartsWith("select"))
            {
                try
                {
                    var ds = ExportHelper.ReturnDataSet(txtQuery.Text);
                    if (ds != null)
                    {
                        GridDS.DataSource = ds;
                        GridDS.DataBind();

                        //Check if the gridds has records on it, we show the exporter buttons
                        liExportCsv.Visible = GridDS.Rows.Count > 0;
                        liExportXml.Visible = GridDS.Rows.Count > 0;
                    }
                }
                catch (Exception ex)
                {
                    Skin.AddModuleMessage(this, "Query Error: " + ex.Message, ModuleMessage.ModuleMessageType.RedError);
                    GridDS.DataSource = null;
                    GridDS.DataBind();
                }
            }
            else
            {
                Skin.AddModuleMessage(this, "Your query MUST start with the word SELECT",
                                      ModuleMessage.ModuleMessageType.RedError);
            }
        }

        /// <summary>
        /// Reset the module to the default layout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void linkClear_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(TabId));
        }


        /// <summary>
        /// export to csv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportToCSV_Click(object sender, EventArgs e)
        {
            if (IsEditable)
            {
                try
                {
                    var ds = ExportHelper.ReturnDataSet(txtQuery.Text);

                    var oCsvBuilder = new StringBuilder();

                    //get data table
                    var dt = ds.Tables[0];

                    //create the header
                    var fieldsHeader = new String[dt.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        fieldsHeader[i] = dt.Columns[i].ColumnName;
                        fieldsHeader[i] = GetFieldValue(fieldsHeader[i]);
                    }
                    oCsvBuilder.AppendLine(string.Join(",", fieldsHeader));

                    //write the content
                    var fields = new String[dt.Columns.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            object obj = dt.Rows[i][j];
                            fields[j] = GetFieldValue(obj);
                        }
                        oCsvBuilder.AppendLine(string.Join(",", fields));
                    }

                    ExportHelper.ForceContentToUser(oCsvBuilder.ToString(), "export.csv", "text/csv");
                }
                catch (Exception ex)
                {
                    Skin.AddModuleMessage(this, "Export Error:" + ex.Message,
                                          ModuleMessage.ModuleMessageType.RedError);
                }
            }
        }

        /// <summary>
        /// get the field value
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string GetFieldValue(object obj)
        {
            if (obj == null || obj == Convert.DBNull)
                return "";
            
            if (obj.ToString().IndexOf(",", StringComparison.Ordinal) == -1)
                return obj.ToString();
            
            return "\"" + obj + "\"";
        }

        /// <summary>
        /// export to xml
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExportToXML_Click(object sender, EventArgs e)
        {
            try
            {
                var ds = ExportHelper.ReturnDataSet(txtQuery.Text);

                using (var stringWriter = new StringWriter())
                using (var xmlWriter = new XmlTextWriter(stringWriter))
                {
                    ds.WriteXml(xmlWriter);

                    //download the file
                    ExportHelper.ForceContentToUser(stringWriter.ToString(), "export.xml", "text/xml");
                }
            }
            catch (Exception ex)
            {
                Skin.AddModuleMessage(this, "Export Error: " + ex.Message, ModuleMessage.ModuleMessageType.RedError);
            }
        }
    }
}