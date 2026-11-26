using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Co_Po_Mapping_Master_Co_Po_Mapping : System.Web.UI.Page
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
            FillCourse();
        }
    }

    private void FillCourse()
    {
        try
        {
            string facultyId = Session["UID"].ToString();
            string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";
            string sesnId = Session["SesnID"].ToString();

            string query = "SELECT DISTINCT c.CourseName, c.CourseId FROM Course c " +
                           "INNER JOIN FacultySubjectChoice fsc ON c.CourseId = fsc.CourseId " +
                           "WHERE fsc.FacultyId = '" + facultyId + "' AND fsc.InstituteID = '" + instId + "' AND fsc.SessionID = '" + sesnId + "' AND fsc.Approvalstatus = 'Approved'";

            objfun.FillDropdownlist(ddlCourse, "CourseName", "CourseId", query, "--Select Course--");
        }
        catch (Exception ex)
        {
            objfun.MsgBox(ex.Message, this);
        }
    }

    protected void ddlCourse_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            string facultyId = Session["UID"].ToString();
            string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";
            string sesnId = Session["SesnID"].ToString();

            string query = "SELECT CAST(Year AS NVARCHAR(10)) + '-' + CourseYear AS yr, sid FROM Semester_View " +
                           "WHERE CourseId = '" + ddlCourse.SelectedValue + "' AND inst_id = '" + instId + "' AND sessionyear = '" + sesnId + "' AND Active = 'true' " +
                           "AND sid IN (SELECT DISTINCT SemesterID FROM FacultySubjectChoice WHERE FacultyId = '" + facultyId + "' AND CourseId = '" + ddlCourse.SelectedValue + "' AND InstituteID = '" + instId + "' AND SessionID = '" + sesnId + "' AND Approvalstatus = 'Approved') " +
                           "ORDER BY Year";

            objfun.FillDropdownlist(Ddl_Semes, "yr", "sid", query, "--Select Semester--");
            Ddl_Specl.Items.Clear();
            ddlSubject.Items.Clear();
            pnlMatrix.Visible = false;
            pnlNotApproved.Visible = false;
        }
        catch (Exception ex)
        {
            objfun.MsgBox(ex.Message, this);
        }
    }

    protected void Ddl_Semes_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            string facultyId = Session["UID"].ToString();
            string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";
            string sesnId = Session["SesnID"].ToString();

            string query = "SELECT shortName, SpecilisationID FROM dbo.Specilisation WHERE InstituteID = '" + instId + "' " +
                           "AND SpecilisationID IN (SELECT DISTINCT SpecilizationID FROM FacultySubjectChoice WHERE FacultyId = '" + facultyId + "' AND CourseId = '" + ddlCourse.SelectedValue + "' AND SemesterID = '" + Ddl_Semes.SelectedValue + "' AND InstituteID = '" + instId + "' AND SessionID = '" + sesnId + "' AND Approvalstatus = 'Approved')";

            objfun.FillDropdownlist(Ddl_Specl, "shortName", "SpecilisationID", query, "--Select Specialization--");
            ddlSubject.Items.Clear();
            pnlMatrix.Visible = false;
            pnlNotApproved.Visible = false;
        }
        catch (Exception ex)
        {
            objfun.MsgBox(ex.Message, this);
        }
    }

    protected void Ddl_Specl_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            string facultyId = Session["UID"].ToString();
            string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";
            string sesnId = Session["SesnID"].ToString();

            string query = "SELECT DISTINCT CASE WHEN SubjectCode = '' THEN SubjectName ELSE SubjectName + ' ( ' + SubjectCode + ' )' END AS SubjectName, SubjectId " +
                           "FROM subject_vw WHERE InstituteID = '" + instId + "' AND CourseId = '" + ddlCourse.SelectedValue + "' AND SemesterID = '" + Ddl_Semes.SelectedValue + "' AND SpecilizationID = '" + Ddl_Specl.SelectedValue + "' AND SessionID = '" + sesnId + "' AND Active = 'true' " +
                           "AND SubjectId IN (SELECT SubjectId FROM FacultySubjectChoice WHERE FacultyId = '" + facultyId + "' AND SemesterID = '" + Ddl_Semes.SelectedValue + "' AND SpecilizationID = '" + Ddl_Specl.SelectedValue + "' AND InstituteID = '" + instId + "' AND SessionID = '" + sesnId + "' AND Approvalstatus = 'Approved')";

            objfun.FillDropdownlist(ddlSubject, "SubjectName", "SubjectId", query, "--Select Subject--");
            pnlMatrix.Visible = false;
            pnlNotApproved.Visible = false;
        }
        catch (Exception ex)
        {
            objfun.MsgBox(ex.Message, this);
        }
    }

    protected void ddlSubject_SelectedIndexChanged(object sender, EventArgs e)
    {
        CheckApprovalAndBuildMatrix();
    }

    private void CheckApprovalAndBuildMatrix()
    {
        if (ddlSubject.SelectedIndex <= 0)
        {
            pnlMatrix.Visible = false;
            pnlNotApproved.Visible = false;
            return;
        }

        string sesnId = Session["SesnID"].ToString();
        string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";
        string courseId = ddlCourse.SelectedValue;
        string subjectId = ddlSubject.SelectedValue;

        // 1. Check Course Outcomes (CO) Approval Status
        string checkApprovalQuery = "SELECT ApprovalStatus FROM tbl_CO_Approval_Status " +
                                    "WHERE CourseId = '" + courseId + "' " +
                                    "AND SemesterId = '" + Ddl_Semes.SelectedValue + "' " +
                                    "AND SpecializationId = '" + Ddl_Specl.SelectedValue + "' " +
                                    "AND SubjectId = '" + subjectId + "' " +
                                    "AND AcademicYear = '" + sesnId + "'";
        string status = objfun.Get_details(checkApprovalQuery);

        if (status != "Approved")
        {
            lblNotApprovedMsg.Text = "Is subject ke Course Outcomes abhi HOD/Admin dwara APPROVE nahi hue hain. Approval ke baad hi mapping shuru ki ja sakti hai.";
            pnlNotApproved.Visible = true;
            pnlMatrix.Visible = false;
            return;
        }

        // 2. Strict PO Check: Admin dwara POs define hue hain ya nahi
        string checkPoQuery = "SELECT COUNT(*) FROM tbl_PO_Master WHERE CourseId = '" + courseId + "' AND InstituteID = '" + instId + "'";
        int poCount = 0;
        int.TryParse(objfun.Get_details(checkPoQuery), out poCount);

        if (poCount == 0)
        {
            lblNotApprovedMsg.Text = "Is Course ke liye Admin dwara Program Outcomes (PO) define nahi kiye gaye hain. Pehle PO Master se PO define karein.";
            pnlNotApproved.Visible = true;
            pnlMatrix.Visible = false;
            return;
        }

        // 3. Strict Level Check: Dean dwara Mapping Levels define hue hain ya nahi
        string checkLevelQuery = "SELECT COUNT(*) FROM tbl_Mapping_Level_Master WHERE CourseId = '" + courseId + "' AND InstituteID = '" + instId + "'";
        int levelCount = 0;
        int.TryParse(objfun.Get_details(checkLevelQuery), out levelCount);

        if (levelCount == 0)
        {
            lblNotApprovedMsg.Text = "Is Course ke liye Dean dwara Mapping Levels define nahi kiye gaye hain. Pehle Mapping Level Master setup karein.";
            pnlNotApproved.Visible = true;
            pnlMatrix.Visible = false;
            return;
        }

        pnlNotApproved.Visible = false;
        pnlMatrix.Visible = true;

        // 4. Check Current CO-PO Approval Status
        string appQ = "SELECT ApprovalStatus, ISNULL(DeanRemarks, '') AS DeanRemarks FROM tbl_CO_PO_Approval_Status " +
                      "WHERE SubjectId = '" + subjectId + "' AND AcademicYear = '" + sesnId + "' AND InstituteID = '" + instId + "'";
        DataTable dtApp = objfun.FillDataTable(appQ);

        string currentStatus = "Draft";
        string deanRemarks = "";
        if (dtApp != null && dtApp.Rows.Count > 0)
        {
            currentStatus = dtApp.Rows[0]["ApprovalStatus"].ToString();
            deanRemarks = dtApp.Rows[0]["DeanRemarks"].ToString();
        }

        bool isLocked = false;

        if (currentStatus == "Approved")
        {
            litStatusBadge.Text = "<span class='status-badge badge-approved'><i class='fa-solid fa-circle-check'></i> Approved (Locked)</span>";
            pnlDeanRemarks.Visible = false;
            btnSaveMapping.Visible = false;
            btnResetMapping.Visible = false;
            btnSubmitForApproval.Visible = false;
            isLocked = true;
        }
        else if (currentStatus == "Submitted")
        {
            litStatusBadge.Text = "<span class='status-badge badge-submitted'><i class='fa-solid fa-clock'></i> Submitted to Dean (Pending Review)</span>";
            pnlDeanRemarks.Visible = false;
            btnSaveMapping.Visible = false;
            btnResetMapping.Visible = false;
            btnSubmitForApproval.Visible = false;
            isLocked = true;
        }
        else if (currentStatus == "Rejected")
        {
            litStatusBadge.Text = "<span class='status-badge badge-rejected'><i class='fa-solid fa-circle-xmark'></i> Rejected by Dean</span>";
            lblDeanRemarksText.Text = deanRemarks;
            pnlDeanRemarks.Visible = true;
            btnSaveMapping.Visible = true;
            btnResetMapping.Visible = true;
            btnSubmitForApproval.Visible = true;
            isLocked = false;
        }
        else // Draft
        {
            litStatusBadge.Text = "<span class='status-badge badge-draft'><i class='fa-solid fa-pencil'></i> Draft (In Progress)</span>";
            pnlDeanRemarks.Visible = false;
            btnSaveMapping.Visible = true;
            btnResetMapping.Visible = true;
            btnSubmitForApproval.Visible = true;
            isLocked = false;
        }

        BuildMatrixTable(isLocked);
    }

    private void BuildMatrixTable(bool isLocked)
    {
        string sesnId = Session["SesnID"].ToString();
        string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";
        string courseId = ddlCourse.SelectedValue;

        // 1. Subject ke approved Course Outcomes fetch karein
        string coQuery = "SELECT CoId, CoCode, CoStatement FROM tbl_Course_Outcomes " +
                         "WHERE CourseId = '" + courseId + "' " +
                         "AND SemesterId = '" + Ddl_Semes.SelectedValue + "' " +
                         "AND SpecializationId = '" + Ddl_Specl.SelectedValue + "' " +
                         "AND SubjectId = '" + ddlSubject.SelectedValue + "' " +
                         "AND AcademicYear = '" + sesnId + "' ORDER BY CAST(SUBSTRING(CoCode, 3, LEN(CoCode)) AS INT) ASC";
        DataTable dtCO = objfun.FillDataTable(coQuery);

        if (dtCO == null || dtCO.Rows.Count == 0)
        {
            litMatrix.Text = "<p style='color:red; padding:10px;'>Is subject ke liye koi approved COs nahi mile.</p>";
            btnSaveMapping.Visible = false;
            btnResetMapping.Visible = false;
            btnSubmitForApproval.Visible = false;
            return;
        }

        // 2. POs aur unke Statements fetch karein
        string poQuery = "SELECT PoCode, PoStatement FROM tbl_PO_Master " +
                         "WHERE CourseId = '" + courseId + "' AND InstituteID = '" + instId + "' " +
                         "ORDER BY CAST(SUBSTRING(PoCode, 3, LEN(PoCode)) AS INT) ASC";
        DataTable dtPO = objfun.FillDataTable(poQuery);

        if (dtPO == null || dtPO.Rows.Count == 0)
        {
            lblNotApprovedMsg.Text = "Is Course ke liye Admin dwara Program Outcomes (PO) define nahi kiye gaye hain.";
            pnlNotApproved.Visible = true;
            pnlMatrix.Visible = false;
            return;
        }

        // PO Guide List populate karein
        StringBuilder sbGuide = new StringBuilder();
        foreach (DataRow po in dtPO.Rows)
        {
            sbGuide.Append("<div class='po-guide-item'>");
            sbGuide.Append("<b>" + po["PoCode"].ToString() + ":</b> ");
            sbGuide.Append(Server.HtmlEncode(po["PoStatement"].ToString()));
            sbGuide.Append("</div>");
        }
        litPoGuideList.Text = sbGuide.ToString();

        // 3. Pehle se saved mapping data fetch karein
        string mapQuery = "SELECT CoId, PoCode, CorrelationLevel FROM tbl_CO_PO_Mapping " +
                          "WHERE SubjectId = '" + ddlSubject.SelectedValue + "' AND AcademicYear = '" + sesnId + "'";
        DataTable dtSaved = objfun.FillDataTable(mapQuery);

        Dictionary<string, int> savedMap = new Dictionary<string, int>();
        if (dtSaved != null)
        {
            foreach (DataRow r in dtSaved.Rows)
            {
                string key = r["CoId"].ToString() + "_" + r["PoCode"].ToString();
                savedMap[key] = Convert.ToInt32(r["CorrelationLevel"]);
            }
        }

        // 4. Matrix Table generate karein
        StringBuilder sb = new StringBuilder();
        sb.Append("<table class='matrix-table'>");

        // Header Row (with tooltips)
        sb.Append("<thead><tr>");
        sb.Append("<th style='width: 80px;'>CO Code</th>");
        sb.Append("<th class='co-stmt-col'>Course Outcome Statement</th>");
        foreach (DataRow po in dtPO.Rows)
        {
            string pCode = po["PoCode"].ToString();
            string pStmt = Server.HtmlEncode(po["PoStatement"].ToString());
            sb.Append("<th class='po-header-cell' title='" + pCode + ": " + pStmt + "'>");
            sb.Append(pCode + " <i class='fa-solid fa-circle-question' style='font-size:11px; opacity:0.7;'></i>");
            sb.Append("</th>");
        }
        sb.Append("</tr></thead><tbody>");

        Dictionary<string, double> sumPo = new Dictionary<string, double>();
        Dictionary<string, int> countPo = new Dictionary<string, int>();
        foreach (DataRow po in dtPO.Rows)
        {
            sumPo[po["PoCode"].ToString()] = 0;
            countPo[po["PoCode"].ToString()] = 0;
        }

        // CO Data Rows
        foreach (DataRow co in dtCO.Rows)
        {
            string coId = co["CoId"].ToString();
            string coCode = co["CoCode"].ToString();
            string coStmt = co["CoStatement"].ToString();

            sb.Append("<tr>");
            sb.Append("<td><b>" + coCode + "</b></td>");
            sb.Append("<td class='co-stmt-col'>" + coStmt + "</td>");

            foreach (DataRow po in dtPO.Rows)
            {
                string poCode = po["PoCode"].ToString();
                string key = coId + "_" + poCode;
                int currentVal = savedMap.ContainsKey(key) ? savedMap[key] : 0;

                if (currentVal > 0)
                {
                    sumPo[poCode] += currentVal;
                    countPo[poCode]++;
                }

                sb.Append("<td>");
                if (isLocked)
                {
                    // Locked mode (Approved ya Submitted) me dropdown ki jagah text label dikhega
                    sb.Append("<b>" + (currentVal > 0 ? currentVal.ToString() : "-") + "</b>");
                }
                else
                {
                    sb.Append("<select name='map_" + coId + "_" + poCode + "' class='ddl-level'>");
                    sb.Append("<option value='0'" + (currentVal == 0 ? " selected" : "") + ">-</option>");
                    sb.Append("<option value='1'" + (currentVal == 1 ? " selected" : "") + ">1</option>");
                    sb.Append("<option value='2'" + (currentVal == 2 ? " selected" : "") + ">2</option>");
                    sb.Append("<option value='3'" + (currentVal == 3 ? " selected" : "") + ">3</option>");
                    sb.Append("</select>");
                }
                sb.Append("</td>");
            }

            sb.Append("</tr>");
        }

        // Average Footer Row
        sb.Append("<tr class='avg-row'>");
        sb.Append("<td colspan='2' style='text-align: right; padding-right: 15px;'><b>Average Correlation:</b></td>");
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
        sb.Append("</tr>");

        sb.Append("</tbody></table>");
        litMatrix.Text = sb.ToString();
    }

    private void SaveMappingInternal()
    {
        string courseId = ddlCourse.SelectedValue;
        string semesterId = Ddl_Semes.SelectedValue;
        string specId = Ddl_Specl.SelectedValue;
        string subjectId = ddlSubject.SelectedValue;
        string sesnId = Session["SesnID"].ToString();
        string facultyId = Session["UID"].ToString();
        string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";

        foreach (string formKey in Request.Form.AllKeys)
        {
            if (formKey != null && formKey.StartsWith("map_"))
            {
                string[] parts = formKey.Split('_');
                if (parts.Length == 3)
                {
                    string coId = parts[1];
                    string poCode = parts[2];
                    int level = Convert.ToInt32(Request.Form[formKey]);

                    string checkQ = "SELECT COUNT(*) FROM tbl_CO_PO_Mapping " +
                                    "WHERE SubjectId = '" + subjectId + "' AND AcademicYear = '" + sesnId + "' " +
                                    "AND CoId = '" + coId + "' AND PoCode = '" + poCode + "'";
                    int count = Convert.ToInt32(objfun.Get_details(checkQ));

                    if (count > 0)
                    {
                        string updateQ = "UPDATE tbl_CO_PO_Mapping SET CorrelationLevel = '" + level + "', FacultyId = '" + facultyId + "' " +
                                         "WHERE SubjectId = '" + subjectId + "' AND AcademicYear = '" + sesnId + "' " +
                                         "AND CoId = '" + coId + "' AND PoCode = '" + poCode + "'";
                        objfun.ExecuteDelete(updateQ);
                    }
                    else
                    {
                        string insertQ = "INSERT INTO tbl_CO_PO_Mapping (CourseId, SemesterId, SpecializationId, SubjectId, AcademicYear, CoId, PoCode, CorrelationLevel, FacultyId) " +
                                         "VALUES ('" + courseId + "', '" + semesterId + "', '" + specId + "', '" + subjectId + "', '" + sesnId + "', '" + coId + "', '" + poCode + "', '" + level + "', '" + facultyId + "')";
                        objfun.ExecuteDelete(insertQ);
                    }
                }
            }
        }

        // Agar approval record nahi hai toh 'Draft' create karein
        string appCheckQ = "SELECT COUNT(*) FROM tbl_CO_PO_Approval_Status WHERE SubjectId = '" + subjectId + "' AND AcademicYear = '" + sesnId + "' AND InstituteID = '" + instId + "'";
        int appCount = Convert.ToInt32(objfun.Get_details(appCheckQ));
        if (appCount == 0)
        {
            string insertAppQ = "INSERT INTO tbl_CO_PO_Approval_Status (InstituteID, CourseId, SemesterId, SpecializationId, SubjectId, AcademicYear, FacultyId, ApprovalStatus) " +
                                "VALUES ('" + instId + "', '" + courseId + "', '" + semesterId + "', '" + specId + "', '" + subjectId + "', '" + sesnId + "', '" + facultyId + "', 'Draft')";
            objfun.ExecuteDelete(insertAppQ);
        }
    }

    protected void btnSaveMapping_Click(object sender, EventArgs e)
    {
        try
        {
            SaveMappingInternal();
            objfun.MsgBox("CO-PO Mapping Draft Saved Successfully!", this);
            CheckApprovalAndBuildMatrix();
        }
        catch (Exception ex)
        {
            objfun.MsgBox("Error: " + ex.Message.Replace("'", " "), this);
        }
    }

    protected void btnSubmitForApproval_Click(object sender, EventArgs e)
    {
        try
        {
            SaveMappingInternal();

            string subjectId = ddlSubject.SelectedValue;
            string sesnId = Session["SesnID"].ToString();
            string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";
            string facultyId = Session["UID"].ToString();

            string updateAppQ = "UPDATE tbl_CO_PO_Approval_Status SET ApprovalStatus = 'Submitted', FacultyId = '" + facultyId + "', SubmittedDate = GETDATE() " +
                                "WHERE SubjectId = '" + subjectId + "' AND AcademicYear = '" + sesnId + "' AND InstituteID = '" + instId + "'";
            objfun.ExecuteDelete(updateAppQ);

            objfun.MsgBox("CO-PO Mapping has been SUBMITTED to Dean for approval!", this);
            CheckApprovalAndBuildMatrix(); // Page lock ho jayega
        }
        catch (Exception ex)
        {
            objfun.MsgBox("Error: " + ex.Message.Replace("'", " "), this);
        }
    }

    protected void btnResetMapping_Click(object sender, EventArgs e)
    {
        try
        {
            string subjectId = ddlSubject.SelectedValue;
            string sesnId = Session["SesnID"].ToString();

            string deleteQuery = "DELETE FROM tbl_CO_PO_Mapping WHERE SubjectId = '" + subjectId + "' AND AcademicYear = '" + sesnId + "'";
            objfun.ExecuteDelete(deleteQuery);

            objfun.MsgBox("Mapping reset/cleared successfully!", this);
            CheckApprovalAndBuildMatrix();
        }
        catch (Exception ex)
        {
            objfun.MsgBox("Error: " + ex.Message.Replace("'", " "), this);
        }
    }
}