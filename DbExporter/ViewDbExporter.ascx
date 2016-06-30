<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewDbExporter.ascx.cs" Inherits="ICG.Modules.DbExporter.ViewDbExporter" %>
<%@ Register TagName="Label" TagPrefix="dnn" Src="~/Controls/LabelControl.ascx" %>


<asp:Panel ID="pnlViewer" runat="server">
    <div class="dnnForm">
        <fieldset>
            <ul class="dnnActions">
                <li><asp:LinkButton ID="linkShowTables" runat="server" CausesValidation="false" OnClick="linkShowTables_Click" resourcekey="linkShowTables" CssClass="dnnSecondaryAction"/></li>
                <li><asp:LinkButton ID="linkClear" runat="server" CausesValidation="false" onClick="linkClear_Click" resourcekey="linkClear" CssClass="dnnSecondaryAction"/></li>
            </ul>
            <div class="dnnFormItem dnnClear">
                <dnn:label id="lblQuery" runat="server" controlname="txtQuery" suffix=":" />
                <asp:TextBox ID="txtQuery" runat="server" Rows="10" TextMode="MultiLine" Width="100%" CssClass="dnnFormRequired" />
                <asp:RequiredFieldValidator ID="reqQuery" runat="server" Display="dynamic" ControlToValidate="txtQuery" SetFocusOnError="true" CssClass="dnnFormError dnnFormMessage" resourcekey="reqQuery" />
            </div>
            <ul class="dnnActions">
                <li><asp:LinkButton ID="btnExecuteQueryNow" runat="server" OnClick="btnExecuteQuery_Click" resourcekey="btnExecuteQuery" CssClass="dnnPrimaryAction"/></li>
                <li id="liExportCsv" runat="server" visible="False"><asp:LinkButton ID="btnExportToCSV" runat="server" OnClick="btnExportToCSV_Click" resourcekey="btnExportToCSV" CssClass="dnnSecondaryAction"/></li>
                <li id="liExportXml" runat="server" Visible="False"><asp:LinkButton ID="btnExportToXML" runat="server" OnClick="btnExportToXML_Click" resourcekey="btnExportToXML" CssClass="dnnSecondaryAction"/></li>
            </ul>
        </fieldset>
    </div>


    <asp:Panel ID="pnlGrid" runat="server">
        <div class="resultGridHolder">
            <asp:GridView runat="server" ID="GridDS" AutoGenerateColumns ="true" CssClass="dnnGrid">
                <HeaderStyle CssClass="dnnGridHeader" />
                <RowStyle CssClass="dnnGridItem" />
                <AlternatingRowStyle CssClass="dnnGridAltItem" />
            </asp:GridView>
        </div>

    </asp:Panel>
</asp:Panel>

<p><a href="http://www.iowacomputergurus.com" target="_blank">This free module provided by IowaComputerGurus Inc.</a></p>