using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ConsoleApplication1;
using MySql.Data.MySqlClient;
using System.Collections;

namespace ConsoleApplication1
{
    class Program
    {
        //set Ip and Port
        private static System.Timers.Timer aTimer;
        private Thread threadConnect;//connect Thread
        static string MySqlConnectString = "datasource=127.0.0.1;port=3306;username=server;password=;database=project;";
        const string ip = "140.136.150.77";
        //const string ip = "114.25.5.233";
        //const string ip = "192.168.1.105";
        public static ServerThread st;
        const int _port = 8877;
        static Socket[] arraySocket;
        static bool[] offlinebool;
        static string[] offlinestring;
        static int SocketNum;
        static int DataLength = 1024;
        private static bool isSend;
        static byte[] buffer = new byte[1024];
        static int attempts = 0;
        static int monsternumber = 10;
        static int gobalspeed = 1000;
        static int offlinetime = 1;
        static int globalrest = 0;
        static int globalhp = 100;
        static int globaltype = 0;
        static float globaly = 0;
        static String S = null;
        static int globalrangemin, globalrangemax;
        static int globalstart;
        static int rangemax = -260;
        static int rangemin = -45; 
        public static Timer[] timers;
        public static Timer offtimers;
        static char[] delimitchar = {',', ':','\t'};
        //socket can    use specifies scheme
        public static Timer ThreadTimer;
        public static bool[] updatebuffer;
        public static StatusChecker[] monsters;
        public struct xyz
        {
            public int hp;
            public int rightx;
            public int leftx;
            public float y;
        }
        public static xyz[] storexyz;

        static void Main(string[] args)
        {
            //DataDelete("1");
            //NewData("3", "3Joe", "3Ho", "china");
            //NewAccountXY("1", "2", "3");
            //DataUpdateX("0", "1", "Test");
            //NewMonster(0,1,0, 1, 2, 100);
            definebool();
            DataDelete("monster");
           ThreadTimers(0,40,0,-45,260,-11,1000,10);



            //ThreadTimers(10,10,1,-45,260,-230,2000,10);
            //Console.ReadLine();
            //ThreadTimers(20,10,2,-45, 260,-459, 1500, 10);
            //Console.ReadLine();
            //ThreadTimers(30,10,3,-45, 260,-715, 750, 10);
            //Console.ReadLine();
            //ThreadTimers(40,10,4,-45, 260, -986, 2500, 10);
            //Console.ReadLine();
            //Console.WriteLine(":3");
            Listen2();
            connectoffline(2);

            //Updateoffline("bar",0);
            //UpdateMonsterHp(0, 0, 20, 99999);
            // UpdateMonster(0, 0, 40);
            Console.ReadLine();

        }
        public static void definebool()
        {
            updatebuffer = new bool[100];
            for (int i = 0; i < 100; i++)
            {
                updatebuffer[i] = true;
            }
            offlinebool = new bool[100];
            offlinestring = new string[100];
        }

