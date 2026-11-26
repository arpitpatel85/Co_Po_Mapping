<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Po_Define.aspx.cs" Inherits="Co_Po_Mapping_Master_Po_Define" %>

<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <title>PO Master - Define Program Outcomes</title>
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
            grid-template-columns: 350px;
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
            outline: none;
            background: #fff;
        }

            .form-control:focus {
                border-color: var(--primary-navy);
                box-shadow: 0 0 0 3px rgba(22, 61, 120, 0.18);
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
            text-decoration: none;
            white-space: nowrap;
        }

        .btn-primary {
            background-color: var(--primary-navy);
            color: #fff;
        }

            .btn-primary:hover {
                background-color: var(--primary-navy-hover);
            }

        .btn-secondary {
            background-color: #64748b;
            color: #fff;
        }

            .btn-secondary:hover {
                background-color: #475569;
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

            .custom-grid tr:hover {
                background-color: #f8fafc;
            }

        .badge-po {
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
            color: #fff !important;
            padding: 6px 14px;
            border-radius: 4px;
            font-size: 12px;
            border: none;
            cursor: pointer;
            text-decoration: none !important;
        }

        .btn-action-delete {
            background-color: #ef4444;
            color: #fff !important;
            padding: 6px 14px;
            border-radius: 4px;
            font-size: 12px;
            border: none;
            cursor: pointer;
            text-decoration: none !important;
        }

        @media (max-width: 900px) {
            .selection-grid {
                grid-template-columns: 1fr;
            }

            .form-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <div class="main-wrapper">
            <asp:HiddenField ID="hdnEditPoId" runat="server" Value="0" />

            <div class="page-header">
                <div class="header-icon">
                    <i class="fa-solid fa-bullseye"></i>
                </div>
                <div class="header-title">
                    <h2>Define Program Outcomes (PO)</h2>
                    <p>Select program/course and define standard Program Outcomes (PO1 to PO12).</p>
                </div>
            </div>

            <!-- Course Selection -->
            <div class="custom-card">
                <div class="card-header-title">
                    <i class="fa-solid fa-filter"></i>Course Selection
                </div>
                <div class="selection-grid">
                    <div class="form-group">
                        <label>Course / Program <span>*</span></label>
                        <asp:DropDownList ID="ddlCourse" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCourse_SelectedIndexChanged" />
                    </div>
                </div>
            </div>

            <!-- Add / Update PO Card -->
            <div class="custom-card">
                <div class="card-header-title">
                    <i class="fa-solid fa-circle-plus"></i>
                    <asp:Label ID="lblFormTitle" runat="server" Text="Add Program Outcome" />
                </div>
                <div class="form-grid">
                    <div class="form-group">
                        <label>PO Identifier <span>*</span></label>
                        <asp:DropDownList ID="ddlPoCode" runat="server" CssClass="form-control" />
                    </div>

                    <div class="form-group">
                        <label>PO Statement / Description <span>*</span></label>
                        <asp:TextBox ID="txtPoStatement" runat="server" CssClass="form-control" placeholder="Enter program outcome statement" />
                    </div>

                    <asp:LinkButton ID="btnAddPo" runat="server" CssClass="btn btn-primary" OnClick="btnAddPo_Click">
                        <i class="fa-solid fa-circle-plus"></i> Add PO
                    </asp:LinkButton>

                    <asp:LinkButton ID="btnCancelEdit" runat="server" CssClass="btn btn-secondary" Visible="false" OnClick="btnCancelEdit_Click">
                        <i class="fa-solid fa-xmark"></i> Cancel
                    </asp:LinkButton>
                </div>
            </div>

            <!-- Defined POs Grid Card -->
            <div class="custom-card">
                <div class="card-header-title">
                    <i class="fa-solid fa-list-ul"></i>Defined Program Outcomes (PO)
                </div>
                <div class="table-container">
                    <asp:GridView ID="gvPoList" runat="server" AutoGenerateColumns="False"
                        CssClass="custom-grid" GridLines="None" OnRowCommand="gvPoList_RowCommand">
                        <Columns>
                            <asp:TemplateField HeaderText="S.No" ItemStyle-Width="60px">
                                <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="PO Code" ItemStyle-Width="120px">
                                <ItemTemplate>
                                    <span class="badge-po"><%# Eval("PoCode") %></span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="PoStatement" HeaderText="Program Outcome Statement" />

                            <asp:TemplateField HeaderText="Action" ItemStyle-Width="190px" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <div class="action-cell">
                                        <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn-action-edit" CommandName="EditRow" CommandArgument='<%# Eval("PoId") %>'>
                                            <i class="fa-solid fa-pen"></i> Edit
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn-action-delete" CommandName="DeleteRow" CommandArgument='<%# Eval("PoId") %>' OnClientClick="return confirm('Are you sure you want to delete this PO?');">
                                            <i class="fa-solid fa-trash-can"></i> Delete
                                        </asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

        </div>
    </form>
</body>
</html>
