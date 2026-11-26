using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Co_Po_Mapping_Master_Co_Approval : System.Web.UI.Page
{
    DbFunctions objfun = new DbFunctions();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UID"] == null)
        {
            Response.Redirect("../error_404_2.html");
            return;
        }

        if (!IsPostBack)
        {
            BindApprovalGrid();
        }
    }

    private void BindApprovalGrid()
    {
        try
        {
            string sesnId = Session["SesnID"] != null ? Session["SesnID"].ToString() : "";
            string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";

            // Yahan har master/view table ko distinct karke join kiya gaya hai taaki duplicate rows na banen
            string query = @"SELECT 
                                app.ApprovalId,
                                app.CourseId,
                                app.SemesterId,
                                app.SpecializationId,
                                app.SubjectId,
                                app.FacultyId,
                                app.ApprovalStatus,
                                ISNULL(app.Remarks, '') AS Remarks,
                                ISNULL(emp.empName + ' (' + emp.empCode + ')', app.FacultyId) AS FacultyInfo,
                                ISNULL(c.CourseName, 'Course') AS CourseName,
                                ISNULL(sem.SemesterName, 'Semester') AS SemesterName,
                                ISNULL(spec.shortName, 'All') AS SpecializationName,
                                ISNULL(sub.SubjectName, 'Subject') AS SubjectName,
                                STUFF((
                                    SELECT '<br/><b>' + co.CoCode + ':</b> ' + co.CoStatement
                                    FROM tbl_Course_Outcomes co
                                    WHERE co.CourseId = app.CourseId 
                                      AND co.SemesterId = app.SemesterId 
                                      AND co.SpecializationId = app.SpecializationId 
                                      AND co.SubjectId = app.SubjectId 
                                      AND co.AcademicYear = app.AcademicYear
                                    ORDER BY CAST(SUBSTRING(co.CoCode, 3, LEN(co.CoCode)) AS INT) ASC
                                    FOR XML PATH(''), TYPE
                                ).value('.', 'NVARCHAR(MAX)'), 1, 5, '') AS AllCOs
                            FROM tbl_CO_Approval_Status app
                            LEFT JOIN (SELECT DISTINCT empId, MAX(empName) AS empName, MAX(empCode) AS empCode FROM Employee_Master GROUP BY empId) emp ON app.FacultyId = CAST(emp.empId AS VARCHAR(50))
                            LEFT JOIN Course c ON app.CourseId = c.CourseId
                            LEFT JOIN (SELECT DISTINCT sid, MAX(CAST(Year AS NVARCHAR(10)) + '-' + CourseYear) AS SemesterName FROM Semester_View GROUP BY sid) sem ON app.SemesterId = sem.sid
                            LEFT JOIN dbo.Specilisation spec ON app.SpecializationId = spec.SpecilisationID
                            LEFT JOIN (SELECT DISTINCT SubjectId, MAX(SubjectName) AS SubjectName FROM subject_vw GROUP BY SubjectId) sub ON app.SubjectId = sub.SubjectId
                            WHERE ('" + sesnId + @"' = '' OR app.AcademicYear = '" + sesnId + @"' and app.ApprovalStatus='Submitted')
                            ORDER BY app.ApprovalId DESC";

            DataTable dt = objfun.FillDataTable(query);
            gvApprovalRequests.DataSource = dt;
            gvApprovalRequests.DataBind();
        }
        catch (Exception ex)
        {
            objfun.MsgBox("Error loading grid: " + ex.Message.Replace("'", " "), this);
        }
    }

    protected void gvApprovalRequests_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            string approvalId = e.CommandArgument.ToString();
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            TextBox txtGridRemark = (TextBox)row.FindControl("txtGridRemark");
            string remark = txtGridRemark != null ? txtGridRemark.Text.Trim().Replace("'", "''") : "";
            string adminId = Session["UID"] != null ? Session["UID"].ToString() : "";

            if (e.CommandName == "AcceptRow")
            {
                string updateQuery = "UPDATE tbl_CO_Approval_Status SET ApprovalStatus = 'Approved', " +
                                     "Remarks = '" + remark + "', ActionBy = '" + adminId + "', ActionDate = GETDATE() " +
                                     "WHERE ApprovalId = '" + approvalId + "'";

                objfun.ExecuteDelete(updateQuery);
                objfun.MsgBox("Course Outcomes Approved Successfully!", this);
                BindApprovalGrid();
            }
            else if (e.CommandName == "RejectRow")
            {
                if (string.IsNullOrWhiteSpace(remark))
                {
                    objfun.MsgBox("Please enter a remark explaining the rejection reason!", this);
                    if (txtGridRemark != null) txtGridRemark.Focus();
                    return;
                }

                string updateQuery = "UPDATE tbl_CO_Approval_Status SET ApprovalStatus = 'Rejected', " +
                                     "Remarks = '" + remark + "', ActionBy = '" + adminId + "', ActionDate = GETDATE() " +
                                     "WHERE ApprovalId = '" + approvalId + "'";

                objfun.ExecuteDelete(updateQuery);
                objfun.MsgBox("Course Outcomes Rejected!", this);
                BindApprovalGrid();
            }
        }
        catch (Exception ex)
        {
            objfun.MsgBox("Error: " + ex.Message.Replace("'", " "), this);
        }
    }
}