        public static void connectoffline(int localofftime)
        {
            offlinetime = localofftime;
            Thread Timers = new Thread(offlinetimers);
            Timers.Start();
            
        }
        public static void ThreadTimers(int monsterstart,int monsternum,int type,int rangemin,int rangemax,float rangeymax,int localspeed,int rangerest)
        {
            //Random r = new Random();
            //r.Next(rangemin, rangemax);
            globaltype = type;
            globalstart = monsterstart;
            gobalspeed = localspeed;
            monsternumber = monsternum;
            globalrangemin = rangemin;
            globalrangemax = rangemax;
            globalrest = rangerest;
            globaly = rangeymax;
 
            Thread Timers = new Thread(TestTimer);
            Timers.Start();
            Console.WriteLine("1");
        }
        private static void TestTimer()
        {
            var autoEvent = new AutoResetEvent(false);
            monsters = new StatusChecker[monsternumber];
            timers = new Timer[monsternumber];
            ////////////
            storexyz = new xyz[5];
            storexyz[0].rightx = -45;
            storexyz[0].leftx = 260;
            storexyz[0].y = -11f;
            storexyz[0].hp = 100;
            storexyz[1].rightx = -45;
            storexyz[1].leftx = 260;
            storexyz[1].y = -230f;
            storexyz[1].hp = 200;
            storexyz[2].rightx = -45;
            storexyz[2].leftx = 260;
            storexyz[2].y = -459f;
            storexyz[2].hp = 300;
            storexyz[3].rightx = -45;
            storexyz[3].leftx = 260;
            storexyz[3].y = -715f;
            storexyz[3].hp = 300;
            storexyz[4].rightx = -45;
            storexyz[4].leftx = 260;
            storexyz[4].y = -986f;
            storexyz[4].hp = 400;
            //////
            int count = 0;
            globaltype = 0;
            //DataDelete("monster");
            for (int i = 0; i < monsters.Length; i++)
            {
                globaltype = i / 10;
                //Console.WriteLine("globaltype = " + globaltype);
                //Console.WriteLine("i = " + i);
                NewMonster(i, 1, globaltype, 0, storexyz[globaltype].y, storexyz[globaltype].hp);
                //Console.WriteLine("globaltype = " + globaltype);
                // Console.WriteLine("globalstart = "+ globalstart);
                Random r = new Random();
                monsters[i]= new StatusChecker(0, storexyz[globaltype].hp, storexyz[globaltype].rightx, storexyz[globaltype].leftx, i, globalrest);
                timers[i] = new Timer(monsters[i].CheckStatus,
                           autoEvent, r.Next(0, 500), gobalspeed);
            }
            
             //var statusChecker = new StatusChecker(10, 100, 0, 10,100);
            //    var statusChecker1 = new StatusChecker(10, 100, 0, 10);
            //Console.WriteLine("{0:h:mm:ss.fff} Creating timer.\n",
                 // DateTime.Now);
            //var stateTimer = new Timer(statusChecker.CheckStatus,
             //             autoEvent, 0, gobalspeed);
           //var stateTimer1 = new Timer(monsters[i].CheckStatus,autoEvent, 0, 50);

        }
        public static void Listen2()
        {
            Array.Resize(ref arraySocket, 1);
            arraySocket[0] = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            arraySocket[0].Bind(new IPEndPoint(IPAddress.Parse(ip), _port));
            arraySocket[0].Listen(10);
            Console.WriteLine("wait Connecd...1");
            AcceptThread();
        }
        public static void offlinetimers()
        {
            
            var autoEvent = new AutoResetEvent(false);
            offtimers = new Timer(offline, autoEvent, 0, offlinetime);
            //timers[i] = new Timer(monsters[i].CheckStatus,autoEvent, 0, gobalspeed);
        }

        public static void offline(Object stateInfo)
        {
            for (int i = 1; i < arraySocket.Length; i++)
            {
                if (arraySocket[i] != null)
                {
                    if (arraySocket[i].Connected == false && offlinebool[i] == true) 
                    {
                        offlinebool[i] = false;
                        Console.WriteLine("arraySocket[i].Connected :" + arraySocket[i].Connected);
                        Console.WriteLine("offlinestring[i] :" + offlinestring[i]);
                        Updateoffline(offlinestring[i], 0);
                     }
                }
            }
        }

