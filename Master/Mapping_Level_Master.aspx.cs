using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Co_Po_Mapping_Master_Mapping_Level_Master : System.Web.UI.Page
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
        BindLevelsGrid();
        LoadNextAvailableLevel();
    }

    private void BindLevelsGrid()
    {
        if (ddlCourse.SelectedIndex > 0)
        {
            string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";
            string query = "SELECT LevelId, CorrelationLevel, LevelName, PercentageFrom, PercentageTo, ISNULL(Description, '') AS Description " +
                           "FROM tbl_Mapping_Level_Master " +
                           "WHERE CourseId = '" + ddlCourse.SelectedValue + "' AND InstituteID = '" + instId + "' " +
                           "ORDER BY CorrelationLevel ASC";

            DataTable dt = objfun.FillDataTable(query);
            gvLevels.DataSource = dt;
            gvLevels.DataBind();
        }
        else
        {
            gvLevels.DataSource = null;
            gvLevels.DataBind();
        }
    }

    // Sequence Rule: Pehle Level 1 aayega, fir Level 2, fir Level 3
    private void LoadNextAvailableLevel()
    {
        ddlLevel.Items.Clear();

        if (ddlCourse.SelectedIndex <= 0)
        {
            ddlLevel.Items.Insert(0, new ListItem("--Select Course First--", ""));
            ddlLevel.Enabled = false;
            btnSaveLevel.Enabled = false;
            return;
        }

        string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";

        // Count kitne levels ban chuke hain is course ke liye
        string countQuery = "SELECT COUNT(*) FROM tbl_Mapping_Level_Master WHERE CourseId = '" + ddlCourse.SelectedValue + "' AND InstituteID = '" + instId + "'";
        string existingCountStr = objfun.Get_details(countQuery);
        int existingCount = 0;
        int.TryParse(existingCountStr, out existingCount);

        int nextLevel = existingCount + 1;

        if (nextLevel > 3)
        {
            ddlLevel.Items.Insert(0, new ListItem("All Levels Defined (Max Level 3)", ""));
            ddlLevel.Enabled = false;
            btnSaveLevel.Enabled = false;
        }
        else
        {
            ddlLevel.Items.Insert(0, new ListItem("Level " + nextLevel, nextLevel.ToString()));
            ddlLevel.Enabled = false; // Locked taaki user khud se change na kar sake
            btnSaveLevel.Enabled = true;

            // Default level name suggest kar sakte hain
            if (nextLevel == 1) txtLevelName.Text = "Low / Slight";
            else if (nextLevel == 2) txtLevelName.Text = "Medium / Moderate";
            else if (nextLevel == 3) txtLevelName.Text = "High / Substantial";
        }
    }

    protected void btnSaveLevel_Click(object sender, EventArgs e)
    {
        if (ddlCourse.SelectedIndex <= 0)
        {
            objfun.MsgBox("Please select Course / Program first!", this);
            ddlCourse.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(ddlLevel.SelectedValue))
        {
            objfun.MsgBox("Valid Level select nahi hai!", this);
            return;
        }

        if (string.IsNullOrWhiteSpace(txtLevelName.Text))
        {
            objfun.MsgBox("Please enter Level Name (e.g., Low, Medium, High)!", this);
            txtLevelName.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtMinPer.Text) || string.IsNullOrWhiteSpace(txtMaxPer.Text))
        {
            objfun.MsgBox("Please enter Min and Max Attainment Percentage Range!", this);
            return;
        }

        try
        {
            string instId = Session["InstID"] != null ? Session["InstID"].ToString() : "1";
            string courseId = ddlCourse.SelectedValue;
            int level = Convert.ToInt32(ddlLevel.SelectedValue);
            string levelName = txtLevelName.Text.Trim().Replace("'", "''");
            decimal minPer = Convert.ToDecimal(txtMinPer.Text.Trim());
            decimal maxPer = Convert.ToDecimal(txtMaxPer.Text.Trim());
            string desc = txtDesc.Text.Trim().Replace("'", "''");
            string adminId = Session["UID"].ToString();
            int editId = Convert.ToInt32(hdnEditLevelId.Value);

            if (editId == 0)
            {
                // Strict check: Kya पिछला level exist karta hai? (Jaise Level 2 ke liye Level 1 hona zaroori hai)
                if (level > 1)
                {
                    int prevLevel = level - 1;
                    string prevCheck = "SELECT COUNT(*) FROM tbl_Mapping_Level_Master WHERE CourseId = '" + courseId + "' AND InstituteID = '" + instId + "' AND CorrelationLevel = '" + prevLevel + "'";
                    int prevExists = Convert.ToInt32(objfun.Get_details(prevCheck));

                    if (prevExists == 0)
                    {
                        objfun.MsgBox("Pehle Level " + prevLevel + " define karein! Sequence break nahi kar sakte.", this);
                        return;
                    }
                }

                // Duplicate check
                string checkQuery = "SELECT COUNT(*) FROM tbl_Mapping_Level_Master WHERE CourseId = '" + courseId + "' AND InstituteID = '" + instId + "' AND CorrelationLevel = '" + level + "'";
                int count = Convert.ToInt32(objfun.Get_details(checkQuery));

                if (count > 0)
                {
                    objfun.MsgBox("Level " + level + " is already defined for this Course!", this);
                    return;
                }

                string insertQuery = "INSERT INTO tbl_Mapping_Level_Master (InstituteID, CourseId, CorrelationLevel, LevelName, PercentageFrom, PercentageTo, Description, CreatedBy, CreatedDate) " +
                                     "VALUES ('" + instId + "', '" + courseId + "', '" + level + "', '" + levelName + "', '" + minPer + "', '" + maxPer + "', '" + desc + "', '" + adminId + "', GETDATE())";

                objfun.ExecuteDelete(insertQuery);
                objfun.MsgBox("Level " + level + " Configuration Saved Successfully!", this);
            }
            else
            {
                // Update
                string updateQuery = "UPDATE tbl_Mapping_Level_Master SET LevelName = '" + levelName + "', PercentageFrom = '" + minPer + "', PercentageTo = '" + maxPer + "', Description = '" + desc + "' WHERE LevelId = '" + editId + "'";
                objfun.ExecuteDelete(updateQuery);
                objfun.MsgBox("Level Configuration Updated Successfully!", this);
            }

            ResetForm();
            BindLevelsGrid();
            LoadNextAvailableLevel();
        }
        catch (Exception ex)
        {
            objfun.MsgBox("Error: " + ex.Message.Replace("'", " "), this);
        }
    }

    protected void gvLevels_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string levelId = e.CommandArgument.ToString();

        if (e.CommandName == "EditRow")
        {
            try
            {
                string query = "SELECT LevelId, CorrelationLevel, LevelName, PercentageFrom, PercentageTo, Description FROM tbl_Mapping_Level_Master WHERE LevelId = '" + levelId + "'";
                DataTable dt = objfun.FillDataTable(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    hdnEditLevelId.Value = dt.Rows[0]["LevelId"].ToString();

                    ddlLevel.Items.Clear();
                    string lvlVal = dt.Rows[0]["CorrelationLevel"].ToString();
                    ddlLevel.Items.Add(new ListItem("Level " + lvlVal, lvlVal));
                    ddlLevel.SelectedValue = lvlVal;
                    ddlLevel.Enabled = false; // Edit ke samay level number locked rahega

                    txtLevelName.Text = dt.Rows[0]["LevelName"].ToString();
                    txtMinPer.Text = dt.Rows[0]["PercentageFrom"].ToString();
                    txtMaxPer.Text = dt.Rows[0]["PercentageTo"].ToString();
                    txtDesc.Text = dt.Rows[0]["Description"].ToString();

                    lblFormTitle.Text = "Update Level " + lvlVal;
                    btnSaveLevel.Text = "<i class='fa-solid fa-check'></i> Update Level";
                    btnSaveLevel.Enabled = true;
                    btnCancelEdit.Visible = true;
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

                // 1. Sequence Delete Protection: Check karein kya yeh sabse highest/last level hai
                string currentLevel = objfun.Get_details("SELECT CorrelationLevel FROM tbl_Mapping_Level_Master WHERE LevelId = '" + levelId + "'");
                string maxLevel = objfun.Get_details("SELECT TOP 1 CorrelationLevel FROM tbl_Mapping_Level_Master WHERE CourseId = '" + courseId + "' AND InstituteID = '" + instId + "' ORDER BY CorrelationLevel DESC");

                if (currentLevel != maxLevel)
                {
                    objfun.MsgBox("Sequence maintain rakhne ke liye aap sirf aakhiri defined Level (" + maxLevel + ") ko hi delete kar sakte hain!", this);
                    return;
                }

                // 2. Delete execute karein
                objfun.ExecuteDelete("DELETE FROM tbl_Mapping_Level_Master WHERE LevelId = '" + levelId + "'");
                objfun.MsgBox("Deleted Successfully!", this);

                ResetForm();
                BindLevelsGrid();
                LoadNextAvailableLevel();
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
        LoadNextAvailableLevel();
    }

    private void ResetForm()
    {
        hdnEditLevelId.Value = "0";
        txtLevelName.Text = "";
        txtMinPer.Text = "";
        txtMaxPer.Text = "";
        txtDesc.Text = "";
        lblFormTitle.Text = "Define Correlation Level & Threshold";
        btnSaveLevel.Text = "<i class='fa-solid fa-check'></i> Save Level";
        btnCancelEdit.Visible = false;
    }
}