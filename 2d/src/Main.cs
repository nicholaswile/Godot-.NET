using Godot;

public partial class Main : Node
{
	[Export]
	public PackedScene MobScene { get; set; }

	private int						_score;
	private Hud						_hud;
	private Timer					_mobTimer;
	private Timer					_scoreTimer;
	private Timer					_startTimer;
	private Player					_player;
	private Marker2D				_startPosition;
	private PathFollow2D			_mobSpawnLoc;
	private AudioStreamPlayer		_bgm;
	private AudioStreamPlayer		_gameoverSound;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_hud = GetNode<Hud>("HUD");
		_mobTimer = GetNode<Timer>("MobTimer");
		_scoreTimer = GetNode<Timer>("ScoreTimer");
		_startTimer = GetNode<Timer>("StartTimer");
		_player = GetNode<Player>("Player(Area2D)");
        _startPosition = GetNode<Marker2D>("StartPos");
		_mobSpawnLoc = GetNode<PathFollow2D>("MobPath/MobSpawnLoc");
		_bgm = GetNode<AudioStreamPlayer>("BGM");
		_gameoverSound = GetNode<AudioStreamPlayer>("DeathSound");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		CheckInput();
	}

	private void CheckInput()
	{
		if (Input.IsActionPressed("quit"))
		{
			GetTree().Quit(); 
		}
	}

	public void GameOver()
	{
		_mobTimer.Stop();
		_scoreTimer.Stop();
		_hud.ShowGameOver();
		_bgm.Stop();
		_gameoverSound.Play();
	}

	public void NewGame()
	{
		// Remove the enemies
		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);

        _score = 0;
		_player.Start(_startPosition.Position);
		_startTimer.Start();
		_hud.UpdateScore(_score);
		_hud.ShowMessage("Get Ready!");
		_bgm.Play();
	}

	// Every x-seconds (num seconds set in MobTimer inspector), create a new mob
	// In the mob class, a random mob type is chosen
	private void OnMobTimerTimeout()
	{
		// New instance of mob 
		Mob mob = MobScene.Instantiate<Mob>();

		// Choose random loc on Path2D
		_mobSpawnLoc = GetNode<PathFollow2D>("MobPath/MobSpawnLoc");
		_mobSpawnLoc.ProgressRatio = GD.Randf();

		// Set mob's dir perpendicular to player path
		float direction = _mobSpawnLoc.Rotation + Mathf.Pi / 2;

		// Set mob's pos to random loc
		mob.Position = _mobSpawnLoc.Position;

		// Randomize dir
		direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
		mob.Rotation = direction;

		// Randomize velocity
		Vector2 velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		mob.LinearVelocity = velocity.Rotated(direction);

		// Spawn mob by adding to main scene
		AddChild(mob);
	}	

	private void OnScoreTimerTimeout()
	{
		_score++;
		_hud.UpdateScore(_score);
	}

	private void OnStartTimerTimeout()
	{
		_mobTimer.Start();
		_scoreTimer.Start();
	}
}