<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Co_Define.aspx.cs" Inherits="Co_Po_Mapping_Master_Co_Define" %>

<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <title>CO-PO Mapping - Define Course Outcomes</title>
    <link href="https://fonts.googleapis.com/css2?family=Public+Sans:wght@400;500;600;700&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />

    <style>
        :root {
            --primary-navy: #163d78;
            --primary-navy-hover: #102e5b;
            --primary-navy-light: #e8eef8;
            --primary-navy-border: #c8d8f0;
        }

        * {
            box-sizing: border-box;
            font-family: 'Public Sans', sans-serif;
        }

        body {
            background-color: #f4f6fa;
            margin: 0;
            padding: 24px;
            color: #334155;
        }

        .main-wrapper {
            max-width: 1200px;
            margin: 0 auto;
            display: flex;
            flex-direction: column;
            gap: 20px;
        }

        .page-header {
            display: flex;
            align-items: center;
            gap: 16px;
        }

        .header-icon {
            width: 52px;
            height: 52px;
            background-color: var(--primary-navy);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            color: #ffffff;
            font-size: 24px;
        }

        .header-title h2 {
            margin: 0 0 4px 0;
            font-size: 22px;
            font-weight: 700;
            color: var(--primary-navy);
        }

        .header-title p {
            margin: 0;
            font-size: 13px;
            color: #64748b;
        }

        .custom-card {
            background: #ffffff;
            border: 1px solid #e2e8f0;
            border-radius: 10px;
            padding: 22px 24px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.02);
        }

        .card-header-title {
            display: flex;
            align-items: center;
            gap: 10px;
            font-size: 16px;
            font-weight: 700;
            color: var(--primary-navy);
            margin-bottom: 18px;
        }

        .selection-grid {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 16px;
        }

        .form-grid {
            display: grid;
            grid-template-columns: 200px 1fr auto auto;
            gap: 12px;
            align-items: end;
        }

        .form-group {
            display: flex;
            flex-direction: column;
            gap: 6px;
        }

        .form-group label {
            font-size: 13px;
            font-weight: 600;
            color: var(--primary-navy);
        }

        .form-group label span {
            color: #ef4444;
        }

        .form-control {
            height: 42px;
            width: 100%;
            padding: 8px 12px;
            border: 1px solid #cbd5e1;
            border-radius: 6px;
            font-size: 13px;
            color: #334155;
            background-color: #fff;
            outline: none;
            transition: all 0.2s;
        }

        .form-control:focus {
            border-color: var(--primary-navy);
            box-shadow: 0 0 0 3px rgba(22, 61, 120, 0.18);
        }

        .form-control:disabled {
            background-color: #f1f5f9;
            cursor: not-allowed;
        }

        .btn {
            height: 42px;
            padding: 0 20px;
            border: none;
            border-radius: 6px;
            font-size: 13px;
            font-weight: 600;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            transition: all 0.2s;
            text-decoration: none;
            white-space: nowrap;
        }

        .btn-primary {
            background-color: var(--primary-navy);
            color: #ffffff;
        }

        .btn-primary:hover {
            background-color: var(--primary-navy-hover);
        }

        .btn-secondary {
            background-color: #64748b;
            color: #ffffff;
        }

        .btn-secondary:hover {
            background-color: #475569;
        }

        .btn-success {
            background-color: #16a34a;
            color: #ffffff;
        }

        .btn-success:hover {
            background-color: #15803d;
        }

        .btn:disabled, .btn[disabled] {
            opacity: 0.55;
            cursor: not-allowed;
            pointer-events: none;
        }

        .table-container {
            overflow-x: auto;
            border-radius: 6px;
            border: 1px solid #e2e8f0;
        }

        .custom-grid {
            width: 100%;
            border-collapse: collapse;
            font-size: 13.5px;
        }

        .custom-grid th {
            background-color: var(--primary-navy-light);
            color: var(--primary-navy);
            font-weight: 600;
            text-align: left;
            padding: 12px 16px;
            border-bottom: 1px solid var(--primary-navy-border);
        }

        .custom-grid td {
            padding: 12px 16px;
            border-bottom: 1px solid #edf2f7;
            color: #334155;
            vertical-align: middle;
        }

        .custom-grid tr:last-child td {
            border-bottom: none;
        }

        .custom-grid tr:hover {
            background-color: #f8fafc;
        }

        .badge-co {
            display: inline-block;
            background-color: var(--primary-navy-light);
            color: var(--primary-navy);
            padding: 4px 12px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 600;
            border: 1px solid var(--primary-navy-border);
        }

        .action-cell {
            display: inline-flex;
            align-items: center;
            justify-content: flex-end;
            gap: 8px;
            white-space: nowrap;
        }

        .btn-action-edit {
            background-color: var(--primary-navy);
            color: #ffffff !important;
            padding: 6px 14px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 500;
            border: none;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 5px;
            text-decoration: none !important;
            line-height: 1.4;
        }

        .btn-action-delete {
            background-color: #ef4444;
            color: #ffffff !important;
            padding: 6px 14px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 500;
            border: none;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 5px;
            text-decoration: none !important;
            line-height: 1.4;
        }

        .status-box {
            padding: 12px 18px;
            border-radius: 8px;
            font-size: 13.5px;
            font-weight: 600;
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 16px;
        }

        .status-submitted {
            background-color: #fef9c3;
            border: 1px solid #fde047;
            color: #854d0e;
        }

        .status-approved {
            background-color: #dcfce7;
            border: 1px solid #86efac;
            color: #166534;
        }

        .status-rejected {
            background-color: #fee2e2;
            border: 1px solid #fca5a5;
            color: #991b1b;
        }

        .status-draft {
            background-color: #e0f2fe;
            border: 1px solid #bae6fd;
            color: #0369a1;
        }

        .bottom-bar {
            margin-top: 18px;
            display: flex;
            justify-content: flex-end;
            gap: 12px;
        }

        @media (max-width: 900px) {
            .selection-grid { grid-template-columns: repeat(2, 1fr); }
            .form-grid { grid-template-columns: 1fr; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <div class="main-wrapper">
            <asp:HiddenField ID="hdnEditCoId" runat="server" Value="0" />

            <div class="page-header">
                <div class="header-icon">
                    <i class="fa-solid fa-graduation-cap"></i>
                </div>
                <div class="header-title">
                    <h2>CO–PO Mapping</h2>
                    <p>Select academic details, define Course Outcomes and submit for Approval.</p>
                </div>
            </div>

            <div class="custom-card">
                <div class="card-header-title">
                    <i class="fa-solid fa-filter"></i> Course & Subject Selection
                </div>
                <div class="selection-grid">
                    <div class="form-group">
                        <label>Course <span>*</span></label>
                        <asp:DropDownList ID="ddlCourse" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCourse_SelectedIndexChanged" />
                    </div>

                    <div class="form-group">
                        <label>Semester <span>*</span></label>
                        <asp:DropDownList ID="Ddl_Semes" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="Ddl_Semes_SelectedIndexChanged" />
                    </div>

                    <div class="form-group">
                        <label>Specialization <span>*</span></label>
                        <asp:DropDownList ID="Ddl_Specl" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="Ddl_Specl_SelectedIndexChanged" />
                    </div>

                    <div class="form-group">
                        <label>Subject <span>*</span></label>
                        <asp:DropDownList ID="ddlSubject" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlSubject_SelectedIndexChanged" />
                    </div>
                </div>
            </div>

            <asp:Panel ID="pnlApprovalStatus" runat="server" Visible="false">
                <div id="divStatusBox" runat="server" class="status-box">
                    <div>
                        <i id="iconStatus" runat="server" class="fa-solid fa-circle-info"></i>
                        <asp:Label ID="lblStatusMessage" runat="server" />
                    </div>
                    <div>
                        <asp:Label ID="lblRemarks" runat="server" />
                    </div>
                </div>
            </asp:Panel>

            <div class="custom-card">
                <div class="card-header-title">
                    <i class="fa-solid fa-circle-plus"></i> <asp:Label ID="lblFormTitle" runat="server" Text="Add Course Outcome" />
                </div>
                <div class="form-grid">
                    <div class="form-group">
                        <label>CO Identifier <span>*</span></label>
                        <asp:DropDownList ID="ddlCoIdentifier" runat="server" CssClass="form-control" />
                    </div>

                    <div class="form-group">
                        <label>CO Statement <span>*</span></label>
                        <asp:TextBox ID="txtCoStatement" runat="server" CssClass="form-control" placeholder="Enter course outcome statement" />
                    </div>

                    <asp:LinkButton ID="btnAddCo" runat="server" CssClass="btn btn-primary" OnClick="btnAddCo_Click">
                        <i class="fa-solid fa-circle-plus"></i> Add CO
                    </asp:LinkButton>

                    <asp:LinkButton ID="btnCancelEdit" runat="server" CssClass="btn btn-secondary" Visible="false" OnClick="btnCancelEdit_Click">
                        <i class="fa-solid fa-xmark"></i> Cancel
                    </asp:LinkButton>
                </div>
            </div>

            <div class="custom-card">
                <div class="card-header-title">
                    <i class="fa-solid fa-list-ul"></i> Defined Course Outcomes
                </div>
                <div class="table-container">
                    <asp:GridView ID="gvCourseOutcomes" runat="server" AutoGenerateColumns="False" 
                        CssClass="custom-grid" GridLines="None" OnRowCommand="gvCourseOutcomes_RowCommand">
                        <Columns>
                            <asp:TemplateField HeaderText="S.No" ItemStyle-Width="60px">
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="CO Code" ItemStyle-Width="120px">
                                <ItemTemplate>
                                    <span class="badge-co"><%# Eval("CoCode") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:BoundField DataField="CoStatement" HeaderText="Course Outcome Statement" />

                            <asp:TemplateField HeaderText="Action" ItemStyle-Width="190px" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <div class="action-cell">
                                        <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn-action-edit" CommandName="EditRow" CommandArgument='<%# Eval("CoId") %>'>
                                            <i class="fa-solid fa-pen"></i> Edit
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn-action-delete" CommandName="DeleteRow" CommandArgument='<%# Eval("CoId") %>' OnClientClick="return confirm('Are you sure you want to delete this CO?');">
                                            <i class="fa-solid fa-trash-can"></i> Delete
                                        </asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>

                <div class="bottom-bar">
                    <asp:LinkButton ID="btnSubmitApproval" runat="server" CssClass="btn btn-success" OnClick="btnSubmitApproval_Click" OnClientClick="return confirm('Are you sure you want to send these COs for Approval? Once submitted, you cannot add, edit, or delete until reviewed.');">
                        <i class="fa-solid fa-paper-plane"></i> Submit for Approval
                    </asp:LinkButton>
                </div>
            </div>

        </div>
    </form>
</body>
</html>