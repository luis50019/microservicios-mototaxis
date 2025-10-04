using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

public class UserConnectionManager
{
    // Diccionario seguro para multihilo
    private static ConcurrentDictionary<string, HashSet<string>> _connections =
        new ConcurrentDictionary<string, HashSet<string>>();

    public void AddConnection(string userId, string connectionId)
    {
        _connections.AddOrUpdate(userId,
            _ => new HashSet<string> { connectionId },
            (_, ids) => { ids.Add(connectionId); return ids; });
    }

    public void RemoveConnection(string userId, string connectionId)
    {
        if (_connections.TryGetValue(userId, out var ids))
        {
            ids.Remove(connectionId);
            if (ids.Count == 0)
            {
                _connections.TryRemove(userId, out _);
            }
        }
    }

    public IEnumerable<string> GetConnections(string userId)
    {
        return _connections.TryGetValue(userId, out var ids) ? ids : Enumerable.Empty<string>();
    }

    public IEnumerable<string> GetAllUsers() => _connections.Keys;
}

