using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Co_Po_Mapping_Master_Po_Define : System.Web.UI.Page
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
            FillCourses();
            ResetForm();
        }
    }

    private void FillCourses()
    {
        try
        {
            string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";
            string query = "SELECT CourseId, CourseName FROM Course WHERE InstituteID = '" + instId + "' ORDER BY CourseName";
            objfun.FillDropdownlist(ddlCourse, "CourseName", "CourseId", query, "--Select Course--");
        }
        catch (Exception ex)
        {
            objfun.MsgBox(ex.Message, this);
        }
    }

    protected void ddlCourse_SelectedIndexChanged(object sender, EventArgs e)
    {
        ResetForm();
        BindPoGrid();
        LoadNextAvailablePO();
    }

    private void BindPoGrid()
    {
        if (ddlCourse.SelectedIndex > 0)
        {
            string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";

            // Natural order me PO1, PO2... PO12 sort karein
            string query = "SELECT PoId, PoCode, PoStatement FROM tbl_PO_Master " +
                           "WHERE CourseId = '" + ddlCourse.SelectedValue + "' AND InstituteID = '" + instId + "' " +
                           "ORDER BY CAST(SUBSTRING(PoCode, 3, LEN(PoCode)) AS INT) ASC";

            DataTable dt = objfun.FillDataTable(query);
            gvPoList.DataSource = dt;
            gvPoList.DataBind();
        }
        else
        {
            ClearGrid();
        }
    }

    private void ClearGrid()
    {
        gvPoList.DataSource = null;
        gvPoList.DataBind();
    }

    // Rule: PO1 ke bina PO2 na aaye - Sirf agla valid PO hi load karna
    private void LoadNextAvailablePO()
    {
        ddlPoCode.Items.Clear();

        if (ddlCourse.SelectedIndex <= 0)
        {
            ddlPoCode.Items.Insert(0, new ListItem("--Select Course First--", ""));
            ddlPoCode.Enabled = false;
            btnAddPo.Enabled = false;
            return;
        }

        string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";

        // Is course ke liye kitne POs pehle se ban chuke hain
        string countQuery = "SELECT COUNT(*) FROM tbl_PO_Master WHERE CourseId = '" + ddlCourse.SelectedValue + "' AND InstituteID = '" + instId + "'";
        string existingCountStr = objfun.Get_details(countQuery);
        int existingCount = 0;
        int.TryParse(existingCountStr, out existingCount);

        // Agla PO hamesha count + 1 hoga (e.g. 0 hai toh PO1, 1 hai toh PO2)
        int nextPoIndex = existingCount + 1;

        if (nextPoIndex > 12)
        {
            ddlPoCode.Items.Insert(0, new ListItem("All POs Defined (Max 12)", ""));
            ddlPoCode.Enabled = false;
            btnAddPo.Enabled = false;
        }
        else
        {
            string nextPoCode = "PO" + nextPoIndex;
            ddlPoCode.Items.Insert(0, new ListItem(nextPoCode, nextPoCode));
            ddlPoCode.Enabled = false; // Locked taaki user galat sequence na chun sake
            btnAddPo.Enabled = true;
        }
    }

    protected void btnAddPo_Click(object sender, EventArgs e)
    {
        if (ddlCourse.SelectedIndex <= 0)
        {
            objfun.MsgBox("Please select Course first!", this);
            ddlCourse.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(ddlPoCode.SelectedValue))
        {
            objfun.MsgBox("Please select a valid PO Identifier!", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(txtPoStatement.Text))
        {
            objfun.MsgBox("Please enter PO statement!", this);
            txtPoStatement.Focus();
            return;
        }

        try
        {
            string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";
            string courseId = ddlCourse.SelectedValue;
            string poCode = ddlPoCode.SelectedValue.Trim();
            string statement = txtPoStatement.Text.Trim().Replace("'", "''");
            string adminId = Session["UID"].ToString();
            int editPoId = Convert.ToInt32(hdnEditPoId.Value);

            if (editPoId == 0)
            {
                // Duplicate check
                string checkQuery = "SELECT COUNT(*) FROM tbl_PO_Master WHERE CourseId = '" + courseId + "' AND InstituteID = '" + instId + "' AND PoCode = '" + poCode + "'";
                int count = Convert.ToInt32(objfun.Get_details(checkQuery));

                if (count > 0)
                {
                    objfun.MsgBox(poCode + " is already defined for this Course!", this);
                    return;
                }

                string insertQuery = "INSERT INTO tbl_PO_Master (InstituteID, CourseId, PoCode, PoStatement, CreatedBy, CreatedDate) " +
                                     "VALUES ('" + instId + "', '" + courseId + "', '" + poCode + "', '" + statement + "', '" + adminId + "', GETDATE())";

                objfun.ExecuteDelete(insertQuery);
                objfun.MsgBox("PO Added Successfully!", this);
            }
            else
            {
                // Update statement
                string updateQuery = "UPDATE tbl_PO_Master SET PoStatement = '" + statement + "' WHERE PoId = '" + editPoId + "'";
                objfun.ExecuteDelete(updateQuery);
                objfun.MsgBox("PO Updated Successfully!", this);
            }

            ResetForm();
            BindPoGrid();
            LoadNextAvailablePO();
        }
        catch (Exception ex)
        {
            objfun.MsgBox("Error: " + ex.Message.Replace("'", " "), this);
        }
    }

    protected void gvPoList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string poId = e.CommandArgument.ToString();

        if (e.CommandName == "EditRow")
        {
            try
            {
                string query = "SELECT PoId, PoCode, PoStatement FROM tbl_PO_Master WHERE PoId = '" + poId + "'";
                DataTable dt = objfun.FillDataTable(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    hdnEditPoId.Value = dt.Rows[0]["PoId"].ToString();

                    ddlPoCode.Items.Clear();
                    string pCode = dt.Rows[0]["PoCode"].ToString();
                    ddlPoCode.Items.Add(new ListItem(pCode, pCode));
                    ddlPoCode.SelectedValue = pCode;
                    ddlPoCode.Enabled = false; // Edit ke dauran code change nahi hoga

                    txtPoStatement.Text = dt.Rows[0]["PoStatement"].ToString();

                    lblFormTitle.Text = "Update " + pCode;
                    btnAddPo.Text = "<i class='fa-solid fa-check'></i> Update PO";
                    btnAddPo.Enabled = true;
                    btnCancelEdit.Visible = true;
                    txtPoStatement.Focus();
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
                string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";
                string courseId = ddlCourse.SelectedValue;

                // 1. Sequence check: Kya yeh aakhiri PO hai?
                string currentPoCode = objfun.Get_details("SELECT PoCode FROM tbl_PO_Master WHERE PoId = '" + poId + "'");

                string maxPoCode = objfun.Get_details("SELECT TOP 1 PoCode FROM tbl_PO_Master WHERE CourseId = '" + courseId + "' AND InstituteID = '" + instId + "' " +
                                                      "ORDER BY CAST(SUBSTRING(PoCode, 3, LEN(PoCode)) AS INT) DESC");

                if (currentPoCode != maxPoCode)
                {
                    objfun.MsgBox("Sequence maintain rakhne ke liye aap sirf aakhiri defined PO (" + maxPoCode + ") ko hi delete kar sakte hain!", this);
                    return;
                }

                // 2. Safe mapping check (agar mapping table exist karti ho)
                string checkTableQuery = "SELECT COUNT(*) FROM sys.tables WHERE name = 'tbl_CO_PO_Mapping'";
                string tableExists = objfun.Get_details(checkTableQuery);

                if (!string.IsNullOrEmpty(tableExists) && Convert.ToInt32(tableExists) > 0)
                {
                    string usedCount = objfun.Get_details("SELECT COUNT(*) FROM tbl_CO_PO_Mapping WHERE PoCode = '" + currentPoCode + "' AND CourseId = '" + courseId + "'");
                    if (!string.IsNullOrEmpty(usedCount) && Convert.ToInt32(usedCount) > 0)
                    {
                        objfun.MsgBox("Cannot delete " + currentPoCode + " because it is already mapped with Course Outcomes!", this);
                        return;
                    }
                }

                // 3. Delete execute karein
                objfun.ExecuteDelete("DELETE FROM tbl_PO_Master WHERE PoId = '" + poId + "'");
                objfun.MsgBox("Deleted Successfully!", this);

                ResetForm();
                BindPoGrid();
                LoadNextAvailablePO();
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
        LoadNextAvailablePO();
    }

    private void ResetForm()
    {
        hdnEditPoId.Value = "0";
        txtPoStatement.Text = "";
        lblFormTitle.Text = "Add Program Outcome";
        btnAddPo.Text = "<i class='fa-solid fa-circle-plus'></i> Add PO";
        btnAddPo.Enabled = true;
        btnCancelEdit.Visible = false;
    }
}