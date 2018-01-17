using Rewired;

public class PlayerEntity
{
    public float playerID = 0;
    public Player rewiredPlayer;
    public PlayerController playerController;
    public bool active;

    public int heroID;

    public PlayerEntity(int _playerID, Player _player)
    {
        playerID = _playerID;
        rewiredPlayer = _player;
        playerController = new PlayerController();
    }
}