        public static void AcceptThread()
        {
            bool FlagSocket = false;
            Console.WriteLine("wait Connecd...2");
            for (int i = 1; i < arraySocket.Length; i++)
            {
                if (arraySocket[i] != null)
                {
                    if (arraySocket[i].Connected == false)
                    {
                        SocketNum = i;
                        FlagSocket = true;
                        break;
                    }
                }
            }
            if(FlagSocket == false)
            {
                SocketNum = arraySocket.Length;
                Array.Resize(ref arraySocket, SocketNum+1);
            }
            Console.WriteLine("wait Connecd...3");
            Thread SocketAccept = new Thread(FSocketFunction);
            SocketAccept.Start();

        }
        private static void FSocketFunction()
        {
            Console.WriteLine("wait Connecd...4");
            try
            {
                Console.WriteLine("Wait Client Connected....");
                arraySocket[SocketNum] = arraySocket[0].Accept();
                Console.WriteLine("Client Connected");
                int FSFnum = SocketNum;
                AcceptThread();
                long IntAcceptData;
                byte[] clientData = new byte[DataLength];
                bool one = true;
                offlinebool[FSFnum] = true;
                while (true)
                {              
                    IntAcceptData = arraySocket[FSFnum].Receive(clientData);
                    // char[] bufferchar = new char[IntAcceptData];
                    // using ()
                    S = Encoding.Default.GetString(clientData);
                    S = S.Substring(0, (int)IntAcceptData);
                    
                    Console.WriteLine("arraySocket{0}.receive = " + S, FSFnum);
                    //BroadcastClient();
                    
                    AnalysisString(S, FSFnum);
                    //
                    //Console.WriteLine("123");
                    //Console.WriteLine("S" + S);
                    // S = null;
                    // Console.WriteLine("S" + S);
                    if (one)
                    {
                       Console.WriteLine("wait Connecd...5");
                       arraySocket[FSFnum].Send(Encoding.ASCII.GetBytes("2," + FSFnum));
                        Console.WriteLine("2 , " + FSFnum);
                        one = false;
                    }

                }
            }
            catch
            {


            }
        }

