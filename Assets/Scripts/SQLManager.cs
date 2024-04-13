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

    string connectionString = "Server=DESKTOP-J9EJ7HN;Database=OnlineRPG_DB;User Id=kjunwoo234;Pwd=1234;";
    SqlConnection connection;
    

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        TryConnect();

        connection = new SqlConnection(connectionString);
        connection.Open();

        ReadTable(tables[0], "dbo.users");
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
                ReadTable(tables[i], "dbo.users");
                break;
            case 1:
                ReadTable(tables[i], "dbo.class");
                break;
            case 2:
                ReadTable(tables[i], "dbo.skill");
                break;
            case 3:
                ReadTable(tables[i], "dbo.weaponType");
                break;
            case 4:
                ReadTable(tables[i], "dbo.boss");
                break;
            case 5:
                ReadTable(tables[i], "dbo.raidPartyRoom");
                break;
            case 6:
                ReadTable(tables[i], "dbo.auction");
                break;

        }
    }

    void ReadTable(TableLayout table, string tableName)
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
