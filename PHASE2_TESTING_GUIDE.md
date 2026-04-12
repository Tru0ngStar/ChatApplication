# Phase 2 Testing Guide - Group Chat UI Enhancement

## 📋 Overview

This guide provides step-by-step instructions to test the enhanced group chat system with ListBox-based group selection instead of manual text input.

---

## 🎯 New Features in Phase 2

### ✅ Completed Changes:

1. **GroupId System (Int-based)**
   - Groups now use ID (0, 1, 2) instead of string names
   - Group 1 → GroupId = 0
   - Group 2 → GroupId = 1
   - Group 3 → GroupId = 2

2. **ListBox Group Selection**
   - Replaced manual text input (txtGroupName) with ListBox (lstGroups)
   - Shows 3 predefined groups: "Group 1", "Group 2", "Group 3"
   - Single-selection ListBox for clarity

3. **Dynamic Join/Leave Buttons**
   - Join button enables/disables based on group selection
   - Leave button only enabled when user is in selected group
   - Button text updates to show membership status

4. **Server-side Routing**
   - Dictionary<int, List<ClientHandler>> for group management
   - Messages only sent to group members (not broadcast to all)
   - Efficient group lookup by GroupId

5. **New Packet Type**
   - Added `PacketType.LeaveGroup` for proper leave handling
   - All group packets now use `GroupId` (int) property

---

## 🚀 Quick Start Testing (5 minutes)

### Step 1: Start the Server
```powershell
cd ChatServer
dotnet run
# Expected output: 
# [DB] Cơ sở dữ liệu tài khoản đã sẵn sàng.
# SERVER CHAT ĐÃ KHỞI ĐỘNG
# Cổng: 9999
```

### Step 2: Launch 3 Client Instances

**Method A - Visual Studio (Recommended):**
1. Right-click on `ChatClient` project
2. Select `Debug` → `Start New Instance`
3. Repeat 2 more times to open 3 windows

**Method B - PowerShell:**
```powershell
# Terminal 1
cd ChatClient && dotnet run

# Terminal 2 (new window)
cd ChatClient && dotnet run

# Terminal 3 (new window)
cd ChatClient && dotnet run
```

### Step 3: Login to All Clients
- Client 1: Username `alice`, Password `123` (or any new account)
- Client 2: Username `bob`, Password `123`
- Client 3: Username `charlie`, Password `123`

---

## 🧪 Test Scenario 1: Basic Group Selection

**Objective:** Verify ListBox displays groups and selection works

### Actions:
1. In **Client 1 (Alice)**:
   - Look at the "Nhóm Chat" section
   - Verify ListBox shows: `Group 1`, `Group 2`, `Group 3`
   - Click on `Group 1`
   - Verify "Tham gia Nhóm" button is enabled
   - Verify "Rời Nhóm" button is **disabled**

### Expected Result: ✅
```
ListBox items visible and selectable
"Tham gia Nhóm" button: ENABLED
"Rời Nhóm" button: DISABLED
```

---

## 🧪 Test Scenario 2: Join and Send Group Messages

**Objective:** Verify joining groups and group-isolated messaging

### Setup:
- Client 1 (Alice) → Group 1
- Client 2 (Bob) → Group 1
- Client 3 (Charlie) → Group 2

### Actions:

**Alice (Group 1):**
1. Select "Group 1" in ListBox
2. Click "Tham gia Nhóm"
3. Verify: Button text changes, "Rời Nhóm" button enables
4. Verify: Chat shows green system message: `✅ Tham gia nhóm: Group 1`
5. Type message: "Hello Group 1"
6. Click "Gửi" or press Enter

**Bob (Group 1):**
1. Do same steps as Alice (join Group 1)
2. Verify: Receives Alice's message with red text: `alice: Hello Group 1`
3. Type reply: "Hi Alice from Group 1"
4. Click "Gửi"

**Charlie (Group 2):**
1. Select "Group 2"
2. Click "Tham gia Nhóm"
3. Type test message: "Charlie in Group 2"
4. Verify: **DOES NOT SEE** Alice or Bob's messages ❌ (should be isolated)

### Expected Result: ✅
- Alice ↔ Bob can see each other's messages
- Charlie does NOT see Group 1 messages
- Charlie can send message in Group 2 isolation

### Verification in Server Console:
```
[GROUP 0] alice: Hello Group 1
[GROUP 0] bob: Hi Alice from Group 1
[GROUP 1] charlie: Charlie in Group 2
```

---

## 🧪 Test Scenario 3: Leave Group and Switch Groups

**Objective:** Verify leaving and switching groups clears chat history per group

### Setup:
- Alice in Group 1 with Bob
- Charlie in Group 2

### Actions:

**Alice:**
1. Send message: "Message in Group 1"
2. Bob receives and replies: "Reply in Group 1"
3. Alice sees reply
4. Click "Rời Nhóm" button
5. Verify: Chat is cleared
6. Verify: Button states reset (Tham gia enabled, Rời disabled)
7. Select "Group 2"
8. Click "Tham gia Nhóm"
9. Verify: Chat is cleared (different group context)
10. Send: "Alice joins Group 2"
11. Verify: Charlie receives this message