        private static void AnalysisString(string clientData,int FSFnum)
        {
            string[] words = clientData.Split(delimitchar);
                    if (words[0].Equals("S"))
                    {
                    BroadcastClient(words[1]+","+ words[2]);
                // Console.WriteLine("words[1] = "+ words[1]);
                //  S = "";
                     }
                    else if (words[0].Equals("A"))
                    {
                    Console.WriteLine("words[1] = " + words[1]);
                // DataUpdateTable(words[1], FSFnum);
                    monsters[int.Parse(words[1])].start = false;
                    }
                    else if (words[0].Equals("M"))
                    {
                //Console.WriteLine("1...");
                //int buffer =  r.Next(0, 260 - (-45)) + (-45);
                //Console.WriteLine("2...");
                    monsters[int.Parse(words[1])].boolupdate = false;
                    //UpdateMonsterHp(int.Parse(words[1]), 0, 0, ((int.Parse(words[1])/10)+1)*100);
                    //Console.WriteLine("buffer = " + buffer);
                    //monsters[int.Parse(words[1])].monster.Monsterx =(float) buffer;
                    /////////////////
                    //globaltype = int.Parse(words[1]) / 10;
                    // var autoEvent = new AutoResetEvent(false);

                //timers[int.Parse(words[1])] = new Timer(monsters[int.Parse(words[1])].CheckStatus,
                //       autoEvent, r.Next(0, 500), gobalspeed);
            }
                    else if (words[0].Equals("1"))
                    {
                     offlinestring[FSFnum] = words[1];
                    //Console.WriteLine("words[1] = " + offlinestring[FSFnum]);
                    }
                    else if (words[0].Equals("H"))
                    {
                    BroadcastClient(words[0] + "," + words[1]);
                    }
                    else if (words[0].Equals("D"))
                    {
                    BroadcastClient(words[0] + "," + words[1]);
                    }
        }
        private static void BroadcastClient(string BroadString)
        {
            try
            {
                for (int num = 1; num < arraySocket.Length; num++)
                {
                    if (null != arraySocket[num] && arraySocket[num].Connected == true)
                    {
                        //string SendS = "EFGHI";
                        arraySocket[num].Send(Encoding.ASCII.GetBytes(BroadString));
                    }
                }
            }
            catch
            {


            }
        }
        private static string DataSearch(String Search,String From,String WhereString)
        {
            string buffer;
             //string sql =  "SELECT * FROM `user`";
            //string sql = "SELECT first_name FROM user WHERE id = 0";
            string sql = "SELECT " + Search + " FROM " + From + " WHERE " + WhereString;
            MySqlConnection myData = new MySqlConnection(MySqlConnectString);
                MySqlDataAdapter myDataAdapter = new MySqlDataAdapter(sql, myData);
            
                MySqlCommand myCommand = new MySqlCommand(sql, myData);
            try
            {
                myData.Open();
                MySqlDataReader myReader = myCommand.ExecuteReader();
                if (myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        //Console.WriteLine(myReader.GetString(0)+" - "+ myReader.GetString(1) + " - " + myReader.GetString(2)+" - "+ myReader.GetString(3));
                        buffer = myReader.GetString(0);
                        return buffer;
                    }
                }
                return null;
                Console.WriteLine("Connected");
                myReader.Close();
                myData.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("DataSearch error");
                Console.WriteLine(e);
                return null;
            }
               
        }
        private static bool DataSearchAccount(String Search)
        {

            string sql = "SELECT account FROM account WHERE " + Search + "";
            MySqlConnection myData = new MySqlConnection(MySqlConnectString);
            MySqlDataAdapter myDataAdapter = new MySqlDataAdapter(sql, myData);

            MySqlCommand myCommand = new MySqlCommand(sql, myData);
            try
            {
                myData.Open();
                MySqlDataReader myReader = myCommand.ExecuteReader();
                if (myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        Console.WriteLine(myReader.GetString(0) + " - " + myReader.GetString(1));
                    }

                }
                Console.WriteLine("Connected");
                myReader.Close();
                myData.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("DataSearch error");
                Console.WriteLine(e);
                return false;
            }
            return false;
        }

        private static void NewAccountXY(string stringOne, string stringTwo,string stringThree)
        {
            string query = "INSERT INTO charxy(`account`, `x`, `y`) VALUES ('" + stringOne + "', '" + stringTwo + "', '" + stringThree + "')";
            //string query = "INSERT INTO user(`id`, `first_name`, `last_name`, `address`) VALUES ('" + stringOne + "', '" + stringTwo + "', '" + stringThree + "', '" + stringFour + "')";
            //string query = "INSERT INTO user(`id`, `first_name`, `last_name`, `address`) VALUES ('2','徐', '子翔', '中和')"
            //string query = "INSERT INTO user(`id`, `first_name`, `last_name`, `address`) VALUES ('2','test1', 'test2', 'test3')";
            MySqlConnection datebaseConnection = new MySqlConnection(MySqlConnectString);
            MySqlCommand commandDatebase = new MySqlCommand(query, datebaseConnection);
            MySqlDataReader reader;
            try
            {
                datebaseConnection.Open();
                reader = commandDatebase.ExecuteReader();

                Console.WriteLine("DataNew sucessfull");
                datebaseConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("NewDatabase error");
                Console.WriteLine(e);
            }
        }
        private static void UpdateMonsterHp(int newid, int newmap, float newmonsterx,int Hp)
        {
            //string query = "UPDATE charxy SET " + UpdateTable + " = " + UpdateX + "' WHERE account = " + Account + ";";
            //string query = "UPDATE monster SET monsterx = '" + newmonsterx + "' WHERE id = '" + newid + "' AND map = '" + newmap + "';";
            //string query = "UPDATE monster SET x = '" + newmonsterx + "' , hp ='"+ Hp + "' WHERE id = '" + newid + "';";
            string query = "UPDATE monster SET hp ='" + Hp + "' WHERE id = '" + newid + "';";
            //string query = "UPDATE charxy SET x = '" + UpdateX + "' WHERE account = '" + Num + "';"
            MySqlConnection datebaseConnection = new MySqlConnection(MySqlConnectString);
            MySqlCommand commandDatebase = new MySqlCommand(query, datebaseConnection);
            MySqlDataReader reader;
            try
            {
                datebaseConnection.Open();
                reader = commandDatebase.ExecuteReader();
                datebaseConnection.Close();
            }
            catch (Exception e)
            {
                //Console.WriteLine("DataUpdateX error");
                Console.WriteLine(e);
            }
        }


        private static void UpdateMonster(int newid,int newmap, float newmonsterx)
        {
            //string query = "UPDATE charxy SET " + UpdateTable + " = " + UpdateX + "' WHERE account = " + Account + ";";
            //string query = "UPDATE monster SET monsterx = '" + newmonsterx + "' WHERE id = '" + newid + "' AND map = '" + newmap + "';";
            string query = "UPDATE monster SET x = '" + newmonsterx + "' WHERE id = '" + newid + "';";
            //string query = "UPDATE charxy SET x = '" + UpdateX + "' WHERE account = '" + Num + "';"
            MySqlConnection datebaseConnection = new MySqlConnection(MySqlConnectString);
            MySqlCommand commandDatebase = new MySqlCommand(query, datebaseConnection);
            MySqlDataReader reader;
            try
            {
                datebaseConnection.Open();
                reader = commandDatebase.ExecuteReader();
                datebaseConnection.Close();
            }
            catch (Exception e)
            {
                //Console.WriteLine("DataUpdateX error");
                Console.WriteLine(e);
            }
        }
        private static void NewMonster(int newid,int alive,int type,float newmonsterx, float newmonstery,int monsterhp)
        {
            //string query = "INSERT INTO monster(`ip`, `map`, `monsterx`, `monstery`, `monsterhp`) VALUES ('" + newid + "', '" + newmap + "', '" + newmonsterx + "')";
            //string query = "INSERT INTO `monster`(`ip`, `map`, `monsterx`, `monstery`, `monsterhp`) VALUES(0,0,1.0,2.0,100)";
            //string query = "INSERT INTO `monster` (`id`,`alive`,`type`,`x`,`y`,`hp`) VALUES('" + newid + "', '" + type + "', '" + newmonsterx + "', '" + newmonstery + "', '" + monsterhp + "')";
            string query = "INSERT INTO `monster` (`id`,`alive`,`type`,`x`,`y`,`hp`) VALUES('" + newid + "', '" + alive + "', '" + type + "', '" + newmonsterx + "', '" + newmonstery + "', '" + monsterhp + "')";
            //string query = "INSERT INTO monster(`account`, `x`, `y`) VALUES ('" + stringOne + "', '" + stringTwo + "', '" + stringThree + "')"
            MySqlConnection datebaseConnection = new MySqlConnection(MySqlConnectString);
            MySqlCommand commandDatebase = new MySqlCommand(query, datebaseConnection);
            MySqlDataReader reader;
            try
            {
                datebaseConnection.Open();
                reader = commandDatebase.ExecuteReader();

                //Console.WriteLine("DataNew sucessfull");
                datebaseConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("NewMonster error");
                Console.WriteLine(e);
            }
        }

        private static void DataUpdateX(string UpdateTable, string UpdateX, string Num)
        {
            //string query = "UPDATE charxy SET first_name = 'testtest' WHERE id = 0;";
            string query = "UPDATE charxy SET x = '" + UpdateX + "' WHERE account = '" + Num + "';";
            //string query    = "UPDATE charxy SET " + UpdateTable + "' = '" + UpdateX + "' WHERE account = " + Num + ";";
            //string query = "UPDATE charxy SET x = '" + UpdateX + "' WHERE account = " + Num +";";
            //string query = "UPDATE user SET x = '1' WHERE id = 0;";
            MySqlConnection datebaseConnection = new MySqlConnection(MySqlConnectString);
            MySqlCommand commandDatebase = new MySqlCommand(query, datebaseConnection);
            MySqlDataReader reader;
            try
            {
                datebaseConnection.Open();
                reader = commandDatebase.ExecuteReader();
                datebaseConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("DataUpdateX error");
                //Console.WriteLine(e);
            }
        }
        private static void DataUpdateAccount(string UpdateTable, string UpdateX, string Account)
        {
            //string query = "UPDATE charxy SET " + UpdateTable + " = " + UpdateX + "' WHERE account = " + Account + ";";
            string query = "UPDATE user SET first_name = '" + UpdateX + "' WHERE id = " + Account + ";";
            //string query = "UPDATE user SET first_name = 'testtest' WHERE id = 0;";
            MySqlConnection datebaseConnection = new MySqlConnection(MySqlConnectString);
            MySqlCommand commandDatebase = new MySqlCommand(query, datebaseConnection);
            MySqlDataReader reader;
            try
            {
                datebaseConnection.Open();
                reader = commandDatebase.ExecuteReader();
                datebaseConnection.Close();
            }
            catch (Exception e)
            {
                //Console.WriteLine("DataUpdateX error");
                Console.WriteLine(e);
            }
        }
        private static void DataDelete(string delete)
        {
            string query = "DELETE FROM " + delete;
            MySqlConnection datebaseConnection = new MySqlConnection(MySqlConnectString);
            MySqlCommand commandDatebase = new MySqlCommand(query, datebaseConnection);
            MySqlDataReader reader;
            try
            {
                datebaseConnection.Open();
                reader = commandDatebase.ExecuteReader();
                datebaseConnection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("DataDelete error");
                Console.WriteLine(e);
            }

        }
        public static void Updateoffline(string newid, int newonline)
        {
            //string query = "UPDATE charxy SET " + UpdateTable + " = " + UpdateX + "' WHERE account = " + Account + ";";
            //string query = "UPDATE monster SET monsterx = '" + newmonsterx + "' WHERE id = '" + newid + "' AND map = '" + newmap + "';";
            string query = "UPDATE player SET online = '" + newonline + "' WHERE account = '" + newid + "';";
            //string query = "UPDATE charxy SET x = '" + UpdateX + "' WHERE account = '" + Num + "';"
            MySqlConnection datebaseConnection = new MySqlConnection(MySqlConnectString);
            MySqlCommand commandDatebase = new MySqlCommand(query, datebaseConnection);
            MySqlDataReader reader;
            try
            {
                datebaseConnection.Open();
                reader = commandDatebase.ExecuteReader();
                datebaseConnection.Close();
            }
            catch (Exception e)
            {
                //Console.WriteLine("DataUpdateX error");
                Console.WriteLine(e);
            }
        }

    }
}


