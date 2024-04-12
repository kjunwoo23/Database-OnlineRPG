using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class ConnectManager : MonoBehaviour
{
    public static ConnectManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //SQL 인증
        string connectionString = "Server=DESKTOP-J9EJ7HN;Database=OnlineRPG_DB;User Id=kjunwoo234;Pwd=1234;";

        using (SqlConnection connection = new SqlConnection(connectionString))
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

            // DB 서버 접속 종료
            connection.Close();
        }
    }

        // Update is called once per frame
        void Update()
    {
        
    }
}
