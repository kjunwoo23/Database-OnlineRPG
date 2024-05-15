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

    public TMP_Dropdown textSearch, valueSearch;
    public TMP_InputField inputMin, inputMax;

    bool isGM;

    int[] orderTypes;

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

        GMInit();

        orderTypes = new int[tables.Length];

        OnClickTableOrderBy(5);
        //ShowUserTable(tables[0], 0);
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

    public void OnClickShowTableButton(int i)
    {
        for (int j = 0; j < tables.Length; j++)
            tables[j].gameObject.SetActive(false);

        tables[i].gameObject.SetActive(true);

        orderTypes[i] = 0;
        SwitchTables(i, 0);
    }

    void SwitchTables(int i, int orderType)
    {
        switch (i)
        {
            case 0:
                ShowUserTable(tables[i], orderType);
                break;
            case 1:
                ShowClassTable(tables[i], orderType);
                break;
            case 2:
                ShowSkillTable(tables[i], orderType);
                break;
            case 3:
                ShowWeaponTypeTable(tables[i], orderType);
                break;
            case 4:
                ShowBossTable(tables[i], orderType);
                break;
            case 5:
                ShowRaidPartyTable(tables[i], orderType);
                break;
            case 6:
                ShowAuctionTable(tables[i], orderType);
                break;
            case 7:
                ShowBanTable(tables[i], orderType);
                break;
            case 8:
                ShowElementTable(tables[i], orderType);
                break;
        }
        InitTextSearchDropDown(tables[i]);
        InitValueSearchDropDown(tables[i]);
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

    public void OnClickTableOrderBy(int code)
    {
        int tableType = code / 10;
        int orderType = code % 10;

        if (orderTypes[tableType] == orderType)
            orderType *= -1;

        orderTypes[tableType] = orderType;
        SwitchTables(tableType, orderType);
    }



    void ShowUserTable(TableLayout table, int orderType)
    {
        string query = @"select * from UserView";

        if (!isGM)
        {
            query += @"
                where user_name not in
		        (select user_name
		        from dbo.users as users
		        left outer join dbo.banList as banList
		        on users.user_ID = banList.user_ID
		        left outer join dbo.GM as GM
		        on users.user_ID = GM.GM_user_ID
		        where users.user_ID = banList.user_ID or users.user_ID = gm.GM_user_ID)";
        }

        switch (orderType)
        {
            case 0:
                break;
            case 1:
                query += "\norder by level";
                break;
            case -1:
                query += "\norder by level desc";
                break;
            case 2:
                query += "\norder by user_name";
                break;
            case -2:
                query += "\norder by user_name desc";
                break;
            case 3:
                query += "\norder by class_name";
                break;
            case -3:
                query += "\norder by class_name desc";
                break;
            case 4:
                query += "\norder by weapon_name";
                break;
            case -4:
                query += "\norder by weapon_name desc";
                break;
            case 5:
                query += "\norder by user_rank";
                break;
            case -5:
                query += "\norder by user_rank desc";
                break;
        }

        ReadTableWithQuery(table, query);
    }
    void ShowUserTable(TableLayout table, string target, string text)
    {
        string query = @"select * from UserView";

        switch (target)
        {
            case "Lv Rank":
                query += "\nwhere user_rank like '%" + text + "%'";
                break;
            case "Name":
                query += "\nwhere user_name like '%" + text + "%'";
                break;
            case "Class":
                query += "\nwhere class_name like '%" + text + "%'";
                break;
            case "Weapon":
                query += "\nwhere weapon_name like '%" + text + "%'";
                break;
        }
        
        if (!isGM)
        {
            query += @" and user_name not in
		        (select user_name
		        from dbo.users as users
		        left outer join dbo.banList as banList
		        on users.user_ID = banList.user_ID
		        left outer join dbo.GM as GM
		        on users.user_ID = GM.GM_user_ID
		        where users.user_ID = banList.user_ID or users.user_ID = gm.GM_user_ID)";
        }

        //Debug.Log(query);
        ReadTableWithQuery(table, query);
    }
    void ShowUserTable(TableLayout table, string target, float min, float max)
    {
        string query = @"select * from UserView";

        switch (target)
        {
            case "Lv":
                query += "\nwhere level between " + min.ToString() + " and " + max.ToString();
                break;
        }
        
        if (!isGM)
        {
            query += @" and user_name not in
		        (select user_name
		        from dbo.users as users
		        left outer join dbo.banList as banList
		        on users.user_ID = banList.user_ID
		        left outer join dbo.GM as GM
		        on users.user_ID = GM.GM_user_ID
		        where users.user_ID = banList.user_ID or users.user_ID = gm.GM_user_ID)";
        }

        //Debug.Log(query);
        ReadTableWithQuery(table, query);
    }



    void ShowClassTable(TableLayout table, int orderType)
    {
        string query = @"select * from ClassView";

        switch (orderType)
        {
            case 0:
                break;
            case 1:
                query += "\norder by class_name";
                break;
            case -1:
                query += "\norder by class_name desc";
                break;
            case 2:
                query += "\norder by required_level";
                break;
            case -2:
                query += "\norder by required_level desc";
                break;
            case 3:
                query += "\norder by default_HP";
                break;
            case -3:
                query += "\norder by default_HP desc";
                break;
            case 4:
                query += "\norder by default_MP";
                break;
            case -4:
                query += "\norder by default_MP desc";
                break;
        }
        ReadTableWithQuery(table, query);
    }
    void ShowClassTable(TableLayout table, string target, string text)
    {
        string query = @"select * from ClassView";

        switch (target)
        {
            case "Name":
                query += "\nwhere class_name like '%" + text + "%'";
                break;
        }

        //Debug.Log(query);
        ReadTableWithQuery(table, query);
    }
    void ShowClassTable(TableLayout table, string target, float min, float max)
    {
        string query = @"select * from ClassView";

        switch (target)
        {
            case "Req Lv":
                query += "\nwhere required_level between " + min.ToString() + " and " + max.ToString();
                break;
            case "Default HP":
                query += "\nwhere default_HP between " + min.ToString() + " and " + max.ToString();
                break;
            case "Default MP":
                query += "\nwhere default_MP between " + min.ToString() + " and " + max.ToString();
                break;
        }

        //Debug.Log(query);
        ReadTableWithQuery(table, query);
    }



    void ShowSkillTable(TableLayout table, int orderType)
    {
        string query = @"select * from SkillView";

        switch (orderType)
        {
            case 0:
                break;
            case 1:
                query += "\norder by skill_name";
                break;
            case -1:
                query += "\norder by skill_name desc";
                break;
            case 2:
                query += "\norder by element_name";
                break;
            case -2:
                query += "\norder by element_name desc";
                break;
            case 3:
                query += "\norder by class_name";
                break;
            case -3:
                query += "\norder by class_name desc";
                break;
            case 4:
                query += "\norder by required_level";
                break;
            case -4:
                query += "\norder by required_level desc";
                break;
            case 5:
                query += "\norder by damage_coefficient";
                break;
            case -5:
                query += "\norder by damage_coefficient desc";
                break;
            case 6:
                query += "\norder by MP_cost";
                break;
            case -6:
                query += "\norder by MP_cost desc";
                break;
        }

        ReadTableWithQuery(table, query);
    }
    void ShowSkillTable(TableLayout table, string target, string text)
    {
        string query = @"select * from SkillView";

        switch (target)
        {
            case "Name":
                query += "\nwhere skill_name like '%" + text + "%'";
                break;
            case "Element":
                query += "\nwhere element_name like '%" + text + "%'";
                break;
            case "Req Class":
                query += "\nwhere class_name like '%" + text + "%'";
                break;
        }

        //Debug.Log(query);
        ReadTableWithQuery(table, query);
    }
    void ShowSkillTable(TableLayout table, string target, float min, float max)
    {
        string query = @"select * from SkillView";

        switch (target)
        {
            case "Req Lv":
                query += "\nwhere required_level between " + min.ToString() + " and " + max.ToString();
                break;
            case "Damage":
                query += "\nwhere damage_coefficient between " + min.ToString() + " and " + max.ToString();
                break;
            case "MP Cost":
                query += "\nwhere MP_cost between " + min.ToString() + " and " + max.ToString();
                break;
        }

        //Debug.Log(query);
        ReadTableWithQuery(table, query);
    }



    void ShowWeaponTypeTable(TableLayout table, int orderType)
    {
        string query = @"select * from WeaponTypeView";

        switch (orderType)
        {
            case 0:
                break;
            case 1:
                query += "\norder by weapon_type_name";
                break;
            case -1:
                query += "\norder by weapon_type_name desc";
                break;
            case 2:
                query += "\norder by class_name";
                break;
            case -2:
                query += "\norder by class_name desc";
                break;
            case 3:
                query += "\norder by required_level";
                break;
            case -3:
                query += "\norder by required_level desc";
                break;
        }

        ReadTableWithQuery(table, query);
    }
    void ShowWeaponTypeTable(TableLayout table, string target, string text)
    {
        string query = @"select * from WeaponTypeView";

        switch (target)
        {
            case "Name":
                query += "\nwhere weapon_type_name like '%" + text + "%'";
                break;
            case "Class":
                query += "\nwhere class_name like '%" + text + "%'";
                break;
        }

        //Debug.Log(query);
        ReadTableWithQuery(table, query);
    }
    void ShowWeaponTypeTable(TableLayout table, string target, float min, float max)
    {
        string query = @"select * from WeaponTypeView";

        switch (target)
        {
            case "Req Lv":
                query += "\nwhere required_level between " + min.ToString() + " and " + max.ToString();
                break;
        }

        //Debug.Log(query);
        ReadTableWithQuery(table, query);
    }



    void ShowBossTable(TableLayout table, int orderType)
    {
        string query = @"select * from BossView";

        switch (orderType)
        {
            case 0:
                break;
            case 1:
                query += "\norder by boss_name";
                break;
            case -1:
                query += "\norder by boss_name desc";
                break;
            case 2:
                query += "\norder by element_name";
                break;
            case -2:
                query += "\norder by element_name desc";
                break;
            case 3:
                query += "\norder by HP";
                break;
            case -3:
                query += "\norder by HP desc";
                break;
            case 4:
                query += "\norder by MP";
                break;
            case -4:
                query += "\norder by MP desc";
                break;
            case 5:
                query += "\norder by required_level";
                break;
            case -5:
                query += "\norder by required_level desc";
                break;
            case 6:
                query += "\norder by required_member";
                break;
            case -6:
                query += "\norder by required_member desc";
                break;
        }

        ReadTableWithQuery(table, query);
    }
    void ShowBossTable(TableLayout table, string target, string text)
    {
        string query = @"select * from BossView";

        switch (target)
        {
            case "Name":
                query += "\nwhere boss_name like '%" + text + "%'";
                break;
            case "Element":
                query += "\nwhere element_name like '%" + text + "%'";
                break;
        }

        //Debug.Log(query);
        ReadTableWithQuery(table, query);
    }
    void ShowBossTable(TableLayout table, string target, float min, float max)
    {
        string query = @"select * from BossView";

        switch (target)
        {
            case "HP":
                query += "\nwhere HP between " + min.ToString() + " and " + max.ToString();
                break;
            case "MP":
                query += "\nwhere MP between " + min.ToString() + " and " + max.ToString();
                break;
            case "Req Lv":
                query += "\nwhere required_level between " + min.ToString() + " and " + max.ToString();
                break;
            case "Req Member":
                query += "\nwhere required_member between " + min.ToString() + " and " + max.ToString();
                break;
        }

        //Debug.Log(query);
        ReadTableWithQuery(table, query);
    }



    void ShowRaidPartyTable(TableLayout table, int orderType)
    {
        string query = @"select * from RaidPartyView";
        
        if (!isGM)
        {
            query += @"	
	            where leader not in
		        (select user_name
		        from dbo.users as users
		        inner join dbo.banList as banList
		        on users.user_ID = banList.user_ID)";
        }

        switch (orderType)
        {
            case 0:
                break;
            case 1:
                query += "\norder by party_name";
                break;
            case -1:
                query += "\norder by party_name desc";
                break;
            case 2:
                query += "\norder by boss_name";
                break;
            case -2:
                query += "\norder by boss_name desc";
                break;
            case 3:
                query += "\norder by leader";
                break;
            case -3:
                query += "\norder by leader desc";
                break;
            case 4:
                query += "\norder by required_member";
                break;
            case -4:
                query += "\norder by required_member desc";
                break;
            case 5:
                query += "\norder by current_member";
                break;
            case -5:
                query += "\norder by current_member desc";
                break;
        }

        ReadTableWithQuery(table, query);
    }
    void ShowRaidPartyTable(TableLayout table, string target, string text)
    {
        string query = @"select * from RaidPartyView";

        switch (target)
        {
            case "Name":
                query += "\nwhere party_name like '%" + text + "%'";
                break;
            case "Boss":
                query += "\nwhere boss_name like '%" + text + "%'";
                break;
            case "Leader":
                query += "\nwhere leader like '%" + text + "%'";
                break;
        }

        if (!isGM)
        {
            query += @"	and leader not in
		        (select user_name
		        from dbo.users as users
		        inner join dbo.banList as banList
		        on users.user_ID = banList.user_ID)";
        }
        //Debug.Log(query);
        ReadTableWithQuery(table, query);
    }
    void ShowRaidPartyTable(TableLayout table, string target, float min, float max)
    {
        string query = @"select * from RaidPartyView";

        switch (target)
        {
            case "Req Member":
                query += "\nwhere required_member between " + min.ToString() + " and " + max.ToString();
                break;
            case "Cur Member":
                query += "\nwhere current_member between " + min.ToString() + " and " + max.ToString();
                break;
        }

        if (!isGM)
        {
            query += @"	and leader not in
		        (select user_name
		        from dbo.users as users
		        inner join dbo.banList as banList
		        on users.user_ID = banList.user_ID)";
        }
        //Debug.Log(query);
        ReadTableWithQuery(table, query);
    }



    void ShowAuctionTable(TableLayout table, int orderType)
    {
        string query = @"select * from AuctionView";

        if (!isGM)
        {
            query += @"
	            where seller not in
		        (select user_name
		        from dbo.users as users
		        inner join dbo.banList as banList
		        on users.user_ID = banList.user_ID)";
        }

        switch (orderType)
        {
            case 0:
                break;
            case 1:
                query += "\norder by weapon_name";
                break;
            case -1:
                query += "\norder by weapon_name desc";
                break;
            case 2:
                query += "\norder by weapon_type_name";
                break;
            case -2:
                query += "\norder by weapon_type_name desc";
                break;
            case 3:
                query += "\norder by element_name";
                break;
            case -3:
                query += "\norder by element_name desc";
                break;
            case 4:
                query += "\norder by damage_coefficient";
                break;
            case -4:
                query += "\norder by damage_coefficient desc";
                break;
            case 5:
                query += "\norder by price";
                break;
            case -5:
                query += "\norder by price desc";
                break;
            case 6:
                query += "\norder by seller";
                break;
            case -6:
                query += "\norder by seller desc";
                break;
        }

        ReadTableWithQuery(table, query);
    }
    void ShowAuctionTable(TableLayout table, string target, string text)
    {
        string query = @"select * from AuctionView";

        switch (target)
        {
            case "Name":
                query += "\nwhere weapon_name like '%" + text + "%'";
                break;
            case "Type":
                query += "\nwhere weapon_type_name like '%" + text + "%'";
                break;
            case "Element":
                query += "\nwhere element_name like '%" + text + "%'";
                break;
            case "Seller":
                query += "\nwhere seller like '%" + text + "%'";
                break;
        }

        if (!isGM)
        {
            query += @" and seller not in
		        (select user_name
		        from dbo.users as users
		        inner join dbo.banList as banList
		        on users.user_ID = banList.user_ID)";
        }
        //Debug.Log(query);
        ReadTableWithQuery(table, query);
    }
    void ShowAuctionTable(TableLayout table, string target, float min, float max)
    {
        string query = @"select * from AuctionView";

        switch (target)
        {
            case "Damage":
                query += "\nwhere damage_coefficient between " + min.ToString() + " and " + max.ToString();
                break;
            case "Price":
                query += "\nwhere price between " + min.ToString() + " and " + max.ToString();
                break;
        }

        if (!isGM)
        {
            query += @" and seller not in
		        (select user_name
		        from dbo.users as users
		        inner join dbo.banList as banList
		        on users.user_ID = banList.user_ID)";
        }
        //Debug.Log(query);
        ReadTableWithQuery(table, query);
    }



    void ShowBanTable(TableLayout table, int orderType)
    {
        string query = @"select * from BanView";

        switch (orderType)
        {
            case 0:
                break;
            case 1:
                query += "\norder by banned_user_name";
                break;
            case -1:
                query += "\norder by banned_user_name desc";
                break;
            case 2:
                query += "\norder by GM_name";
                break;
            case -2:
                query += "\norder by GM_name desc";
                break;
            case 3:
                query += "\norder by GM_grade";
                break;
            case -3:
                query += "\norder by GM_grade desc";
                break;
            case 4:
                query += "\norder by banned_date";
                break;
            case -4:
                query += "\norder by banned_date desc";
                break;
            case 5:
                query += "\norder by unban_date";
                break;
            case -5:
                query += "\norder by unban_date desc";
                break;
        }

        if (!isGM)
        {
            query = " ";
        }

        ReadTableWithQuery(table, query);
    }
    void ShowBanTable(TableLayout table, string target, string text)
    {
        string query = @"select * from BanView";

        switch (target)
        {
            case "Banned User":
                query += "\nwhere banned_user_name like '%" + text + "%'";
                break;
            case "GM in Charge":
                query += "\nwhere GM_name like '%" + text + "%'";
                break;
            case "GM Grade":
                query += "\nwhere GM_grade like '%" + text + "%'";
                break;
            case "Banned date":
                query += "\nwhere banned_date like '%" + text + "%'";
                break;
            case "Unban date":
                query += "\nwhere unban_date like '%" + text + "%'";
                break;
        }

        if (!isGM)
        {
            query = " ";
        }
        //Debug.Log(query);
        ReadTableWithQuery(table, query);
    }



    void ShowElementTable(TableLayout table, int orderType)
    {
        string query = @"select * from ElementView";

        switch (orderType)
        {
            case 0:
                break;
            case 1:
                query += "\norder by Name";
                break;
            case -1:
                query += "\norder by Name desc";
                break;
            case 2:
                query += "\norder by Synergy";
                break;
            case -2:
                query += "\norder by Synergy desc";
                break;
            case 3:
                query += "\norder by Weak";
                break;
            case -3:
                query += "\norder by Weak desc";
                break;
        }

        ReadTableWithQuery(table, query);
    }
    void ShowElementTable(TableLayout table, string target, string text)
    {
        string query = @"select * from ElementView";

        switch (target)
        {
            case "Element Name":
                query += "\nwhere Name like '%" + text + "%'";
                break;
            case "Synergy Element":
                query += "\nwhere Synergy like '%" + text + "%'";
                break;
            case "Weak Element":
                query += "\nwhere Weak like '%" + text + "%'";
                break;
        }

        //Debug.Log(query);
        ReadTableWithQuery(table, query);
    }



    void ReadTableWithQuery(TableLayout table, string query)
    {
        //table.ClearRows();
        while (table.Rows.Count > 1)
            DestroyImmediate(table.Rows[table.Rows.Count - 1].gameObject);
        //table.AddRow();

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


    int CurTable()
    {
        int tmp = 0;
        for (int i = 0; i < tables.Length; i++)
            if (tables[i].gameObject.activeSelf)
            {
                tmp = i;
                break;
            }
        return tmp;
    }

    void GMInit()
    {
        string query = @"revoke insert, delete on database::OnlineRPG_DB to kjunwoo234
                        EXEC sp_droprolemember 'GMRole', 'kjunwoo234';";

        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        rdr.Close();
        rdr.Dispose();
    }

    public void OnVauleChangedGM(Toggle toggle)
    {
        isGM = toggle.isOn;

        string query;

        if (isGM)        query = @"EXEC sp_addrolemember 'GMRole', 'kjunwoo234';";
        else             query = @"EXEC sp_droprolemember 'GMRole', 'kjunwoo234';";

        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        rdr.Close();
        rdr.Dispose();


        int curTable = CurTable();
        OnClickShowTableButton(curTable);
    }

    void InitTextSearchDropDown(TableLayout table)
    {
        textSearch.options.Clear();

        for (int i = 0; i < table.Rows[0].CellCount; i++)
            if (table.Rows[0].Cells[i].GetComponentInChildren<TextMeshProUGUI>().fontStyle != (FontStyles.Italic | FontStyles.Bold))
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                option.text = table.Rows[0].Cells[i].GetComponentInChildren<TextMeshProUGUI>().text;
                textSearch.options.Add(option);
            }

        textSearch.value = 0;
        textSearch.RefreshShownValue();
    }

    void InitValueSearchDropDown(TableLayout table)
    {
        valueSearch.options.Clear();

        for (int i = 0; i < table.Rows[0].CellCount; i++)
            if (table.Rows[0].Cells[i].GetComponentInChildren<TextMeshProUGUI>().fontStyle == (FontStyles.Italic | FontStyles.Bold))
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
                option.text = table.Rows[0].Cells[i].GetComponentInChildren<TextMeshProUGUI>().text;
                valueSearch.options.Add(option);
            }

        valueSearch.value = 0;
        valueSearch.RefreshShownValue();
    }

    public void OnValueChangeTextSearch(TMP_InputField inputField)
    {
        int tmp = CurTable();

        switch (tmp)
        {
            case 0:
                if (inputField.text == "")
                    ShowUserTable(tables[tmp], 0);
                ShowUserTable(tables[tmp], textSearch.options[textSearch.value].text, inputField.text);
                break;
            case 1:
                if (inputField.text == "")
                    ShowClassTable(tables[tmp], 0);
                ShowClassTable(tables[tmp], textSearch.options[textSearch.value].text, inputField.text);
                break;
            case 2:
                if (inputField.text == "")
                    ShowSkillTable(tables[tmp], 0);
                ShowSkillTable(tables[tmp], textSearch.options[textSearch.value].text, inputField.text);
                break;
            case 3:
                if (inputField.text == "")
                    ShowWeaponTypeTable(tables[tmp], 0);
                ShowWeaponTypeTable(tables[tmp], textSearch.options[textSearch.value].text, inputField.text);
                break;
            case 4:
                if (inputField.text == "")
                    ShowBossTable(tables[tmp], 0);
                ShowBossTable(tables[tmp], textSearch.options[textSearch.value].text, inputField.text);
                break;
            case 5:
                if (inputField.text == "")
                    ShowRaidPartyTable(tables[tmp], 0);
                ShowRaidPartyTable(tables[tmp], textSearch.options[textSearch.value].text, inputField.text);
                break;
            case 6:
                if (inputField.text == "")
                    ShowAuctionTable(tables[tmp], 0);
                ShowAuctionTable(tables[tmp], textSearch.options[textSearch.value].text, inputField.text);
                break;
            case 7:
                if (inputField.text == "")
                    ShowBanTable(tables[tmp], 0);
                ShowBanTable(tables[tmp], textSearch.options[textSearch.value].text, inputField.text);
                break;
            case 8:
                if (inputField.text == "")
                    ShowElementTable(tables[tmp], 0);
                ShowElementTable(tables[tmp], textSearch.options[textSearch.value].text, inputField.text);
                break;

        }
    }

    public void OnValueChangeValueSearch(TMP_InputField input)
    {
        int tmp = CurTable();
        float tmpMin, tmpMax;

        if (inputMin.text == "") tmpMin = float.MinValue;
        else tmpMin = int.Parse(inputMin.text);
        if (inputMax.text == "") tmpMax = float.MaxValue;
        else tmpMax = int.Parse(inputMax.text);

        switch (tmp)
        {
            case 0:
                ShowUserTable(tables[tmp], valueSearch.options[valueSearch.value].text, tmpMin, tmpMax);
                break;
            case 1:
                ShowClassTable(tables[tmp], valueSearch.options[valueSearch.value].text, tmpMin, tmpMax);
                break;
            case 2:
                ShowSkillTable(tables[tmp], valueSearch.options[valueSearch.value].text, tmpMin, tmpMax);
                break;
            case 3:
                ShowWeaponTypeTable(tables[tmp], valueSearch.options[valueSearch.value].text, tmpMin, tmpMax);
                break;
            case 4:
                ShowBossTable(tables[tmp], valueSearch.options[valueSearch.value].text, tmpMin, tmpMax);
                break;
            case 5:
                ShowRaidPartyTable(tables[tmp], valueSearch.options[valueSearch.value].text, tmpMin, tmpMax);
                break;
            case 6:
                ShowAuctionTable(tables[tmp], valueSearch.options[valueSearch.value].text, tmpMin, tmpMax);
                break;
        }
    }


    public void OnClickReset()
    {
        string query = "exec RefreshTable";

        try
        {
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader rdr = command.ExecuteReader();

            rdr.Close();
            rdr.Dispose();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        int tmp = CurTable();

        OnClickShowTableButton(tmp);
    }

    public void OnClickRandomAdd()
    {
        int tmp = CurTable();

        try
        {
            switch (tmp)
            {
                case 0:
                    RandomAddUsers();
                    break;
                case 1:
                    RandomAddClass();
                    break;
                case 2:
                    RandomAddSkill();
                    break;
                case 3:
                    RandomAddWeaponType();
                    break;
                case 4:
                    RandomAddBoss();
                    break;
                case 5:
                    RandomAddBossRaid();
                    break;
                case 6:
                    RandomAddAuction();
                    break;
                case 7:
                    RandomAddBan();
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }        
    }
    string RandomAddUsers()
    {
        int userSize = CountUsers();
        int classSize = CountClass();

        userSize++;
        string randID = userSize.ToString("D3");
        string randName = "Player" + UnityEngine.Random.Range(0, 100000).ToString("D5");
        int randLv = UnityEngine.Random.Range(12, 201);
        string randomClass = UnityEngine.Random.Range(1, classSize + 1).ToString("D3");
        //string randomWeapon;

        string query = "insert into users values('" + randID + "', '" + randName + "', " + randLv + ", '" + randomClass + "', NULL);";
        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();
        rdr.Close();
        rdr.Dispose();

        OnClickShowTableButton(0);
        return randID;
    }
    void RandomAddClass()
    {
        int classSize = CountClass();

        classSize++;
        string randID = classSize.ToString("D3");
        string randName = "Class" + UnityEngine.Random.Range(0, 100000).ToString("D5");
        int randReqLev = UnityEngine.Random.Range(0, 12);
        int randDefaultHP = UnityEngine.Random.Range(50, 151);
        int randDefaultMP = UnityEngine.Random.Range(50, 151);
        //string randomWeapon;

        string query = "insert into class values('" + randID + "', '" + randName + "', "
            + randReqLev + ", " + randDefaultHP + ", " + randDefaultMP + ");";
        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();
        rdr.Close(); // <- too easy to forget
        rdr.Dispose(); // <- too easy to forget

        OnClickShowTableButton(1);
    }
    void RandomAddSkill()
    {
        int skillSize = CountSkill();
        int classSize = CountClass();
        int elementSize = CountElement();

        skillSize++;
        string randID = skillSize.ToString("D3");
        string randName = "Skill" + UnityEngine.Random.Range(0, 100000).ToString("D5");
        string randReqClass = UnityEngine.Random.Range(1, classSize + 1).ToString("D3");
        int randReqLev = UnityEngine.Random.Range(0, 200);
        float randDmg = UnityEngine.Random.Range(0f, 5f);
        string randomElement = UnityEngine.Random.Range(1, elementSize + 1).ToString("D3");
        float randMPCost = UnityEngine.Random.Range(10f, 100f);
        //string randomWeapon;

        string query = "insert into skill values('" + randID + "', '" + randName + "', '"
            + randReqClass + "', " + randReqLev + ", " + randDmg + ", '" + randomElement + "', " + randMPCost + ");";
        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();
        rdr.Close();
        rdr.Dispose();

        OnClickShowTableButton(2);
    }
    void RandomAddWeaponType()
    {
        int weaponTypeSize = CountWeaponType();
        int classSize = CountClass();

        weaponTypeSize++;
        string randID = weaponTypeSize.ToString("D3");
        string randName = "Type" + UnityEngine.Random.Range(0, 100000).ToString("D5");
        string randReqClass = UnityEngine.Random.Range(1, classSize + 1).ToString("D3");
        int randReqLev = UnityEngine.Random.Range(0, 100);

        string query = "insert into weaponType values('" + randID + "', '" + randName + "', '"
            + randReqClass + "', " + randReqLev + ");";

        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();
        rdr.Close();
        rdr.Dispose();

        OnClickShowTableButton(3);
    }
    void RandomAddBoss()
    {
        int bossSize = CountBoss();
        int elementSize = CountElement();


        bossSize++;
        string randID = bossSize.ToString("D3");
        string randName = "Boss" + UnityEngine.Random.Range(0, 100000).ToString("D5");
        string randReqClass = UnityEngine.Random.Range(1, elementSize + 1).ToString("D3");
        float randHP = UnityEngine.Random.Range(500f, 1000000f);
        float randMP = UnityEngine.Random.Range(500f, 3000f);
        int randReqLv = UnityEngine.Random.Range(5, 200);
        int randReqMember = UnityEngine.Random.Range(1, 20);

        string query = "insert into boss values('" + randID + "', '" + randName + "', '"
            + randReqClass + "', " + randHP + ", " + randMP + ", " + randReqLv + ", " + randReqMember + ");";

        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();
        rdr.Close();
        rdr.Dispose();

        OnClickShowTableButton(4);
    }
    void RandomAddBossRaid()
    {
        int bossRaidSize = CountBossRaid();
        int bossSize = CountBoss();
        int usersSize = CountUsers();
        int reqMember = 0;

        string randBoss = UnityEngine.Random.Range(1, bossSize + 1).ToString("D3");

        string query = @"select required_member
            from dbo.boss as boss
	        where boss.boss_ID = " + randBoss;


        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        while (rdr.Read())
            for (int i = 0; i < rdr.FieldCount; i++)
                reqMember = int.Parse(rdr[i].ToString());

        rdr.Close();
        rdr.Dispose();

        bossRaidSize++;
        string randID = bossRaidSize.ToString("D3");
        string randName = "Party" + UnityEngine.Random.Range(0, 100000).ToString("D5");
        //string randBoss = UnityEngine.Random.Range(1, bossSize + 1).ToString("D3");
        string randLeader = UnityEngine.Random.Range(1, usersSize + 1).ToString("D3");
        int randCurMember = UnityEngine.Random.Range(1, reqMember);

        query = "insert into raidPartyRoom values('" + randID + "', '" + randName + "', '"
            + randBoss + "', '" + randLeader + "', " + randCurMember + ");";

        command = new SqlCommand(query, connection);
        rdr = command.ExecuteReader();

        rdr.Close();
        rdr.Dispose();

        OnClickShowTableButton(5);
    }
    void RandomAddAuction()
    {
        int weaponSize = CountWeapon();
        int weaponTypeSize = CountWeaponType();
        int elementSize = CountElement();

        int auctionSize = CountAuction();
        int userSize = CountUsers();

        weaponSize++;
        string randWeaponID = weaponSize.ToString("D3");
        string randName = "Weapon" + UnityEngine.Random.Range(0, 100000).ToString("D5");
        string randWeaponType = UnityEngine.Random.Range(1, weaponTypeSize + 1).ToString("D3");
        string randElement = UnityEngine.Random.Range(1, elementSize + 1).ToString("D3");
        float randDmg = UnityEngine.Random.Range(0f, 5f);

        string query = "insert into weapon values('" + randWeaponID + "', '" + randName + "', '"
            + randWeaponType + "', '" + randElement + "', " + randDmg + ");";

        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();
        rdr.Close();
        rdr.Dispose();


        auctionSize++;
        string auctionID = auctionSize.ToString("D3");
        //randWeaponID;
        string randUsers = UnityEngine.Random.Range(1, userSize + 1).ToString("D3");
        int randPrice = UnityEngine.Random.Range(5000, 100000);

        query = "insert into auction values('" + auctionID + "', '" + randWeaponID + "', '"
            + randUsers + "', " + randPrice + ");";

        command = new SqlCommand(query, connection);
        rdr = command.ExecuteReader();
        rdr.Close();
        rdr.Dispose();

        OnClickShowTableButton(6);
    }
    void RandomAddBan()
    {
        int banSize = CountBan();
        int usersSize = CountUsers();
        int gmSize = CountGM();

        banSize++;
        string randID = banSize.ToString("D3");
        string randUsers = RandomAddUsers();

        string randGM = UnityEngine.Random.Range(1, gmSize + 1).ToString("D3");

        string randBannedDate = DateTime.Now.ToString(("yyyy-MM-dd HH:mm:ss"));
        string randUnbanDate = DateTime.MaxValue.ToString(("yyyy-MM-dd HH:mm:ss"));

        string query = "insert into banList values('" + randID + "', '" + randUsers + "', '" + randGM + "', '"
            + randBannedDate + "', '" + randUnbanDate + "');";

        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        rdr.Close();
        rdr.Dispose();

        OnClickShowTableButton(7);
    }



    int CountUsers()
    {
        string query;

        query = @"select count(*)
            from dbo.users";

        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        int tableSize = 0;
        while (rdr.Read())
            for (int i = 0; i < rdr.FieldCount; i++)
                tableSize = int.Parse(rdr[i].ToString());

        rdr.Close();
        rdr.Dispose();

        return tableSize;
    }
    int CountClass()
    {
        string query;

        query = @"select count(*)
            from dbo.class";

        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        int tableSize = 0;
        while (rdr.Read())
            for (int i = 0; i < rdr.FieldCount; i++)
                tableSize = int.Parse(rdr[i].ToString());

        rdr.Close();
        rdr.Dispose();

        return tableSize;
    }
    int CountElement()
    {
        string query;

        query = @"select count(*)
            from dbo.element";

        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        int tableSize = 0;
        while (rdr.Read())
            for (int i = 0; i < rdr.FieldCount; i++)
                tableSize = int.Parse(rdr[i].ToString());

        rdr.Close();
        rdr.Dispose();

        return tableSize;
    }
    int CountSkill()
    {
        string query;

        query = @"select count(*)
            from dbo.skill";

        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        int tableSize = 0;
        while (rdr.Read())
            for (int i = 0; i < rdr.FieldCount; i++)
                tableSize = int.Parse(rdr[i].ToString());

        rdr.Close();
        rdr.Dispose();

        return tableSize;
    }
    int CountWeaponType()
    {
        string query;

        query = @"select count(*)
            from dbo.weaponType";

        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        int tableSize = 0;
        while (rdr.Read())
            for (int i = 0; i < rdr.FieldCount; i++)
                tableSize = int.Parse(rdr[i].ToString());

        rdr.Close();
        rdr.Dispose();

        return tableSize;
    }
    int CountBoss()
    {
        string query;

        query = @"select count(*)
            from dbo.boss";

        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        int tableSize = 0;
        while (rdr.Read())
            for (int i = 0; i < rdr.FieldCount; i++)
                tableSize = int.Parse(rdr[i].ToString());

        rdr.Close();
        rdr.Dispose();

        return tableSize;
    }
    int CountBossRaid()
    {
        string query;

        query = @"select count(*)
            from dbo.raidPartyRoom";

        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        int tableSize = 0;
        while (rdr.Read())
            for (int i = 0; i < rdr.FieldCount; i++)
                tableSize = int.Parse(rdr[i].ToString());

        rdr.Close();
        rdr.Dispose();

        return tableSize;
    }
    int CountAuction()
    {
        string query;

        query = @"select count(*)
            from dbo.auction";

        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        int tableSize = 0;
        while (rdr.Read())
            for (int i = 0; i < rdr.FieldCount; i++)
                tableSize = int.Parse(rdr[i].ToString());

        rdr.Close();
        rdr.Dispose();

        return tableSize;
    }
    int CountBan()
    {
        string query;

        query = @"select count(*)
            from dbo.banList";

        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        int tableSize = 0;
        while (rdr.Read())
            for (int i = 0; i < rdr.FieldCount; i++)
                tableSize = int.Parse(rdr[i].ToString());

        rdr.Close();
        rdr.Dispose();

        return tableSize;
    }
    int CountWeapon()
    {
        string query;

        query = @"select count(*)
            from dbo.weapon";

        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        int tableSize = 0;
        while (rdr.Read())
            for (int i = 0; i < rdr.FieldCount; i++)
                tableSize = int.Parse(rdr[i].ToString());

        rdr.Close();
        rdr.Dispose();

        return tableSize;
    }
    int CountGM()
    {
        string query;

        query = @"select count(*)
            from dbo.GM";

        SqlCommand command = new SqlCommand(query, connection);
        SqlDataReader rdr = command.ExecuteReader();

        int tableSize = 0;
        while (rdr.Read())
            for (int i = 0; i < rdr.FieldCount; i++)
                tableSize = int.Parse(rdr[i].ToString());

        rdr.Close();
        rdr.Dispose();

        return tableSize;
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

