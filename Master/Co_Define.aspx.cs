using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Co_Po_Mapping_Master_Co_Define : System.Web.UI.Page
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
            ResetForm();
            btnSubmitApproval.Visible = false;
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
            ResetForm();
            ClearGrid();
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
            ResetForm();
            ClearGrid();
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
            ResetForm();
            ClearGrid();
        }
        catch (Exception ex)
        {
            objfun.MsgBox(ex.Message, this);
        }
    }

    protected void ddlSubject_SelectedIndexChanged(object sender, EventArgs e)
    {
        ResetForm();
        BindCoGrid();
        CheckApprovalStatusAndLock();
    }

    private void BindCoGrid()
    {
        if (ddlSubject.SelectedIndex > 0)
        {
            string sesnId = Session["SesnID"].ToString();
            string query = "SELECT CoId, CoCode, CoStatement FROM tbl_Course_Outcomes " +
                           "WHERE CourseId = '" + ddlCourse.SelectedValue + "' " +
                           "AND SemesterId = '" + Ddl_Semes.SelectedValue + "' " +
                           "AND SpecializationId = '" + Ddl_Specl.SelectedValue + "' " +
                           "AND SubjectId = '" + ddlSubject.SelectedValue + "' " +
                           "AND AcademicYear = '" + sesnId + "' ORDER BY CAST(SUBSTRING(CoCode, 3, LEN(CoCode)) AS INT) ASC";

            DataTable dt = objfun.FillDataTable(query);
            gvCourseOutcomes.DataSource = dt;
            gvCourseOutcomes.DataBind();

            btnSubmitApproval.Visible = (dt != null && dt.Rows.Count > 0);
        }
        else
        {
            ClearGrid();
            btnSubmitApproval.Visible = false;
        }
    }

    private void ClearGrid()
    {
        gvCourseOutcomes.DataSource = null;
        gvCourseOutcomes.DataBind();
        pnlApprovalStatus.Visible = false;
    }

    private void CheckApprovalStatusAndLock()
    {
        if (ddlSubject.SelectedIndex <= 0)
        {
            pnlApprovalStatus.Visible = false;
            return;
        }

        string sesnId = Session["SesnID"].ToString();

        string query = "SELECT ApprovalStatus, ISNULL(Remarks, '') AS Remarks FROM tbl_CO_Approval_Status " +
                       "WHERE CourseId = '" + ddlCourse.SelectedValue + "' " +
                       "AND SemesterId = '" + Ddl_Semes.SelectedValue + "' " +
                       "AND SpecializationId = '" + Ddl_Specl.SelectedValue + "' " +
                       "AND SubjectId = '" + ddlSubject.SelectedValue + "' " +
                       "AND AcademicYear = '" + sesnId + "'";

        DataTable dt = objfun.FillDataTable(query);

        string status = "Draft";
        string remarks = "";

        if (dt != null && dt.Rows.Count > 0)
        {
            status = dt.Rows[0]["ApprovalStatus"].ToString();
            remarks = dt.Rows[0]["Remarks"].ToString().Trim();
        }

        pnlApprovalStatus.Visible = true;

        if (status == "Submitted")
        {
            divStatusBox.Attributes["class"] = "status-box status-submitted";
            iconStatus.Attributes["class"] = "fa-solid fa-clock";
            lblStatusMessage.Text = " COs are submitted for Approval. Editing and Adding are LOCKED.";
            lblRemarks.Text = "";

            LockControls(true);
            btnSubmitApproval.Enabled = false;
            btnSubmitApproval.Text = "<i class='fa-solid fa-clock'></i> Pending Approval";
        }
        else if (status == "Approved")
        {
            divStatusBox.Attributes["class"] = "status-box status-approved";
            iconStatus.Attributes["class"] = "fa-solid fa-circle-check";
            lblStatusMessage.Text = " Course Outcomes are APPROVED!";
            lblRemarks.Text = ""; // Purana remark clear kar diya

            LockControls(true);
            btnSubmitApproval.Enabled = false;
            btnSubmitApproval.Text = "<i class='fa-solid fa-circle-check'></i> Approved";
        }
        else if (status == "Rejected")
        {
            divStatusBox.Attributes["class"] = "status-box status-rejected";
            iconStatus.Attributes["class"] = "fa-solid fa-circle-xmark";
            lblStatusMessage.Text = " Submission was REJECTED. Please revise the COs and submit again.";
            lblRemarks.Text = !string.IsNullOrEmpty(remarks) ? "Reason: " + remarks : ""; // Sirf reject par reason display hoga

            LockControls(false);
            LoadNextAvailableCO();
            btnSubmitApproval.Enabled = true;
            btnSubmitApproval.Text = "<i class='fa-solid fa-paper-plane'></i> Re-Submit for Approval";
        }
        else // Draft
        {
            divStatusBox.Attributes["class"] = "status-box status-draft";
            iconStatus.Attributes["class"] = "fa-solid fa-file-pen";
            lblStatusMessage.Text = " Draft Mode: You can define and modify COs before submitting.";
            lblRemarks.Text = "";

            LockControls(false);
            LoadNextAvailableCO();
            btnSubmitApproval.Enabled = true;
            btnSubmitApproval.Text = "<i class='fa-solid fa-paper-plane'></i> Submit for Approval";
        }
    }

    private void LockControls(bool lockState)
    {
        txtCoStatement.Enabled = !lockState;
        btnAddCo.Enabled = !lockState;
        ddlCoIdentifier.Enabled = false;

        foreach (GridViewRow row in gvCourseOutcomes.Rows)
        {
            LinkButton btnEdit = (LinkButton)row.FindControl("btnEdit");
            LinkButton btnDelete = (LinkButton)row.FindControl("btnDelete");

            if (btnEdit != null)
            {
                btnEdit.Enabled = !lockState;
                btnEdit.Style.Add("opacity", lockState ? "0.4" : "1");
            }
            if (btnDelete != null)
            {
                btnDelete.Enabled = !lockState;
                btnDelete.Style.Add("opacity", lockState ? "0.4" : "1");
            }
        }
    }

    private void LoadNextAvailableCO()
    {
        ddlCoIdentifier.Items.Clear();

        if (ddlSubject.SelectedIndex <= 0)
        {
            ddlCoIdentifier.Items.Insert(0, new ListItem("--Select Subject First--", ""));
            return;
        }

        string sesnId = Session["SesnID"].ToString();

        string countQuery = "SELECT COUNT(*) FROM tbl_Course_Outcomes WHERE CourseId = '" + ddlCourse.SelectedValue + "' " +
                            "AND SemesterId = '" + Ddl_Semes.SelectedValue + "' " +
                            "AND SpecializationId = '" + Ddl_Specl.SelectedValue + "' " +
                            "AND SubjectId = '" + ddlSubject.SelectedValue + "' " +
                            "AND AcademicYear = '" + sesnId + "'";
        string existingCountStr = objfun.Get_details(countQuery);
        int existingCount = 0;
        int.TryParse(existingCountStr, out existingCount);

        int nextCoIndex = existingCount + 1;

        if (nextCoIndex > 12)
        {
            ddlCoIdentifier.Items.Insert(0, new ListItem("All COs Defined (Max 12)", ""));
            ddlCoIdentifier.Enabled = false;
            btnAddCo.Enabled = false;
        }
        else
        {
            string nextCoCode = "CO" + nextCoIndex;
            ddlCoIdentifier.Items.Insert(0, new ListItem(nextCoCode, nextCoCode));
            ddlCoIdentifier.Enabled = false;
            btnAddCo.Enabled = true;
        }
    }

    protected void btnAddCo_Click(object sender, EventArgs e)
    {
        if (ddlCourse.SelectedIndex <= 0 || Ddl_Semes.SelectedIndex <= 0 || Ddl_Specl.SelectedIndex <= 0 || ddlSubject.SelectedIndex <= 0)
        {
            objfun.MsgBox("Please select Course, Semester, Specialization, and Subject first!", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(ddlCoIdentifier.SelectedValue))
        {
            objfun.MsgBox("Please Select a valid CO Identifier!", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(txtCoStatement.Text))
        {
            objfun.MsgBox("Please Enter CO Statement!", this);
            txtCoStatement.Focus();
            return;
        }

        try
        {
            string selectedCo = ddlCoIdentifier.SelectedValue.Trim();
            string statement = txtCoStatement.Text.Trim().Replace("'", "''");
            string courseId = ddlCourse.SelectedValue;
            string semesterId = Ddl_Semes.SelectedValue;
            string specId = Ddl_Specl.SelectedValue;
            string subjectId = ddlSubject.SelectedValue;
            string sesnId = Session["SesnID"].ToString();
            string facultyId = Session["UID"].ToString();
            int editCoId = Convert.ToInt32(hdnEditCoId.Value);

            if (editCoId == 0)
            {
                string checkQuery = "SELECT COUNT(*) FROM tbl_Course_Outcomes WHERE CourseId = '" + courseId + "' " +
                                    "AND SemesterId = '" + semesterId + "' AND SpecializationId = '" + specId + "' " +
                                    "AND SubjectId = '" + subjectId + "' AND AcademicYear = '" + sesnId + "' AND CoCode = '" + selectedCo + "'";
                string countStr = objfun.Get_details(checkQuery);
                int count = 0;
                int.TryParse(countStr, out count);

                if (count > 0)
                {
                    objfun.MsgBox(selectedCo + " is already defined for this subject!", this);
                    return;
                }

                string insertQuery = "INSERT INTO tbl_Course_Outcomes (CourseId, SemesterId, SpecializationId, SubjectId, AcademicYear, CoCode, CoStatement, FacultyId, CreatedDate) " +
                                     "VALUES ('" + courseId + "', '" + semesterId + "', '" + specId + "', '" + subjectId + "', '" + sesnId + "', '" + selectedCo + "', '" + statement + "', '" + facultyId + "', GETDATE())";

                objfun.ExecuteDelete(insertQuery);
                objfun.MsgBox("CO Added Successfully!", this);
            }
            else
            {
                string updateQuery = "UPDATE tbl_Course_Outcomes SET CoStatement = '" + statement + "' WHERE CoId = '" + editCoId + "'";
                objfun.ExecuteDelete(updateQuery);
                objfun.MsgBox("CO Updated Successfully!", this);
            }

            ResetForm();
            BindCoGrid();
            CheckApprovalStatusAndLock();
        }
        catch (Exception ex)
        {
            objfun.MsgBox("Error: " + ex.Message.Replace("'", " "), this);
        }
    }

    protected void btnSubmitApproval_Click(object sender, EventArgs e)
    {
        if (gvCourseOutcomes.Rows.Count == 0)
        {
            objfun.MsgBox("Please define at least one CO before submitting for approval!", this);
            return;
        }

        try
        {
            string courseId = ddlCourse.SelectedValue;
            string semesterId = Ddl_Semes.SelectedValue;
            string specId = Ddl_Specl.SelectedValue;
            string subjectId = ddlSubject.SelectedValue;
            string sesnId = Session["SesnID"].ToString();
            string facultyId = Session["UID"].ToString();

            string checkQuery = "SELECT COUNT(*) FROM tbl_CO_Approval_Status WHERE CourseId = '" + courseId + "' " +
                                "AND SemesterId = '" + semesterId + "' AND SpecializationId = '" + specId + "' " +
                                "AND SubjectId = '" + subjectId + "' AND AcademicYear = '" + sesnId + "'";
            int exists = Convert.ToInt32(objfun.Get_details(checkQuery));

            if (exists == 0)
            {
                string insertStatus = "INSERT INTO tbl_CO_Approval_Status (CourseId, SemesterId, SpecializationId, SubjectId, AcademicYear, FacultyId, ApprovalStatus, SubmittedDate) " +
                                      "VALUES ('" + courseId + "', '" + semesterId + "', '" + specId + "', '" + subjectId + "', '" + sesnId + "', '" + facultyId + "', 'Submitted', GETDATE())";
                objfun.ExecuteDelete(insertStatus);
            }
            else
            {
                string updateStatus = "UPDATE tbl_CO_Approval_Status SET ApprovalStatus = 'Submitted', SubmittedDate = GETDATE(), FacultyId = '" + facultyId + "' " +
                                      "WHERE CourseId = '" + courseId + "' AND SemesterId = '" + semesterId + "' AND SpecializationId = '" + specId + "' " +
                                      "AND SubjectId = '" + subjectId + "' AND AcademicYear = '" + sesnId + "'";
                objfun.ExecuteDelete(updateStatus);
            }

            objfun.MsgBox("Course Outcomes submitted for Approval successfully! All actions are now locked.", this);
            CheckApprovalStatusAndLock();
        }
        catch (Exception ex)
        {
            objfun.MsgBox("Error: " + ex.Message.Replace("'", " "), this);
        }
    }

    protected void gvCourseOutcomes_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string coId = e.CommandArgument.ToString();

        if (e.CommandName == "EditRow")
        {
            try
            {
                string query = "SELECT CoId, CoCode, CoStatement FROM tbl_Course_Outcomes WHERE CoId = '" + coId + "'";
                DataTable dt = objfun.FillDataTable(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    hdnEditCoId.Value = dt.Rows[0]["CoId"].ToString();

                    ddlCoIdentifier.Items.Clear();
                    string coCode = dt.Rows[0]["CoCode"].ToString();
                    ddlCoIdentifier.Items.Add(new ListItem(coCode, coCode));
                    ddlCoIdentifier.SelectedValue = coCode;
                    ddlCoIdentifier.Enabled = false;

                    txtCoStatement.Text = dt.Rows[0]["CoStatement"].ToString();

                    lblFormTitle.Text = "Update Course Outcome (" + coCode + ")";
                    btnAddCo.Text = "<i class='fa-solid fa-check'></i> Update CO";
                    btnCancelEdit.Visible = true;
                    txtCoStatement.Focus();
                }
            }
            catch (Exception ex)
            {
                objfun.MsgBox("Error: " + ex.Message.Replace("'", " "), this);
            }
        }
        else if (e.CommandName == "DeleteRow")
        {
            try
            {
                string currentCoCode = objfun.Get_details("SELECT CoCode FROM tbl_Course_Outcomes WHERE CoId = '" + coId + "'");
                string maxCoCode = objfun.Get_details("SELECT TOP 1 CoCode FROM tbl_Course_Outcomes WHERE CourseId = '" + ddlCourse.SelectedValue + "' " +
                                                      "AND SemesterId = '" + Ddl_Semes.SelectedValue + "' AND SpecializationId = '" + Ddl_Specl.SelectedValue + "' " +
                                                      "AND SubjectId = '" + ddlSubject.SelectedValue + "' AND AcademicYear = '" + Session["SesnID"].ToString() + "' ORDER BY CoId DESC");

                if (currentCoCode != maxCoCode)
                {
                    objfun.MsgBox("You can only delete the last defined CO (" + maxCoCode + ") to maintain sequence!", this);
                    return;
                }

                objfun.ExecuteDML("DELETE FROM tbl_Course_Outcomes WHERE CoId = '" + coId + "'");
                objfun.MsgBox("Deleted Successfully!", this);

                ResetForm();
                BindCoGrid();
                CheckApprovalStatusAndLock();
            }
            catch (Exception ex)
            {
                objfun.MsgBox("Error: " + ex.Message.Replace("'", " "), this);
            }
        }
    }

    protected void btnCancelEdit_Click(object sender, EventArgs e)
    {
        ResetForm();
        LoadNextAvailableCO();
    }

    private void ResetForm()
    {
        hdnEditCoId.Value = "0";
        txtCoStatement.Text = "";
        lblFormTitle.Text = "Add Course Outcome";
        btnAddCo.Text = "<i class='fa-solid fa-circle-plus'></i> Add CO";
        btnAddCo.Enabled = true;
        btnCancelEdit.Visible = false;
    }
}