### Expected Result: ✅
- Chat clears when leaving group
- Chat clears when switching to different group
- Previous messages not visible in new group context
- Can communicate with new group members

---

## 🧪 Test Scenario 4: Multiple Group Scenarios

**Objective:** Complex group interaction with 3+ groups

### Setup:
- 3 clients connected and logged in

### Actions:

| Step | Alice | Bob | Charlie | Expected |
|------|-------|-----|---------|----------|
| 1 | Join G1 | Join G1 | Join G2 | All joined |
| 2 | Send "A→B" in G1 | - | - | B sees, C doesn't |
| 3 | - | Send "B→A" in G1 | - | A sees, C doesn't |
| 4 | Leave G1 | - | - | A chat clears |
| 5 | Join G2 | Join G2 | - | A & C joined, B alone |
| 6 | Send "A→C" in G2 | - | - | C sees, B doesn't |
| 7 | - | Join G1 (alone) | - | B can send in G1 alone |
| 8 | - | Send "B solo" | - | Only B sees in console |

### Expected Result: ✅
- All routing works correctly
- Messages isolated per group
- Users can join/leave without affecting others

---

## 🛠️ Troubleshooting

### Issue 1: Buttons Don't Enable/Disable
**Cause:** `lstGroups_SelectedIndexChanged` event not firing
**Solution:**
```csharp
// Make sure this code is in Form1.Designer.cs InitializeComponent:
lstGroups.SelectedIndexChanged += lstGroups_SelectedIndexChanged;
```

### Issue 2: Old TextBox Still Visible
**Cause:** Form1.Designer not updated properly
**Solution:** Rebuild the solution and restart clients

### Issue 3: Server Shows "Create new group" Message
**Cause:** `PacketType.CreateGroup` still being used in old code
**Solution:** The new version only uses `PacketType.JoinGroup` - no "create" action needed

### Issue 4: "Server chưa bật!" Error
**Cause:** Server not running on localhost:9999
**Solution:**
```powershell
# Ensure server is running in separate window/terminal
cd ChatServer && dotnet run
# Check for: "SERVER CHAT ĐÃ KHỞI ĐỘNG"
```

### Issue 5: Messages Not Appearing in Group
**Cause:** Possible GroupId mismatch between client and server
**Solution:** Check server console for `[GROUP X]` messages

---

## ✅ Verification Checklist

- [ ] **ListBox Display**: Groups 1-3 appear in ListBox
- [ ] **Button Behavior**: Buttons enable/disable correctly on selection
- [ ] **Join Action**: Joining group shows system message and buttons update
- [ ] **Chat Isolation**: Messages only go to group members
- [ ] **Chat Clear**: Chat clears when leaving/switching groups
- [ ] **Leave Action**: Leaving group removes from member list on server
- [ ] **Multiple Groups**: 3+ clients in different groups all work correctly
- [ ] **Server Routing**: Console shows correct `[GROUP X]` logging
- [ ] **No Broadcast**: Messages don't go to users in other groups
- [ ] **UI Responsive**: No freezing when sending/receiving messages

---

## 📊 Test Results Template

Copy this template to document test results:

```markdown
### Test: [Scenario Name]
**Date:** [Date]
**Client Count:** [Number]
**Result:** ✅ PASS / ❌ FAIL

**Details:**
- Group 1: [Users] - [Status]
- Group 2: [Users] - [Status]
- Group 3: [Users] - [Status]

**Issues Found:**
- [Issue 1]
- [Issue 2]

**Notes:**
[Additional observations]
```

---

## 🎓 Key Code Changes Summary

### Server-side (Program.cs):
```csharp
// Before: Dictionary<string, List<ClientHandler>> _groups
// After:  Dictionary<int, List<ClientHandler>> _groups
private static readonly Dictionary<int, List<ClientHandler>> _groups = new();

// Method signature changed:
// Before: BroadcastToGroupAsync(string groupName, ...)
// After:  BroadcastToGroupAsync(int groupId, ...)
```

### Client-side (Form1.cs):
```csharp
// Before: private string _currentGroup
// After:  private int _currentGroupId = -1

// New event handler:
private void lstGroups_SelectedIndexChanged(object sender, EventArgs e) { ... }

// Packets now use GroupId:
packet.GroupId = _currentGroupId;  // Instead of packet.GroupName
```

---

## 📝 Notes

- **Default State**: Users start NOT in any group (_currentGroupId = -1)
- **Hardcoded Groups**: Groups 1-3 are predefined (not user-created)
- **No Create Action**: Users "join" existing groups (no "create" functionality)
- **Group Persistence**: Groups exist during server runtime only
- **File Sharing**: Files can be sent to specific groups (if GroupId set)

---

## 🚀 Next Steps (Optional)

After successful Phase 2 testing, consider:
1. Add group chat history persistence (database)
2. Implement user roles (admin/member/moderator)
3. Add group-specific settings (private/public)
4. Implement group search and discovery
5. Add rich typing indicators ("User X is typing...")

