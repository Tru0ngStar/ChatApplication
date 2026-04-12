# Phase 2 Code Reference - Quick Lookup

## 📚 Table of Contents
1. [Packet Structure](#packet-structure)
2. [Server Group Management](#server-group-management)
3. [Client Group UI](#client-group-ui)
4. [Message Routing](#message-routing)
5. [State Management](#state-management)

---

## Packet Structure

### GroupId Mapping
```csharp
// GroupId → Display Name
0 → "Group 1"
1 → "Group 2"
2 → "Group 3"
-1 → Not in group (default)
```

### Packet Types
```csharp
public enum PacketType
{
    Register,        // User registration
    Login,          // User login
    Message,        // Broadcast message to all
    File,           // File transfer
    UpdateUserList, // Update online users
    CreateGroup,    // Deprecated (kept for compatibility)
    JoinGroup,      // Join a group
    GroupMessage,   // Send message to group
    LeaveGroup      // Leave a group (NEW)
}
```

### Packet Properties
```csharp
public class Packet
{
    public PacketType Type { get; set; }
    public string? Sender { get; set; }
    public string? Password { get; set; }
    public string? Content { get; set; }
    public bool IsSuccess { get; set; }
    public string? FileName { get; set; }
    public long FileSize { get; set; }
    public byte[]? FileData { get; set; }
    
    // GROUP CHAT PROPERTIES
    public string? GroupName { get; set; }      // Deprecated
    public int GroupId { get; set; }             // NEW: ID-based routing
    public List<string>? UserList { get; set; }
}
```

### Creating Packets
```csharp
// Join Group
var joinPacket = new Packet(PacketType.JoinGroup)
{
    GroupId = 0,           // Group 1
    Sender = "alice"
};

// Send Group Message
var msgPacket = new Packet(PacketType.GroupMessage)
{
    GroupId = 0,           // Send to Group 1
    Sender = "alice",
    Content = "Hello Group 1"
};

// Leave Group
var leavePacket = new Packet(PacketType.LeaveGroup)
{
    GroupId = 0,           // Leave Group 1
    Sender = "alice"
};
```

---

## Server Group Management

### Group Dictionary
```csharp
// Server.cs
private static readonly Dictionary<int, List<ClientHandler>> _groups = new();
private static readonly object _groupLock = new();

// Group ID → Display Name
private static readonly Dictionary<int, string> _groupNames = new()
{
    { 0, "Group 1" },
    { 1, "Group 2" },
    { 2, "Group 3" }
};
```

### Add Client to Group
```csharp
public static void AddClientToGroup(int groupId, ClientHandler handler)
{
    lock (_groupLock)
    {
        if (!_groups.ContainsKey(groupId))
            _groups[groupId] = new();
        
        if (!_groups[groupId].Contains(handler))
            _groups[groupId].Add(handler);
    }
    
    string groupName = _groupNames.ContainsKey(groupId) 
        ? _groupNames[groupId] 
        : $"Group {groupId}";
    Console.WriteLine($"[GROUP] {handler.Username} joined: {groupName}");
}
```

### Remove Client from Group
```csharp
public static void RemoveClientFromGroup(int groupId, ClientHandler handler)
{
    lock (_groupLock)
    {
        if (_groups.ContainsKey(groupId))
            _groups[groupId].Remove(handler);
    }
}
```

### Broadcast to Group
```csharp
public static async Task BroadcastToGroupAsync(int groupId, Packet packet, ClientHandler? exclude = null)
{
    lock (_groupLock)
    {
        if (!_groups.ContainsKey(groupId)) return;
        
        var clientsCopy = _groups[groupId].ToList();
        var json = JsonSerializer.Serialize(packet);
        var data = Encoding.UTF8.GetBytes(json);
        
        foreach (var client in clientsCopy)
        {
            if (client == exclude || !client.IsConnected) continue;
            
            try 
            { 
                _ = Task.Run(() => client.SendAsync(data)); 
            }
            catch 
            { 
                client.Disconnect(); 
            }
        }
    }
}
```

### Server Console Output Examples
```
[GROUP 0] alice: Hello Group 1
[GROUP 0] bob: Hi Alice!
[GROUP 1] charlie: Alone in Group 2
[GROUP] alice left: Group 1
```

---

## Client Group UI

### ListBox Initialization
```csharp
// Form1.Designer.cs - InitializeComponent()
lstGroups = new ListBox();
lstGroups.Location = new Point(20, 50);
lstGroups.Size = new Size(260, 50);
lstGroups.Items.Add("Group 1");
lstGroups.Items.Add("Group 2");
lstGroups.Items.Add("Group 3");
lstGroups.SelectedIndexChanged += lstGroups_SelectedIndexChanged;
```

### Button State Management
```csharp
// Disable by default
btnJoinGroup.Enabled = false;
btnLeaveGroup.Enabled = false;

// Enable on selection
if (lstGroups.SelectedIndex >= 0)
{
    btnJoinGroup.Enabled = true;
    btnLeaveGroup.Enabled = (_currentGroupId == lstGroups.SelectedIndex);
}
```

### Group Selection Handler
```csharp
private void lstGroups_SelectedIndexChanged(object sender, EventArgs e)
{
    if (lstGroups.SelectedIndex < 0)
    {
        btnJoinGroup.Enabled = false;
        btnLeaveGroup.Enabled = false;
        return;
    }

    btnJoinGroup.Enabled = true;
    btnLeaveGroup.Enabled = (_currentGroupId == lstGroups.SelectedIndex);
    
    // Update button text
    if (_currentGroupId == lstGroups.SelectedIndex)
    {
        btnJoinGroup.Text = "Đã tham gia";
        btnJoinGroup.Enabled = false;
    }
    else
    {
        btnJoinGroup.Text = "Tham gia Nhóm";
    }
}
```

---

## Message Routing

### Join Group Flow
```
USER ACTION: Click "Tham gia Nhóm" button
     ↓
Client: Get ListBox.SelectedIndex → GroupId
     ↓
Client: _currentGroupId = GroupId
     ↓
Client: rtbChat.Clear() (new context)
     ↓
Client: Send Packet(JoinGroup, GroupId=0)
     ↓
Server: Receive JoinGroup packet
     ↓
Server: Program.AddClientToGroup(0, this)
     ↓
Server: Broadcast system message to Group 0
     ↓
All Group 0 members: Receive "{Username} joined Group 1"
```

### Send Message Flow
```
USER ACTION: Type "Hello" + Click Send
     ↓
Client: Check if _currentGroupId >= 0
     ↓
if true (in group):
  Send Packet(GroupMessage, GroupId=0, Content="Hello")
else (not in group):
  Send Packet(Message, Content="Hello")
     ↓
Server: Receive packet
     ↓
if GroupMessage:
  Program.BroadcastToGroupAsync(0, packet)
    → Send ONLY to Group 0 members
else:
  Program.BroadcastAsync(packet)
    → Send to ALL clients
```

### Leave Group Flow
```
USER ACTION: Click "Rời Nhóm" button
     ↓
Client: int departingGroupId = _currentGroupId
     ↓
Client: _currentGroupId = -1 (reset state)
     ↓
Client: rtbChat.Clear()
     ↓
Client: Send Packet(LeaveGroup, GroupId=departingGroupId)
     ↓
Server: Receive LeaveGroup packet
     ↓
Server: Program.RemoveClientFromGroup(departingGroupId, this)
     ↓
Server: Broadcast system message to group
     ↓
Group members: Receive "{Username} left Group X"
```

---

## State Management

### Client State Variables
```csharp
// Client login state
private TcpClient? _client;
private NetworkStream? _stream;
private string? _currentUsername;

// Group chat state (NEW VARIABLES)
private int _currentGroupId = -1;  // Current group (-1 = not in any group)

// Group display names (for UI)
private readonly Dictionary<int, string> _groupNames = new()
{
    { 0, "Group 1" },
    { 1, "Group 2" },
    { 2, "Group 3" }
};
```

### State Transitions
```csharp
// Not logged in
_currentUsername = null;
_currentGroupId = -1;

// After login
_currentUsername = "alice";
_currentGroupId = -1;  // Still not in a group

// After joining Group 1
_currentGroupId = 0;   // Now in Group 1

// After switching to Group 2
_currentGroupId = 1;   // Now in Group 2

// After leaving group
_currentGroupId = -1;  // Back to not in a group
```

### Checking Group State
```csharp
// Check if user is in a group
if (_currentGroupId >= 0)
{
    // User is in a group
    string groupName = _groupNames[_currentGroupId];
    Console.WriteLine($"In {groupName}");
}
else
{
    // User is NOT in any group
    Console.WriteLine("Not in any group");
}
```

---

## Common Patterns

### Pattern 1: Send Group Message
```csharp
if (_currentGroupId >= 0)
{
    var packet = new Packet(PacketType.GroupMessage)
    {
        GroupId = _currentGroupId,
        Sender = _currentUsername,
        Content = "Your message"
    };
    await SendPacket(packet);
}
```

### Pattern 2: Get Group Name from ID
```csharp
string GetGroupName(int groupId)
{
    return _groupNames.ContainsKey(groupId) 
        ? _groupNames[groupId] 
        : $"Group {groupId}";
}

// Usage
string name = GetGroupName(_currentGroupId);  // "Group 1"
```

### Pattern 3: Check Group Membership
```csharp
bool IsInGroup(int groupId)
{
    return _currentGroupId == groupId;
}

// Usage
if (IsInGroup(0))
{
    Console.WriteLine("In Group 1");
}
```

### Pattern 4: Switch Groups
```csharp
private void SwitchGroup(int newGroupId)
{
    _currentGroupId = newGroupId;
    rtbChat.Clear();  // Clear old group's chat
    AppendSystemMessage($"Switched to {_groupNames[newGroupId]}", Color.Green);
}
```

---

## Error Handling

### Validation Patterns
```csharp
// Check before sending group message
if (_currentGroupId < 0)
{
    MessageBox.Show("You must be in a group to send group messages!");
    return;
}

// Check if group exists on server
if (!_groups.ContainsKey(groupId))
{
    // Group not initialized yet, create it
    _groups[groupId] = new List<ClientHandler>();
}

// Check client connectivity
if (client == null || !client.IsConnected)
{
    // Remove from group
    RemoveClientFromGroup(groupId, client);
}
```

### Try-Catch Patterns
```csharp
// Sending packet
try
{
    await SendPacket(packet);
}
catch (Exception ex)
{
    MessageBox.Show($"Error sending message: {ex.Message}");
}

// Receiving packet
try
{
    var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(packet));
    await _stream.WriteAsync(data, 0, data.Length);
}
catch
{
    client.Disconnect();
}
```

---

## Debugging Tips

### Check Group Membership
```csharp
// On Server
void PrintGroupMembers(int groupId)
{
    if (_groups.ContainsKey(groupId))
    {
        var count = _groups[groupId].Count;
        var names = string.Join(", ", _groups[groupId].Select(c => c.Username));
        Console.WriteLine($"Group {groupId}: {count} members - {names}");
    }
}

// Output: Group 0: 2 members - alice, bob
```

### Check Server State
```csharp
// Print all groups and members
void PrintAllGroups()
{
    foreach (var kvp in _groups)
    {
        Console.WriteLine($"Group {kvp.Key}: {kvp.Value.Count} members");
        foreach (var member in kvp.Value)
        {
            Console.WriteLine($"  - {member.Username}");
        }
    }
}
```

### Trace Message Routing
```csharp
// On Server - Add logging
public static async Task BroadcastToGroupAsync(int groupId, Packet packet, ...)
{
    lock (_groupLock)
    {
        if (!_groups.ContainsKey(groupId))
        {
            Console.WriteLine($"[DEBUG] Group {groupId} not found!");
            return;
        }
        
        var count = _groups[groupId].Count;
        Console.WriteLine($"[DEBUG] Sending to Group {groupId}: {count} recipients");
        
        // ... rest of implementation
    }
}
```

---

## Testing Commands

### Test Join Group
```csharp
// Simulate user joining Group 1
lstGroups.SelectedIndex = 0;  // Select Group 1
btnJoinGroup_Click(null, null);  // Click join button
```

### Test Send Message
```csharp
// Simulate sending group message
txtInput.Text = "Test message";
btnSend_Click(null, null);  // Click send button
```

### Test Leave Group
```csharp
// Simulate leaving group
btnLeaveGroup_Click(null, null);  // Click leave button
```

---

## References

- **Files Modified**: 5
  - ChatShared/Packet.cs
  - ChatServer/Program.cs
  - ChatServer/ClientHandler.cs
  - ChatClient/Form1.Designer.cs
  - ChatClient/Form1.cs

- **New Packet Type**: LeaveGroup

- **New UI Controls**: ListBox (lstGroups), Label (lblGroupList)

- **Removed UI Controls**: TextBox (txtGroupName), Button (btnCreateGroup)

- **Key Algorithms**: Group ID mapping, thread-safe dictionary access, state transitions

---

## Quick Links
- [PHASE2_TESTING_GUIDE.md](./PHASE2_TESTING_GUIDE.md) - Detailed testing scenarios
- [PHASE2_IMPLEMENTATION_DETAILS.md](./PHASE2_IMPLEMENTATION_DETAILS.md) - Full implementation breakdown
- [FEATURES.md](./FEATURES.md) - Phase 1 features reference

