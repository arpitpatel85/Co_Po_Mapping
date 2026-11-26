<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Co_Approval.aspx.cs" Inherits="Co_Po_Mapping_Master_Co_Approval" %>

<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <title>CO Approval Portal</title>
    <link href="https://fonts.googleapis.com/css2?family=Public+Sans:wght@400;500;600;700&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />

    <style>
        :root {
            --primary-navy: #163d78;
            --primary-navy-hover: #102e5b;
            --primary-navy-light: #e8eef8;
            --primary-navy-border: #c8d8f0;
        }

        * { box-sizing: border-box; font-family: 'Public Sans', sans-serif; }
        body { background-color: #f4f6fa; margin: 0; padding: 24px; color: #334155; }
        .main-wrapper { max-width: 1300px; margin: 0 auto; display: flex; flex-direction: column; gap: 20px; }

        .page-header { display: flex; align-items: center; gap: 16px; }
        .header-icon { width: 52px; height: 52px; background-color: var(--primary-navy); border-radius: 50%; display: flex; align-items: center; justify-content: center; color: #fff; font-size: 24px; }
        .header-title h2 { margin: 0 0 4px 0; font-size: 22px; font-weight: 700; color: var(--primary-navy); }
        .header-title p { margin: 0; font-size: 13px; color: #64748b; }

        .custom-card { background: #fff; border: 1px solid #e2e8f0; border-radius: 10px; padding: 22px 24px; box-shadow: 0 2px 4px rgba(0,0,0,0.02); }
        .card-header-title { display: flex; align-items: center; gap: 10px; font-size: 16px; font-weight: 700; color: var(--primary-navy); margin-bottom: 18px; }

        .table-container { overflow-x: auto; border-radius: 6px; border: 1px solid #e2e8f0; }
        .custom-grid { width: 100%; border-collapse: collapse; font-size: 13px; }
        .custom-grid th { background-color: var(--primary-navy-light); color: var(--primary-navy); font-weight: 600; text-align: left; padding: 12px 14px; border-bottom: 1px solid var(--primary-navy-border); white-space: nowrap; }
        .custom-grid td { padding: 12px 14px; border-bottom: 1px solid #edf2f7; color: #334155; vertical-align: middle; }
        .custom-grid tr:hover { background-color: #f8fafc; }

        .badge-status { padding: 4px 10px; border-radius: 12px; font-size: 11px; font-weight: 600; display: inline-block; white-space: nowrap; }
        .badge-submitted { background: #fef9c3; color: #854d0e; border: 1px solid #fde047; }
        .badge-approved { background: #dcfce7; color: #166534; border: 1px solid #86efac; }
        .badge-rejected { background: #fee2e2; color: #991b1b; border: 1px solid #fca5a5; }

        /* Action Buttons Container & Elements (Fixed Overlap) */
        .action-cell {
            display: inline-flex;
            align-items: center;
            justify-content: flex-start;
            gap: 8px;
            white-space: nowrap;
        }

        .btn-action-accept { 
            background: #16a34a; 
            color: #ffffff !important; 
            padding: 6px 14px; 
            border-radius: 4px; 
            font-size: 12px; 
            border: none; 
            cursor: pointer; 
            text-decoration: none !important; 
            font-weight: 500; 
            display: inline-flex;
            align-items: center;
            gap: 5px;
            line-height: 1.4;
            transition: background 0.2s ease;
        }
        .btn-action-accept:hover { background: #15803d; }

        .btn-action-reject { 
            background: #dc2626; 
            color: #ffffff !important; 
            padding: 6px 14px; 
            border-radius: 4px; 
            font-size: 12px; 
            border: none; 
            cursor: pointer; 
            text-decoration: none !important; 
            font-weight: 500; 
            display: inline-flex;
            align-items: center;
            gap: 5px;
            line-height: 1.4;
            transition: background 0.2s ease;
        }
        .btn-action-reject:hover { background: #b91c1c; }

        .txt-remark { 
            width: 140px; 
            padding: 6px 8px; 
            border: 1px solid #cbd5e1; 
            border-radius: 4px; 
            font-size: 12px; 
            outline: none;
        }
        .txt-remark:focus {
            border-color: var(--primary-navy);
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="main-wrapper">
            
            <div class="page-header">
                <div class="header-icon">
                    <i class="fa-solid fa-list-check"></i>
                </div>
                <div class="header-title">
                    <h2>Course Outcomes Approval Portal</h2>
                    <p>Review, Accept or Reject Course Outcomes submitted by faculty members.</p>
                </div>
            </div>

            <div class="custom-card">
                <div class="card-header-title">
                    <i class="fa-solid fa-inbox"></i> Pending & Submitted CO Requests
                </div>
                
                <div class="table-container">
                    <asp:GridView ID="gvApprovalRequests" runat="server" AutoGenerateColumns="False" 
                        CssClass="custom-grid" GridLines="None" OnRowCommand="gvApprovalRequests_RowCommand">
                        <Columns>
                            <asp:TemplateField HeaderText="S.No" ItemStyle-Width="45px">
                                <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="FacultyInfo" HeaderText="Teacher" />
                            <asp:BoundField DataField="CourseName" HeaderText="Course" />
                            <asp:BoundField DataField="SemesterName" HeaderText="Semester" />
                            <asp:BoundField DataField="SpecializationName" HeaderText="Specialization" />
                            <asp:BoundField DataField="SubjectName" HeaderText="Subject" />
                            
                            <asp:TemplateField HeaderText="Defined Outcomes (COs)">
                                <ItemTemplate>
                                    <asp:Literal ID="litOutcomes" runat="server" Text='<%# Eval("AllCOs") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Status" ItemStyle-Width="110px">
                                <ItemTemplate>
                                    <span class='<%# "badge-status badge-" + Eval("ApprovalStatus").ToString().ToLower() %>'>
                                        <%# Eval("ApprovalStatus") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Remarks" ItemStyle-Width="160px">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtGridRemark" runat="server" CssClass="txt-remark" placeholder="Enter remarks..." Text='<%# Eval("Remarks") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Action" ItemStyle-Width="190px">
                                <ItemTemplate>
                                    <div class="action-cell">
                                        <asp:LinkButton ID="btnAccept" runat="server" CssClass="btn-action-accept" CommandName="AcceptRow" CommandArgument='<%# Eval("ApprovalId") %>'>
                                            <i class="fa-solid fa-check"></i> Accept
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnReject" runat="server" CssClass="btn-action-reject" CommandName="RejectRow" CommandArgument='<%# Eval("ApprovalId") %>' OnClientClick="return confirm('Are you sure you want to REJECT this submission?');">
                                            <i class="fa-solid fa-xmark"></i> Reject
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