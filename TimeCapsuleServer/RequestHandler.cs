using System.Collections.Specialized;
using System.Security.Cryptography;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace TimeCapsuleServer;

public class RequestHandler
{
    public Dictionary<string, Func<NameValueCollection, string>> QueryHandlers;

    public RequestHandler()
    {
        QueryHandlers = new Dictionary<string, Func<NameValueCollection, string>>();
        QueryHandlers.Add("hello", HandleHello);
        QueryHandlers.Add("encrypt", GenerateKey);
        QueryHandlers.Add("open", UnlockCap);
    }

    private string HandleHello(NameValueCollection query)
    {
        Console.WriteLine("Hello!!");
        return "<html><body><h1>Hello World!</h1></body></html>";
    }

    private string GenerateKey(NameValueCollection query)
    {
        Console.WriteLine("serving key");
        Console.WriteLine("they want timestamp " + query["encrypt"]);
        var guid = Guid.NewGuid();
        var key = RandomNumberGenerator.GetBytes(32);
        var iv = RandomNumberGenerator.GetBytes(16);
        var newCap = new TimeCapsuleContainer
        {
            encryptKey = key,
            ivBytes = iv,
            capsuleGuid = guid,
            openTimestamp = ulong.Parse(query["encrypt"])
        };
        CapsuleMgr.capsules.Add(guid, newCap);

        var ser = JsonSerializer.Create();
        var writer = new StreamWriter("c:/users/Travis/OneDrive/desktop/db.json");
        ser.Serialize(writer, CapsuleMgr.capsules);
        writer.Flush();
        writer.Close();
 
        return guid + ":" + Convert.ToBase64String(key) + ":" + Convert.ToBase64String(iv);
    }

    private string UnlockCap(NameValueCollection query)
    {
        Console.WriteLine("client wants to open the box");
        var capsId = Guid.Parse(query["open"]);
        CapsuleMgr.capsules.TryGetValue(capsId, out var cap);
        if (cap == null)
            throw new Exception("we dont have their capsule");
        var time = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (time < cap.openTimestamp)
            return "";
        var key = Convert.ToBase64String(cap.encryptKey);
        var iv = Convert.ToBase64String(cap.ivBytes);
        return key + ":" + iv;
    }
}