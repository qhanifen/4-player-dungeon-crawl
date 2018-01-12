using Rewired;

public class PlayerEntity
{
    public float playerID = 0;
    public Player player;
    public PlayerController playerController;

    public int heroID;

    public PlayerEntity(int _playerID, Player _player)
    {
        playerID = _playerID;
        player = _player;
        playerController = new PlayerController();
    }
}
