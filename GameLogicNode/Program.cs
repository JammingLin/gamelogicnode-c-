using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otp;
namespace GameLogicNode
{
    public class Program
    {

        public static string gNode = @"game_logic_node@192.168.70.123";
        public static string gCookie = @"knowledge";
        public static string gGameNode = @"knowledge_game@192.168.70.124";
        public class LogicNode
        {
            public OtpNode node { get; set; }
            public OtpMbox mbox { get; set; }
            
            public Dictionary<int, GameLogic> gamelogics { get; set; }
            public LogicNode(string nodename, string cookie)
            {
                node = new OtpNode(nodename, true, cookie);
                mbox = node.createMbox("game_logic");
                
                node.connection(gGameNode, cookie);

                gamelogics = new Dictionary<int, GameLogic>();

                System.Console.Out.WriteLine("create logic node " + nodename + " cookie " + cookie);
                
            }

            public void close()
            {
                if (mbox != null) mbox.close();
                if (node != null) node.close();
            }

            public void process()
            {
                while (true)
                {
                    try
                    {
                        Otp.Erlang.Object msg = mbox.receive();
                        Otp.Erlang.Tuple t = (Otp.Erlang.Tuple)msg;
                        
                        if (t.arity() == 3 && t.elementAt(0) is Otp.Erlang.Pid)
                        {
                            onReceiveMsg(t);
                            Console.WriteLine("receive remote msg " + t.elementAt(1).stringValue());
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Console.Out.WriteLine("receive remote msg exception :" + ex.ToString());
                    }
                }
            }

            public void onReceiveMsg(Otp.Erlang.Tuple t)
            {
                string request = t.elementAt(1).stringValue();
                Otp.Erlang.Tuple request_content = t.elementAt(2) as Otp.Erlang.Tuple;
                switch(request)
                {
                    case "create_room":
                        create_room(request_content);
                        break;
                    case "get_room_info":
                        get_room_info(request_content);
                        break;
                    default:
                        break;
                };
            }

            public void create_room(Otp.Erlang.Tuple content)
            {
                if (content.arity() == 2)
                {
                    int roomid = content.elementAt(0).intValue();
                    int maxplayer = content.elementAt(1).intValue();
                    if (!gamelogics.ContainsKey(roomid))
                    {
                        GameLogic gl = new GameLogic();
                        gl.RoomID = roomid;
                        gl.MaxPlayer = maxplayer;
                        gamelogics.Add(gl.RoomID, gl);
                    }

                    Console.Out.WriteLine("create room room id " + roomid + "current room num is " + gamelogics.Count);
                }
                else
                {
                    Console.Out.WriteLine("bad request arity is not 2 aritynum is "+content.arity());
                }
            }

            public void get_room_info(Otp.Erlang.Tuple content)
            {
                if (content.arity() == 1)
                {

                    int roomid = content.elementAt(0).intValue();
                    GameLogic gl = null;
                    gamelogics.TryGetValue(roomid, out gl);
                    if (gl != null)
                    {
                        mbox.rpcCall(gGameNode, "game_logic_node", "room_info", new Otp.Erlang.List(new Otp.Erlang.Int(roomid), new Otp.Erlang.Int(gl.MaxPlayer)));
                    }
                }
                else
                {
                    Console.Out.WriteLine("bad request arity is not 2 aritynum is " + content.arity());
                }
           }
        }
        public class GameLogic
        {
            public int RoomID { get; set; }
            public int MaxPlayer { get; set; }

        }

        static void Main(string[] args)
        {
            LogicNode l_node = new LogicNode(gNode, gCookie);
            l_node.process();
            l_node.close();
        }
    }
}
