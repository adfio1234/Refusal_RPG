public enum RoomType
{
    Hub,
    Combat,
    Shop,
    Treasure,
    Boss
}

public static class SelectedRoomData
{
    public static RoomType SelectedRoomType = RoomType.Hub;

    public static void ResetRun()
    {
        SelectedRoomType = RoomType.Hub;
    }

    public static void SelectRoom(RoomType roomType)
    {
        SelectedRoomType = roomType;
    }
}