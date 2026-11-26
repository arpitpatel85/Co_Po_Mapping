using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Co_Po_Mapping_Master_Co_Po_Approval : System.Web.UI.Page
{
    DbFunctions objfun = new DbFunctions();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UID"] == null || Session["SesnID"] == null)
        {
            Response.Redirect("../error_404_2.html");
            return;
        }

        if (!IsPostBack)
        {
            LoadPendingSubmissions();
        }
    }

    private void LoadPendingSubmissions()
    {
        try
        {
            string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";
            string sesnId = Session["SesnID"].ToString();

            // Strict: Sirf 'Submitted' requests hi Dean ke pass aayengi
            string query = @"SELECT DISTINCT 
                                app.CourseId, 
                                app.SemesterId, 
                                app.SpecializationId, 
                                app.SubjectId, 
                                app.AcademicYear,
                                ISNULL(c.CourseName, 'Program ' + CAST(app.CourseId AS VARCHAR)) AS CourseName,
                                ISNULL(CAST(sv.Year AS VARCHAR) + '-' + sv.CourseYear, 'Sem ' + CAST(app.SemesterId AS VARCHAR)) AS SemesterName,
                                ISNULL(sp.shortName, 'General') AS SpecializationName,
                                ISNULL(sb.SubjectName + ' (' + sb.SubjectCode + ')', sb.SubjectName) AS SubjectDisplay,
                                COALESCE(NULLIF(app.FacultyId, ''), NULLIF(m.FacultyId, ''), '-') AS FacultyId,
                                CONVERT(VARCHAR(12), ISNULL(app.SubmittedDate, app.CreatedDate), 106) AS SubmittedOn
                             FROM tbl_CO_PO_Approval_Status app
                             INNER JOIN Course c ON app.CourseId = c.CourseId
                             INNER JOIN Semester_View sv ON app.SemesterId = sv.sid AND sv.sessionyear = app.AcademicYear
                             INNER JOIN dbo.Specilisation sp ON app.SpecializationId = sp.SpecilisationID
                             INNER JOIN subject_vw sb ON app.SubjectId = sb.SubjectId AND sb.SessionID = app.AcademicYear
                             LEFT JOIN (
                                 SELECT DISTINCT SubjectId, AcademicYear, FacultyId 
                                 FROM tbl_CO_PO_Mapping 
                                 WHERE FacultyId IS NOT NULL AND FacultyId <> ''
                             ) m ON app.SubjectId = m.SubjectId AND app.AcademicYear = m.AcademicYear
                             WHERE app.InstituteID = '" + instId + "' " +
                             "AND app.AcademicYear = '" + sesnId + "' " +
                             "AND app.ApprovalStatus = 'Submitted' " +
                             "ORDER BY app.CourseId, app.SemesterId";

            DataTable dt = objfun.FillDataTable(query);
            gvSubmissions.DataSource = dt;
            gvSubmissions.DataBind();
        }
        catch (Exception ex)
        {
            objfun.MsgBox(ex.Message, this);
        }
    }

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        LoadPendingSubmissions();
    }

    protected void gvSubmissions_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "ReviewMap")
        {
            string[] args = e.CommandArgument.ToString().Split('|');
            if (args.Length >= 6)
            {
                hdnSelectedCourseId.Value = args[0];
                hdnSelectedSemId.Value = args[1];
                hdnSelectedSpecId.Value = args[2];
                hdnSelectedSubjectId.Value = args[3];
                hdnSelectedSesnId.Value = args[4];
                lblModalSubject.Text = args[5];

                // Faculty ID bind
                string fId = (args.Length > 6 && !string.IsNullOrWhiteSpace(args[6])) ? args[6] : "-";
                lblModalFacultyId.Text = fId;

                txtDeanRemarks.Text = "";
                RenderMatrixPreview(args[0], args[3], args[4]);

                // Modal display open
                pnlModal.Visible = true;
            }
        }
    }

    private void RenderMatrixPreview(string courseId, string subjectId, string sesnId)
    {
        string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";

        // 1. COs
        string coQ = "SELECT CoId, CoCode, CoStatement FROM tbl_Course_Outcomes " +
                     "WHERE CourseId = '" + courseId + "' AND SubjectId = '" + subjectId + "' AND AcademicYear = '" + sesnId + "' " +
                     "ORDER BY CAST(SUBSTRING(CoCode, 3, LEN(CoCode)) AS INT) ASC";
        DataTable dtCO = objfun.FillDataTable(coQ);

        // 2. POs
        string poQ = "SELECT PoCode, PoStatement FROM tbl_PO_Master WHERE CourseId = '" + courseId + "' AND InstituteID = '" + instId + "' " +
                     "ORDER BY CAST(SUBSTRING(PoCode, 3, LEN(PoCode)) AS INT) ASC";
        DataTable dtPO = objfun.FillDataTable(poQ);

        // 3. Saved Mappings
        string mapQ = "SELECT CoId, PoCode, CorrelationLevel FROM tbl_CO_PO_Mapping " +
                      "WHERE SubjectId = '" + subjectId + "' AND AcademicYear = '" + sesnId + "'";
        DataTable dtSaved = objfun.FillDataTable(mapQ);

        Dictionary<string, int> savedMap = new Dictionary<string, int>();
        if (dtSaved != null)
        {
            foreach (DataRow r in dtSaved.Rows)
            {
                string key = r["CoId"].ToString() + "_" + r["PoCode"].ToString();
                savedMap[key] = Convert.ToInt32(r["CorrelationLevel"]);
            }
        }

        // Table Builder
        StringBuilder sb = new StringBuilder();
        sb.Append("<table class='matrix-table'>");

        sb.Append("<thead><tr><th style='width:70px;'>CO Code</th><th class='co-stmt-col'>Course Outcome Statement</th>");
        foreach (DataRow po in dtPO.Rows)
        {
            sb.Append("<th title='" + Server.HtmlEncode(po["PoStatement"].ToString()) + "'>" + po["PoCode"].ToString() + "</th>");
        }
        sb.Append("</tr></thead><tbody>");

        Dictionary<string, double> sumPo = new Dictionary<string, double>();
        Dictionary<string, int> countPo = new Dictionary<string, int>();
        foreach (DataRow po in dtPO.Rows)
        {
            sumPo[po["PoCode"].ToString()] = 0;
            countPo[po["PoCode"].ToString()] = 0;
        }

        foreach (DataRow co in dtCO.Rows)
        {
            string coId = co["CoId"].ToString();
            sb.Append("<tr><td><b>" + co["CoCode"].ToString() + "</b></td>");
            sb.Append("<td class='co-stmt-col'>" + co["CoStatement"].ToString() + "</td>");

            foreach (DataRow po in dtPO.Rows)
            {
                string poCode = po["PoCode"].ToString();
                string key = coId + "_" + poCode;
                int currentVal = savedMap.ContainsKey(key) ? savedMap[key] : 0;

                if (currentVal > 0)
                {
                    sumPo[poCode] += currentVal;
                    countPo[poCode]++;
                    sb.Append("<td><span class='level-chip chip-" + currentVal + "'>" + currentVal + "</span></td>");
                }
                else
                {
                    sb.Append("<td><span class='level-chip chip-0'>-</span></td>");
                }
            }
            sb.Append("</tr>");
        }

        // Average Footer
        sb.Append("<tr class='avg-row'><td colspan='2' style='text-align:right; padding-right:12px;'><b>Average:</b></td>");
        foreach (DataRow po in dtPO.Rows)
        {
            string poCode = po["PoCode"].ToString();
            if (countPo[poCode] > 0)
            {
                double avg = sumPo[poCode] / countPo[poCode];
                sb.Append("<td>" + avg.ToString("0.00") + "</td>");
            }
            else
            {
                sb.Append("<td>-</td>");
            }
        }
        sb.Append("</tr></tbody></table>");

        litMatrixPreview.Text = sb.ToString();
    }

    protected void btnApprove_Click(object sender, EventArgs e)
    {
        ProcessAction("Approved");
    }

    protected void btnReject_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtDeanRemarks.Text))
        {
            objfun.MsgBox("Reject karne ke liye Remarks / Reason likhna anivarya hai!", this);
            txtDeanRemarks.Focus();
            return;
        }

        ProcessAction("Rejected");
    }

    private void ProcessAction(string newStatus)
    {
        try
        {
            string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";
            string subjectId = hdnSelectedSubjectId.Value;
            string sesnId = hdnSelectedSesnId.Value;
            string deanId = Session["UID"].ToString();
            string remarks = txtDeanRemarks.Text.Trim().Replace("'", "''");

            string updateQ = "UPDATE tbl_CO_PO_Approval_Status SET " +
                             "ApprovalStatus = '" + newStatus + "', DeanRemarks = '" + remarks + "', " +
                             "ApprovedBy = '" + deanId + "', ApprovedDate = GETDATE() " +
                             "WHERE SubjectId = '" + subjectId + "' AND AcademicYear = '" + sesnId + "' AND InstituteID = '" + instId + "'";
            objfun.ExecuteDelete(updateQ);

            objfun.MsgBox("CO-PO Mapping " + newStatus + " Successfully!", this);

            // Modal band karein aur pending list refresh karein
            pnlModal.Visible = false;
            LoadPendingSubmissions();
        }
        catch (Exception ex)
        {
            objfun.MsgBox("Error: " + ex.Message.Replace("'", " "), this);
        }
    }

    protected void btnCloseModal_Click(object sender, EventArgs e)
    {
        pnlModal.Visible = false;
    }
}