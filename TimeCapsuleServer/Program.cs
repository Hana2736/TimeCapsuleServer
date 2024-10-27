using System.Text.Json;

namespace TimeCapsuleServer;

public class Program
{
    public static void Main()
    {
        //start the server
        var listener = new ServerListener();
        new CapsuleMgr();
        try
        {
            var reader = File.OpenRead("c:/users/Travis/OneDrive/desktop/db.json");
            CapsuleMgr.capsules =
                (Dictionary<Guid, TimeCapsuleContainer>)JsonSerializer.Deserialize(reader,
                    CapsuleMgr.capsules.GetType());
            reader.Close();
        }
        catch (Exception e)
        {
            //irdgaf
        }


        listener.StartServer(new RequestHandler());
    }
}