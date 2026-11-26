<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Co_Po_Mapping.aspx.cs" Inherits="Co_Po_Mapping_Master_Co_Po_Mapping" %>

<!DOCTYPE html>
<html lang="en">
<head id="Head1" runat="server">
    <meta charset="utf-8" />
    <title>CO-PO Correlation Mapping</title>
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

        .selection-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; }
        .form-group { display: flex; flex-direction: column; gap: 6px; }
        .form-group label { font-size: 13px; font-weight: 600; color: var(--primary-navy); }
        .form-group label span { color: #ef4444; }

        .form-control {
            height: 42px; width: 100%; padding: 8px 12px; border: 1px solid #cbd5e1; border-radius: 6px; font-size: 13px; outline: none; background: #fff;
        }
        .form-control:focus { border-color: var(--primary-navy); }

        .guide-box {
            background-color: #eff6ff; border: 1px solid #bfdbfe; border-radius: 6px; padding: 12px 16px; font-size: 13px; color: #1e40af; margin-bottom: 16px; display: flex; gap: 24px; align-items: center;
        }

        /* PO Reference Accordion Box */
        .po-guide-card {
            background-color: #f8fafc;
            border: 1px solid #cbd5e1;
            border-radius: 8px;
            margin-bottom: 16px;
            overflow: hidden;
        }
        .po-guide-header {
            background-color: var(--primary-navy-light);
            padding: 10px 16px;
            font-weight: 600;
            color: var(--primary-navy);
            cursor: pointer;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        .po-guide-body {
            padding: 14px 18px;
            display: none;
            font-size: 13px;
            max-height: 250px;
            overflow-y: auto;
        }
        .po-guide-item {
            margin-bottom: 8px;
            line-height: 1.5;
        }
        .po-guide-item b {
            color: var(--primary-navy);
            display: inline-block;
            width: 70px;
        }

        /* Matrix Table */
        .table-responsive { overflow-x: auto; border: 1px solid #cbd5e1; border-radius: 8px; }
        .matrix-table { width: 100%; border-collapse: collapse; font-size: 13px; text-align: center; }
        .matrix-table th { background-color: var(--primary-navy-light); color: var(--primary-navy); padding: 10px 8px; border: 1px solid #cbd5e1; font-weight: 700; white-space: nowrap; }
        .matrix-table td { padding: 8px 6px; border: 1px solid #e2e8f0; vertical-align: middle; }
        .matrix-table tr:hover { background-color: #f8fafc; }

        .po-header-cell { cursor: help; }
        .co-stmt-col { text-align: left; min-width: 280px; padding-left: 12px !important; }
        
        .ddl-level {
            width: 55px; height: 32px; border: 1px solid #cbd5e1; border-radius: 4px; text-align: center; font-weight: 600; color: var(--primary-navy); background: #fff; outline: none;
        }
        .ddl-level:focus { border-color: var(--primary-navy); }

        .avg-row td { background-color: #f1f5f9; font-weight: 700; color: var(--primary-navy); }

        .button-bar {
            display: flex; justify-content: flex-end; gap: 12px; margin-top: 18px;
        }
        .btn-save {
            height: 42px; padding: 0 24px; background-color: var(--primary-navy); color: #fff; border: none; border-radius: 6px; font-weight: 600; cursor: pointer; display: inline-flex; align-items: center; gap: 8px; font-size: 13.5px;
        }
        .btn-save:hover { background-color: var(--primary-navy-hover); }

        .btn-submit {
            height: 42px; padding: 0 24px; background-color: #16a34a; color: #fff; border: none; border-radius: 6px; font-weight: 600; cursor: pointer; display: inline-flex; align-items: center; gap: 8px; font-size: 13.5px;
        }
        .btn-submit:hover { background-color: #15803d; }

        .btn-reset {
            height: 42px; padding: 0 20px; background-color: #dc2626; color: #fff; border: none; border-radius: 6px; font-weight: 600; cursor: pointer; display: inline-flex; align-items: center; gap: 8px; font-size: 13.5px;
        }
        .btn-reset:hover { background-color: #b91c1c; }

        .status-badge { display: inline-flex; align-items: center; gap: 6px; padding: 5px 14px; border-radius: 20px; font-size: 12.5px; font-weight: 700; }
        .badge-draft { background-color: #f1f5f9; color: #475569; border: 1px solid #cbd5e1; }
        .badge-submitted { background-color: #fef3c7; color: #92400e; border: 1px solid #fde68a; }
        .badge-approved { background-color: #dcfce7; color: #166534; border: 1px solid #86efac; }
        .badge-rejected { background-color: #fee2e2; color: #991b1b; border: 1px solid #fca5a5; }

        .status-warn { background-color: #fef2f2; border: 1px solid #fecaca; color: #991b1b; padding: 14px; border-radius: 6px; font-weight: 600; font-size: 13.5px; }

        .remarks-alert { background-color: #fff1f2; border: 1px solid #fecdd3; border-radius: 6px; padding: 12px 16px; font-size: 13px; color: #9f1239; margin-bottom: 16px; }

        @media (max-width: 900px) {
            .selection-grid { grid-template-columns: 1fr 1fr; }
        }
    </style>

    <script type="text/javascript">
        function togglePoGuide() {
            var body = document.getElementById("poGuideBody");
            var icon = document.getElementById("poGuideIcon");
            if (body.style.display === "block") {
                body.style.display = "none";
                icon.className = "fa-solid fa-chevron-down";
            } else {
                body.style.display = "block";
                icon.className = "fa-solid fa-chevron-up";
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <div class="main-wrapper">
            
            <div class="page-header">
                <div class="header-icon">
                    <i class="fa-solid fa-table-cells"></i>
                </div>
                <div class="header-title">
                    <h2>CO–PO Correlation Mapping</h2>
                    <p>Map approved Course Outcomes with Program Outcomes (PO1 to PO12) and submit to Dean for approval.</p>
                </div>
            </div>

            <!-- Filters -->
            <div class="custom-card">
                <div class="card-header-title">
                    <span><i class="fa-solid fa-filter"></i> Select Course & Subject</span>
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

            <!-- Pre-condition Alert Panel -->
            <asp:Panel ID="pnlNotApproved" runat="server" Visible="false">
                <div class="status-warn">
                    <i class="fa-solid fa-circle-exclamation"></i>
                    <asp:Label ID="lblNotApprovedMsg" runat="server" />
                </div>
            </asp:Panel>

            <!-- Mapping Matrix Container -->
            <asp:Panel ID="pnlMatrix" runat="server" Visible="false" CssClass="custom-card">
                <div class="card-header-title">
                    <span><i class="fa-solid fa-network-wired"></i> CO-PO Correlation Matrix</span>
                    <asp:Literal ID="litStatusBadge" runat="server" />
                </div>

                <!-- Dean Feedback if Rejected -->
                <asp:Panel ID="pnlDeanRemarks" runat="server" Visible="false">
                    <div class="remarks-alert">
                        <i class="fa-solid fa-circle-xmark"></i> <b>Dean Feedback / Reason:</b> <asp:Label ID="lblDeanRemarksText" runat="server" />
                    </div>
                </asp:Panel>

                <!-- Expandable PO Reference Guide for Faculty -->
                <div class="po-guide-card">
                    <div class="po-guide-header" onclick="togglePoGuide()">
                        <span><i class="fa-solid fa-circle-info"></i> Click here to View Program Outcomes (PO1 to PO12) Descriptions</span>
                        <i id="poGuideIcon" class="fa-solid fa-chevron-down"></i>
                    </div>
                    <div id="poGuideBody" class="po-guide-body">
                        <asp:Literal ID="litPoGuideList" runat="server" />
                    </div>
                </div>

                <div class="guide-box">
                    <span><i class="fa-solid fa-info-circle"></i> <b>Level Guide:</b></span>
                    <span><b>3:</b> Substantial (High)</span>
                    <span><b>2:</b> Moderate (Medium)</span>
                    <span><b>1:</b> Slight (Low)</span>
                    <span><b>- :</b> No Correlation</span>
                </div>

                <div class="table-responsive">
                    <asp:Literal ID="litMatrix" runat="server" />
                </div>

                <div class="button-bar">
                    <asp:Button ID="btnResetMapping" runat="server" Text="Reset / Clear Mapping" CssClass="btn-reset" OnClick="btnResetMapping_Click" OnClientClick="return confirm('Are you sure you want to clear all mappings for this subject?');" />
                    <asp:Button ID="btnSaveMapping" runat="server" Text="Save as Draft" CssClass="btn-save" OnClick="btnSaveMapping_Click" />
                    <asp:Button ID="btnSubmitForApproval" runat="server" Text="Submit for Approval" CssClass="btn-submit" OnClick="btnSubmitForApproval_Click" OnClientClick="return confirm('Are you sure you want to submit this CO-PO Mapping to the Dean for approval? Once submitted, you cannot make any changes until it has been reviewed by the Dean.');" />
                </div>
            </asp:Panel>

        </div>
    </form>
</body>
</html>