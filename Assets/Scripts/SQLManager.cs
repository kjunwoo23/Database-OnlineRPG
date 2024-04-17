using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using UI.Tables;
using TMPro;

public class SQLManager : MonoBehaviour
{
    public static SQLManager instance;

    public TableLayout[] tables;
    public RectTransform content;

    public TMP_FontAsset font;
    public bool isLaptop;

    string connectionString;
    SqlConnection connection;
    

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (isLaptop)
            connectionString = "Server=DESKTOP-O8B5HP1;Database=OnlineRPG_DB;User Id=kjunwoo234;Pwd=1234;";
        else
            connectionString = "Server=DESKTOP-J9EJ7HN;Database=OnlineRPG_DB;User Id=kjunwoo234;Pwd=1234;";
        TryConnect();

        connection = new SqlConnection(connectionString);
        connection.Open();

        ShowUserTable(tables[0]);
    }

        // Update is called once per frame
        void Update()
    {
        
    }

    void TryConnect()
    {
        //SQL 인증
        using (connection = new SqlConnection(connectionString))
        {
            try
            {
                // DB 서버 접속
                connection.Open();
                Debug.Log("Connection successful.");
            }
            catch (Exception ex) // DB 서버 접속 실패 시
            {
                Debug.Log("Error connecting to database: " + ex.Message);
            }
        }
        // DB 서버 접속 종료
        connection.Close();
    }

    public void OnClickTableButton(int i)
    {
        for (int j = 0; j < tables.Length; j++)
            tables[j].gameObject.SetActive(false);

        tables[i].gameObject.SetActive(true);

        switch (i)
        {
            case 0:
                ShowUserTable(tables[i]);
                break;
            case 1:
                ShowClassTable(tables[i]);
                break;
            case 2:
                ShowSkillTable(tables[i]);
                break;
            case 3:
                ShowWeaponTypeTable(tables[i]);
                break;
            case 4:
                ShowBossTable(tables[i]);
                break;
            case 5:
                ShowRaidPartyTable(tables[i]);
                break;
            case 6:
                ShowAuctionTable(tables[i]);
                break;

        }
    }

    void SimpleReadTable(TableLayout table, string tableName)
    {
        //table.ClearRows();
        foreach (var row in table.Rows)
            DestroyImmediate(row.gameObject);
        table.AddRow();


        string query = "SELECT * FROM " + tableName + ";";
        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        string temp = string.Empty;
        if (rdr == null) temp = "No return";
        else
        {
            int x = 0;
            int y = 1;
            while (rdr.Read())
            {
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    GameObject empty = new GameObject();

                    if (table.Rows.Count <= y)
                        table.AddRow();

                    if (table.Rows[y].Cells.Count <= x)
                        table.Rows[y].AddCell();

                    empty.transform.parent = table.Rows[y].Cells[x].transform;
                    empty.transform.localPosition *= 0;
                    empty.transform.localScale = new Vector3(1, 1, 1);

                    //Debug.Log(i);
                    
                    TextMeshProUGUI text = empty.AddComponent<TextMeshProUGUI>();
                    //text.transform.localPosition *= 0;
                    if (rdr[i].ToString() == "")
                        text.text = "NULL";
                    else
                        text.text = rdr[i].ToString();
                    text.alignment = TextAlignmentOptions.Center;
                    //text.color = new Color(0, 1, 0);
                    text.font = font;
                    text.fontSize = 20;

                    if (i != rdr.FieldCount - 1)
                    {
                        temp += rdr[i] + ";";    // parser 넣어주기
                        
                        x++;
                    }
                    else if (i == rdr.FieldCount - 1)
                    {
                        temp += rdr[i] + "\n";
                        y++;
                        x = 0;
                    }
                }
            }
            table.GetComponent<RectTransform>().sizeDelta
                = new Vector2(table.GetComponent<RectTransform>().sizeDelta.x, 50 * table.Rows.Count);
            content.sizeDelta = new Vector2(content.sizeDelta.x, table.GetComponent<RectTransform>().sizeDelta.y);
            //Debug.Log(x + "," + y);
        }
        //Debug.Log(temp);

        rdr.Close(); // <- too easy to forget
        rdr.Dispose(); // <- too easy to forget
    }

    void ShowUserTable(TableLayout table)
    {
        //table.ClearRows();
        while (table.Rows.Count > 1)
            DestroyImmediate(table.Rows[table.Rows.Count - 1].gameObject);
        table.AddRow();


        string query = @"SELECT level, user_name, class_name, weapon_name
            FROM dbo.users as users
            left outer join dbo.class as class
            on users.class_ID = class.class_ID
            left outer join dbo.weapon as weapon
            on users.weapon_ID = weapon.weapon_ID"
            + "\norder by level desc";
        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        string temp = string.Empty;
        if (rdr == null) temp = "No return";
        else
        {
            int x = 0;
            int y = 1;
            while (rdr.Read())
            {
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    GameObject empty = new GameObject();

                    if (table.Rows.Count <= y)
                        table.AddRow();

                    if (table.Rows[y].Cells.Count <= x)
                        table.Rows[y].AddCell();

                    empty.transform.parent = table.Rows[y].Cells[x].transform;
                    empty.transform.localPosition *= 0;
                    empty.transform.localScale = new Vector3(1, 1, 1);

                    //Debug.Log(i);

                    TextMeshProUGUI text = empty.AddComponent<TextMeshProUGUI>();
                    //text.transform.localPosition *= 0;
                    if (rdr[i].ToString() == "")
                        text.text = "NULL";
                    else
                        text.text = rdr[i].ToString();
                    text.alignment = TextAlignmentOptions.Center;
                    //text.color = new Color(0, 1, 0);
                    text.font = font;
                    text.fontSize = 20;

                    if (i != rdr.FieldCount - 1)
                    {
                        temp += rdr[i] + ";";    // parser 넣어주기

                        x++;
                    }
                    else if (i == rdr.FieldCount - 1)
                    {
                        temp += rdr[i] + "\n";
                        y++;
                        x = 0;
                    }
                }
            }
            table.GetComponent<RectTransform>().sizeDelta
                = new Vector2(table.GetComponent<RectTransform>().sizeDelta.x, 50 * table.Rows.Count);
            content.sizeDelta = new Vector2(content.sizeDelta.x, table.GetComponent<RectTransform>().sizeDelta.y);
            //Debug.Log(x + "," + y);
        }
        //Debug.Log(temp);

        rdr.Close(); // <- too easy to forget
        rdr.Dispose(); // <- too easy to forget
    }

    void ShowClassTable(TableLayout table)
    {
        //table.ClearRows();
        while (table.Rows.Count > 1)
            DestroyImmediate(table.Rows[table.Rows.Count - 1].gameObject);
        table.AddRow();


        string query = @"SELECT class_name, required_level, default_HP, default_MP
            FROM dbo.class"
            + "\norder by class_name";
        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        string temp = string.Empty;
        if (rdr == null) temp = "No return";
        else
        {
            int x = 0;
            int y = 1;
            while (rdr.Read())
            {
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    GameObject empty = new GameObject();

                    if (table.Rows.Count <= y)
                        table.AddRow();

                    if (table.Rows[y].Cells.Count <= x)
                        table.Rows[y].AddCell();

                    empty.transform.parent = table.Rows[y].Cells[x].transform;
                    empty.transform.localPosition *= 0;
                    empty.transform.localScale = new Vector3(1, 1, 1);

                    //Debug.Log(i);

                    TextMeshProUGUI text = empty.AddComponent<TextMeshProUGUI>();
                    //text.transform.localPosition *= 0;
                    if (rdr[i].ToString() == "")
                        text.text = "NULL";
                    else
                        text.text = rdr[i].ToString();
                    text.alignment = TextAlignmentOptions.Center;
                    //text.color = new Color(0, 1, 0);
                    text.font = font;
                    text.fontSize = 20;

                    if (i != rdr.FieldCount - 1)
                    {
                        temp += rdr[i] + ";";    // parser 넣어주기

                        x++;
                    }
                    else if (i == rdr.FieldCount - 1)
                    {
                        temp += rdr[i] + "\n";
                        y++;
                        x = 0;
                    }
                }
            }
            table.GetComponent<RectTransform>().sizeDelta
                = new Vector2(table.GetComponent<RectTransform>().sizeDelta.x, 50 * table.Rows.Count);
            content.sizeDelta = new Vector2(content.sizeDelta.x, table.GetComponent<RectTransform>().sizeDelta.y);
            //Debug.Log(x + "," + y);
        }
        //Debug.Log(temp);

        rdr.Close(); // <- too easy to forget
        rdr.Dispose(); // <- too easy to forget
    }

    void ShowSkillTable(TableLayout table)
    {
        //table.ClearRows();
        while (table.Rows.Count > 1)
            DestroyImmediate(table.Rows[table.Rows.Count - 1].gameObject);
        table.AddRow();

        string query = @"SELECT skill_name, element_name, class_name, skill.required_level, damage_coefficient, MP_cost
            FROM dbo.skill as skill
	        inner join dbo.element as element
	        on skill.element_ID = element.element_ID
	        inner join dbo.class as class
	        on skill.required_class_ID = class.class_ID"
            + "\norder by skill_name";
        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        string temp = string.Empty;
        if (rdr == null) temp = "No return";
        else
        {
            int x = 0;
            int y = 1;
            while (rdr.Read())
            {
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    GameObject empty = new GameObject();

                    if (table.Rows.Count <= y)
                        table.AddRow();

                    if (table.Rows[y].Cells.Count <= x)
                        table.Rows[y].AddCell();

                    empty.transform.parent = table.Rows[y].Cells[x].transform;
                    empty.transform.localPosition *= 0;
                    empty.transform.localScale = new Vector3(1, 1, 1);

                    //Debug.Log(i);

                    TextMeshProUGUI text = empty.AddComponent<TextMeshProUGUI>();
                    //text.transform.localPosition *= 0;
                    if (rdr[i].ToString() == "")
                        text.text = "NULL";
                    else
                        text.text = rdr[i].ToString();
                    text.alignment = TextAlignmentOptions.Center;
                    //text.color = new Color(0, 1, 0);
                    text.font = font;
                    text.fontSize = 20;

                    if (i != rdr.FieldCount - 1)
                    {
                        temp += rdr[i] + ";";    // parser 넣어주기

                        x++;
                    }
                    else if (i == rdr.FieldCount - 1)
                    {
                        temp += rdr[i] + "\n";
                        y++;
                        x = 0;
                    }
                }
            }
            table.GetComponent<RectTransform>().sizeDelta
                = new Vector2(table.GetComponent<RectTransform>().sizeDelta.x, 50 * table.Rows.Count);
            content.sizeDelta = new Vector2(content.sizeDelta.x, table.GetComponent<RectTransform>().sizeDelta.y);
            //Debug.Log(x + "," + y);
        }
        //Debug.Log(temp);

        rdr.Close(); // <- too easy to forget
        rdr.Dispose(); // <- too easy to forget
    }

    void ShowWeaponTypeTable(TableLayout table)
    {
        //table.ClearRows();
        while (table.Rows.Count > 1)
            DestroyImmediate(table.Rows[table.Rows.Count - 1].gameObject);
        table.AddRow();

        string query = @"SELECT weapon_type_name, class_name, weaponType.required_level
            FROM dbo.weaponType as weaponType
	        inner join dbo.class as class
	        on weaponType.required_class_ID = class.class_ID"
            + "\norder by weapon_type_name";
        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        string temp = string.Empty;
        if (rdr == null) temp = "No return";
        else
        {
            int x = 0;
            int y = 1;
            while (rdr.Read())
            {
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    GameObject empty = new GameObject();

                    if (table.Rows.Count <= y)
                        table.AddRow();

                    if (table.Rows[y].Cells.Count <= x)
                        table.Rows[y].AddCell();

                    empty.transform.parent = table.Rows[y].Cells[x].transform;
                    empty.transform.localPosition *= 0;
                    empty.transform.localScale = new Vector3(1, 1, 1);

                    //Debug.Log(i);

                    TextMeshProUGUI text = empty.AddComponent<TextMeshProUGUI>();
                    //text.transform.localPosition *= 0;
                    if (rdr[i].ToString() == "")
                        text.text = "NULL";
                    else
                        text.text = rdr[i].ToString();
                    text.alignment = TextAlignmentOptions.Center;
                    //text.color = new Color(0, 1, 0);
                    text.font = font;
                    text.fontSize = 20;

                    if (i != rdr.FieldCount - 1)
                    {
                        temp += rdr[i] + ";";    // parser 넣어주기

                        x++;
                    }
                    else if (i == rdr.FieldCount - 1)
                    {
                        temp += rdr[i] + "\n";
                        y++;
                        x = 0;
                    }
                }
            }
            table.GetComponent<RectTransform>().sizeDelta
                = new Vector2(table.GetComponent<RectTransform>().sizeDelta.x, 50 * table.Rows.Count);
            content.sizeDelta = new Vector2(content.sizeDelta.x, table.GetComponent<RectTransform>().sizeDelta.y);
            //Debug.Log(x + "," + y);
        }
        //Debug.Log(temp);

        rdr.Close(); // <- too easy to forget
        rdr.Dispose(); // <- too easy to forget
    }

    void ShowBossTable(TableLayout table)
    {
        //table.ClearRows();
        while (table.Rows.Count > 1)
            DestroyImmediate(table.Rows[table.Rows.Count - 1].gameObject);
        table.AddRow();

        string query = @"SELECT boss_name, element_name, HP, MP, required_level, required_member
            FROM dbo.Boss as boss
	        inner join dbo.element as element
	        on boss.element_ID = element.element_ID"
            + "\norder by boss_name";
        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        string temp = string.Empty;
        if (rdr == null) temp = "No return";
        else
        {
            int x = 0;
            int y = 1;
            while (rdr.Read())
            {
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    GameObject empty = new GameObject();

                    if (table.Rows.Count <= y)
                        table.AddRow();

                    if (table.Rows[y].Cells.Count <= x)
                        table.Rows[y].AddCell();

                    empty.transform.parent = table.Rows[y].Cells[x].transform;
                    empty.transform.localPosition *= 0;
                    empty.transform.localScale = new Vector3(1, 1, 1);

                    //Debug.Log(i);

                    TextMeshProUGUI text = empty.AddComponent<TextMeshProUGUI>();
                    //text.transform.localPosition *= 0;
                    if (rdr[i].ToString() == "")
                        text.text = "NULL";
                    else
                        text.text = rdr[i].ToString();
                    text.alignment = TextAlignmentOptions.Center;
                    //text.color = new Color(0, 1, 0);
                    text.font = font;
                    text.fontSize = 20;

                    if (i != rdr.FieldCount - 1)
                    {
                        temp += rdr[i] + ";";    // parser 넣어주기

                        x++;
                    }
                    else if (i == rdr.FieldCount - 1)
                    {
                        temp += rdr[i] + "\n";
                        y++;
                        x = 0;
                    }
                }
            }
            table.GetComponent<RectTransform>().sizeDelta
                = new Vector2(table.GetComponent<RectTransform>().sizeDelta.x, 50 * table.Rows.Count);
            content.sizeDelta = new Vector2(content.sizeDelta.x, table.GetComponent<RectTransform>().sizeDelta.y);
            //Debug.Log(x + "," + y);
        }
        //Debug.Log(temp);

        rdr.Close(); // <- too easy to forget
        rdr.Dispose(); // <- too easy to forget
    }

    void ShowRaidPartyTable(TableLayout table)
    {
        //table.ClearRows();
        while (table.Rows.Count > 1)
            DestroyImmediate(table.Rows[table.Rows.Count - 1].gameObject);
        table.AddRow();

        string query = @"SELECT party_name, boss_name, user_name, required_member, current_member
            FROM dbo.raidPartyRoom as raidPartyRoom
	        inner join dbo.boss as boss
	        on raidPartyRoom.boss_ID = boss.boss_ID
	        inner join dbo.users as users
	        on raidPartyRoom.leader_ID = users.user_ID"
            + "\norder by party_name";
        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        string temp = string.Empty;
        if (rdr == null) temp = "No return";
        else
        {
            int x = 0;
            int y = 1;
            while (rdr.Read())
            {
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    GameObject empty = new GameObject();

                    if (table.Rows.Count <= y)
                        table.AddRow();

                    if (table.Rows[y].Cells.Count <= x)
                        table.Rows[y].AddCell();

                    empty.transform.parent = table.Rows[y].Cells[x].transform;
                    empty.transform.localPosition *= 0;
                    empty.transform.localScale = new Vector3(1, 1, 1);

                    //Debug.Log(i);

                    TextMeshProUGUI text = empty.AddComponent<TextMeshProUGUI>();
                    //text.transform.localPosition *= 0;
                    if (rdr[i].ToString() == "")
                        text.text = "NULL";
                    else
                        text.text = rdr[i].ToString();
                    text.alignment = TextAlignmentOptions.Center;
                    //text.color = new Color(0, 1, 0);
                    text.font = font;
                    text.fontSize = 20;

                    if (i != rdr.FieldCount - 1)
                    {
                        temp += rdr[i] + ";";    // parser 넣어주기

                        x++;
                    }
                    else if (i == rdr.FieldCount - 1)
                    {
                        temp += rdr[i] + "\n";
                        y++;
                        x = 0;
                    }
                }
            }
            table.GetComponent<RectTransform>().sizeDelta
                = new Vector2(table.GetComponent<RectTransform>().sizeDelta.x, 50 * table.Rows.Count);
            content.sizeDelta = new Vector2(content.sizeDelta.x, table.GetComponent<RectTransform>().sizeDelta.y);
            //Debug.Log(x + "," + y);
        }
        //Debug.Log(temp);

        rdr.Close(); // <- too easy to forget
        rdr.Dispose(); // <- too easy to forget
    }

    void ShowAuctionTable(TableLayout table)
    {
        //table.ClearRows();
        while (table.Rows.Count > 1)
            DestroyImmediate(table.Rows[table.Rows.Count - 1].gameObject);
        table.AddRow();

        string query = @"SELECT weapon_name, weapon_type_name, element_name, damage_coefficient, price, user_name
            FROM dbo.auction as auction
	        inner join dbo.weapon as weapon
	        on auction.weapon_ID = weapon.weapon_ID 
	        inner join dbo.weaponType as weaponType
	        on weapon.weapon_type_ID = weaponType.weapon_type_ID
	        inner join dbo.element as element
	        on weapon.element_ID = element.element_ID
	        inner join dbo.users as users
	        on auction.seller_ID = users.user_ID"
            + "\norder by weapon_name";
        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        string temp = string.Empty;
        if (rdr == null) temp = "No return";
        else
        {
            int x = 0;
            int y = 1;
            while (rdr.Read())
            {
                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    GameObject empty = new GameObject();

                    if (table.Rows.Count <= y)
                        table.AddRow();

                    if (table.Rows[y].Cells.Count <= x)
                        table.Rows[y].AddCell();

                    empty.transform.parent = table.Rows[y].Cells[x].transform;
                    empty.transform.localPosition *= 0;
                    empty.transform.localScale = new Vector3(1, 1, 1);

                    //Debug.Log(i);

                    TextMeshProUGUI text = empty.AddComponent<TextMeshProUGUI>();
                    //text.transform.localPosition *= 0;
                    if (rdr[i].ToString() == "")
                        text.text = "NULL";
                    else
                        text.text = rdr[i].ToString();
                    text.alignment = TextAlignmentOptions.Center;
                    //text.color = new Color(0, 1, 0);
                    text.font = font;
                    text.fontSize = 20;

                    if (i != rdr.FieldCount - 1)
                    {
                        temp += rdr[i] + ";";    // parser 넣어주기

                        x++;
                    }
                    else if (i == rdr.FieldCount - 1)
                    {
                        temp += rdr[i] + "\n";
                        y++;
                        x = 0;
                    }
                }
            }
            table.GetComponent<RectTransform>().sizeDelta
                = new Vector2(table.GetComponent<RectTransform>().sizeDelta.x, 50 * table.Rows.Count);
            content.sizeDelta = new Vector2(content.sizeDelta.x, table.GetComponent<RectTransform>().sizeDelta.y);
            //Debug.Log(x + "," + y);
        }
        //Debug.Log(temp);

        rdr.Close(); // <- too easy to forget
        rdr.Dispose(); // <- too easy to forget
    }



    public void OnClickQuit()
    {
        connection.Close();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
