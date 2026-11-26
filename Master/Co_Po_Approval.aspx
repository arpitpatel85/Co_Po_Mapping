<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Co_Po_Approval.aspx.cs" Inherits="Co_Po_Mapping_Master_Co_Po_Approval" MaintainScrollPositionOnPostback="false" %>

<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <title>Pending CO-PO Mapping Approvals (Dean)</title>
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
        .main-wrapper { max-width: 1400px; margin: 0 auto; display: flex; flex-direction: column; gap: 20px; }

        .page-header { display: flex; align-items: center; gap: 16px; }
        .header-icon { width: 52px; height: 52px; background-color: var(--primary-navy); border-radius: 50%; display: flex; align-items: center; justify-content: center; color: #fff; font-size: 24px; }
        .header-title h2 { margin: 0 0 4px 0; font-size: 22px; font-weight: 700; color: var(--primary-navy); }
        .header-title p { margin: 0; font-size: 13px; color: #64748b; }

        .custom-card { background: #fff; border: 1px solid #e2e8f0; border-radius: 10px; padding: 22px 24px; box-shadow: 0 2px 4px rgba(0,0,0,0.02); }
        .card-header-title { display: flex; align-items: center; justify-content: space-between; font-size: 16px; font-weight: 700; color: var(--primary-navy); margin-bottom: 18px; }

        .table-container { overflow-x: auto; border: 1px solid #e2e8f0; border-radius: 8px; }
        .custom-grid { width: 100%; border-collapse: collapse; font-size: 13.5px; }
        .custom-grid th { background-color: var(--primary-navy-light); color: var(--primary-navy); font-weight: 600; text-align: left; padding: 12px 14px; border-bottom: 1px solid var(--primary-navy-border); white-space: nowrap; }
        .custom-grid td { padding: 12px 14px; border-bottom: 1px solid #edf2f7; color: #334155; vertical-align: middle; }
        .custom-grid tr:hover { background-color: #f8fafc; }

        .faculty-badge { background: #e0e7ff; color: #3730a3; padding: 4px 10px; border-radius: 4px; font-weight: 600; font-size: 12px; display: inline-flex; align-items: center; gap: 6px; }

        .btn-review { background-color: var(--primary-navy); color: #fff !important; padding: 7px 14px; border-radius: 6px; font-size: 12.5px; border: none; cursor: pointer; text-decoration: none !important; font-weight: 600; display: inline-flex; align-items: center; gap: 6px; }
        .btn-review:hover { background-color: var(--primary-navy-hover); }

        /* ================= MODAL FIX (SCREEN VIEWPORT LOCK) ================= */
        .modal-overlay {
            position: fixed !important;
            top: 0 !important;
            left: 0 !important;
            width: 100vw !important;
            height: 100vh !important;
            background: rgba(15, 23, 42, 0.75) !important;
            backdrop-filter: blur(2px);
            z-index: 9999999 !important;
            display: flex !important;
            justify-content: center !important;
            align-items: flex-start !important; /* Top aligned */
            padding-top: 40px !important;       /* Top se 40px fix distance */
            box-sizing: border-box !important;
        }

        .modal-box {
            background: #ffffff;
            width: 95%;
            max-width: 1150px;
            max-height: calc(100vh - 80px);
            border-radius: 10px;
            display: flex;
            flex-direction: column;
            overflow: hidden;
            box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5);
            margin: 0 auto;
        }

        .modal-header {
            padding: 14px 20px;
            background: var(--primary-navy);
            color: #fff;
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-shrink: 0;
        }
        .modal-header h3 { margin: 0; font-size: 16px; font-weight: 600; }
        .btn-close-modal { background: none; border: none; color: #fff; font-size: 24px; line-height: 1; cursor: pointer; text-decoration: none !important; }

        .modal-body {
            padding: 20px;
            overflow-y: auto;
            display: flex;
            flex-direction: column;
            gap: 14px;
        }

        .strip-info {
            background: #f8fafc;
            border: 1px solid #e2e8f0;
            padding: 10px 14px;
            border-radius: 6px;
            font-size: 13px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        /* Matrix Table View */
        .matrix-table { width: 100%; border-collapse: collapse; font-size: 12.5px; text-align: center; }
        .matrix-table th { background-color: var(--primary-navy-light); color: var(--primary-navy); padding: 9px 6px; border: 1px solid #cbd5e1; font-weight: 700; white-space: nowrap; }
        .matrix-table td { padding: 8px 6px; border: 1px solid #e2e8f0; }
        .co-stmt-col { text-align: left; min-width: 260px; padding-left: 10px !important; }
        .avg-row td { background-color: #f1f5f9; font-weight: 700; color: var(--primary-navy); }

        .level-chip { display: inline-block; width: 30px; height: 26px; line-height: 26px; border-radius: 4px; font-weight: 700; font-size: 12.5px; }
        .chip-3 { background: #dcfce7; color: #166534; }
        .chip-2 { background: #fef9c3; color: #854d0e; }
        .chip-1 { background: #fee2e2; color: #991b1b; }
        .chip-0 { color: #94a3b8; font-weight: normal; }

        .modal-footer {
            padding: 14px 20px;
            background: #f8fafc;
            border-top: 1px solid #e2e8f0;
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-shrink: 0;
        }
        .form-control { width: 100%; padding: 8px 12px; border: 1px solid #cbd5e1; border-radius: 6px; font-size: 13px; outline: none; }
        
        .btn { height: 38px; padding: 0 20px; border: none; border-radius: 6px; font-weight: 600; cursor: pointer; display: inline-flex; align-items: center; gap: 8px; font-size: 13px; }
        .btn-approve { background-color: #16a34a; color: #fff; }
        .btn-approve:hover { background-color: #15803d; }
        .btn-reject { background-color: #dc2626; color: #fff; }
        .btn-reject:hover { background-color: #b91c1c; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <div class="main-wrapper">
            
            <div class="page-header">
                <div class="header-icon">
                    <i class="fa-solid fa-stamp"></i>
                </div>
                <div class="header-title">
                    <h2>Pending CO–PO Mapping Approvals</h2>
                    <p>Review faculty submitted correlation mappings. Once approved or rejected, the request will be removed from this list.</p>
                </div>
            </div>

            <!-- Grid Card -->
            <div class="custom-card">
                <div class="card-header-title">
                    <span><i class="fa-solid fa-clock-rotate-left"></i> Submitted Requests Awaiting Dean Approval</span>
                    <asp:LinkButton ID="btnRefresh" runat="server" CssClass="btn-review" OnClick="btnRefresh_Click">
                        <i class="fa-solid fa-rotate-right"></i> Refresh
                    </asp:LinkButton>
                </div>

                <div class="table-container">
                    <asp:GridView ID="gvSubmissions" runat="server" AutoGenerateColumns="False" 
                        CssClass="custom-grid" GridLines="None" OnRowCommand="gvSubmissions_RowCommand" EmptyDataText="Currently, no new mapping is pending for approval.">
                        <Columns>
                            <asp:TemplateField HeaderText="S.No" ItemStyle-Width="45px">
                                <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="CourseName" HeaderText="Course / Program" />
                            <asp:BoundField DataField="SemesterName" HeaderText="Semester" ItemStyle-Width="110px" />
                            <asp:BoundField DataField="SpecializationName" HeaderText="Specialization" />
                            <asp:BoundField DataField="SubjectDisplay" HeaderText="Subject" ItemStyle-Font-Bold="true" />

                            <asp:TemplateField HeaderText="Faculty ID" ItemStyle-Width="150px">
                                <ItemTemplate>
                                    <span class="faculty-badge">
                                        <i class="fa-solid fa-id-card"></i> <%# Eval("FacultyId") %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="SubmittedOn" HeaderText="Submitted Date" ItemStyle-Width="125px" />

                            <asp:TemplateField HeaderText="Action" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnReview" runat="server" CssClass="btn-review" 
                                        CommandName="ReviewMap" 
                                        CommandArgument='<%# Eval("CourseId") + "|" + Eval("SemesterId") + "|" + Eval("SpecializationId") + "|" + Eval("SubjectId") + "|" + Eval("AcademicYear") + "|" + Eval("SubjectDisplay") + "|" + Eval("FacultyId") %>'>
                                        <i class="fa-solid fa-eye"></i> Review 
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

        </div>

        <!-- ================= MODAL PANEL OUTSIDE WRAPPER (DIRECT CHILD OF FORM) ================= -->
        <asp:Panel ID="pnlModal" runat="server" Visible="false" CssClass="modal-overlay">
            <div class="modal-box">
                <div class="modal-header">
                    <h3><i class="fa-solid fa-table-cells"></i> <asp:Label ID="lblModalSubject" runat="server" /></h3>
                    <asp:LinkButton ID="btnCloseModal" runat="server" CssClass="btn-close-modal" OnClick="btnCloseModal_Click">&times;</asp:LinkButton>
                </div>

                <div class="modal-body">
                    <asp:HiddenField ID="hdnSelectedCourseId" runat="server" />
                    <asp:HiddenField ID="hdnSelectedSemId" runat="server" />
                    <asp:HiddenField ID="hdnSelectedSpecId" runat="server" />
                    <asp:HiddenField ID="hdnSelectedSubjectId" runat="server" />
                    <asp:HiddenField ID="hdnSelectedSesnId" runat="server" />

                    <div class="strip-info">
                        <span><b>Submitted By Faculty ID:</b> <asp:Label ID="lblModalFacultyId" runat="server" Font-Bold="true" ForeColor="#163d78" /></span>
                        <span><b>Scale:</b> 1 (Low), 2 (Medium), 3 (High), - (No Correlation)</span>
                    </div>

                    <div class="table-container">
                        <asp:Literal ID="litMatrixPreview" runat="server" />
                    </div>

                    <div>
                        <label style="font-size:13px; font-weight:600; color:var(--primary-navy);">Dean Remarks (Required if rejecting):</label>
                        <asp:TextBox ID="txtDeanRemarks" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control" style="margin-top:6px;" placeholder="Enter comments or reason if rejecting..." />
                    </div>
                </div>

                <div class="modal-footer">
                    <asp:Button ID="btnClose" runat="server" Text="Cancel" CssClass="btn" style="background:#e2e8f0; color:#334155;" OnClick="btnCloseModal_Click" />
                    <div style="display:flex; gap:10px;">
                        <asp:Button ID="btnReject" runat="server" Text="Reject Mapping" CssClass="btn btn-reject" OnClick="btnReject_Click" OnClientClick="return confirm('Kya aap is mapping ko REJECT karna chahte hain?');" />
                        <asp:Button ID="btnApprove" runat="server" Text="Approve Mapping" CssClass="btn btn-approve" OnClick="btnApprove_Click" OnClientClick="return confirm('Kya aap is mapping ko APPROVE karna chahte hain?');" />
                    </div>
                </div>
            </div>
        </asp:Panel>

    </form>
</body>
</html>