class StatusChecker
{
    static string MySqlConnectString = "datasource=127.0.0.1;port=3306;username=server;password=;database=project;";
    int count = 0;
    private int MonsterMap;
    private int MonsterNum;
    public Boolean start = true;
    public Boolean boolupdate = true;
    public Monster monster;
    public int rangemin;
    public int rangemax;
    private static Boolean ret = true;
    public StatusChecker(int MonsterMap, int MonsterHp, int rangemin, int rangemaxm, int monsterip, int restmonster)
    {
        Random r = new Random();
        this.rangemax = 260;
        this.rangemin = -45;
        this.MonsterMap = MonsterMap;
        int buffer = r.Next(0, 305);
        buffer = buffer -45;
        monster = new Monster(MonsterHp,(float) buffer, 0, monsterip);
    }
    // This method is called by the timer delegate.
    public void CheckStatus(Object stateInfo)
    {
        AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
        //for (int i = 0; i < MonsterNum; i++)
        //{
        // Console.WriteLine("BeforeTime: {0:h:mm:ss.ffffff}", DateTime.Now);
        if (!boolupdate)
        {
            Random r = new Random();
            int buffer = r.Next(rangemin, rangemax);
            monster.Monsterx = (float) buffer;
            UpdateMonsterHp(monster.Monsterid, monster.Monsterhp);
            UpdateMonster(monster.Monsterid, 0, monster.Monsterx);
            Console.WriteLine("buffer :" + buffer);
            Console.WriteLine("monster.Monsterx :" + monster.Monsterx);
            Console.WriteLine("monster.Monsterid :" + monster.Monsterid);
            Console.WriteLine("ret :" + ret);
            Console.WriteLine("rangemax:" + rangemax);
            Console.WriteLine("start :" + start);
            boolupdate = true;
            start = true;
        }


        if (monster.Monsterx < rangemax && ret)
        {
            if (start)
            {
                monster.Monsterx = monster.Monsterx + 5f;
                if(monster.Monsterx >= rangemax)
                {
                    ret = false;
                }
            }
            else
            {
                count++;
                if (count == 5)
                {
                    start = true;
                }
            }
            UpdateMonster(monster.Monsterid, 0, monster.Monsterx);
            //count++;
            /*if (monster.Monsterx >= rangemax)
            {
                ret = false;
            }
            else
            {
                count++;
                if (count == 5)
                {
                    start = true;
                }
            }*/
        }
        else if (!ret && monster.Monsterx > rangemin)
        {
            if (start)
            {
                monster.Monsterx = monster.Monsterx - 5f;
                if (monster.Monsterx <= rangemin)
                {
                    ret = true;
                }
            }
            else
            {
                count++;
                if (count == 5)
                {
                    start = true;
                }
            }
            UpdateMonster(monster.Monsterid, 0, monster.Monsterx);
            /*if (monster.Monsterx <= rangemin)
            {
                ret = true;
            }
            else
            {
                count++;
                if (count == 5)
                {
                    start = true;
                }

            }*/
        }

        //Console.WriteLine("monsterip : [" + monster.Monsterid + "]  monsterX : " + monster.Monsterx +"     "+ "monsterY : "  + monster.Monstery);
        //Console.WriteLine("NowTime: {0:h:mm:ss.ffffff}", DateTime.Now);
        //Console.WriteLine("count :"+ count);
        //}
    }
    private static void UpdateMonster(int newid, int newmap, float newmonsterx)
    {
        //string query = "UPDATE charxy SET " + UpdateTable + " = " + UpdateX + "' WHERE account = " + Account + ";";
        //string query = "UPDATE monster SET monsterx = '" + newmonsterx + "' WHERE id = '" + newid + "' AND map = '" + newmap + "';";
        string query = "UPDATE monster SET x = '" + newmonsterx + "' WHERE id = '" + newid + "';";
        //string query = "UPDATE charxy SET x = '" + UpdateX + "' WHERE account = '" + Num + "';"
        MySqlConnection datebaseConnection = new MySqlConnection(MySqlConnectString);
        MySqlCommand commandDatebase = new MySqlCommand(query, datebaseConnection);
        MySqlDataReader reader;
        try
        {
            datebaseConnection.Open();
            reader = commandDatebase.ExecuteReader();
            datebaseConnection.Close();
        }
        catch (Exception e)
        {
            //Console.WriteLine("DataUpdateX error");
            Console.WriteLine(e);
        }
    }
    private static void UpdateMonsterHp(int newid,float newmonsterhp)
    {
        //string query = "UPDATE charxy SET " + UpdateTable + " = " + UpdateX + "' WHERE account = " + Account + ";";
        //string query = "UPDATE monster SET monsterx = '" + newmonsterx + "' WHERE id = '" + newid + "' AND map = '" + newmap + "';";
        string query = "UPDATE monster SET hp = '" + newmonsterhp + "' WHERE id = '" + newid + "';";
        //string query = "UPDATE charxy SET x = '" + UpdateX + "' WHERE account = '" + Num + "';"
        MySqlConnection datebaseConnection = new MySqlConnection(MySqlConnectString);
        MySqlCommand commandDatebase = new MySqlCommand(query, datebaseConnection);
        MySqlDataReader reader;
        try
        {
            datebaseConnection.Open();
            reader = commandDatebase.ExecuteReader();
            datebaseConnection.Close();
        }
        catch (Exception e)
        {
            //Console.WriteLine("DataUpdateX error");
            Console.WriteLine(e);
        }
    }

    public class Monster
    {
        public int Monsterid;
        public int Monsterhp;
        public float Monsterx;
        public float Monstery;
        public float speed;
        public Monster(int MonsterHp, float MonsterX, float MonsterY, int Monsterid)
        {
            Monsterhp = MonsterHp;
            Monsterx = MonsterX;
            Monstery = MonsterY;
            this.Monsterid = Monsterid;
        }

        public void AttackMonster(int Attack)
        {
            Monsterhp = Monsterhp - Attack;
        }
        public double getx() { return Monsterx; }
        public double gety() { return Monstery; }
    }
}

