using UnityEngine;

public static class RespawnTicket
{
    private static bool   hasTicket;
    private static Vector3 pos;
    private static Quaternion rot;
    private static bool   reopenPuzzle;

    public static void Set(Transform target, bool reopen)
    {
        if (!target) return;
        hasTicket    = true;
        pos          = target.position;
        rot          = target.rotation;
        reopenPuzzle = reopen;
    }

    public static bool Consume(out Vector3 p, out Quaternion r, out bool reopen)
    {
        if (hasTicket)
        {
            p       = pos;
            r       = rot;
            reopen  = reopenPuzzle;
            hasTicket = false;
            return true;
        }
        p = default; r = default; reopen = false; 
        return false;
    }
}
