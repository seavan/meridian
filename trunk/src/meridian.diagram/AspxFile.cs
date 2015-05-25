namespace meridian.diagram
{
    public class AspxFile : BasicFile
    {
        public string ProtoName { get; set; }
        public string Namespace { get; set;  }

        public string AdminControllerName
        {
            get { return "admin_" + ProtoName; }
        }
    }

    public class AspxGridFile : AspxFile
    {

        public override void WriteCode(CSharpWriter _writer)
        {
            base.WriteCode(_writer);
            _writer.Write(
                @"
<%@ Control Language=""C#"" Inherits=""System.Web.Mvc.ViewUserControl<IEnumerable<{0}.proto.{1}>>"" %>
<div style=""padding-top: 30px; margin-top: 10px;  border-top: 1px dotted silver;"" >
<%

    Html.Telerik().Grid<{0}.proto.{1}>()
        .Name(""t_{0}List"")
        .DataBinding(dataBinding => dataBinding.Ajax()
        .Select(""_Select"", ""{2}"") 
        .Delete(""_Delete"", ""{2}"")
        .Update(""_Update"", ""{2}"")
        .Insert(""_Insert"", ""{2}"") 
        )
        .DataKeys(keys => keys.Add(""id""))
        .Columns(
            c =>
                {{
                    c.Bound(col => col.id).Width(80).Id();
                    
 

                    c.Bound(col => col.id).Edit(""{2}"").Width(100);
                    c.Command(cmd =>
                                  {{
                                      cmd.Delete();
                                  }}
                        ).Width(100);
                }}
        )

        .Resizable( rs => rs.Columns(true))
        .Sortable( st => st.OrderBy( so => so.Add(e => e.id).Descending()).Enabled(true))
        .Filterable()
        .Scrollable( scr => scr.Height(453) )
        .Pageable(
            pager => pager.PageSize(10)
        )
        .ClientEvents(
        s =>
            s.OnDataBound(""updateGrids"")
            .OnEdit(""gridEdit"").OnRowDataBound(""bindRow"")
            )
        .Render();
%>
</div>


", Namespace, ProtoName, AdminControllerName
                );
        }

    }

    public class AspxIndexFile : AspxFile
    {
        public override void WriteCode(CSharpWriter _writer)
        {
            base.WriteCode(_writer);
            _writer.Write(
                @"
<%@ Page Title="""" Language=""C#"" MasterPageFile=""~/Views/Master/Admin.Master"" Inherits=""System.Web.Mvc.ViewPage<dynamic>"" UICulture=""ru-RU"" %>

<asp:Content ID=""Content2"" ContentPlaceHolderID=""MainContent"" runat=""server"">
            <div style=""text-align: right; float: right"">
            <a class=""_link"" href=""/{0}/Single"">Создать</a></div>
    <%=
        Html.Partial(""{1}_grid"", ViewData) %>

</asp:Content>
", AdminControllerName, ProtoName);
        }
        
    }

    public class AspxSingleFile : AspxFile
    {
        public override void WriteCode(CSharpWriter _writer)
        {
            base.WriteCode(_writer);
            _writer.Write(
                @"
<%@ Page Title="""" Language=""C#"" MasterPageFile=""~/Views/Master/Admin.Master"" Inherits=""System.Web.Mvc.ViewPage<{0}.proto.{1}>""
    ValidateRequest=""false"" %>

<asp:Content ID=""Content2"" ContentPlaceHolderID=""MainContent"" runat=""server"">
    <% using (Html.BeginForm())
       {{
    %>
    <div class=""_controlRow"" style=""margin-bottom: 12px"">
        <input type=""submit"" value=""Сохранить"" />
        <div style=""text-align: right; float: left"">
            <a class=""_link"" href=""/{2}/Index"">К списку</a></div>


    </div>
    <%= Html.EditorForModel()%>
    <div class=""_controlRow"">
        <input type=""submit"" value=""Сохранить"" />
        <div style=""text-align: right; float: left"">
            <a class=""_link"" href=""/{2}/Index"">К списку</a></div>
    </div>
    <%
   }} %>
</asp:Content>

", Namespace, ProtoName, AdminControllerName
                );
        }
    }
}