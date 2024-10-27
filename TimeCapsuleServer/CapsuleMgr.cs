namespace TimeCapsuleServer;

public class CapsuleMgr
{
    public static Dictionary<Guid, TimeCapsuleContainer> capsules;

    public CapsuleMgr()
    {
        capsules = new Dictionary<Guid, TimeCapsuleContainer>();
    }
}

public class TimeCapsuleContainer
{
    public Guid capsuleGuid;
    public byte[] encryptKey;
    public byte[] ivBytes;
    public ulong openTimestamp;
}